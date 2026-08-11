using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using System.Globalization;

using NDesk.Options;

using IFME.OSManager;

namespace IFME
{
    static class Program
    {
        public static bool ArgsHelp = false;
        public static bool ArgsSkipAVX = false;
        public static bool ArgsSkipAVX2 = false;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            var o = new OptionSet()
            {
                { "h|?|help", "Show this message and exit", h => ArgsHelp = h != null },

                { "skip-avx", "Bypass AVX instruction set checks", x => ArgsSkipAVX = x != null },
                { "skip-avx2", "Bypass AVX2 instruction set checks", x => ArgsSkipAVX2 = x != null },
            };

            try
            {
                o.Parse(args);
            }
            catch (OptionException e)
            {
                Console.Error.WriteLine(e.Message);
                Console.Error.WriteLine($"Try `IFME.exe --help' for more information.");
            }

            if (ArgsHelp)
            {
                Console.Error.WriteLine($"Usage: IFME.exe [OPTIONS]+");
                Console.Error.WriteLine("Mandatory arguments to long option are mandatory for short options too.");
                Console.Error.WriteLine("\nOptions:");
                o.WriteOptionDescriptions(Console.Error);
                Console.Error.WriteLine();

                return;
            }

            var culture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            if (Properties.Settings.Default.UpgradeRequired)
            {
                Properties.Settings.Default.Upgrade();
                Properties.Settings.Default.UpgradeRequired = false;
                Properties.Settings.Default.Save();
            }

            Environment.CurrentDirectory = Path.GetDirectoryName(Application.ExecutablePath);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            if (!CheckCpuSupport())
                return;

            Application.Run(new frmMain());
        }

        /// <summary>
        /// Verifies required CPU instruction sets before the message loop starts. Doing this
        /// inside the frmMain constructor was ineffective: Application.Exit() is a no-op
        /// before Application.Run, so an unsupported CPU carried on into startup anyway.
        /// </summary>
        private static bool CheckCpuSupport()
        {
            if (!ArgsSkipAVX && !CPU.HasAVX)
            {
                MessageBox.Show(
                    "AVX instruction set not detected. A modern CPU with AVX support is required to continue. Please ensure your hardware is compatible. The program will now exit.",
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return false;
            }

            if (!ArgsSkipAVX2 && !CPU.HasAVX2)
            {
                MessageBox.Show(
                    "AVX2 instruction set not detected. A modern CPU with AVX2 support is required to continue. Please ensure your hardware is compatible.",
                    "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return true;
        }
	}
}
