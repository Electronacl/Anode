using Anode.Base;
using Anode.Common;
using System;
using System.Collections.Generic;
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

        _2a03 CPU;
        _2c0x PPU;
        NESIO IO;

        void EmuCore.AdvanceFrame()
        {
            throw new NotImplementedException();
        }

        bool EmuCore.CanEmulatorRun()
        {
            throw new NotImplementedException();
        }

        byte[] EmuCore.GetAudioBuffer()
        {
            throw new NotImplementedException();
        }

        byte[] EmuCore.GetCompatibleFeatures()
        {
            throw new NotImplementedException();
        }

        Renderer EmuCore.GetRenderer()
        {
            throw new NotImplementedException();
        }

        float EmuCore.GetSpeed()
        {
            throw new NotImplementedException();
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
        }

        void EmuCore.SoftReset()
        {
            throw new NotImplementedException();
        }
    }
}
