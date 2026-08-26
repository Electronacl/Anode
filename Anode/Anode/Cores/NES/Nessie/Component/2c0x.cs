using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anode.Cores.NES.Nessie
{
    internal class _2c0x
    {
        public bool RenderPixel;
        public int xRender;
        public int yRender;
        public byte r;
        public byte g;
        public byte b;

        public byte[] CHRData = new byte[0x2000]; // Character data
        byte[] VRAM = new byte[0x800]; // Video Random Access Memory
        byte[] PaletteRAM = new byte[32]; // Palette Random Access Memory
        byte[] OAM = new byte[0x100]; // Object Attribute Memory
        byte[] SecondaryOAM = new byte[0x20]; // Secondary Object Attribute Memory
    }
}
