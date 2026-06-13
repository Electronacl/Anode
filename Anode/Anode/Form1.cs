using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ThreadState = System.Threading.ThreadState;

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
                }
            }
            if (rompath != null)
            {
                // A new ROM has been added, so the emulator should start
                // A new thread is created to run the emulator
                processThread = new Thread(Run_Emulator);
                processThread.SetApartmentState(ApartmentState.STA);
                processThread.IsBackground = true;
                processThread.Start();
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

            // For locking
            ScreenObject = pictureBox1;

            while (!emulator.CPU_Halted)
            {
                // 1 cycle at a time
                emulator.Run();
                // When the emulator has completed a frame, or crashed
                if (emulator.frame_Ready)
                {
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
                    }
                }
            }

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
            if (processThread != null)
            {
                // Check if the process is already running and stop it if it is
                if (processThread.ThreadState == ThreadState.Running)
                {
                    processThread.Abort();
                }
                processThread = new Thread(Run_Emulator);
                processThread.SetApartmentState(ApartmentState.STA);
                processThread.IsBackground = true;
                processThread.Start();
            }
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
    }
}
