using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Anode
{
    public partial class Form1 : Form
    {
        // Anode windows
        Form aboutForm;

        // Initialise variables which are used later
        Emulator emulator;
        string rompath;
        string romname;
        Thread processThread;
        // bool testenabled = false;
        bool tracelogging = false;
        PictureBox ScreenObject;


        bool fileUpdated = false;

        bool paused = false;
        bool waitingForFrame = false;

        readonly double NTSC_time = 1d / 60d;
        readonly double PAL_time = 1d / 50d;
        bool throttled = true;
        double timetaken;
        Stopwatch throttler = new Stopwatch();
        bool is_NTSC = true;
        bool detect_region = true;
        bool NTSC_FPS_Forced = false;

        bool auto_detect_ines = true;
        byte ines_version = 0;

        // Winforms stuff
        public Form1()
        {
            InitializeComponent();
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(dragDropEnter);
            this.DragDrop += new DragEventHandler(dragDropFile);
            this.Text = $"Anode {Constants.version_name}";
        }

        private void dragDropEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void dragDropFile(object sender, DragEventArgs e)
        {
            string[] files = e.Data.GetData(DataFormats.FileDrop) as string[];
            if (files != null && files.Any())
            {
                rompath = files.First();
                init_Emulator();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileUpdated = false;
            // Base code from
            // https://learn.microsoft.com/en-us/dotnet/api/system.windows.forms.openfiledialog
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = Path.GetDirectoryName(Application.ExecutablePath);
                openFileDialog.Filter = "NES Files|*.nes|All files (*.*)|*.*"; // .nes files are used
                openFileDialog.FilterIndex = 1;
                openFileDialog.RestoreDirectory = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    rompath = openFileDialog.FileName;
                    fileUpdated = true;
                }
            }
            if (rompath != null && fileUpdated == true)
            {
                // A new ROM has been added, so the emulator should start
                // A new thread is created to run the emulator

                init_Emulator();
            }
        }

        void Run_Emulator()
        {
            // Setup emulator

            // Personal debug only
            // testenabled = true;

            // Init the emulator
            emulator = new Emulator();
            emulator.filepath = rompath;
            emulator.logging = tracelogging;
            emulator.tracepath = Path.GetDirectoryName(Application.ExecutablePath) + "/tracelog.txt";

            // Update the emulator's user settings
            emulator.inesversion = ines_version;
            emulator.detectines = auto_detect_ines;

            emulator.NTSC = is_NTSC;
            emulator.detect_region = detect_region;

            // Get the emulator to prepare for running
            emulator.Reset();

            // The header value is used if it's both supported by the ROM, and it's enabled.
            if (emulator.CheckHeader())
            {
                ChangeTitle($"{emulator.GetTitle()} (Anode {Constants.version_name})");
            }

            // override for nestest only
            // emulator.ProgramCounter = 0xC000;

            // For locking
            ScreenObject = pictureBox1;
            // Start the throttler
            throttler.Start();
            while (!emulator.CPU_Halted && !emulator.incompatible)
            {
                if ((!paused) || waitingForFrame)
                {
                    // 1 cycle at a time
                    emulator.Advance_Frame();
                    // When the emulator has completed a frame, or crashed
                    lock (ScreenObject)
                    {
                        // Update the frame
                        if (pictureBox1.InvokeRequired)
                        {
                            pictureBox1.Invoke(new MethodInvoker(
                                delegate ()
                                {
                                    // This method runs if something is using the object
                                    pictureBox1.Image = emulator.output;
                                    pictureBox1.Update();
                                }));
                        }
                        else
                        {
                            // Otherwise, just update
                            pictureBox1.Image = emulator.output;
                            pictureBox1.Update();
                        }
                    }
                    emulator.frame_Ready = false;
                    if (throttled && !waitingForFrame)
                    {
                        //throttler.Stop();
                        timetaken = ((double)throttler.ElapsedTicks + 5d) / (double)Stopwatch.Frequency;
                        while (timetaken < ((emulator.NTSC || NTSC_FPS_Forced) ? NTSC_time : PAL_time))
                        {
                            // Spin loop
                            timetaken = ((double)throttler.ElapsedTicks) / (double)Stopwatch.Frequency;
                        }
                        // Reset the throttler
                        throttler.Stop();
                        throttler.Reset();
                        throttler.Start();
                    }
                    // Setup the next frame
                    waitingForFrame = false;
                    emulator.InitFrame();
                }
            }

            // Prevent the throttler from constantly running
            throttler.Stop();

            // Disable the menu items that only work when running
            enableMenuItem(forceHaltToolStripMenuItem, false);
            enableMenuItem(pauseToolStripMenuItem, false);
            enableMenuItem(advanceFrameToolStripMenuItem, false);

            // Use the tester if I've enabled that
            /*if (testenabled)
            {
                Tester tester = new Tester();
                tester.Test_Ram(emulator);
            }*/
        }

        private void haltToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Halt the CPU
            if (emulator != null)
            {
                emulator.CPU_Halted = true;
            }
        }

        private void hardResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Effectively power cycles and resets all RAM and registers.
            init_Emulator();
        }

        /*private void debugTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Unused code from an earlier version with the debug tests accessible
            // Still here as I might allow the user to use the tests
            testenabled = !testenabled;
            Console.WriteLine($"Testing: {testenabled}");
        }*/

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            // Properly close and prevent it from running in the background
            if (processThread != null)
            {
                processThread.Abort();
            }
        }

        private void toggleTracelogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Toggles the tracelog
            tracelogging = !tracelogging;
            toggleTracelogToolStripMenuItem.Text = "Tracelog " + (tracelogging ? "enabled" : "disabled");
            toggleTracelogToolStripMenuItem.Checked = tracelogging;
        }

        private void init_Emulator()
        {
            romname = Path.GetFileName(rompath);

            this.Text = $"{romname} (Anode {Constants.version_name})";

            pauseToolStripMenuItem.Text = "Pause";
            pauseToolStripMenuItem.Enabled = true;
            forceHaltToolStripMenuItem.Enabled = true;
            hardResetToolStripMenuItem.Enabled = true;
            if (processThread != null)
            {
                processThread.Abort();
            }

            processThread = new Thread(Run_Emulator);
            processThread.SetApartmentState(ApartmentState.STA);
            processThread.IsBackground = true;
            processThread.Start();
        }

        private void forceHaltToolStripMenuItem_Click(object sender, EventArgs e)
        {
            emulator.CPU_Halted = true;
        }

        private void enableMenuItem(ToolStripMenuItem menuItem, bool enabled)
        {
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                menuItem.Enabled = enabled;
            }));
        }

        private void ChangeTitle(string newname)
        {
            this.BeginInvoke(new MethodInvoker(delegate ()
            {
                this.Text = newname;
            }));
        }

        private void pauseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            paused = !paused;
            if (paused)
            {
                pauseToolStripMenuItem.Text = "Play";
                enableMenuItem(advanceFrameToolStripMenuItem, true);
            }
            else
            {
                pauseToolStripMenuItem.Text = "Pause";
                enableMenuItem(advanceFrameToolStripMenuItem, false);
            }
        }

        private void advanceFrameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            waitingForFrame = true;
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            aboutForm = new AnodeAbout();
            aboutForm.Show();
        }

        private void disableThrottlerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            throttled = !throttled;
            disableThrottlerToolStripMenuItem.Text = "Throttler " + (throttled ? "enabled" : "disabled");
            disableThrottlerToolStripMenuItem.Checked = throttled;
        }

        private void force60hzForPALToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NTSC_FPS_Forced = !NTSC_FPS_Forced;
            force60hzForPALToolStripMenuItem.Checked = NTSC_FPS_Forced;
        }

        private void DisableAlliNESOptions()
        {
            iNESToolStripMenuItem.Checked = false;
            iNESToolStripMenuItem1.Checked = false;
            archaicINESToolStripMenuItem.Checked = false;
            nES20ToolStripMenuItem.Checked = false;
        }

        private void iNESToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Auto-detect
            auto_detect_ines = true;
            DisableAlliNESOptions();
            iNESToolStripMenuItem.Checked = true;
        }

        private void archaicINESToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ines_version = 0;
            DisableAlliNESOptions();
            archaicINESToolStripMenuItem.Checked = true;
        }

        private void iNESToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            ines_version = 1;
            DisableAlliNESOptions();
            iNESToolStripMenuItem1.Checked = true;
        }

        private void nES20ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ines_version = 2;
            DisableAlliNESOptions();
            nES20ToolStripMenuItem.Checked = true;
        }

        private void autodetectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autodetectToolStripMenuItem.Checked = true;
            nTSCToolStripMenuItem.Checked = false;
            pALToolStripMenuItem.Checked = false;
            detect_region = true;
            if (emulator != null)
            {
                init_Emulator();
            }
        }

        private void nTSCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            nTSCToolStripMenuItem.Checked = true;
            pALToolStripMenuItem.Checked = false;
            autodetectToolStripMenuItem.Checked = false;
            is_NTSC = true;
            detect_region = false;
            if (emulator != null)
            {
                init_Emulator();
            }
        }

        private void pALToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pALToolStripMenuItem.Checked = true;
            nTSCToolStripMenuItem.Checked = false;
            autodetectToolStripMenuItem.Checked = false;
            is_NTSC = false;
            detect_region = false;
            if (emulator != null)
            {
                init_Emulator();
            }
        }
    }
}
