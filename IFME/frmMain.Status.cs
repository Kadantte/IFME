using System;
using System.Text;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IFME
{
    class VSDesignerBugFixB { }

    public partial class frmMain
    {
        internal static frmMain frmMainStatic = null;

        // Encoders emit hundreds of lines per second. Marshalling each one with a blocking
        // Invoke stalled the encoding thread and forced a RichTextBox relayout per line.
        // Producers now only enqueue; a UI timer drains everything in one batch.
        private const int LogFlushIntervalMs = 100;
        private const int LogMaxLines = 5000;
        private const int LogTrimToLines = 4000;

        private readonly ConcurrentQueue<string> logQueue = new ConcurrentQueue<string>();
        private readonly ConcurrentDictionary<int, string> pendingProgress = new ConcurrentDictionary<int, string>();
        private readonly ConcurrentDictionary<int, string> pendingStatus = new ConcurrentDictionary<int, string>();

        private Timer logPumpTimer;

        private void InitializeLogPump()
        {
            logPumpTimer = new Timer { Interval = LogFlushIntervalMs };
            logPumpTimer.Tick += LogPumpTimer_Tick;
            logPumpTimer.Start();
        }

        private void LogPumpTimer_Tick(object sender, EventArgs e)
        {
            FlushProgress();
            FlushLog();
        }

        private void FlushLog()
        {
            if (logQueue.IsEmpty)
                return;

            var batch = new StringBuilder();
            var lines = 0;

            while (logQueue.TryDequeue(out var line))
            {
                batch.Append(line).Append(Environment.NewLine);

                // Bound a single flush so a very chatty encoder cannot monopolise the tick.
                if (++lines >= 500)
                    break;
            }

            if (batch.Length == 0)
                return;

            rtfConsole.SuspendLayout();

            TrimConsole();
            rtfConsole.AppendText(batch.ToString());

            // Keep the caret pinned to the end without a per-line ScrollToCaret.
            rtfConsole.SelectionStart = rtfConsole.TextLength;
            rtfConsole.ScrollToCaret();

            rtfConsole.ResumeLayout();
        }

        /// <summary>
        /// The console used to grow unbounded for an entire session. Drop the oldest lines
        /// once it exceeds the cap.
        /// </summary>
        private void TrimConsole()
        {
            if (rtfConsole.Lines.Length <= LogMaxLines)
                return;

            var keep = new string[LogTrimToLines];
            Array.Copy(rtfConsole.Lines, rtfConsole.Lines.Length - LogTrimToLines, keep, 0, LogTrimToLines);
            rtfConsole.Lines = keep;
        }

        private void FlushProgress()
        {
            ApplyPending(pendingProgress, 5);
            ApplyPending(pendingStatus, 4);
        }

        /// <summary>
        /// Applies the most recent text queued per row. Intermediate values are discarded
        /// on purpose: only the latest progress line is worth painting.
        /// </summary>
        private void ApplyPending(ConcurrentDictionary<int, string> pending, int subItemIndex)
        {
            if (pending.IsEmpty)
                return;

            foreach (var key in new List<int>(pending.Keys))
            {
                if (!pending.TryRemove(key, out var text))
                    continue;

                if (key < 0 || key >= lstFile.Items.Count)
                    continue;

                lstFile.Items[key].SubItems[subItemIndex].Text = text;
            }
        }

        /// <summary>
        /// Queues row text for an explicit queue index. Goes through the same pump as
        /// PrintStatus/PrintProgress so a later value always wins over an earlier one.
        /// </summary>
        internal static void SetRowStatus(int index, string status, string progress)
        {
            var form = frmMainStatic;

            if (form == null)
                return;

            form.pendingStatus[index] = status;
            form.pendingProgress[index] = progress;
        }

        public static void PrintLog(string value)
        {
            if (value == null)
                return;

            frmMainStatic?.logQueue.Enqueue(value);
        }

        public static void PrintProgress(string value)
        {
            var form = frmMainStatic;

            if (form == null)
                return;

            form.pendingProgress[MediaEncoding.CurrentIndex] = value;
        }

        public static void PrintStatus(string value)
        {
            var form = frmMainStatic;

            if (form == null)
                return;

            form.pendingStatus[MediaEncoding.CurrentIndex] = value;
        }
    }
}
