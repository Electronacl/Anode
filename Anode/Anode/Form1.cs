using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace Anode
{
    public partial class Form1 : Form
    {
        // Initialise variables which are used later
        Emulator emulator;
        string rompath;
        Thread processThread;
        bool testenabled = false;
        bool tracelogging = false;
        PictureBox ScreenObject;


        bool FileUpdated = false;

        bool paused = false;
        bool waitingForFrame = false;

        // Winforms stuff
        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
                    FileUpdated = true;
                }
            }
            if (rompath != null && FileUpdated == true)
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

            emulator = new Emulator();
            emulator.filepath = rompath;
            emulator.logging = tracelogging;
            emulator.tracepath = Path.GetDirectoryName(Application.ExecutablePath) + "/tracelog.txt";

            // Get the emulator to prepare for running
            emulator.Reset();

            // override for nestest only
            // emulator.ProgramCounter = 0xC000;

            // For locking
            ScreenObject = pictureBox1;

            while (!emulator.CPU_Halted)
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
                                    pictureBox1.Image = emulator.output;
                                    pictureBox1.Update();
                                }));
                        }
                        else
                        {
                            pictureBox1.Image = emulator.output;
                            pictureBox1.Update();
                        }
                        emulator.frame_Ready = false;
                        waitingForFrame = false;
                    }
                }
            }

            enableMenuItem(forceHaltToolStripMenuItem, false);
            enableMenuItem(pauseToolStripMenuItem, false);
            enableMenuItem(advanceFrameToolStripMenuItem, false);

            // Use the tester if I've enabled that
            if (testenabled)
            {
                Tester tester = new Tester();
                tester.Test_Ram(emulator);
            }
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

        private void debugTestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Unused code from an earlier version with the debug tests accessible
            // Still here as I might allow the user to use the tests
            testenabled = !testenabled;
            Console.WriteLine($"Testing: {testenabled}");
        }

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
            Console.WriteLine($"Tracelogging: {tracelogging}");
            toggleTracelogToolStripMenuItem.Text = (tracelogging ? "Disable" : "Enable") + " tracelog";
        }

        private void init_Emulator()
        {
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
    }
}
