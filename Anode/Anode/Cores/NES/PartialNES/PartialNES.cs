using Anode.Base;
using Anode.Common;
using System;

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
