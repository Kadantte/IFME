using System;
using System.IO;
using System.Drawing;
using System.Threading;
using System.Reflection;
using System.Windows.Forms;
using System.ComponentModel;

namespace IFME
{
    public partial class frmSplashScreen : Form
    {
        private Image splashImage;

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            try
            {
                splashImage = WAD.Resource.LoadImage("SplashScreen14.png");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load splash image: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                splashImage = null;
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            WindowUtils.EnableAcrylic(this, Color.FromArgb(71, 18, 18, 18));
            base.OnHandleCreated(e);
        }

        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                e.Graphics.Clear(Color.Transparent);
            else
                e.Graphics.Clear(Color.Black);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (splashImage != null)
            {
                e.Graphics.DrawImage(splashImage, new Rectangle(0, 0, Width, Height));
            }
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            fadeTimer?.Stop();
            fadeTimer?.Dispose();
            fadeTimer = null;

            splashImage?.Dispose();
            splashImage = null;

            frmSplashScreenStatus = null;

            base.OnFormClosed(e);
        }


        private readonly BackgroundWorker bgThread = new BackgroundWorker();

        public frmSplashScreen()
        {
            frmSplashScreenStatus = this;
            InitializeComponent();

            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            Opacity = 0;

            bgThread.DoWork += BgThread_DoWork;
            bgThread.RunWorkerCompleted += BgThread_RunWorkerCompleted;
        }

        private void frmSplashScreen_Load(object sender, EventArgs e)
        {
            lblVersion.Text = $"Release Version {Version.Release} ({Version.CodeName})";
            lblContrib.Text = $"{Version.Contrib}\n\n{Version.TradeMark}";
        }

        private void frmSplashScreen_Shown(object sender, EventArgs e)
        {
            StartFade(fadeIn: true);
            bgThread.RunWorkerAsync();
        }

        private void BgThread_DoWork(object sender, DoWorkEventArgs e)
        {
            lblLoadingUpdate(string.Empty);
            lblStatusUpdate(string.Empty);

            // Detect user machine
            // TODO: Detect user GPU
            lblLoadingUpdate("Initialising...");

            // Load settings
            if (Properties.Settings.Default.FolderOutput.IsDisable())
                Properties.Settings.Default.FolderOutput = AppPath.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos));
            

            if (Properties.Settings.Default.FolderTemporary.IsDisable())
                Properties.Settings.Default.FolderTemporary = AppPath.Combine(Path.GetTempPath(), "IFME");

            Properties.Settings.Default.Save();

            // Temp folder
            try
            {
                if (Directory.Exists(Properties.Settings.Default.FolderTemporary))
                    Directory.Delete(Properties.Settings.Default.FolderTemporary, true);
            }
            catch (Exception ex)
            {
                lblStatusUpdate(ex.Message);
            }

            Directory.CreateDirectory(Properties.Settings.Default.FolderTemporary);

            // Load language
            i18n.LoadLangFiles();
            var lang = Properties.Settings.Default.UILanguage;
            PrintContrib($"i18n: {i18n.Installed[lang]} by {i18n.GetLangAuthor(lang)[0]}\n\n{lblContrib.Text}");

            // Load config
            new PluginsLoad();

            // Finished loading, clear status text
            lblStatusUpdate(string.Empty);

            Thread.Sleep(250);

            lblLoadingUpdate(string.Empty);

            // Wait some CPU free
            Thread.Sleep(1000);

            // If user choose not to test the encoder, wait little longer telling user IFME not test
            if (!Properties.Settings.Default.TestEncoder)
                Thread.Sleep(1000);
        }

        private void BgThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Fade out on the UI thread, then close when it reaches zero.
            StartFade(fadeIn: false);
        }

        // Opacity is a UI property: the old version read and wrote it from a worker thread
        // while queueing BeginInvoke mutations, so the loop condition raced its own updates.
        private System.Windows.Forms.Timer fadeTimer;
        private bool fadeDirectionIn;

        private void StartFade(bool fadeIn)
        {
            fadeDirectionIn = fadeIn;

            if (fadeTimer == null)
            {
                fadeTimer = new System.Windows.Forms.Timer { Interval = 15 };
                fadeTimer.Tick += FadeTimer_Tick;
            }

            fadeTimer.Start();
        }

        private void FadeTimer_Tick(object sender, EventArgs e)
        {
            if (fadeDirectionIn)
            {
                if (Opacity < 1)
                {
                    Opacity = Math.Min(1, Opacity + 0.08);
                    return;
                }

                fadeTimer.Stop();
                return;
            }

            if (Opacity > 0)
            {
                Opacity = Math.Max(0, Opacity - 0.08);
                return;
            }

            fadeTimer.Stop();
            Close();
        }

        private void lblLoadingUpdate(string value)
        {
            BeginInvoke((Action)delegate ()
            {
                lblLoading.Text = value;
            });
        }

        private void lblStatusUpdate(string value)
        {
            BeginInvoke((Action)delegate ()
            {
                lblStatus.Text = value;
            });
        }
    }
}
