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
        Emulator emulator;
        string rompath;
        Thread processThread;
        bool testenabled = false;
        bool tracelogging = false;
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
                openFileDialog.Filter = "NES Files|*.nes|All files (*.*)|*.*";
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
                // A new ROM has been added! Run the emulator!
                processThread = new Thread(Run_Emulator);
                processThread.SetApartmentState(ApartmentState.STA);
                processThread.IsBackground = true;
                processThread.Start();
            }
        }

        void Run_Emulator()
        { 
            // Setup emulator
            emulator = new Emulator();
            emulator.filepath = rompath;
            emulator.logging = tracelogging;
            emulator.tracepath = Path.GetDirectoryName(Application.ExecutablePath) + "/tracelog.txt";
            emulator.Reset();
            while (!emulator.CPU_Halted)
            {
                emulator.Run();
            }

            if (testenabled)
            {
                Tester tester = new Tester();
                tester.Test_Ram(emulator);
            }
        }

        private void haltToolStripMenuItem_Click(object sender, EventArgs e)
        {
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
            testenabled = !testenabled;
            Console.WriteLine($"Testing: {testenabled}");
        }

        private void toggleTracelogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tracelogging = !tracelogging;
            Console.WriteLine($"Tracelogging: {tracelogging}");
        }
    }
}
