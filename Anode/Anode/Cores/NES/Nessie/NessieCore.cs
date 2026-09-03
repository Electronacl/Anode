using Anode.Base;
using Anode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anode.Cores.NES.Nessie
{
    internal class NessieCore : EmuCore
    {
        byte PPUClock;
        byte CPUClock;
        byte APUClock;

        byte MaxPPU;
        byte MaxCPU;

        _2a03 CPU;
        _2c0x PPU;
        NESIO IO;

        Renderer renderer;

        bool devmode = false;
        Tester tester;

        void EmuCore.AdvanceFrame()
        {
            renderer.InitFrame();
            while (!PPU.FrameComplete && !CPU.halt)
            {
                if (CPUClock == MaxCPU)
                {
                    CPU.AddressBus = CPU.DelayedAddr;

                    if (CPU.getRequired)
                    {
                        // The next cycle is a "Get" cycle
                        // This needs to be set *before* the next cycle gets
                        CPU.DataBus = IO.ReadCPU(CPU.AddressBus, CPU.DataBus);
                        CPU.RunCycle();
                    }
                    else
                    {
                        // The next cycle is a "Put" cycle
                        CPU.RunCycle();
                        CPU.DataBus = CPU.DataLatch;
                        IO.WriteCPU(CPU.AddressBus, CPU.DataBus);
                    }
                }

                PPUClock--;
                CPUClock--;
                //APUClock--;

                if (PPUClock == 0)
                {
                    PPUClock = MaxPPU;
                }
                if (CPUClock == 0)
                {
                    CPUClock = MaxCPU;
                }
                APUClock = CPUClock;
            }

            if (CPU.halt)
            {
                if (CPU.logging)
                {
                    tester.Test_Ram(IO.RAM);
                    CPU.tracelog.Close();
                }
            }

            PPU.FrameComplete = false;

            renderer.FinishFrame();
        }

        bool EmuCore.CanEmulatorRun()
        {
            return !CPU.halt && IO.compatible;
        }

        byte[] EmuCore.GetAudioBuffer()
        {
            throw new NotImplementedException();
        }

        byte[] EmuCore.GetCompatibleFeatures()
        {
            byte[] features =
            {
                0b00000100
            };
            return features;
        }

        Renderer EmuCore.GetRenderer()
        {
            return renderer;
        }

        float EmuCore.GetSpeed()
        {
            //throw new NotImplementedException();
            return 1 / 60f;
        }

        string EmuCore.GetTitle()
        {
            return "";
        }

        void EmuCore.HardReset(string ROM)
        {
            CPU = new _2a03();
            PPU = new _2c0x();
            IO = new NESIO();

            IO.LoadCart(ROM);

            MaxPPU = (byte)(IO.region ? 4 : 5);
            MaxCPU = (byte)(IO.region ? 12 : 16);

            PPUClock = MaxPPU;
            CPUClock = MaxCPU;

            CPU.PC = (ushort)((IO.ReadCPU(0xFFFD, 0) << 8) | IO.ReadCPU(0xFFFC, 0));
            CPU.getRequired = true;
            CPU.DelayedAddr = CPU.PC;

            renderer = new Renderer(32 * 8, (30 * 8) - (IO.region ? 0 : 1));

            tester = new Tester();
            CPU.logging = devmode;

            if (devmode)
            {
                CPU.tracepath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\tracelog.txt";
                CPU.tracelog = new StreamWriter(CPU.tracepath);
            }
        }

        void EmuCore.SoftReset()
        {
            throw new NotImplementedException();
        }
    }
}
