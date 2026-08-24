using Anode.Base;
using Anode.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anode.Cores.NES
{
    internal class PartialNES : EmuCore
    {
        Emulator emulator;
        void EmuCore.AdvanceFrame()
        {
            emulator.renderer.InitFrame();
            emulator.Advance_Frame();
            emulator.frame_Ready = false;
        }

        Renderer EmuCore.GetRenderer()
        {
            return emulator.renderer;
        }

        byte[] EmuCore.GetAudioBuffer()
        {
            throw new NotImplementedException();
        }

        string EmuCore.GetTitle()
        {
            return "";
        }

        byte[] EmuCore.GetCompatibleFeatures()
        {
            byte[] features =
            {
                0b00000100
            };
            return features;
        }

        void EmuCore.HardReset(string ROM)
        {
            emulator = new Emulator();
            emulator.filepath = ROM;
            emulator.Reset();
        }

        void EmuCore.SoftReset()
        {
            throw new NotImplementedException();
        }

        bool EmuCore.CanEmulatorRun()
        {
            return !emulator.CPU_Halted && !emulator.incompatible;
        }

        float EmuCore.GetSpeed()
        {
            return emulator.NTSC ? (1 / 60f) : (1 / 50f);
        }
    }
}
