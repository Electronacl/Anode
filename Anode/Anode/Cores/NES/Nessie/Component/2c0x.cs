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
        public bool FrameComplete;

        // OAM registers
        byte ppuSpriteEvalTemp;
        byte ppuOAMAddress;
        byte ppuSecondaryOAMAddress;
        byte ppuSecondaryOAMSize;
        byte ppuSpriteEvalTick;
        bool lastScanLineContainsSpriteZero;
        bool ppuScanLineContainsSpriteZero;
        bool ppuSpriteEvaluationOAMOverflowed;
        bool ppuSecondaryOAMFull;

        // Sprite shift registers
        byte[] ppu_SpriteShiftRegisterL = new byte[8];
        byte[] ppu_SpriteShiftRegisterH = new byte[8];

        byte[] ppu_SpriteAttribute = new byte[8];
        byte[] ppu_SpritePattern = new byte[8];
        byte[] ppu_SpriteXposition = new byte[8];
        byte[] ppu_SpriteYposition = new byte[8];

        // ----- PPU Flags
        bool ppuVRAMInc32Mode;
        bool ppuSpritePatternTable;
        bool ppuBGPatternTable;
        bool ppuUse8x16Sprites;
        bool ppuEnableNMI;

        bool ppuMask_GreyscaleMode;
        bool ppuMask_8pxMaskBG;
        bool ppuMask_8pxMaskSprites;
        bool ppuMask_RenderBG;
        bool ppuMask_RenderSprites;

        bool ppuStatusOverflow;
        bool ppuStatusSprZeroHit;

        // ----- PPU Registers and render info
        bool ppu_w; // Write Latch
        ushort ppu_t; // Transfer Address
        ushort ppu_v; // VRAM Address
        byte ppu_x; // PPU X scroll

        // BG Shift registers
        ushort ppuShiftRegister_patternL;
        ushort ppuShiftRegister_patternH;
        ushort ppuShiftRegister_attributeL;
        ushort ppuShiftRegister_attributeH;

        // Registers used in the 8-step BG render sequence
        byte ppu8Step_patternLowBitPlane;
        byte ppu8Step_patternHighBitPlane;
        byte ppu8Step_attribute;
        byte ppu8Step_NextCharacter;

        // Screen position and info
        int ppuDot;
        int ppuScanLine;
        bool ppuVBlank;
        bool ppuInVBlank;

        // PPU registers
        ushort TempVRAMAddress;
        byte PPUReadBuffer; // Used in reads from $2007

        // public byte[] CHRData = new byte[0x2000]; // Character data
        byte[] VRAM = new byte[0x800]; // Video Random Access Memory
        byte[] PaletteRAM = new byte[32]; // Palette Random Access Memory
        byte[] OAM = new byte[0x100]; // Object Attribute Memory
        byte[] SecondaryOAM = new byte[0x20]; // Secondary Object Attribute Memory

        byte lastPPUIOUpdate = 0;

        byte[] Pal = {
            0x65, 0x65, 0x65, 0x00, 0x2A, 0x84, 0x15, 0x13, 0xA2, 0x3A, 0x01, 0x9E, 0x59, 0x00, 0x7A, 0x6A, 0x00, 0x3E, 0x68, 0x08, 0x00, 0x53, 0x1D, 0x00, 0x32, 0x34, 0x00, 0x0D, 0x46, 0x00, 0x00, 0x4F, 0x00, 0x00, 0x4C, 0x09, 0x00, 0x3F, 0x4B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0xAE, 0xAE, 0xAE, 0x17, 0x5F, 0xD6, 0x43, 0x41, 0xFF, 0x75, 0x29, 0xFA, 0x9E, 0x1D, 0xCA, 0xB4, 0x20, 0x7B, 0xB1, 0x33, 0x22, 0x96, 0x4E, 0x00, 0x6A, 0x6C, 0x00, 0x39, 0x84, 0x00, 0x0F, 0x90, 0x00, 0x00, 0x8D, 0x33, 0x00, 0x7B, 0x8C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0xFE, 0xFE, 0xFE, 0x66, 0xAF, 0xFF, 0x93, 0x90, 0xFF, 0xC5, 0x78, 0xFF, 0xEE, 0x6C, 0xFF, 0xFF, 0x6F, 0xCA, 0xFF, 0x82, 0x71, 0xE6, 0x9E, 0x25, 0xBA, 0xBC, 0x00, 0x88, 0xD5, 0x01, 0x5E, 0xE1, 0x32, 0x47, 0xDD, 0x82, 0x4A, 0xCB, 0xDC, 0x4E, 0x4E, 0x4E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0xFE, 0xFE, 0xFE, 0xC0, 0xDE, 0xFF, 0xD2, 0xD1, 0xFF, 0xE7, 0xC7, 0xFF, 0xF8, 0xC2, 0xFF, 0xFF, 0xC3, 0xE9, 0xFF, 0xCB, 0xC4, 0xF5, 0xD7, 0xA5, 0xE2, 0xE3, 0x94, 0xCE, 0xED, 0x96, 0xBC, 0xF2, 0xAA, 0xB3, 0xF1, 0xCB, 0xB4, 0xE9, 0xF0, 0xB6, 0xB6, 0xB6, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        byte[] Pal_R = new byte[64];
        byte[] Pal_G = new byte[64];
        byte[] Pal_B = new byte[64];

        public void Run_PPU()
        {
            // Rendering and position config
            xRender++;
            if (xRender > 341)
            {
                xRender = 0;
                yRender++;
                if (yRender > 261)
                {
                    yRender = 0;
                }
            }
            if (xRender == 1 && yRender == 241)
            {
                FrameComplete = true;
            }
            if (yRender < 240 && xRender > 0 && xRender <= 256)
            {
                RenderPixel = true;
            }
            else
            {
                RenderPixel = false;
            }
        }

        void Sprite_Eval()
        {
            if (xRender == 0)
            {
                // Init sprite evaluation
                ppuSecondaryOAMAddress = 0;
                ppuOAMAddress = 0; // assumed
                ppuSecondaryOAMFull = false;
                ppuSpriteEvalTick = 0;

                // As sprite eval handles the *next* scanline
                lastScanLineContainsSpriteZero = ppuScanLineContainsSpriteZero;

                ppuScanLineContainsSpriteZero = false;
                ppuSpriteEvaluationOAMOverflowed = false;
                ppuStatusOverflow = false;
            }
        }

        public _2c0x()
        {
            int pal_i = 0;
            for (int j = 0; j < 64; j++)
            {
                Pal_R[j] = Pal[pal_i++];
                Pal_G[j] = Pal[pal_i++];
                Pal_B[j] = Pal[pal_i++];
            }

            xRender = 341;
            yRender = 261;
        }
    }
}
