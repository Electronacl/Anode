// using NAudio.Wave;
using Anode.Base;
using Anode.Common;
using Anode.Cores.NES;
using Anode.Cores.NES.Nessie;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

/*
Copyright © 2026 Electronacl

Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
and associated documentation files (the “Software”), to deal in the Software without restriction, 
including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, 
and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, 
subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial 
portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT 
LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, 
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE 
SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

namespace Anode
{
    public partial class Form1 : Form
    {
        // Anode windows
        Form aboutForm;
        Form optionForm;

        // Initialise variables which are used later
        EmuCore emulator;
        string rompath;
        string romname;
        Thread processThread;
        // bool testenabled = false;
        PictureBox ScreenObject;
        //MemoryStream ms;
        //RawSourceWaveStream rs;
        //WaveOutEvent wo;

        bool fileUpdated = false;

        bool paused = false;
        bool waitingForFrame = false;

        bool throttled = true;
        double timetaken;
        Stopwatch throttler = new Stopwatch();

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
                this.Activate();
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
            switch (Path.GetExtension(rompath).ToLower())
            {
                case ".nes":
                    switch (Properties.Settings.Default.NESCore)
                    {
                        case 0:
                            emulator = new PartialNES();
                            break;
                        case 1:
                            emulator = new NessieCore();
                            break;
                    }
                    break;
            }
            emulator.HardReset(rompath);

            string romHeader = emulator.GetTitle();
            if (romHeader == "") { romHeader = romname; }

            this.ChangeTitle($"{romname} - Anode {Constants.version_name}");
            // For locking
            ScreenObject = pictureBox1;
            // Start the throttler
            throttler.Start();

            while (emulator.CanEmulatorRun())
            {
                if (!paused || waitingForFrame)
                {
                    emulator.AdvanceFrame();
                    Renderer emuRenderer = emulator.GetRenderer();

                    lock (ScreenObject)
                    {
                        // Update the frame
                        if (pictureBox1.InvokeRequired)
                        {
                            pictureBox1.Invoke(new MethodInvoker(
                                delegate ()
                                {
                                    // This method runs if something is using the object
                                    pictureBox1.Image = emuRenderer.outputBitmap;
                                    pictureBox1.Update();
                                }));
                        }
                        else
                        {
                            // Otherwise, just update
                            pictureBox1.Image = emuRenderer.outputBitmap;
                            pictureBox1.Update();
                        }
                    }

                    if (throttled && !waitingForFrame)
                    {
                        /*if (wo != null)
                        {
                            while (wo.PlaybackState == PlaybackState.Playing)
                            {

                            }
                            wo.Dispose();
                        }
                        ms.Write(emulator.processedAPUBuffer, 0, emulator.processedAPUBuffer.Length);
                        wo = new WaveOutEvent();*/
                        //throttler.Stop();
                        float timerequired = emulator.GetSpeed();
                        timetaken = ((double)throttler.ElapsedTicks + 5d) / (double)Stopwatch.Frequency;
                        while (timetaken < timerequired)
                        {
                            // Spin loop
                            timetaken = ((double)throttler.ElapsedTicks) / (double)Stopwatch.Frequency;
                        }
                        // Reset the throttler
                        throttler.Stop();
                        throttler.Reset();
                        throttler.Start();
                        //wo.Init(rs);
                        //wo.Play();
                    }
                    // Setup the next frame
                    waitingForFrame = false;
                }
            }

            throttler.Stop();

            Console.WriteLine("Stopped running emulation");

            // Disable the menu items that only work when running
            enableMenuItem(pauseToolStripMenuItem, false);
            enableMenuItem(advanceFrameToolStripMenuItem, false);
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

        private void init_Emulator()
        {
            romname = Path.GetFileName(rompath);

            this.Text = $"{romname} (Anode {Constants.version_name})";

            pauseToolStripMenuItem.Text = "Pause";
            pauseToolStripMenuItem.Enabled = true;
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

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            optionForm = new AnodeOptions();
            optionForm.Show();
        }
    }
}
