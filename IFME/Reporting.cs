using System;

namespace IFME
{
    /// <summary>
    /// Where the encoding engine sends its output. Implemented by the UI, but the engine
    /// only ever sees this interface, so <see cref="MediaEncoding"/> and
    /// <see cref="ProcessManager"/> can run headless (CLI, batch, tests) with no form loaded.
    /// </summary>
    public interface IEncodeReporter
    {
        /// <summary>Free-form log line.</summary>
        void Log(string message);

        /// <summary>Short state for a queue row, e.g. "Encoding video".</summary>
        void Status(int queueIndex, string text);

        /// <summary>Progress detail for a queue row, e.g. the percentage/ETA line.</summary>
        void Progress(int queueIndex, string text);
    }

    /// <summary>
    /// Discards everything. Used until a real reporter is registered, so the engine never
    /// has to null-check.
    /// </summary>
    internal sealed class NullEncodeReporter : IEncodeReporter
    {
        public void Log(string message) { }
        public void Status(int queueIndex, string text) { }
        public void Progress(int queueIndex, string text) { }
    }

    /// <summary>
    /// Ambient reporter used by the encoding engine. Register the real sink once at
    /// startup with <see cref="Use"/>.
    /// </summary>
    public static class Report
    {
        private static IEncodeReporter sink = new NullEncodeReporter();

        /// <summary>
        /// False when output would be discarded, letting hot paths skip formatting work.
        /// </summary>
        public static bool HasSink => !(sink is NullEncodeReporter);

        public static void Use(IEncodeReporter reporter)
        {
            sink = reporter ?? new NullEncodeReporter();
        }

        public static void Log(string message)
        {
            sink.Log(message);
        }

        /// <summary>Reports against the queue item currently being processed.</summary>
        public static void Status(string text)
        {
            sink.Status(MediaEncoding.CurrentIndex, text);
        }

        /// <summary>Reports against the queue item currently being processed.</summary>
        public static void Progress(string text)
        {
            sink.Progress(MediaEncoding.CurrentIndex, text);
        }

        public static void Status(int queueIndex, string text)
        {
            sink.Status(queueIndex, text);
        }

        public static void Progress(int queueIndex, string text)
        {
            sink.Progress(queueIndex, text);
        }
    }
}
