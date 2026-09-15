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

        // If you're wondering why some values say "PPU" and others don't, it's because
        // Nessie reuses some of its code from PartialNES

        // Although, it's annoying to port the code and I'm trying to improve it in the process too.

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

        // PPU registers
        ushort TempVRAMAddress;
        byte PPUReadBuffer; // Used in reads from $2007

        public byte[] CHRData = new byte[0x2000]; // Character data
        byte[] VRAM = new byte[0x800]; // Video Random Access Memory
        byte[] PaletteRAM = new byte[32]; // Palette Random Access Memory
        public byte[] OAM = new byte[0x100]; // Object Attribute Memory
        byte[] SecondaryOAM = new byte[0x20]; // Secondary Object Attribute Memory

        byte lastPPUIOUpdate = 0;
        byte PPUIOBus;

        ushort ppuDecayTime;

        byte[] Pal = {
            0x65, 0x65, 0x65, 0x00, 0x2A, 0x84, 0x15, 0x13, 0xA2, 0x3A, 0x01, 0x9E, 0x59, 0x00, 0x7A, 0x6A, 0x00, 0x3E, 0x68, 0x08, 0x00, 0x53, 0x1D, 0x00, 0x32, 0x34, 0x00, 0x0D, 0x46, 0x00, 0x00, 0x4F, 0x00, 0x00, 0x4C, 0x09, 0x00, 0x3F, 0x4B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0xAE, 0xAE, 0xAE, 0x17, 0x5F, 0xD6, 0x43, 0x41, 0xFF, 0x75, 0x29, 0xFA, 0x9E, 0x1D, 0xCA, 0xB4, 0x20, 0x7B, 0xB1, 0x33, 0x22, 0x96, 0x4E, 0x00, 0x6A, 0x6C, 0x00, 0x39, 0x84, 0x00, 0x0F, 0x90, 0x00, 0x00, 0x8D, 0x33, 0x00, 0x7B, 0x8C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0xFE, 0xFE, 0xFE, 0x66, 0xAF, 0xFF, 0x93, 0x90, 0xFF, 0xC5, 0x78, 0xFF, 0xEE, 0x6C, 0xFF, 0xFF, 0x6F, 0xCA, 0xFF, 0x82, 0x71, 0xE6, 0x9E, 0x25, 0xBA, 0xBC, 0x00, 0x88, 0xD5, 0x01, 0x5E, 0xE1, 0x32, 0x47, 0xDD, 0x82, 0x4A, 0xCB, 0xDC, 0x4E, 0x4E, 0x4E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0xFE, 0xFE, 0xFE, 0xC0, 0xDE, 0xFF, 0xD2, 0xD1, 0xFF, 0xE7, 0xC7, 0xFF, 0xF8, 0xC2, 0xFF, 0xFF, 0xC3, 0xE9, 0xFF, 0xCB, 0xC4, 0xF5, 0xD7, 0xA5, 0xE2, 0xE3, 0x94, 0xCE, 0xED, 0x96, 0xBC, 0xF2, 0xAA, 0xB3, 0xF1, 0xCB, 0xB4, 0xE9, 0xF0, 0xB6, 0xB6, 0xB6, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        byte[] Pal_R = new byte[64];
        byte[] Pal_G = new byte[64];
        byte[] Pal_B = new byte[64];

        byte DataBus;
        byte AddressBus;

        ushort PPUTargetAddress;

        public bool MirrorMode;
        public bool IsCHRData;

        bool VBlank;
        bool inVBlank;

        public void Run_PPU()
        {
            // Decay the PPU IO Bus
            if (lastPPUIOUpdate > ppuDecayTime)
            {
                PPUIOBus = 0;
            }
            else
            {
                lastPPUIOUpdate++;
            }

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
                VBlank = true;
                inVBlank = true;
            }
            if (xRender == 1 && yRender == 261)
            {
                VBlank = false;
                inVBlank = false;
                ppuStatusOverflow = false;
                ppuStatusSprZeroHit = false;
            }

            if ((xRender & 1) == 0)
            {
                // Read!
                DataBus = Read((ushort)((AddressBus << 8) | DataBus));
            }

            if (yRender < 240 || yRender == 261)
            {
                if (ppuMask_RenderBG || ppuMask_RenderSprites)
                {
                    // Shift sprite shift registers
                    // This never occurs on the same cycle as they are updated
                    // so it doesn't matter if they are before or after sprite
                    // eval
                    if (xRender > 1 && xRender <= 256)
                    {
                        for (int i = 0; i < 8; i++)
                        {
                            if (ppu_SpriteXposition[i] > 0)
                            {
                                ppu_SpriteXposition[i]--;
                            }
                            else
                            {
                                ppu_SpriteShiftRegisterL[i] <<= 1;
                                ppu_SpriteShiftRegisterH[i] <<= 1;
                            }
                        }
                    }
                }
                Sprite_Eval();
                if (ppuMask_RenderBG || ppuMask_RenderSprites)
                {
                    if ((xRender > 0 && xRender <= 256) || (xRender > 320 && xRender <= 336))
                    {
                        if (ppuMask_RenderBG)
                        {
                            // Shift BG shift registers
                            ppuShiftRegister_patternL <<= 1;
                            ppuShiftRegister_patternH <<= 1;
                            ppuShiftRegister_attributeL <<= 1;
                            ppuShiftRegister_attributeH <<= 1;
                        }
                        // Visible pixel or preparing next scanline

                        byte cycleTick;
                        cycleTick = (byte)((xRender - 1) & 7);
                        switch (cycleTick)
                        {
                            case 0:
                                // Update BG shift registers
                                ppuShiftRegister_patternL = (ushort)((ppuShiftRegister_patternL & 0xFF00) | ppu8Step_patternLowBitPlane);
                                ppuShiftRegister_patternH = (ushort)((ppuShiftRegister_patternH & 0xFF00) | ppu8Step_patternHighBitPlane);
                                ppuShiftRegister_attributeL = (ushort)((ppuShiftRegister_attributeL & 0xFF00) | ((ppu8Step_attribute & 1) == 1 ? 0xFF : 0));
                                ppuShiftRegister_attributeH = (ushort)((ppuShiftRegister_attributeH & 0xFF00) | ((ppu8Step_attribute & 2) == 2 ? 0xFF : 0));
                                // Set the address
                                PPUTargetAddress = (ushort)(0x2000 | (ppu_v & 0x0FFF));
                                break;
                            case 1:
                                // Load data into the register for the next character
                                ppu8Step_NextCharacter = DataBus;
                                break;
                            case 2:
                                // Set the address
                                PPUTargetAddress = (ushort)(0x23C0 | (ppu_v & 0x0C00) | ((ppu_v >> 4) & 0x38) | ((ppu_v >> 2) & 0x07));
                                break;
                            case 3:
                                // Load data for the attribute
                                ppu8Step_attribute = DataBus;
                                // Determine which tile attribute data is for
                                if ((ppu_v & 3) >= 2) // Right tile
                                {
                                    ppu8Step_attribute >>= 2;
                                }
                                if ((((ppu_v & 0x03E0) >> 5) & 3) >= 2) // Bottom tile
                                {
                                    ppu8Step_attribute >>= 4;
                                }
                                ppu8Step_attribute &= 3;
                                break;
                            case 4:
                                // Set the address
                                PPUTargetAddress = (ushort)(((ppu_v & 0x7000) >> 12) | (ppu8Step_NextCharacter << 4) | (ppuBGPatternTable ? 0x1000 : 0));
                                break;
                            case 5:
                                // Load the data into the low bit plane
                                ppu8Step_patternLowBitPlane = DataBus;
                                break;
                            case 6:
                                // Increment the address
                                PPUTargetAddress += 8;
                                break;
                            case 7:
                                // Load the data into the high bit plane
                                ppu8Step_patternHighBitPlane = DataBus;
                                if ((ppu_v & 0x001F) == 0x001F)
                                {
                                    ppu_v &= 0xFFE0; // Reset scroll
                                    ppu_v ^= 0x0400; // Cross into next nametable
                                }
                                else
                                {
                                    ppu_v++;
                                }
                                break;
                        }
                    }

                    if (yRender < 240)
                    {
                        if (xRender == 256)
                        {
                            PPU_IncrementScrollY();
                        }
                        else if (xRender == 257)
                        {
                            PPU_ResetXScroll();
                        }
                    }
                    if (xRender >= 280 && xRender <= 304 && yRender == 261)
                    {
                        PPU_ResetYScroll();
                    }
                }
            }

            if (yRender < 240 && xRender > 0 && xRender <= 256)
            {
                // Rendering!
                RenderPixel = true;

                // BG Rendering
                byte PalHi = 0; // Colour palette
                byte PalLow = 0; // Index in palette
                if (ppuMask_RenderBG && (xRender > 8 || ppuMask_8pxMaskBG))
                {
                    byte ppu_x_index = (byte)(0xF - ppu_x);
                    byte col0 = (byte)((ppuShiftRegister_patternL >> ppu_x_index) & 1);
                    byte col1 = (byte)((ppuShiftRegister_patternH >> ppu_x_index) & 1);
                    PalLow = (byte)((col1 << 1) | col0);

                    byte pal0 = (byte)(((ppuShiftRegister_attributeL) >> ppu_x_index) & 1);
                    byte pal1 = (byte)(((ppuShiftRegister_attributeH) >> ppu_x_index) & 1);
                    PalHi = (byte)((pal1 << 1) | pal0);
                }

                // Sprite rendering
                byte SpritePalHi = 0; // Colour palette
                byte SpritePalLow = 0; // Index in palette
                bool SpritePriority = false; // In front or behind BG?
                if (ppuMask_RenderSprites && (xRender > 8 || ppuMask_8pxMaskSprites))
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (ppu_SpriteXposition[i] == 0 && i < (ppuSecondaryOAMSize / 4))
                        {
                            bool SpixelL = ((ppu_SpriteShiftRegisterL[i]) & 0x80) != 0; // Takes bit from shift register to get low bit plane
                            bool SpixelH = ((ppu_SpriteShiftRegisterH[i]) & 0x80) != 0; // Takes bit from shift register to get high bit plane

                            SpritePalLow = (byte)(SpixelL ? 1 : 0);
                            SpritePalLow |= (byte)(SpixelH ? 2 : 0);

                            SpritePalHi = (byte)((ppu_SpriteAttribute[i] & 0x03) | 0x04);
                            SpritePriority = ((ppu_SpriteAttribute[i] >> 5) & 1) == 0;
                        }
                        else
                        {
                            continue;
                        }

                        if (SpritePalLow != 0)
                        {
                            if (i == 0 && lastScanLineContainsSpriteZero && SpritePalLow != 0 && PalLow != 0 && ppuMask_RenderBG && xRender < 256)
                            {
                                ppuStatusSprZeroHit = true;
                            }
                            break;
                        }
                    }
                }

                // Transparency behind sprites
                if (PalLow == 0 || (SpritePriority && SpritePalLow != 0))
                {
                    PalLow = SpritePalLow;
                    PalHi = SpritePalHi;
                }

                // Use the BG colour
                if (PalLow == 0) { PalHi = 0; }

                // Get the index into palette RAM
                int colourIndex = PaletteRAM[PalHi * 4 + PalLow] & 0x3F;

                // Disable lower nybble on greyscale mode
                if (ppuMask_GreyscaleMode)
                {
                    colourIndex &= 0x30;
                }

                r = Pal_R[colourIndex];
                g = Pal_G[colourIndex];
                b = Pal_B[colourIndex];
            }
            else
            {
                RenderPixel = false;
            }

            if ((xRender & 1) != 0)
            {
                // Set address
                DataBus = (byte)PPUTargetAddress;
                AddressBus = (byte)(PPUTargetAddress >> 8);
            }
        }

        private void Sprite_Eval()
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
            else if (xRender > 0 && xRender <= 64)
            {
                if ((xRender & 1) == 1)
                {
                    // Odd cycles load the value $FF
                    ppuSpriteEvalTemp = 0xFF;
                }
                else
                {
                    // And even cycles transfer it into the secondary OAM
                    // This is internal, so it doesn't need to use the DataBus and AddressBus lines?
                    // I need to figure out the registers a bit more.
                    SecondaryOAM[ppuSecondaryOAMAddress] = ppuSpriteEvalTemp;
                    ppuSecondaryOAMAddress++;
                    ppuSecondaryOAMAddress &= 0x1F;
                }
            }
            else if (xRender > 64 && xRender <= 256)
            {
                // Understanding the registers:
                // ppuOAMAddress is known on hardware as "n"
                if ((xRender & 1) == 1)
                {
                    // Odd cycles load the value from OAM
                    ppuSpriteEvalTemp = OAM[ppuOAMAddress];
                }
                else
                {
                    if (!ppuSpriteEvaluationOAMOverflowed)
                    {
                        // Even cycles load it into secondary OAM
                        if (!ppuSecondaryOAMFull)
                        {
                            SecondaryOAM[ppuSecondaryOAMAddress] = ppuSpriteEvalTemp;
                        }

                        if (!ppuSecondaryOAMFull)
                        {
                            if (ppuSpriteEvalTick == 0)
                            {
                                // Index 0 of the object's 4 bytes (Y pos)
                                if (yRender - ppuSpriteEvalTemp >= 0 && yRender - ppuSpriteEvalTemp < (ppuUse8x16Sprites ? 16 : 8))
                                {
                                    // The object is on this scanline
                                    ppuOAMAddress++;
                                    ppuSecondaryOAMAddress++;
                                    if (xRender == 66)
                                    {
                                        // Checks to see whether the sprite 0 is on this scanline
                                        // dot 66 will always be evaluating index 0
                                        ppuScanLineContainsSpriteZero = true;
                                    }
                                    ppuSpriteEvalTick++;
                                    /*else
                                    {
                                        // Ran out of room in secondaryOAM
                                        // This ignores an accuracy edge case, so it's kinda stable here.
                                        ppuStatusOverflow = true;
                                    }*/
                                }
                                else
                                {
                                    ppuOAMAddress += 4;
                                }
                            }
                            else
                            {
                                // For indexes 1, 2 and 3 of an object's OAM data
                                ppuSecondaryOAMAddress++;
                                ppuOAMAddress++;
                                if (ppuSecondaryOAMAddress == 0x20)
                                {
                                    ppuSecondaryOAMFull = true;
                                }
                                ppuSpriteEvalTick++;
                                ppuSpriteEvalTick &= 3;
                            }
                            if (ppuOAMAddress == 0)
                            {
                                ppuSpriteEvaluationOAMOverflowed = true;
                            }
                        }
                        else
                        {
                            ppuOAMAddress++;
                            if (yRender - ppuSpriteEvalTemp >= 0 && yRender - ppuSpriteEvalTemp < (ppuUse8x16Sprites ? 16 : 8))
                            {
                                ppuStatusOverflow = true;
                            }
                        }
                    }
                }
            }
            else if (xRender > 256 && xRender <= 320)
            {
                // As the PPU's BG routine has finished, we're now free to use
                // the address bus and use external memory until it is used again after
                // this routine.
                ppuOAMAddress = 0;
                if (xRender == 257)
                {
                    ppuSecondaryOAMSize = ppuSecondaryOAMAddress;
                    ppuSecondaryOAMAddress = 0;
                    ppuSpriteEvalTick = 0;
                }

                // Reading should occur outside of this routine

                switch (ppuSpriteEvalTick)
                {
                    case 0:
                        ppu_SpriteYposition[ppuSecondaryOAMAddress >> 2] = SecondaryOAM[ppuSecondaryOAMAddress];
                        ppuSecondaryOAMAddress++;
                        break;
                    case 1:
                        ppu_SpritePattern[ppuSecondaryOAMAddress >> 2] = SecondaryOAM[ppuSecondaryOAMAddress];
                        ppuSecondaryOAMAddress++;
                        break;
                    case 2:
                        ppu_SpriteAttribute[ppuSecondaryOAMAddress >> 2] = SecondaryOAM[ppuSecondaryOAMAddress];
                        ppuSecondaryOAMAddress++;
                        break;
                    case 3:
                        ppu_SpriteXposition[ppuSecondaryOAMAddress >> 2] = SecondaryOAM[ppuSecondaryOAMAddress];
                        break;
                    case 4:
                        // Double check sync is correct, I have dobuts about my ability to do that
                        PPUTargetAddress = FindSpritePatternAddress((byte)(ppuSecondaryOAMAddress >> 2));
                        break;
                    case 5:
                        ppuSpriteEvalTemp = DataBus;
                        if (yRender == 261)
                        {
                            ppuSpriteEvalTemp = 0;
                            // Cleared on the pre-render line
                        }
                        if (((ppu_SpriteAttribute[ppuSecondaryOAMAddress >> 2] >> 6) & 1) == 1)
                        {
                            // If the attributes are set to flip X, the order of bits is flipped
                            ppuSpriteEvalTemp = (byte)(((ppuSpriteEvalTemp & 0xF0) >> 4) | ((ppuSpriteEvalTemp & 0xF) << 4));
                            ppuSpriteEvalTemp = (byte)(((ppuSpriteEvalTemp & 0xCC) >> 2) | ((ppuSpriteEvalTemp & 0x33) << 2));
                            ppuSpriteEvalTemp = (byte)(((ppuSpriteEvalTemp & 0xAA) >> 1) | ((ppuSpriteEvalTemp & 0x55) << 1));
                        }
                        ppu_SpriteShiftRegisterL[ppuSecondaryOAMAddress >> 2] = ppuSpriteEvalTemp;
                        break;
                    case 6:
                        PPUTargetAddress += 8;
                        break;
                    case 7:
                        ppuSpriteEvalTemp = DataBus;
                        if (yRender == 261)
                        {
                            ppuSpriteEvalTemp = 0;
                            // Cleared on the pre-render line
                        }
                        if (((ppu_SpriteAttribute[ppuSecondaryOAMAddress >> 2] >> 6) & 1) == 1)
                        {
                            // If the attributes are set to flip X, the order of bits is flipped
                            ppuSpriteEvalTemp = (byte)(((ppuSpriteEvalTemp & 0xF0) >> 4) | ((ppuSpriteEvalTemp & 0xF) << 4));
                            ppuSpriteEvalTemp = (byte)(((ppuSpriteEvalTemp & 0xCC) >> 2) | ((ppuSpriteEvalTemp & 0x33) << 2));
                            ppuSpriteEvalTemp = (byte)(((ppuSpriteEvalTemp & 0xAA) >> 1) | ((ppuSpriteEvalTemp & 0x55) << 1));
                        }
                        ppu_SpriteShiftRegisterH[ppuSecondaryOAMAddress >> 2] = ppuSpriteEvalTemp;
                        ppuSecondaryOAMAddress++;
                        break;
                }

                ppuSpriteEvalTick++;
                ppuSpriteEvalTick &= 7;
                // The address bus should be checked outside this routine
            }
        }

        // I haven't changed this code in ages, but that's because it works
        ushort FindSpritePatternAddress(byte SecondaryOAMSlot)
        {
            if (!ppuUse8x16Sprites) // 8x8
            {
                // Address is $0000 or $1000, depends on pattern table
                // Then, add pattern value from OAM, shifted by 4 bits (x16)
                // Then, add scanlines from top of object
                if (((ppu_SpriteAttribute[SecondaryOAMSlot] >> 7) & 1) == 0) // Don't flip Y
                {
                    return (ushort)((ppuSpritePatternTable ? 0x1000 : 0) + (ppu_SpritePattern[SecondaryOAMSlot] << 4) + (yRender - ppu_SpriteYposition[SecondaryOAMSlot]));
                }
                else // Flip Y
                {
                    return (ushort)((ppuSpritePatternTable ? 0x1000 : 0) + (ppu_SpritePattern[SecondaryOAMSlot] << 4) + ((7 - (yRender - ppu_SpriteYposition[SecondaryOAMSlot])) & 7));
                }
            }
            else // 8x16
            {
                // If bottom half is being drawn, add 16
                if (((ppu_SpriteAttribute[SecondaryOAMSlot] >> 7) & 1) == 0) // Don't flip Y
                {
                    if (yRender - ppu_SpriteYposition[SecondaryOAMSlot] < 8)
                    {
                        return (ushort)((((ppu_SpritePattern[SecondaryOAMSlot] & 1) == 1) ? 0x1000 : 0) | ((ppu_SpritePattern[SecondaryOAMSlot] & 0xFE) << 4) + (yRender - ppu_SpriteYposition[SecondaryOAMSlot]));
                    }
                    else
                    {
                        return (ushort)((((ppu_SpritePattern[SecondaryOAMSlot] & 1) == 1) ? 0x1000 : 0) | (((ppu_SpritePattern[SecondaryOAMSlot] & 0xFE) << 4) + 16) + ((yRender - ppu_SpriteYposition[SecondaryOAMSlot]) & 7));
                    }
                }
                else // Flip Y
                {
                    if (yRender - ppu_SpriteYposition[SecondaryOAMSlot] < 8)
                    {
                        return (ushort)((((ppu_SpritePattern[SecondaryOAMSlot] & 1) == 1) ? 0x1000 : 0) | (((ppu_SpritePattern[SecondaryOAMSlot] & 0xFE) << 4) + 16) + ((yRender - ppu_SpriteYposition[SecondaryOAMSlot]) & 7) + 7);
                    }
                    else
                    {
                        return (ushort)((((ppu_SpritePattern[SecondaryOAMSlot] & 1) == 1) ? 0x1000 : 0) | (((ppu_SpritePattern[SecondaryOAMSlot] & 0xFE) << 4) + 7) + ((yRender - ppu_SpriteYposition[SecondaryOAMSlot]) & 7));
                    }
                }
            }
        }

        void PPU_IncrementScrollY()
        {
            if ((ppu_v & 0x7000) != 0x7000)
            {
                ppu_v += 0x1000;
            }
            else
            {
                ppu_v &= 0x0FFF;
                int y = (ppu_v & 0x03E0) >> 5;

                if (y == 29)
                {
                    y = 0;
                    ppu_v ^= 0x0800;
                }
                else
                {
                    y++;
                    y &= 0x1F;
                }
                ppu_v = (ushort)((ppu_v & 0xFC1F) | (y << 5));
            }
        }

        void PPU_ResetXScroll()
        {
            ppu_v = (ushort)((ppu_v & 0x7BE0) | (ppu_t & 0x041F));
        }

        void PPU_ResetYScroll()
        {
            ppu_v = (ushort)((ppu_v & 0x041F) | (ppu_t & 0x7BE0));
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

            // As the code handling things is different and adjusts at the start to properly calculate some values,
            // I set the values to the max to zero them out when it starts.
            xRender = 341;
            yRender = 261;

            ppuDecayTime = Properties.Settings.Default.NESPPUDECAY;
        }

        byte Read(ushort Address)
        {
            if (Address < 0x2000)
            {
                // Read from pattern table
                return CHRData[Address];
            }
            else if (Address < 0x3F00)
            {
                if (Address <= 0x37FF)
                {
                    // Read from nametables
                    if (MirrorMode)
                    {
                        // Horizontal mirror
                        return VRAM[(Address & 0x3FF) | ((Address & 0x800) >> 1)];
                    }
                    else
                    {
                        // Vertical mirror
                        return VRAM[Address & 0x7FF];
                    }
                }
                else
                {
                    // PPU Open Bus
                    return DataBus;
                }
            }
            else
            {
                // Read palette RAM
                byte PalResult;
                if ((Address & 3) == 0)
                {
                    PalResult = PaletteRAM[Address & 0x0F];
                }
                else
                {
                    PalResult = PaletteRAM[Address & 0x1F];
                }
                if (ppuMask_GreyscaleMode)
                {
                    // Remove lower nybble for greyscale mode
                    PalResult &= 0x30;
                }
                return PalResult;
            }
        }

        public byte CPU_Read_PPU(ushort Address)
        {
            Address &= 0x2007;
            switch (Address)
            {
                case 0x2007:
                    if (ppu_v >= 0x3F00)
                    {
                        // Palette RAM has no buffer
                        byte TempPPURead = Read(ppu_v);
                        TempPPURead &= 0x3F;
                        TempPPURead |= (byte)(PPUIOBus & 0xC0);
                        PPUIOBus = TempPPURead;

                        // Other than this quirk.
                        PPUReadBuffer = Read((ushort)(ppu_v & 0x2FFF));
                    }
                    else
                    {
                        // Buffer data (VRAM)
                        PPUIOBus = PPUReadBuffer;
                        PPUReadBuffer = Read(ppu_v);
                    }

                    // Increment v
                    if ((ppuMask_RenderBG || ppuMask_RenderSprites) & !inVBlank)
                    {
                        PPU_IncrementScrollY();
                    }
                    else
                    {
                        ppu_v += (ushort)(ppuVRAMInc32Mode ? 32 : 1);
                        ppu_v &= 0x3FFF;
                    }

                    break;
                case 0x2002:
                    // Suppress VBLANK or NMI if possible
                    /*if (ppuCanSuppressVBLANK)
                    {
                        ppuVBlankSuppressed = true;
                    }
                    if (ppuCanSuppressNMI)
                    {
                        ppuNMISuppressed = true;
                    }*/

                    // Get the PPU flags and return them
                    byte ppustatus = 0;
                    ppustatus |= (byte)(VBlank ? 0x80 : 0);
                    ppustatus |= (byte)(ppuStatusSprZeroHit ? 0x40 : 0);
                    ppustatus |= (byte)(ppuStatusOverflow ? 0x20 : 0);

                    // Reset flags
                    //ppuStatusSprZeroHit = false;
                    //ppuStatusOverflow = false;
                    VBlank = false;
                    ppu_w = false;

                    // Other bits are PPU open bus
                    PPUIOBus = (byte)((PPUIOBus & 0b00011111) | ppustatus);
                    break;
                case 0x2004:
                    // Read from OAM
                    PPUIOBus = OAM[ppuOAMAddress];

                    // Attributes follow the pattern 4n+2
                    if (((ppuOAMAddress - 2) & 0b11) == 0)
                    {
                        // And are missing some of their bits
                        PPUIOBus &= 0b11100011;
                    }
                    /*if (ppuScanLine < 240 && ppuDot > 0 && ppuDot <= 64 & (ppuMask_RenderSprites || ppuMask_RenderBG))
                    {
                        PPUIOBus = 0xFF;
                    }*/
                    break;
                default:
                    // Stuff I haven't implemented
                    // Console.WriteLine($"Unknown PPU read - {Address:X}");
                    break;
            }
            lastPPUIOUpdate = 0;
            return PPUIOBus;
        }

        public void CPU_Write_PPU(ushort Address, byte Value)
        {
            Address &= 0x2007; // Mirroring
            PPUIOBus = Value;
            lastPPUIOUpdate = 0;
            switch (Address)
            {
                case 0x2000: // PPUCTRL
                             // ppuNametableSelect =    Value & 3;
                    ppu_t = (ushort)((ppu_t & 0b1111001111111111) | ((Value & 3) << 10));
                    ppuVRAMInc32Mode = (Value & 4) != 0;
                    ppuSpritePatternTable = (Value & 8) != 0;
                    ppuBGPatternTable = (Value & 0x10) != 0;
                    ppuUse8x16Sprites = (Value & 0x20) != 0;
                    ppuEnableNMI = (Value & 0x80) != 0;
                    break;
                case 0x2001: // PPUMASK
                    ppuMask_GreyscaleMode = (Value & 1) != 0;
                    ppuMask_8pxMaskBG = (Value & 2) != 0;
                    ppuMask_8pxMaskSprites = (Value & 4) != 0;
                    ppuMask_RenderBG = (Value & 8) != 0;
                    ppuMask_RenderSprites = (Value & 0x10) != 0;
                    break;
                case 0x2002: // PPUSTATUS
                             //Console.WriteLine("PPUSTATUS not implemented");
                             // I don't think writing here does anything
                    break;
                case 0x2003: // OAMADDR
                             //Console.WriteLine("OAMADDR not implemented");
                             // Sets the OAM address, simple!
                    ppuOAMAddress = Value;
                    break;
                case 0x2004: // OAMDATA
                             // Gets the OAM value and increments the address
                    OAM[ppuOAMAddress] = Value;
                    ppuOAMAddress += 1;
                    break;
                case 0x2005: // PPUSCROLL
                    if (!ppu_w)
                    {
                        // Set the x scroll
                        ppu_x = (byte)(Value & 7);
                        TempVRAMAddress = (ushort)((TempVRAMAddress & 0x7FE0) | (Value >> 3));
                    }
                    else
                    {
                        ppu_t = (ushort)((TempVRAMAddress & 0x0C1F) | ((Value & 0xF8) << 2) | ((Value & 7) << 12));
                    }
                    ppu_w = !ppu_w;
                    break;
                case 0x2006: // PPUADDR
                    if (!ppu_w)
                    {
                        // First write sets high byte
                        TempVRAMAddress = (ushort)((Value & 0x3F) << 8);
                    }
                    else
                    {
                        // Then second sets the low
                        ppu_v = (ushort)(TempVRAMAddress | Value);
                        ppu_t = ppu_v;
                    }
                    ppu_w = !ppu_w;
                    break;
                case 0x2007: // PPUDATA
                    if (ppu_v < 0x2000)
                    {
                        // Write to pattern table if supported by the cartridge (CHRROM vs CHRData)
                        if (IsCHRData)
                        {
                            CHRData[ppu_v] = Value;
                        }
                        // Else, it's read only, and nothing happens.
                    }
                    else if (ppu_v < 0x3F00)
                    {
                        // Write to nametables
                        if (MirrorMode)
                        {
                            // Horizontal mirror
                            VRAM[(ppu_v & 0x3FF) | ((ppu_v & 0x800) >> 1)] = Value;
                        }
                        else
                        {
                            // Vertical mirror
                            VRAM[ppu_v & 0x7FF] = Value;
                        }
                    }
                    else
                    {
                        // Write to palette RAM
                        if ((ppu_v & 3) == 0)
                        {
                            PaletteRAM[ppu_v & 0x0F] = Value;
                        }
                        else
                        {
                            PaletteRAM[ppu_v & 0x1F] = Value;
                        }
                    }

                    ppu_v += (ushort)(ppuVRAMInc32Mode ? 32 : 1);
                    ppu_v &= 0x3FFF;
                    break;
            }
        }
        public bool CheckNMIConditions()
        {
            return ppuEnableNMI && VBlank;
        }
    }
}
