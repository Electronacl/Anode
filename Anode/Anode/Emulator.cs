using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace Anode
{
    internal class Emulator
    {
        // PPU code from 100th coin's tutorial, I will refine it for accuuracy later but I need to just get this working atm

        // ----- CPU Regisers
        ushort ProgramCounter;
        byte X;
        byte Y;
        byte A; // Accumulator
        byte SP; // Stack pointer
        byte t;
        byte opcode;
        byte ADD;

        bool flag_Carry;
        bool flag_Zero;
        bool flag_InterruptDisable;
        bool flag_Decimal;
        bool flag_Overflow;
        bool flag_Negative;

        ushort AddressBus;
        byte DataBus;

        // ----- PPU Registers and render info
        bool ppu_w; // Write Latch
        ushort ppu_t; // Transfer Address
        ushort ppu_v; // VRAM Address
        byte ppu_x; // PPU X scroll

        byte ppuDataBus;
        ushort ppuAddressBus;

        ushort ppuShiftRegister_patternL;
        ushort ppuShiftRegister_patternH;
        ushort ppuShiftRegister_attributeL;
        ushort ppuShiftRegister_attributeH;

        byte ppu8Step_patternLowBitPlane;
        byte ppu8Step_patternHighBitPlane;
        byte ppu8Step_attribute;
        byte ppu8Step_temp;
        byte ppu8Step_NextCharacter;

        /*byte[] ppu_SpriteShiftRegisterL = new byte[8];
        byte[] ppu_SpriteShiftRegisterH = new byte[8];

        byte[] ppu_SpriteAttribute = new byte[8];
        byte[] ppu_SpritePattern = new byte[8];
        byte[] ppu_SpriteXposition = new byte[8];
        byte[] ppu_SpriteYposition = new byte[8];*/

        int ppuDot;
        int ppuScanLine;
        bool ppuVBlank;

        ushort TempVRAMAddress;
        byte PPUReadBuffer;

        // ----- PPU Flags
        int ppuNametableSelect;

        bool ppuVRAMInc32Mode;
        bool ppuSpritePatternTable;
        bool ppuBGPatternTable;
        bool ppuUse8x16Sprites;
        bool ppuEnableNMI;

        bool ppuMask_8pxMaskBG;
        bool ppuMask_8pxMaskSprites;
        bool ppuMask_RenderBG;
        bool ppuMask_RenderSprites;

        bool ppuStatusOverflow;
        bool ppuStatusSprZeroHit;

        // ----- NMI
        bool NMILevelDetector;
        bool DoNMI;

        // ----- Clock
        byte Master_Clock = 1;

        // ----- Storage
        public byte[] RAM = new byte[0x800];
        byte[] ROM = new byte[0x8000];
        byte[] CHRData = new byte[0x2000];
        byte[] Header = new byte[0x10];
        byte[] VRAM = new byte[0x800];
        byte[] PaletteRAM = new byte[32];
        byte[] OAM = new byte[0x100]; // Object Attribute Memory
        byte[] SecondaryOAM = new byte[0x20];

        // ----- Emulator specific
        // Timing
        byte op_t;
        bool inc_op_t;
        int signedTemp;

        public bool CPU_Halted;

        // Opcode splits
        byte op_a;
        byte op_b;
        byte op_c;

        // Accumulator, badly named but resharper brokey
        bool a_indexed = false;

        // Paths and logging
        public string filepath;
        public string tracepath;
        public bool logging;

        string this_trace = "";
        StreamWriter tracelog;

        // PPU output data
        public Bitmap output;
        bool NTSC = true; // PAL or NTSC?
        public bool frame_Ready = false;


        // Tracelogger code from 100th Coin's tutorial, I'll make my own one in the future.
        static String[] OpCodeNames =
        {
            "BRK", "ORA", "HLT", "SLO", "NOP", "ORA", "ASL", "SLO", "PHP", "ORA", "ASL", "ANC", "NOP", "ORA", "ASL", "SLO",
            "BPL", "ORA", "HLT", "SLO", "NOP", "ORA", "ASL", "SLO", "CLC", "ORA", "NOP", "SLO", "NOP", "ORA", "ASL", "SLO",
            "JSR", "AND", "HLT", "RLA", "BIT", "AND", "ROL", "RLA", "PLP", "AND", "ROL", "ANC", "BIT", "AND", "ROL", "RLA",
            "BMI", "AND", "HLT", "RLA", "NOP", "AND", "ROL", "RLA", "SEC", "AND", "NOP", "RLA", "NOP", "AND", "ROL", "RLA",
            "RTI", "EOR", "HLT", "SRE", "NOP", "EOR", "LSR", "SRE", "PHA", "EOR", "LSR", "ALR", "JMP", "EOR", "LSR", "SRE",
            "BVC", "EOR", "HLT", "SRE", "NOP", "EOR", "LSR", "SRE", "CLI", "EOR", "NOP", "SRE", "NOP", "EOR", "LSR", "SRE",
            "RTS", "ADC", "HLT", "RRA", "NOP", "ADC", "ROR", "RRA", "PLA", "ADC", "ROR", "ARR", "JMP", "ADC", "ROR", "RRA",
            "BVS", "ADC", "HLT", "RRA", "NOP", "ADC", "ROR", "RRA", "SEI", "ADC", "NOP", "RRA", "NOP", "ADC", "ROR", "RRA",
            "NOP", "STA", "HLT", "SAX", "STY", "STA", "STX", "SAX", "DEY", "NOP", "TXA", "ANE", "STY", "STA", "STX", "SAX",
            "BCC", "STA", "HLT", "SHA", "STY", "STA", "STX", "SAX", "TYA", "STA", "TXS", "SHS", "SHY", "STA", "SHX", "SHA",
            "LDY", "LDA", "LDX", "LAX", "LDY", "LDA", "LDX", "LAX", "TAY", "LDA", "TAX", "LXA", "LDY", "LDA", "LDX", "LAX",
            "BCS", "LDA", "HLT", "LAX", "LDY", "LDA", "LDX", "LAX", "CLV", "LDA", "TSX", "LAE", "LDY", "LDA", "LDX", "LAX",
            "CPY", "CMP", "NOP", "DCP", "CPY", "CMP", "DEC", "DCP", "INY", "CMP", "DEX", "AXS", "CPY", "CMP", "DEC", "DCP",
            "BNE", "CMP", "HLT", "DCP", "NOP", "CMP", "DEC", "DPC", "CLD", "CMP", "NOP", "DCP", "NOP", "CMP", "DEC", "DCP",
            "CPX", "SBC", "NOP", "ISC", "CPX", "SBC", "INC", "ISC", "INX", "SBC", "NOP", "SBC", "CPX", "SBC", "INC", "ISC",
            "BEQ", "SBC", "HLT", "ISC", "NOP", "SBC", "INC", "ISC", "SED", "SBC", "NOP", "ISC", "NOP", "SBC", "INC", "ISC",
        };

        // PPU Palette, from 100th Coin's tutorial. I will probably keep this as the NTSC variant, but it'll be temporary for PAL
        // as I'll use my own if I figure out how to.
        byte[] Pal = {
            0x65, 0x65, 0x65, 0x00, 0x2A, 0x84, 0x15, 0x13, 0xA2, 0x3A, 0x01, 0x9E, 0x59, 0x00, 0x7A, 0x6A, 0x00, 0x3E, 0x68, 0x08, 0x00, 0x53, 0x1D, 0x00, 0x32, 0x34, 0x00, 0x0D, 0x46, 0x00, 0x00, 0x4F, 0x00, 0x00, 0x4C, 0x09, 0x00, 0x3F, 0x4B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0xAE, 0xAE, 0xAE, 0x17, 0x5F, 0xD6, 0x43, 0x41, 0xFF, 0x75, 0x29, 0xFA, 0x9E, 0x1D, 0xCA, 0xB4, 0x20, 0x7B, 0xB1, 0x33, 0x22, 0x96, 0x4E, 0x00, 0x6A, 0x6C, 0x00, 0x39, 0x84, 0x00, 0x0F, 0x90, 0x00, 0x00, 0x8D, 0x33, 0x00, 0x7B, 0x8C, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0xFE, 0xFE, 0xFE, 0x66, 0xAF, 0xFF, 0x93, 0x90, 0xFF, 0xC5, 0x78, 0xFF, 0xEE, 0x6C, 0xFF, 0xFF, 0x6F, 0xCA, 0xFF, 0x82, 0x71, 0xE6, 0x9E, 0x25, 0xBA, 0xBC, 0x00, 0x88, 0xD5, 0x01, 0x5E, 0xE1, 0x32, 0x47, 0xDD, 0x82, 0x4A, 0xCB, 0xDC, 0x4E, 0x4E, 0x4E, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00,
            0xFE, 0xFE, 0xFE, 0xC0, 0xDE, 0xFF, 0xD2, 0xD1, 0xFF, 0xE7, 0xC7, 0xFF, 0xF8, 0xC2, 0xFF, 0xFF, 0xC3, 0xE9, 0xFF, 0xCB, 0xC4, 0xF5, 0xD7, 0xA5, 0xE2, 0xE3, 0x94, 0xCE, 0xED, 0x96, 0xBC, 0xF2, 0xAA, 0xB3, 0xF1, 0xCB, 0xB4, 0xE9, 0xF0, 0xB6, 0xB6, 0xB6, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00
        };

        Color[] Palette = new Color[64];
        int pal_i = 0;

        void Tracelogger(byte opcode)
        {
            if (logging)
            {
                String line = "$" + ProgramCounter.ToString("X4")
                    + "\t" + opcode.ToString("X2")
                    + "\t" + OpCodeNames[opcode]
                    + "\t\tA: " + A.ToString("X2")
                    + "\tX: " + X.ToString("X2")
                    + "\tY: " + Y.ToString("X2")
                    + "\tSP: " + SP.ToString("X2")
                    + "\tProcessor Flags: "
                    + (flag_Negative ? "N" : "n")
                    + (flag_Overflow ? "V" : "v")
                    + "--"
                    + (flag_Decimal ? "D" : "d")
                    + (flag_InterruptDisable ? "I" : "i")
                    + (flag_Zero ? "Z" : "z")
                    + (flag_Carry ? "C" : "c");
                //+ "\tCycle: " + cycle.ToString();
                tracelog.WriteLine(line);
            }
        }

        public void Reset()
        {
            byte[] HeaderedROM = File.ReadAllBytes(filepath);
            Array.Copy(HeaderedROM, Header, 0x10);
            byte size = Header[4];
            Array.Copy(HeaderedROM, 0x10, ROM, 0, 0x4000 * size);

            if (Header[5] != 0)
            {
                Array.Copy(HeaderedROM, 0x4000 * size + 0x10, CHRData, 0, 0x2000); // Load graphics pattern data
            }

            byte PC_Lo = Read_Raw(0xFFFC);
            byte PC_Hi = Read_Raw(0xFFFD);
            ProgramCounter = (ushort)((PC_Hi * 0x100) + PC_Lo);

            SP = 0xFD;
            flag_InterruptDisable = true;

            if (logging)
            {
                tracelog = new StreamWriter(tracepath);
            }

            if (NTSC)
            {
                output = new Bitmap(32 * 8, 30 * 8);
            }

            for (int j = 0; j < 64; j++)
            {
                Palette[j] = Color.FromArgb(Pal[pal_i++], Pal[pal_i++], Pal[pal_i++]);
            }
        }

        public void Run()
        {
            // Clocking
            if (!CPU_Halted)
            {
                if ((Master_Clock - 1) % 4 == 0)
                {
                    Emulate_PPU();
                }

                if (Master_Clock % 12 == 0)
                {
                    Emulate_CPU();
                }

                Master_Clock++;
                if (Master_Clock > 12)
                {
                    Master_Clock = 1;
                }
            }

            if (CPU_Halted)
            {
                Console.WriteLine($"CPU Halted at address {ProgramCounter:X}!");
                //MessageBox.Show($"Encountered a halt instruction (opcode {opcode:X}) at {ProgramCounter:X}",
                //    "NES Error: Halted", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                if (logging)
                {
                    tracelog.Close();
                    Console.WriteLine("Tracelog saved!");
                }
                Render();
                // Also make pixel red
            }
        }

        void Render()
        {
            frame_Ready = true;
        }

        byte Read_Raw(ushort Address)
        {
            if (Address < 0x2000)
            {
                return RAM[Address & 0x7FF];
            }
            else if (Address < 0x4000)
            {
                // Read from PPU
                Address &= 0x2007;
                switch (Address)
                {
                    case 0x2007:
                        byte temp = PPUReadBuffer;

                        if (ppu_v > 0x3F00)
                        {
                            temp = ReadPPU(ppu_v);
                        }
                        else
                        {
                            PPUReadBuffer = ReadPPU(ppu_v);
                        }

                        ppu_v += (ushort)(ppuVRAMInc32Mode ? 32 : 1);
                        ppu_v &= 0x3FFF;
                        return temp;
                    case 0x2002:
                        byte ppustatus = 0;
                        ppustatus |= (byte)(ppuVBlank ? 0x80 : 0);
                        /*ppustatus |= (byte)(ppuStatusSprZeroHit ? 0x40 : 0);
                        ppustatus |= (byte)(ppuStatusOverflow ? 0x20 : 0);*/
                        ppustatus |= 0x40;

                        ppuVBlank = false;
                        ppu_w = false;
                        return ppustatus;
                    case 0x2004:
                        return 0;
                    default:
                        Console.WriteLine($"Unknown PPU read - {Address:X}");
                        return 0;
                }
            }
            else if (Address >= 0x8000)
            {
                return ROM[(Address - 0x8000) & ((Header[4] * 0x4000) - 1)];
            }
            return 0;
        }

        void Read()
        {
            DataBus = Read_Raw(AddressBus);
        }

        void Write_Raw(ushort Address, byte Value)
        {
            if (Address < 0x2000)
            {
                RAM[Address & 0x7FF] = Value;
            }
            else if (Address < 0x4000)
            {
                // Write to PPU
                Address &= 0x2007; // Mirroring
                switch (Address)
                {
                    case 0x2000: // PPUCTRL
                        ppuNametableSelect = Value & 3;
                        ppuVRAMInc32Mode = (Value & 4) != 0;
                        ppuSpritePatternTable = (Value & 8) != 0;
                        ppuBGPatternTable = (Value & 0x10) != 0;
                        ppuUse8x16Sprites = (Value & 0x20) != 0;
                        ppuEnableNMI = (Value & 0x80) != 0;
                        break;
                    case 0x2001: // PPUMASK
                        ppuMask_8pxMaskBG = (Value & 2) != 0;
                        ppuMask_8pxMaskSprites = (Value & 4) != 0;
                        ppuMask_RenderBG = (Value & 8) != 0;
                        ppuMask_RenderSprites = (Value & 0x10) != 0;
                        break;
                    case 0x2002: // PPUSTATUS
                        Console.WriteLine("PPUSTATUS not implemented");
                        break;
                    case 0x2003: // OAMADDR
                        Console.WriteLine("OAMADDR not implemented");
                        // ppuOAMAddress = Value;
                        break;
                    case 0x2004: // OAMDATA
                        Console.WriteLine("OAMDATA not implemented");
                        break;
                    case 0x2005: // PPUSCROLL
                        if (!ppu_w)
                        {
                            ppu_x = (byte)(Value & 7);
                            TempVRAMAddress = (ushort)((TempVRAMAddress & 0b0111111111100000) | (Value >> 3));
                        }
                        else
                        {
                            ppu_t = (ushort)((TempVRAMAddress & 0b0000110000011111) | (((Value & 0xF8) << 2) | ((Value & 7) << 12)));
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
                            // Write to pattern table if supported by the cartridge
                            if (Header[5] == 0)
                            {
                                CHRData[ppu_v] = Value;
                            }
                            // Else, it's read only, and nothing happens.
                        }
                        else if (ppu_v < 0x3F00)
                        {
                            // Write to nametables
                            if ((Header[6] & 1) == 0)
                            {
                                // Horizontal mirror
                                VRAM[(ppu_v & 0x3FF) | (ppu_v & 0x800) >> 1] = Value;
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
        }

        void Write()
        {
            Write_Raw(AddressBus, DataBus);
        }

        byte ReadPPU(ushort Address)
        {
            if (Address < 0x2000)
            {
                // Read from pattern table
                return CHRData[Address];
            }
            else if (Address < 0x3F00)
            {
                // Read from nametables
                if ((Header[6] & 1) == 0)
                {
                    // Horizontal mirror
                    return VRAM[(Address & 0x3FF) | (Address & 0x800) >> 1];
                }
                else
                {
                    // Vertical mirror
                    return VRAM[Address & 0x7FF];
                }
            }
            else
            {
                // Read palette RAM
                if ((Address & 3) == 0)
                {
                    return PaletteRAM[Address & 0x0F];
                }
                else
                {
                    return PaletteRAM[Address & 0x1F];
                }
            }
        }

        void Push()
        {
            AddressBus = (ushort)(0x100 + SP);
            Write();
            SP--;
        }

        void Pull()
        {
            SP++;
            AddressBus = (ushort)(0x100 + SP);
            Read();
        }

        void Read_Operand()
        {
            if (op_b == 2 || op_b == 0)
            {
                if (op_c == 2 && op_a < 4)
                {
                    CPU_Halted = true;
                    Console.WriteLine($"Halt instruction: {opcode:X} ({op_a}, {op_b}, {op_c})");
                }
                else if (op_c == 1 && op_b == 0)
                {
                    // X, Indirect
                    switch (t)
                    {
                        case 1:
                            Read();
                            ProgramCounter++;
                            AddressBus = DataBus;
                            break;
                        case 2:
                            Read(); // Dummy Read
                            AddressBus = (byte)(AddressBus + X);
                            break;
                        case 3:
                            Read();
                            ADD = DataBus;
                            AddressBus = (byte)(AddressBus + 1);
                            break;
                        case 4:
                            Read();
                            AddressBus = (ushort)((DataBus << 8) | ADD);
                            break;
                        case 5:
                            Read();
                            inc_op_t = true;
                            break;
                    }
                }
                else
                {
                    // Immediate
                    Read();
                    ProgramCounter++;
                    inc_op_t = true;
                }
            }
            else if (op_b == 1)
            {
                // Zero page
                switch (t)
                {
                    case 1:
                        Read();
                        ProgramCounter++;
                        break;
                    case 2:
                        AddressBus = DataBus;
                        Read();
                        inc_op_t = true;
                        break;
                }
            }
            else if (op_b == 3)
            {
                // Absolute
                switch (t)
                {
                    case 1:
                        // Hi
                        Read();
                        ProgramCounter++;
                        AddressBus++;
                        break;
                    case 2:
                        // Lo
                        ADD = DataBus;
                        Read();
                        ProgramCounter++;
                        break;
                    case 3:
                        AddressBus = (ushort)((DataBus << 8) | ADD);
                        Read();
                        inc_op_t = true;
                        break;
                }
            }
            else if (op_b == 4)
            {
                if (op_c == 2)
                {
                    // HLT
                    CPU_Halted = true;
                    Console.WriteLine($"Halt instruction: {opcode:X} ({op_a}, {op_b}, {op_c})");
                }
                else
                {
                    // Indirect, Y
                    switch (t)
                    {
                        case 1:
                            Read();
                            ProgramCounter++;
                            AddressBus = DataBus;
                            break;
                        case 2:
                            Read();
                            AddressBus = (byte)(AddressBus + 1);
                            ADD = DataBus;
                            break;
                        case 3:
                            Read();
                            AddressBus = (ushort)((DataBus << 8) | ADD);
                            break;
                        case 4:
                            ushort AddressTemp = (ushort)(AddressBus + Y);
                            AddressBus = (ushort)((AddressBus & 0xFF) | (byte)AddressTemp);
                            if (AddressTemp != AddressBus)
                            {
                                signedTemp = (AddressTemp - AddressBus);
                            }
                            else
                            {
                                inc_op_t = true;
                                t = 5;
                            }
                            Read();
                            break;
                        case 5:
                            AddressBus = (ushort)(AddressBus + signedTemp);
                            Read();
                            inc_op_t = true;
                            break;
                    }
                }
            }
            else if (op_b == 5)
            {
                switch (t)
                {
                    case 1:
                        Read();
                        ProgramCounter++;
                        break;
                    case 2:
                        AddressBus = DataBus;
                        Read();
                        if (op_c < 3 || !(op_a == 4 || op_a == 5))
                        {
                            // Zero Page, X
                            AddressBus = (ushort)((AddressBus & 0xFF00) | (byte)(AddressBus + X));
                        }
                        else
                        {
                            // Zero Page, Y
                            AddressBus = (ushort)((AddressBus & 0xFF00) | (byte)(AddressBus + Y));
                        }
                        break;
                    case 3:
                        Read();
                        inc_op_t = true;
                        break;
                }
                
            }
            else if (op_b == 6 || op_b == 7) // No.
            {
                switch (t)
                {
                    case 1:
                        // Hi
                        Read();
                        ProgramCounter++;
                        AddressBus++;
                        break;
                    case 2:
                        // Lo
                        ADD = DataBus;
                        Read();
                        ProgramCounter++;
                        break;
                    case 3:
                        AddressBus = (ushort)((DataBus << 8) | ADD);
                        ushort AddressTemp;
                        if ((op_c < 2 || !(op_a == 4 || op_a == 5)) && op_b == 7)
                        {
                            // Absolute, X
                            AddressTemp = (ushort)(AddressBus + X);
                        }
                        else
                        {
                            // Absolute, Y
                            AddressTemp = (ushort)(AddressBus + Y);
                        }
                        AddressBus = (ushort)((AddressBus & 0xFF) | (byte)AddressTemp);
                        if (AddressTemp != AddressBus)
                        {
                            signedTemp = (AddressTemp - AddressBus);
                        }
                        else
                        {
                            inc_op_t = true;
                            t = 4;
                        }
                        Read();
                        break;
                    case 4:
                        AddressBus = (ushort)(AddressBus + signedTemp);
                        Read();
                        inc_op_t = true;
                        break;
                }
            }
        }

        void RMW_Instr()
        {
            
            if (op_c != 1 && op_b == 2 & t == 1)
            {
                // Accumulator instruction
                a_indexed = true;
                DataBus = A;
                inc_op_t = true;
                op_t = 1;
            }
            else if (t == 1)
            {
                a_indexed = false;
            }

            if (!inc_op_t)
            {
                Read_Operand();
            }
            else
            {
                if (op_t == 1)
                {
                    if (!a_indexed)
                    {
                        Write();
                    }
                    else
                    {
                        A = DataBus;
                    }
                    switch (op_a)
                    {
                        case 0:
                            // ASL
                            flag_Carry = DataBus > 127;
                            DataBus <<= 1;
                            flag_Zero = DataBus == 0;
                            flag_Negative = DataBus > 127;
                            break;
                        case 1:
                            // ROL
                            bool Futureflag_Carry = DataBus >= 0x80;
                            DataBus <<= 1;
                            if (flag_Carry)
                            {
                                DataBus |= 1;
                            }
                            flag_Carry = Futureflag_Carry;
                            flag_Negative = DataBus > 127;
                            flag_Zero = DataBus == 0;
                            break;
                        case 2:
                            // LSR
                            flag_Carry = (DataBus & 1) != 0;
                            DataBus >>= 1;
                            flag_Negative = DataBus > 127;
                            flag_Zero = DataBus == 0;
                            break;
                        case 3:
                            // ROR
                            bool FutureFlag_Carry = (DataBus & 1) != 0;
                            DataBus >>= 1;
                            if (flag_Carry)
                            {
                                DataBus |= 0x80;
                            }
                            flag_Carry = FutureFlag_Carry;
                            flag_Negative = DataBus > 127;
                            flag_Zero = DataBus == 0;
                            break;
                        case 6:
                            // DEC
                            DataBus--;
                            flag_Negative = DataBus > 127;
                            flag_Zero = DataBus == 0;
                            break;
                        case 7:
                            // INC
                            DataBus++;
                            flag_Negative = DataBus > 127;
                            flag_Zero = DataBus == 0;
                            break;
                    }
                }
                else if (op_t == 2)
                {
                    if (!a_indexed)
                    {
                        Write();
                    }
                    else
                    {
                        A = DataBus;
                    }
                    t = 255;
                }
            }
        }

        void Store_Instr()
        {
            // Read addresses, this is different
            // A lot of code copied from read code. Replace with subroutines in the future?
            if (!inc_op_t)
            {
                switch (op_b)
                {
                    case 0:
                        // X, Indirect
                        switch (t)
                        {
                            case 1:
                                Read();
                                ProgramCounter++;
                                AddressBus = DataBus;
                                break;
                            case 2:
                                Read(); // Dummy Read
                                AddressBus = (byte)(AddressBus + X);
                                break;
                            case 3:
                                Read();
                                ADD = DataBus;
                                AddressBus = (byte)(AddressBus + 1);
                                break;
                            case 4:
                                Read();
                                AddressBus = (ushort)((DataBus << 8) | ADD);
                                inc_op_t = true;
                                break;
                        }
                        break;
                    case 1:
                        // Zero page
                        Read();
                        ProgramCounter++;
                        AddressBus = DataBus;
                        inc_op_t = true;
                        break;
                    case 3:
                        // Absolute
                        if (t == 1)
                        {
                            // Hi
                            Read();
                            ProgramCounter++;
                            AddressBus++;
                        }
                        else if (t == 2)
                        {
                            // Lo
                            ADD = DataBus;
                            Read();
                            ProgramCounter++;
                            AddressBus = (ushort)((DataBus << 8) | ADD);
                            inc_op_t = true;
                        }
                        break;
                    case 4:
                        // Indirect, Y
                        switch (t)
                        {
                            case 1:
                                Read();
                                ProgramCounter++;
                                AddressBus = DataBus;
                                break;
                            case 2:
                                Read();
                                AddressBus = (byte)(AddressBus + 1);
                                ADD = DataBus;
                                break;
                            case 3:
                                Read();
                                AddressBus = (ushort)((DataBus << 8) | ADD);
                                break;
                            case 4:
                                ushort AddressTemp = (ushort)(AddressBus + Y);
                                AddressBus = (ushort)((AddressBus & 0xFF) | (byte)AddressTemp);
                                signedTemp = (AddressTemp - AddressBus);
                                Read();
                                AddressBus = (ushort)(AddressBus + signedTemp); // Shortcut, what could possibly go wrong?
                                inc_op_t = true;
                                break;
                        }
                        break;
                    case 5:
                        switch (t)
                        {
                            case 1:
                                Read();
                                ProgramCounter++;
                                break;
                            case 2:
                                AddressBus = DataBus;
                                Read();
                                if (op_c < 3 || !(op_a == 4 || op_a == 5))
                                {
                                    // Zero Page, X
                                    AddressBus = (ushort)((AddressBus & 0xFF00) | (byte)(AddressBus + X));
                                }
                                else
                                {
                                    // Zero Page, Y
                                    AddressBus = (ushort)((AddressBus & 0xFF00) | (byte)(AddressBus + Y));
                                }
                                inc_op_t = true;
                                break;
                        }
                        break;
                    case 6:
                        switch (t)
                        {
                            case 1:
                                // Hi
                                Read();
                                ProgramCounter++;
                                AddressBus++;
                                break;
                            case 2:
                                // Lo
                                ADD = DataBus;
                                Read();
                                ProgramCounter++;
                                break;
                            case 3:
                                AddressBus = (ushort)((DataBus << 8) | ADD);
                                ushort AddressTemp;
                                if ((op_c < 2 || !(op_a == 4 || op_a == 5)) && op_b == 7)
                                {
                                    // Absolute, X
                                    AddressTemp = (ushort)(AddressBus + X);
                                }
                                else
                                {
                                    // Absolute, Y
                                    AddressTemp = (ushort)(AddressBus + Y);
                                }
                                AddressBus = (ushort)((AddressBus & 0xFF) | (byte)AddressTemp);
                                if (AddressTemp != AddressBus)
                                {
                                    signedTemp = (AddressTemp - AddressBus);
                                }
                                else
                                {
                                    inc_op_t = true;
                                    t = 4;
                                }
                                Read();
                                AddressBus = (ushort)(AddressBus + signedTemp); // Again, shortcut.
                                inc_op_t = true;
                                break;
                        }
                        break;
                }
            }
            else
            {
                if (op_c == 0)
                {
                    // STY
                    DataBus = Y;
                    Write();
                }
                else if (op_c == 1)
                {
                    // STA
                    DataBus = A;
                    Write();
                }
                else if (op_c == 2)
                {
                    // STX
                    DataBus = X;
                    Write();
                }
                t = 255;
            }
        }

        void Branch_Instr()
        {
            switch (t)
            {
                case 1:
                    Read();
                    ProgramCounter++;
                    bool branch_condition = false;
                    switch (op_a)
                    {
                        case 0:
                            // BPL
                            branch_condition = !flag_Negative;
                            break;
                        case 1:
                            // BMI
                            branch_condition = flag_Negative;
                            break;
                        case 2:
                            // BVC
                            branch_condition = !flag_Overflow;
                            break;
                        case 3:
                            // BVS
                            branch_condition = flag_Overflow;
                            break;
                        case 4:
                            // BCC
                            branch_condition = !flag_Carry;
                            break;
                        case 5:
                            // BCS
                            branch_condition = flag_Carry;
                            break;
                        case 6:
                            // BNE
                            branch_condition = !flag_Zero;
                            break;
                        case 7:
                            // BEQ
                            branch_condition = flag_Zero;
                            break;
                    }
                    if (!branch_condition)
                    {
                        t = 255;
                    }
                    break;
                case 2:
                    signedTemp = DataBus;
                    Read();
                    if (signedTemp > 127)
                    {
                        signedTemp -= 256;
                    }

                    ushort BranchTemp = (ushort)(((ProgramCounter + signedTemp) & 0xFF) | (ProgramCounter & 0xFF00));
                    signedTemp = (int)((ProgramCounter + signedTemp) - BranchTemp);

                    ProgramCounter = BranchTemp;
                    AddressBus = ProgramCounter;

                    if (signedTemp == 0)
                    {
                        t = 255;
                    }
                    break;
                case 3:
                    Read();
                    AddressBus = (ushort)(AddressBus + signedTemp);
                    ProgramCounter = AddressBus;
                    t = 255;
                    break;
            }
        }

        void Move_Instr()
        {
            if (op_b == 0)
            {
                switch (op_a)
                {
                    case 0:
                        // BRK
                        switch (t)
                        {
                            case 1:
                                // Dummy read
                                Read();
                                if (!DoNMI)
                                {
                                    ProgramCounter++;
                                }
                                break;
                            case 2:
                                DataBus = (byte)(ProgramCounter >> 8);
                                Push();
                                break;
                            case 3:
                                DataBus = (byte)(ProgramCounter);
                                Push();
                                break;
                            case 4:
                                DataBus = 0;
                                DataBus |= (byte)(flag_Carry ? 1 : 0);
                                DataBus |= (byte)(flag_Zero ? 2 : 0);
                                DataBus |= (byte)(flag_InterruptDisable ? 4 : 0);
                                DataBus |= (byte)(flag_Decimal ? 8 : 0);
                                DataBus += (byte)(DoNMI ? 0 : 0x10);
                                DataBus |= 0x20;
                                DataBus |= (byte)(flag_Overflow ? 0x40 : 0);
                                DataBus |= (byte)(flag_Negative ? 0x80 : 0);
                                Push();
                                break;
                            case 5:
                                // For NMI, FFFA
                                // For RES, FFFC
                                // For BRK, FFFE
                                AddressBus = (ushort)(DoNMI ? 0xFFFA : 0xFFFE);
                                Read();
                                ADD = DataBus;
                                break;
                            case 6:
                                AddressBus++;
                                Read();
                                AddressBus = (ushort)((DataBus << 8) | ADD);
                                ProgramCounter = (ushort)((DataBus << 8) | ADD);
                                DoNMI = false;
                                t = 255;
                                break;
                        }
                        break;
                    case 1:
                        // JSR
                        switch (t)
                        {
                            case 1:
                                Read();
                                ProgramCounter++;
                                ADD = DataBus;
                                break;
                            case 2:
                                AddressBus = (ushort)(0x100 + SP);
                                Read();
                                break;
                            case 3:
                                DataBus = (byte)(ProgramCounter >> 8);
                                Push();
                                break;
                            case 4:
                                DataBus = (byte)(ProgramCounter);
                                Push();
                                break;
                            case 5:
                                AddressBus = ProgramCounter;
                                Read();
                                ProgramCounter = (ushort)((DataBus << 8) | ADD);
                                t = 255;
                                break;
                        }
                        
                        break;
                    case 2:
                        // RTI
                        switch (t)
                        {
                            case 1:
                                Read(); // Dummy read
                                break;
                            case 2:
                                AddressBus = (ushort)(SP + 0x100);
                                Read(); // And another
                                break;
                            case 3:
                                Pull();
                                flag_Carry = (DataBus & 1) != 0;
                                flag_Zero = (DataBus & 2) != 0;
                                flag_InterruptDisable = (DataBus & 4) != 0;
                                flag_Decimal = (DataBus & 8) != 0;
                                flag_Overflow = (DataBus & 0x40) != 0;
                                flag_Negative = (DataBus & 0x80) != 0;
                                break;
                            case 4:
                                Pull();
                                ADD = DataBus;
                                break;
                            case 5:
                                Pull();
                                ProgramCounter = (ushort)((DataBus << 8) | ADD);
                                t = 255;
                                break;
                        }
                        break;
                    case 3:
                        // RTS
                        switch (t)
                        {
                            case 1:
                                Read();
                                break;
                            case 2:
                                AddressBus = (ushort)(SP + 0x100);
                                Read();
                                break;
                            case 3:
                                Pull();
                                ADD = DataBus;
                                break;
                            case 4:
                                Pull();
                                ProgramCounter = (ushort)((DataBus << 8) | ADD);
                                break;
                            case 5:
                                AddressBus = ProgramCounter;
                                Read();
                                ProgramCounter++;
                                t = 255;
                                break;

                        }
                        break;
                }
            }
            else
            {
                // JMP
                if (!inc_op_t)
                {
                    if (op_a == 3)
                    {
                        // Indirect
                        switch (t)
                        {
                            case 1:
                                Read();
                                ADD = DataBus;
                                AddressBus++;
                                break;
                            case 2:
                                Read();
                                AddressBus = (ushort)((DataBus << 8) | ADD);
                                inc_op_t = true;
                                break;
                        }
                    }
                    else
                    {
                        // Absolute
                        inc_op_t = true;
                        op_t = 1;
                    }
                }
                switch (op_t)
                {
                    case 1:
                        Read();
                        ADD = DataBus;
                        AddressBus++;
                        break;
                    case 2:
                        Read();
                        ProgramCounter = (ushort)((DataBus << 8) | ADD);
                        t = 255;
                        break;
                }
            }
        }

        void Stack_Instr()
        {
            if ((op_a & 1) == 0)
            {
                // Push instruction
                switch (t)
                {
                    case 1:
                        // Dummy read
                        Read();
                        break;
                    case 2:
                        if ((op_a & 2) == 0)
                        {
                            // PHP
                            DataBus = 0;
                            DataBus |= (byte)(flag_Carry            ? 1 : 0);
                            DataBus |= (byte)(flag_Zero             ? 2 : 0);
                            DataBus |= (byte)(flag_InterruptDisable ? 4 : 0);
                            DataBus |= (byte)(flag_Decimal          ? 8 : 0);
                            DataBus |=                              0x10;
                            DataBus |=                              0x20;
                            DataBus |= (byte)(flag_Overflow         ? 0x40 : 0);
                            DataBus |= (byte)(flag_Negative         ? 0x80 : 0);
                        }
                        else
                        {
                            // PHA
                            DataBus = A;
                        }
                        t = 255;
                        Push();
                        break;
                }
                
            }
            else
            {
                // Pull instruction
                switch (t)
                {
                    case 1:
                        // Dummy read
                        Read();
                        break;
                    case 2:
                        // Another dummy read
                        AddressBus = (ushort)(SP + 0x100);
                        Read();
                        break;
                    case 3:
                        Pull();
                        if ((op_a & 2) == 0)
                        {
                            // PLP
                            flag_Carry =            (DataBus & 1) != 0;
                            flag_Zero =             (DataBus & 2) != 0;
                            flag_InterruptDisable = (DataBus & 4) != 0;
                            flag_Decimal =          (DataBus & 8) != 0;
                            flag_Overflow =         (DataBus & 0x40) != 0;
                            flag_Negative =         (DataBus & 0x80) != 0;
                        }
                        else
                        {
                            // PLA
                            A = DataBus;
                        }
                        t = 255;
                        break;
                }
            }
        }

        void Single_Byte_Instr()
        {
            Read(); // Dummy Read
            if (op_c == 0)
            {
                if (op_b == 2)
                {
                    switch (op_a)
                    {
                        case 4:
                            // DEY
                            Y--;
                            flag_Zero = Y == 0;
                            flag_Negative = Y > 127;
                            break;
                        case 5:
                            // TAY
                            Y = A;
                            flag_Zero = Y == 0;
                            flag_Negative = Y > 127;
                            break;
                        case 6:
                            // INY
                            Y++;
                            flag_Zero = Y == 0;
                            flag_Negative = Y > 127;
                            break;
                        case 7:
                            // INX
                            X++;
                            flag_Zero = X == 0;
                            flag_Negative = X > 127;
                            break;
                    }
                    
                }
                else
                {
                    switch (op_a)
                    {
                        case 0:
                            // CLC
                            flag_Carry = false;
                            break;
                        case 1:
                            // SEC
                            flag_Carry = true;
                            break;
                        case 2:
                            // CLI
                            flag_InterruptDisable = false;
                            break;
                        case 3:
                            // SEI
                            flag_InterruptDisable = true;
                            break;
                        case 4:
                            // TYA
                            A = Y;
                            flag_Zero = A == 0;
                            flag_Negative = A > 127;
                            break;
                        case 5:
                            // CLV
                            flag_Overflow = false;
                            break;
                        case 6:
                            // CLD
                            flag_Decimal = false;
                            break;
                        case 7:
                            // SED
                            flag_Decimal = true;
                            break;
                    }
                }
            }
            else
            {
                if (op_b == 2)
                {
                    switch (op_a)
                    {
                        case 4:
                            // TXA
                            A = X;
                            flag_Zero = A == 0;
                            flag_Negative = A > 127;
                            break;
                        case 5:
                            // TAX
                            X = A;
                            flag_Zero = X == 0;
                            flag_Negative = X > 127;
                            break;
                        case 6:
                            // DEX
                            X--;
                            flag_Zero = X == 0;
                            flag_Negative = X > 127;
                            break;
                        // 7 is NOP
                    }
                }
                else
                {
                    switch (op_a)
                    {
                        case 4:
                            // TXS
                            SP = X;
                            break;
                        case 5:
                            // TSX
                            X = SP;
                            flag_Zero = X == 0;
                            flag_Negative = X > 127;
                            break;
                        // Others are unofficial NOPs
                    }
                }
            }
            t = 255;
        }

        void Internal_Mem_Instr()
        {
            if (!inc_op_t)
            {
                Read_Operand();
            }

            if(inc_op_t)
            {
                switch (op_a)
                {
                    case 0:
                        // c=0 is NOP, N/A or NOP for c=2
                        if (op_c == 1)
                        {
                            // ORA
                            A |= DataBus;
                            flag_Negative = A > 127;
                            flag_Zero = A == 0;
                        }
                        break;
                    case 1:
                        // N/A or NOP for C=2
                        switch (op_c)
                        {
                            case 0:
                                // BIT
                                flag_Zero = (A & DataBus) == 0;
                                flag_Negative = (DataBus & 0x80) != 0;
                                flag_Overflow = (DataBus & 0x40) != 0;
                                break;
                            case 1:
                                // AND
                                A &= DataBus;
                                flag_Negative = A > 127;
                                flag_Zero = A == 0;
                                break;
                        }
                        break;
                    case 2:
                        // N/A or NOP for C=0 and C=2
                        if (op_c == 1)
                        {
                            // EOR
                            A ^= DataBus;
                            flag_Negative = A > 127;
                            flag_Zero = A == 0;
                        }
                        break;
                    case 3:
                        switch (op_c)
                        {
                            case 1:
                                // ADC
                                int IntSum = DataBus + A + (flag_Carry ? 1 : 0);
                                flag_Overflow = (~(A ^ DataBus) & (A ^ IntSum) & 0x80) != 0;
                                flag_Carry = IntSum > 0xFF;
                                A = (byte)IntSum;
                                flag_Negative = A > 127;
                                flag_Zero = A == 0;
                                break;
                        }
                        break;
                    case 5:
                        // Load instruction
                        if (op_c == 0)
                        {
                            // LDY
                            Y = DataBus;
                        }
                        else if (op_c == 1)
                        {
                            // LDA
                            A = DataBus;
                        }
                        else if (op_c == 2)
                        {
                            // LDX
                            X = DataBus;
                        }
                        flag_Zero = DataBus == 0;
                        flag_Negative = DataBus > 127;
                        break;
                    case 6:
                        switch (op_c)
                        {
                            case 0:
                                // CPY
                                flag_Carry = DataBus >= Y;
                                flag_Zero = DataBus == Y;
                                flag_Negative = (byte)(Y - DataBus) > 127;
                                break;
                            case 1:
                                // CMP
                                flag_Carry = DataBus >= A;
                                flag_Zero = DataBus == A;
                                flag_Negative = (byte)(A - DataBus) > 127;
                                break;
                        }
                        break;
                    case 7:
                        switch (op_c)
                        {
                            case 0:
                                // CPX
                                flag_Carry = DataBus >= X;
                                flag_Zero = DataBus == X;
                                flag_Negative = (byte)(X - DataBus) > 127;
                                break;
                            case 1:
                                // SBC
                                int IntSum = A - DataBus - (flag_Carry ? 0 : 1);
                                flag_Overflow = ((A ^ DataBus) & (A ^ DataBus) & 0x80) != 0;
                                flag_Carry = IntSum > 0;
                                A = (byte)IntSum;
                                flag_Negative = A > 127;
                                flag_Zero = A == 0;
                                break;
                        }
                        break;
                }
                t = 255;
            }
        }

        void General_Instr()
        {
            if ((op_b == 2 || op_b == 6) && (op_c == 0 || (op_c == 2 && op_a >= 4) || op_b == 6))
            {
                // Single byte instructions
                Single_Byte_Instr();
            }
            else
            {
                // Internal memory execution
                Internal_Mem_Instr();
            }
        }

        void Emulate_CPU()
        {
            bool PreviousNMILevelDetector = NMILevelDetector;
            NMILevelDetector = ppuEnableNMI && ppuVBlank;
            if (!PreviousNMILevelDetector && NMILevelDetector)
            {
                DoNMI = true;
            }

            if (t == 0)
            {
                op_t = 0; // 1st instr on 1
                inc_op_t = false;
                // Read next opcode
                AddressBus = ProgramCounter;
                if (!DoNMI)
                {
                    Read();
                    opcode = DataBus;
                    if (logging)
                    {
                        Tracelogger(opcode);
                    }
                    // Increment addresses
                    ProgramCounter++;
                    AddressBus++;
                }
                else
                {
                    opcode = 0x00;
                    if (logging)
                    {
                        Tracelogger(opcode);
                    }
                }
                // Split it up, as this can be used to determine what to do
                op_a = (byte)(opcode >> 5);
                op_b = (byte)((opcode & 0x1C) >> 2);
                op_c = (byte)(opcode & 0x3);
            }
            else
            {
                if ((op_c == 2 && (op_a < 4 || (op_a > 5 && (op_b & 1) == 1))))
                {
                    // RMW instructions
                    RMW_Instr();
                }
                else if (op_a == 4 && ((op_c == 1 && op_b != 2) || (op_b & 1) == 1))
                {
                    // Store instructions
                    Store_Instr();
                }
                else if (op_c == 0)
                {
                    if (op_b == 4)
                    {
                        // Branches
                        Branch_Instr();
                    }
                    else if ((op_b == 0 && op_a < 4) || (op_b == 3 && op_a > 1 && op_a < 4))
                    {
                        // Movement
                        Move_Instr();
                    }
                    else if (op_b == 2 && op_a < 4)
                    {
                        // Stack instructions
                        Stack_Instr();
                    }
                    else
                    {
                        // Single byte or internal execution
                        General_Instr();
                    }
                }
                else
                {
                    // Single byte or internal execution
                    General_Instr();
                }
            }

            // Increment cycle counters
            t++;
            if (inc_op_t) { op_t++; }

            if (t > 20)
            {
                CPU_Halted = true;
                Console.WriteLine($"Opcode ${opcode:X}({op_a:X}, {op_b:X}, {op_c:X}) did not finish; t register exceeded 20.");
                MessageBox.Show($"Opcode ${opcode:X}({op_a:X}, {op_b:X}, {op_c:X}) did not finish; t register exceeded 20. This error should not occur, and should be reported to the developer.", 
                    "CPU Emulation Error: Instruction Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void Emulate_PPU()
        {
            // Again, from my old emulator & the tutorial
            // I need to switch all 0b values for 0x values.

            if (ppuDot == 1 && ppuScanLine == 241)
            {
                Render();
                ppuVBlank = true;
            }
            else if (ppuDot == 1 && ppuScanLine == 261)
            {
                ppuVBlank = false;
                ppuStatusOverflow = false;
                ppuStatusSprZeroHit = false;
            }

            if (ppuScanLine < 240 || ppuScanLine == 261)
            {
                // Visible scanline or pre-render line
                if ((ppuDot > 0 && ppuDot <= 256) || (ppuDot > 320 && ppuDot <= 336))
                {
                    // Visible pixel or preparing next scanline
                    if (ppuMask_RenderBG || ppuMask_RenderSprites)
                    {
                        byte cycleTick;
                        cycleTick = (byte)((ppuDot - 1) & 7);
                        switch (cycleTick)
                        {
                            case 0:
                                ppuShiftRegister_patternL = (ushort)((ppuShiftRegister_patternL & 0xFF00) | ppu8Step_patternLowBitPlane);
                                ppuShiftRegister_patternH = (ushort)((ppuShiftRegister_patternH & 0xFF00) | ppu8Step_patternHighBitPlane);
                                ppuShiftRegister_attributeL = (ushort)((ppuShiftRegister_attributeL & 0xFF00) | ((ppu8Step_attribute & 1) == 1 ? 0xFF : 0));
                                ppuShiftRegister_attributeH = (ushort)((ppuShiftRegister_attributeH & 0xFF00) | ((ppu8Step_attribute & 2) == 2 ? 0xFF : 0));
                                ppuAddressBus = (ushort)(0x2000 + (ppu_v & 0x0FFF));
                                ppu8Step_temp = ReadPPU(ppuAddressBus);
                                break;
                            case 1:
                                ppu8Step_NextCharacter = ppu8Step_temp;
                                break;
                            case 2:
                                ppuAddressBus = (ushort)(0x23C0 | (ppu_v & 0xC00) | ((ppu_v >> 4) & 0x38) | ((ppu_v >> 2) & 0x07));
                                ppu8Step_temp = ReadPPU(ppuAddressBus);
                                break;
                            case 3:
                                ppu8Step_attribute = ppu8Step_temp;
                                // Determine which tile attribute data is for
                                if ((ppu_v & 3) >= 2) // Right tile
                                {
                                    ppu8Step_attribute = (byte)(ppu8Step_attribute >> 2);
                                }
                                if ((((ppu_v & 0b0000001111100000) >> 5) & 3) >= 2) // Bottom tile
                                {
                                    ppu8Step_attribute = (byte)(ppu8Step_attribute >> 4);
                                }
                                ppu8Step_attribute = (byte)(ppu8Step_attribute & 3);
                                break;
                            case 4:
                                ppuAddressBus = (ushort)(((ppu_v & 0b0111000000000000) >> 12) | ppu8Step_NextCharacter * 16 | (ppuBGPatternTable ? 0x1000 : 0));
                                ppu8Step_temp = ReadPPU(ppuAddressBus);
                                break;
                            case 5:
                                ppu8Step_patternLowBitPlane = ppu8Step_temp;
                                ppuAddressBus += 8;
                                break;
                            case 6:
                                ppu8Step_temp = ReadPPU(ppuAddressBus);
                                break;
                            case 7:
                                ppu8Step_patternHighBitPlane = ppu8Step_temp;
                                if ((ppu_v & 0x001F) == 31)
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
                }

                if (ppuMask_RenderBG || ppuMask_RenderSprites)
                {
                    if (ppuScanLine < 240)
                    {
                        if (ppuDot == 256)
                        {
                            PPU_IncrementScrollY();
                        }
                        else if (ppuDot == 257)
                        {
                            PPU_ResetXScroll();
                        }
                    }
                    if (ppuDot >= 280 && ppuDot <= 304 && ppuScanLine == 261)
                    {
                        PPU_ResetYScroll();
                    }
                }
            }

            if (ppuScanLine < 240 && ppuDot > 0 && ppuDot <= 256)
            {
                byte PalHi = 0; // Colour palette
                byte PalLow = 0; // Index in palette
                if (ppuMask_RenderBG && (ppuDot > 8 || ppuMask_8pxMaskBG))
                {
                    byte col0 = (byte)((ppuShiftRegister_patternL >> (15 - ppu_x)) & 1);
                    byte col1 = (byte)((ppuShiftRegister_patternH >> (15 - ppu_x)) & 1);
                    PalLow = (byte)((col1 << 1) | col0);

                    byte pal0 = (byte)(((ppuShiftRegister_attributeL) >> (15 - ppu_x)) & 1);
                    byte pal1 = (byte)(((ppuShiftRegister_attributeH) >> (15 - ppu_x)) & 1);
                    PalHi = (byte)((pal1 << 1) | pal0);

                    if (PalLow == 0 && PalHi != 0)
                    {
                        PalHi = 0;
                    }
                }

                Color outColour = Palette[PaletteRAM[PalHi * 4 + PalLow]];

                output.SetPixel(ppuDot - 1, ppuScanLine, outColour);
            }

            ppuDot++;
            if (ppuDot > 341)
            {
                ppuDot = 0;
                ppuScanLine++;
                if (ppuScanLine > 261)
                {
                    ppuScanLine = 0;
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
            ppu_v = (ushort)((ppu_v & 0b0111101111100000) | (ppu_t & 0b0000010000011111));
        }

        void PPU_ResetYScroll()
        {
            ppu_v = (ushort)((ppu_v & 0b0000010000011111) | (ppu_t & 0b0111101111100000));
        }
    }
}
