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
        static readonly ulong NessieVersion = 2;

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

        bool OAM_Cycle;
        byte OAM_Temp_Value;

        void EmuCore.AdvanceFrame()
        {
            renderer.InitFrame();
            while (!PPU.FrameComplete && !CPU.halt)
            {
                if (CPUClock == MaxCPU)
                {
                    if (!IO.OAMDMA)
                    {
                        CPU.AddressBus = CPU.DelayedAddr;
                        CPU.NMIConditionsMet = PPU.CheckNMIConditions();

                        if (CPU.getRequired)
                        {
                            // The next cycle is a "Get" cycle
                            // This needs to be set *before* the next cycle gets
                            if (CPU.AddressBus >= 0x2000 && CPU.AddressBus < 0x4000)
                            {
                                CPU.DataBus = PPU.CPU_Read_PPU(CPU.AddressBus);
                            }
                            else
                            {
                                CPU.DataBus = IO.ReadCPU(CPU.AddressBus, CPU.DataBus);
                            }
                            CPU.RunCycle();
                        }
                        else
                        {
                            // The next cycle is a "Put" cycle
                            CPU.RunCycle();
                            CPU.DataBus = CPU.DataLatch;
                            if (CPU.AddressBus >= 0x2000 && CPU.AddressBus < 0x4000)
                            {
                                PPU.CPU_Write_PPU(CPU.AddressBus, CPU.DataBus);
                            }
                            else
                            {
                                IO.WriteCPU(CPU.AddressBus, CPU.DataBus);
                            }
                        }
                    }
                    else
                    {
                        // This is here as I'm also adding DMC DMA somewhen
                        // CPU would also need to run but would only do APU.
                        if (IO.OAMDMA)
                        {
                            if (IO.OAMDMAInit < (CPU.CPU_Cycle ? 2 : 1))
                            {
                                // Blank cycles - I just use these to init.
                                OAM_Cycle = false;
                                IO.OAMDMAInit++;
                                OAM_Temp_Value = CPU.DataBus;
                            }
                            else
                            {
                                if (!OAM_Cycle)
                                {
                                    // Read cycle gets value
                                    OAM_Temp_Value = IO.ReadCPU((ushort)((IO.OAM_POS << 8) | IO.OAMDMAAddr), OAM_Temp_Value);
                                }
                                else
                                {
                                    // Write cycle puts value into OAM
                                    PPU.OAM[IO.OAMDMAAddr] = OAM_Temp_Value;
                                    IO.OAMDMAAddr++;
                                    if (IO.OAMDMAAddr == 0x00)
                                    {
                                        IO.OAMDMA = false;
                                    }
                                }
                                OAM_Cycle = !OAM_Cycle;
                            }
                            // RDY is enabled iirc
                            // RDY_history |= 1;
                        }
                    }
                }

                if (PPUClock == MaxPPU)
                {
                    PPU.Run_PPU();
                    if (PPU.RenderPixel)
                    {
                        renderer.SetPixel(PPU.xRender - 1, PPU.yRender, PPU.r, PPU.g, PPU.b);
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

            // For running nestest headless
            if (devmode)
            {
                CPU.PC = 0xC000;
            }

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

            PPU.CHRData = IO.CHRData;
            PPU.MirrorMode = (IO.Header[6] & 1) == 0;
            PPU.IsCHRData = IO.Header[5] == 0;
        }

        void EmuCore.SoftReset()
        {
            throw new NotImplementedException();
        }
    }
}
