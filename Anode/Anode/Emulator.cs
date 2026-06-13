using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Anode
{
    internal class Emulator
    {
        // PPU code from 100th coin's tutorial, I will refine it for accuuracy later but I need to just get this working atm

        // Potential change to be made:
        // Set the R/W register and then read or write accordingly at the end of the cycle
        
        // Requirements:
        // Get video working
        // Fix the CPU
        // Improve memory usage (fix the small leak)

        // Swap the always new variables for permanent ones?? I'm not the most sure as to
        // C# optimisation.

        // ----- CPU Regisers
        ushort ProgramCounter;
        byte X;
        byte Y;
        byte A; // Accumulator
        byte SP; // Stack pointer
        byte t;
        byte opcode;
        byte ADD; // Used as temporary storage

        // P register as separate bools for readability and ease of use
        bool flag_Carry;
        bool flag_Zero;
        bool flag_InterruptDisable;
        bool flag_Decimal;
        bool flag_Overflow;
        bool flag_Negative;

        // Bus registers/lines
        ushort AddressBus; // Stores current accessing address
        byte DataBus; // Stores the data in use

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

        // Sprite registers which aren't implemented yet
        /*byte[] ppu_SpriteShiftRegisterL = new byte[8];
        byte[] ppu_SpriteShiftRegisterH = new byte[8];

        byte[] ppu_SpriteAttribute = new byte[8];
        byte[] ppu_SpritePattern = new byte[8];
        byte[] ppu_SpriteXposition = new byte[8];
        byte[] ppu_SpriteYposition = new byte[8];*/

        // Screen position and info
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
            // Read the ROM and deposit it into the variables
            byte[] HeaderedROM = File.ReadAllBytes(filepath);
            Array.Copy(HeaderedROM, Header, 0x10);
            byte size = Header[4];
            Array.Copy(HeaderedROM, 0x10, ROM, 0, 0x4000 * size);

            // Does the ROM support graphics?
            if (Header[5] != 0)
            {
                Array.Copy(HeaderedROM, 0x4000 * size + 0x10, CHRData, 0, 0x2000); // Load graphics pattern data
            }

            // Find where the program counter should start
            byte PC_Lo = Read_Raw(0xFFFC);
            byte PC_Hi = Read_Raw(0xFFFD);
            ProgramCounter = (ushort)((PC_Hi * 0x100) + PC_Lo);

            // Setup some initial variables (more stuff will be needed when I add the soft reset)
            SP = 0xFD;
            flag_InterruptDisable = true;

            // Check if logging and start writing if so
            if (logging)
            {
                tracelog = new StreamWriter(tracepath);
            }

            // PAL support coming somewhen, but the display size is different IIRC - Unless the NES doesn't do that...
            if (NTSC)
            {
                output = new Bitmap(32 * 8, 30 * 8);
            }

            // Init palette
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
                // PPU runs 1:4
                if ((Master_Clock - 1) % 4 == 0)
                {
                    Emulate_PPU();
                }

                // CPU runs 1:12
                if (Master_Clock % 12 == 0)
                {
                    Emulate_CPU();
                }

                // Reset to prevent weird overflows
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
                // Not really applicable at the moment.
            }
        }

        void Render()
        {
            frame_Ready = true;
            // Sometimes extra code is necessary
        }

        byte Read_Raw(ushort Address)
        {
            if (Address < 0x2000)
            {
                // Returns mirrored RAM
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
                            // Palette RAM has no buffer
                            temp = ReadPPU(ppu_v);
                        }
                        else
                        {
                            // Buffer data (VRAM)
                            PPUReadBuffer = ReadPPU(ppu_v);
                        }

                        ppu_v += (ushort)(ppuVRAMInc32Mode ? 32 : 1);
                        ppu_v &= 0x3FFF;
                        return temp;
                    case 0x2002:
                        // Returns PPU status flags
                        byte ppustatus = 0;
                        ppustatus |= (byte)(ppuVBlank ? 0x80 : 0);
                        /*ppustatus |= (byte)(ppuStatusSprZeroHit ? 0x40 : 0);
                        ppustatus |= (byte)(ppuStatusOverflow ? 0x20 : 0);*/
                        ppustatus |= 0x40;

                        // Also clears VBlank and Write Latch
                        ppuVBlank = false;
                        ppu_w = false;
                        return ppustatus;
                    case 0x2004:
                        // OAM stuff, not emulated yet
                        return 0;
                    default:
                        // Stuff I haven't implemented
                        Console.WriteLine($"Unknown PPU read - {Address:X}");
                        return 0;
                }
            }
            else if (Address >= 0x8000)
            {
                // Read from ROM (this line also mirrors for smaller ROMs)
                return ROM[(Address - 0x8000) & ((Header[4] * 0x4000) - 1)];
            }
            return 0;
        }

        void Read()
        {
            // Avoids a repeated line in case this needs to be used in the future
            DataBus = Read_Raw(AddressBus);
        }

        void Write_Raw(ushort Address, byte Value)
        {
            if (Address < 0x2000)
            {
                // Write to RAM (with mirroring)
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
            // Again, repeated line
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
            // Writes to stack, then decrements stack pointer
            AddressBus = (ushort)(0x100 + SP);
            Write();
            SP--;
        }

        void Pull()
        {
            // Increments stack pointer, then reads from stack
            SP++;
            AddressBus = (ushort)(0x100 + SP);
            Read();
        }

        void Halt_Instr()
        {
            // Groups the musltiple sections containing halts
            MessageBox.Show($"Encountered a halt instruction: opcode ${opcode:X}({op_a}, {op_b}, {op_c}) at {ProgramCounter:X})",
                "NES Error: Halted", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            CPU_Halted = true;
            Console.WriteLine($"Halt instruction: {opcode:X} ({op_a}, {op_b}, {op_c})");
        }

        void Read_Operand()
        {
            if (op_b == 2 || op_b == 0)
            {
                if (op_c == 2 && op_a < 4)
                {
                    Halt_Instr();
                }
                else if (op_c == 1 && op_b == 0)
                {
                    // X, Indirect
                    switch (t)
                    {
                        case 1:
                            // Reads the operand
                            Read();
                            ProgramCounter++;
                            AddressBus = DataBus;
                            break;
                        case 2:
                            Read(); // Dummy Read
                            // Adds X to the operand
                            AddressBus = (byte)(AddressBus + X);
                            break;
                        case 3:
                            // Reads high byte of the indirect operand
                            Read();
                            ADD = DataBus;
                            AddressBus = (byte)(AddressBus + 1);
                            break;
                        case 4:
                            // Reads low byte of the indirect operand, then moves
                            Read();
                            AddressBus = (ushort)((DataBus << 8) | ADD);
                            break;
                        case 5:
                            // Gets the data at the address
                            Read();
                            inc_op_t = true;
                            break;
                    }
                }
                else
                {
                    // Immediate
                    // Reads operand and that's it
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
                        // Reads the operand
                        Read();
                        ProgramCounter++;
                        break;
                    case 2:
                        // Gets the data at the address
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
                        // Reads the high byte of the operand
                        Read();
                        ProgramCounter++;
                        AddressBus++;
                        break;
                    case 2:
                        // Reads the low byte of the operand
                        ADD = DataBus;
                        Read();
                        ProgramCounter++;
                        break;
                    case 3:
                        // Gets the data at the adress
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
                    // Always a HLT instruction
                    Halt_Instr();
                }
                else
                {
                    // Indirect, Y
                    switch (t)
                    {
                        case 1:
                            // Read the operand and move the address bus there
                            Read();
                            ProgramCounter++;
                            AddressBus = DataBus;
                            break;
                        case 2:
                            // Read the high byte at the new address
                            Read();
                            AddressBus = (byte)(AddressBus + 1);
                            ADD = DataBus;
                            break;
                        case 3:
                            // Read the low byte at the new adress, then move again
                            Read();
                            AddressBus = (ushort)((DataBus << 8) | ADD);
                            // Get the address without the Y index
                            ushort AddrTemp = (ushort)(((AddressBus + Y) & 0xFF) | (AddressBus & 0xFF00));
                            // Get the distance to move in the high byte
                            signedTemp = AddressBus + Y - AddrTemp;
                            // Transfer to the address bus
                            AddressBus = AddrTemp;
                            // Skips next cycle if this is the new address
                            if (signedTemp == 0)
                            {
                                t = 4;
                            }
                            break;
                        case 4:
                            Read(); // Dummy read
                            AddressBus = (ushort)(AddressBus + signedTemp);
                            break;
                        case 5:
                            // Final read
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
                        // Read the operand
                        Read();
                        ProgramCounter++;
                        break;
                    case 2:
                        // Move to the new position, but don't cross the page boundary
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
                        // Read the value at the address
                        Read();
                        inc_op_t = true;
                        break;
                }
                
            }
            else if (op_b == 6 || op_b == 7) // Stop it, get some help
            {
                switch (t)
                {
                    case 1:
                        // Read the high byte of the operand
                        Read();
                        ProgramCounter++;
                        AddressBus++;
                        break;
                    case 2:
                        // Read the low byte of the operand
                        ADD = DataBus;
                        Read();
                        ProgramCounter++;
                        break;
                    case 3:
                        // Move there, then add either X or Y depending on the addressing mode
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

                        // Change the low byte
                        AddressBus = (ushort)((AddressBus & 0xFF) | (byte)AddressTemp);
                        if (AddressTemp != AddressBus)
                        {
                            // Page boundary crossed, wait
                            signedTemp = (AddressTemp - AddressBus);
                        }
                        else
                        {
                            // No boundary crossed
                            inc_op_t = true;
                            t = 4;
                        }
                        Read();
                        break;
                    case 4:
                        // Update after page boundary crossed
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
                // Accumulator (A) instruction
                a_indexed = true;
                DataBus = A;
                inc_op_t = true;
                op_t = 1;
            }
            else if (t == 1)
            {
                // Not an A instruction
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
                        // Perform a dummy write
                        Write();
                    }
                    else
                    {
                        // Probably isn't important as I don't think this behaviour can be verified
                        A = DataBus;
                    }
                    switch (op_a)
                    {
                        case 0:
                            // ASL - Arithmetic Shift Left
                            flag_Carry = DataBus > 127;
                            DataBus <<= 1;
                            flag_Zero = DataBus == 0;
                            flag_Negative = DataBus > 127;
                            break;
                        case 1:
                            // ROL - Rotate Left
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
                            // LSR - Logical Shift Right
                            flag_Carry = (DataBus & 1) != 0;
                            DataBus >>= 1;
                            flag_Negative = DataBus > 127;
                            flag_Zero = DataBus == 0;
                            break;
                        case 3:
                            // ROR - Rotate Right
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
                            // DEC - Decrement
                            DataBus--;
                            flag_Negative = DataBus > 127;
                            flag_Zero = DataBus == 0;
                            break;
                        case 7:
                            // INC - Increment
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
                        // Write properly this time
                        Write();
                    }
                    else
                    {
                        A = DataBus;
                    }
                    // Overflow for use in next inctruction
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
                                // Read the operand
                                Read();
                                ProgramCounter++;
                                AddressBus = DataBus;
                                break;
                            case 2:
                                Read(); // Dummy Read
                                // Move, to X index
                                AddressBus = (byte)(AddressBus + X);
                                break;
                            case 3:
                                // Read indirect high byte
                                Read();
                                ADD = DataBus;
                                AddressBus = (byte)(AddressBus + 1);
                                break;
                            case 4:
                                // Read indirect low byte
                                Read();
                                AddressBus = (ushort)((DataBus << 8) | ADD);
                                inc_op_t = true;
                                break;
                        }
                        break;
                    case 1:
                        // Zero page
                        // Read operand
                        Read();
                        ProgramCounter++;
                        AddressBus = DataBus;
                        inc_op_t = true;
                        break;
                    case 3:
                        // Absolute
                        if (t == 1)
                        {
                            // Read high byte
                            Read();
                            ProgramCounter++;
                            AddressBus++;
                        }
                        else if (t == 2)
                        {
                            // Read low byte
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
                                // Read operand
                                Read();
                                ProgramCounter++;
                                AddressBus = DataBus;
                                break;
                            case 2:
                                // Read indirect high byte
                                Read();
                                AddressBus = (byte)(AddressBus + 1);
                                ADD = DataBus;
                                break;
                            case 3:
                                // Read indirect low byte
                                Read();
                                AddressBus = (ushort)((DataBus << 8) | ADD);

                                // Get the address without the Y index
                                ushort AddrTemp = (ushort)(((AddressBus + Y) & 0xFF) | (AddressBus & 0xFF00));
                                // Get the distance to move in the high byte
                                signedTemp = AddressBus + Y - AddrTemp;
                                // Transfer to the address bus
                                AddressBus = AddrTemp;
                                break;
                            case 4:
                                Read(); // Dummy read
                                // Update high byte
                                AddressBus = (ushort)(AddressBus + signedTemp);
                                inc_op_t = true;
                                break;
                        }
                        break;
                    case 5:
                        switch (t)
                        {
                            case 1:
                                // Read operand
                                Read();
                                ProgramCounter++;
                                break;
                            case 2:
                                AddressBus = DataBus;
                                Read(); // Dummy read
                                // Index by either X or Y, depending on the addressing mode
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
                    case 7:
                        switch (t)
                        {
                            case 1:
                                // Read high byte
                                Read();
                                ProgramCounter++;
                                AddressBus++;
                                break;
                            case 2:
                                // Read low byte
                                ADD = DataBus;
                                Read();
                                ProgramCounter++;
                                break;
                            case 3:
                                // Move, then index by X or Y depending on addressing mode
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
                                // Apply to low byte only
                                AddressBus = (ushort)((AddressBus & 0xFF) | (byte)AddressTemp);
                                signedTemp = AddressTemp - AddressBus;
                                Read();
                                // Apply to high byte
                                AddressBus = (ushort)(AddressBus + signedTemp); // Again, shortcut.
                                inc_op_t = true;
                                break;
                        }
                        break;
                }
            }
            else
            {
                // Write to the correct location using the relavent register
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
                    // Determine the branch condition
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
                        // Don't take the branch
                        t = 255;
                    }
                    break;
                case 2:
                    signedTemp = DataBus;
                    Read(); // Dummy read
                    // Convert to signed
                    if (signedTemp > 127)
                    {
                        signedTemp -= 256;
                    }

                    // Change the lower byte only...
                    ushort BranchTemp = (ushort)(((ProgramCounter + signedTemp) & 0xFF) | (ProgramCounter & 0xFF00));
                    // ... and use it to figure out whether it crosses a page boundary
                    signedTemp = ProgramCounter + signedTemp - BranchTemp;

                    // Update the program counter
                    ProgramCounter = BranchTemp;
                    AddressBus = ProgramCounter;

                    // Check if it has crossed a boundary
                    if (signedTemp == 0)
                    {
                        t = 255;
                    }
                    break;
                case 3:
                    Read(); // Dummy read
                    // Move to the new position
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
                                // As the NMI is based on the BRK instruction, but has slight changes
                                // Add to the program counter to go to the next instruction when not an NMI
                                if (!DoNMI)
                                {
                                    ProgramCounter++;
                                }
                                break;
                            case 2:
                                // Push high byte of PC to the stack
                                DataBus = (byte)(ProgramCounter >> 8);
                                Push();
                                break;
                            case 3:
                                // Push low byte of PC to the stack
                                DataBus = (byte)(ProgramCounter);
                                Push();
                                break;
                            case 4:
                                // Push processor flags to the stack
                                DataBus = 0;
                                DataBus |= (byte)(flag_Carry ? 1 : 0);
                                DataBus |= (byte)(flag_Zero ? 2 : 0);
                                DataBus |= (byte)(flag_InterruptDisable ? 4 : 0);
                                DataBus |= (byte)(flag_Decimal ? 8 : 0);
                                DataBus += (byte)(DoNMI ? 0 : 0x10); // NMI has no B flag
                                DataBus |= 0x20;
                                DataBus |= (byte)(flag_Overflow ? 0x40 : 0);
                                DataBus |= (byte)(flag_Negative ? 0x80 : 0);
                                Push();
                                break;
                            case 5:
                                // For NMI, FFFA
                                // For RES, FFFC
                                // For BRK, FFFE
                                // Find where the program counter moves to for the low byte
                                AddressBus = (ushort)(DoNMI ? 0xFFFA : 0xFFFE);
                                Read();
                                ADD = DataBus;
                                break;
                            case 6:
                                // Find high byte for PC
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
                            // Order is weird, but that's just how it is on the CPU itself
                            case 1:
                                // Find low byte of PC
                                Read();
                                ProgramCounter++;
                                ADD = DataBus;
                                break;
                            case 2:
                                //Dummy read from stack
                                AddressBus = (ushort)(0x100 + SP);
                                Read();
                                break;
                            case 3:
                                // Push high byte of PC to the stack
                                DataBus = (byte)(ProgramCounter >> 8);
                                Push();
                                break;
                            case 4:
                                // Push low byte of PC to the stack
                                DataBus = (byte)ProgramCounter;
                                Push();
                                break;
                            case 5:
                                // Finally, get high byte of PC and move back
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
                                // Processor flags pulled from the stack and transferred
                                Pull();
                                flag_Carry = (DataBus & 1) != 0;
                                flag_Zero = (DataBus & 2) != 0;
                                flag_InterruptDisable = (DataBus & 4) != 0;
                                flag_Decimal = (DataBus & 8) != 0;
                                flag_Overflow = (DataBus & 0x40) != 0;
                                flag_Negative = (DataBus & 0x80) != 0;
                                break;
                            case 4:
                                // Get low byte of PC
                                Pull();
                                ADD = DataBus;
                                break;
                            case 5:
                                // Get high byte of PC
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
                            // There's a lot of dummy reads in this one...
                            // Anyways, I suspect that I could maybe get RTI to share this code
                            case 1:
                                // Dummy read
                                Read();
                                break;
                            case 2:
                                // Dummy read from stack
                                AddressBus = (ushort)(SP + 0x100);
                                Read();
                                break;
                            case 3:
                                // Get low byte of PC
                                Pull();
                                ADD = DataBus;
                                break;
                            case 4:
                                // Get high byte of PC
                                Pull();
                                ProgramCounter = (ushort)((DataBus << 8) | ADD);
                                break;
                            case 5:
                                // Dummy read
                                ProgramCounter++;
                                AddressBus = ProgramCounter;
                                Read();
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
                                // Get the low byte of PC
                                Read();
                                ADD = DataBus;
                                AddressBus++;
                                break;
                            case 2:
                                // Get the high byte of PC
                                Read();
                                AddressBus = (ushort)((DataBus << 8) | ADD);
                                inc_op_t = true;
                                break;
                        }
                    }
                    else
                    {
                        // Absolute
                        // Can skip straight to the new PC position
                        op_t = 1;
                        inc_op_t = true;
                    }
                }
                switch (op_t)
                {
                    case 1:
                        // Read low byte of address
                        Read();
                        ADD = DataBus;
                        AddressBus++;
                        break;
                    case 2:
                        // Read high byte of address
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
                // Push instructions
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
                            DataBus |=                              0x10; // Always set
                            DataBus |=                              0x20; // Always set
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
                            // Subtract 1 from Y and update flags
                            Y--;
                            flag_Zero = Y == 0;
                            flag_Negative = Y > 127;
                            break;
                        case 5:
                            // TAY
                            // Sets Y to A and then updates flags
                            Y = A;
                            flag_Zero = Y == 0;
                            flag_Negative = Y > 127;
                            break;
                        case 6:
                            // INY
                            // Increments 1 on Y and update flags
                            Y++;
                            flag_Zero = Y == 0;
                            flag_Negative = Y > 127;
                            break;
                        case 7:
                            // INX
                            // Increments 1 on X and updates flags
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
                            // Clears the carry flag
                            flag_Carry = false;
                            break;
                        case 1:
                            // SEC
                            // Sets the carry flag
                            flag_Carry = true;
                            break;
                        case 2:
                            // CLI
                            // Clears the interrupt disable flag
                            flag_InterruptDisable = false;
                            break;
                        case 3:
                            // SEI
                            // Sets the interrupt disable flag
                            flag_InterruptDisable = true;
                            break;
                        case 4:
                            // TYA
                            // Copies Y over to the A register and sets flags
                            A = Y;
                            flag_Zero = A == 0;
                            flag_Negative = A > 127;
                            break;
                        case 5:
                            // CLV
                            // Clears the overflow register
                            flag_Overflow = false;
                            break;
                        case 6:
                            // CLD
                            // Clears the decimal register
                            flag_Decimal = false;
                            break;
                        case 7:
                            // SED
                            // Sets the decimal register
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
                            // Copies the X register to the A register and sets flags
                            A = X;
                            flag_Zero = A == 0;
                            flag_Negative = A > 127;
                            break;
                        case 5:
                            // TAX
                            // Copies the A register to the X register and sets flags
                            X = A;
                            flag_Zero = X == 0;
                            flag_Negative = X > 127;
                            break;
                        case 6:
                            // DEX
                            // Subtracts 1 from the X register and sets flags
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
                            // Transfers the X to the SP
                            // Doesn't set flags
                            SP = X;
                            break;
                        case 5:
                            // TSX
                            // Transfers the SP to X and sets flags
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
                // All of these nead to read the operand first
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
                            // Binary ORs with the value and sets flags
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
                                // I don't really know why this exists, but it isn't commonly used.
                                flag_Zero = (A & DataBus) == 0;
                                flag_Negative = (DataBus & 0x80) != 0;
                                flag_Overflow = (DataBus & 0x40) != 0;
                                break;
                            case 1:
                                // AND
                                // Binary ands with value and sets flags
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
                            // Binary exclusive ORs, aka XOR, and sets flags
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
                                // Add with carry - this one is kinda complex...

                                // Find the result of the calculation
                                int IntSum = DataBus + A + (flag_Carry ? 1 : 0);

                                // Figure out whether it causes an overflow (2 positives
                                // ends up being negative, or 2 negatives is positive)
                                // This is used in signed calculations
                                flag_Overflow = (~(A ^ DataBus) & (A ^ IntSum) & 0x80) != 0;

                                // Find whether it carries over to the next bit
                                flag_Carry = IntSum > 0xFF;

                                // Now store in A and set other flags as normal
                                A = (byte)IntSum;
                                flag_Negative = A > 127;
                                flag_Zero = A == 0;
                                break;
                        }
                        break;
                    case 5:
                        // Load instruction
                        // Transfers into the correct register and then sets flags
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
                                // Compares the Y register with the data bus to set flags
                                flag_Carry = Y >= DataBus;
                                flag_Zero = DataBus == Y;
                                flag_Negative = (byte)(Y - DataBus) > 127;
                                break;
                            case 1:
                                // CMP
                                // Compares the accumulator with the data bus to set flags
                                flag_Carry = A >= DataBus;
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
                                // Compares the x register with the data bus to set flags
                                flag_Carry = X >= DataBus;
                                flag_Zero = DataBus == X;
                                flag_Negative = (byte)(X - DataBus) > 127;
                                break;
                            case 1:
                                // SBC
                                // Ahh, another complex one

                                // Get the result of the calculation
                                int IntSum = A - DataBus - (flag_Carry ? 0 : 1);
                                // In case of the signed overflow
                                flag_Overflow = ((A ^ DataBus) & (A ^ DataBus) & 0x80) != 0;
                                // Unsigned overflow
                                flag_Carry = IntSum >= 0;
                                // Transfer to A and set regular flags
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
            // Check whether NMI should occur
            bool PreviousNMILevelDetector = NMILevelDetector;
            NMILevelDetector = ppuEnableNMI && ppuVBlank;
            if (!PreviousNMILevelDetector && NMILevelDetector)
            {
                DoNMI = true;
            }

            // Reading opcodes or starting NMI.
            if (t == 0)
            {
                op_t = 0; // 1st instruction on 1
                inc_op_t = false;
                // Read next opcode
                AddressBus = ProgramCounter;
                if (!DoNMI)
                {
                    // Read the opcode
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
                    // NMI is similar to BRK
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
                if ((op_c == 2) && ((op_a < 4) || ((op_a > 5) && ((op_b & 1) != 0))))
                {
                    // RMW instructions
                    RMW_Instr();
                }
                else if ((op_a == 4) && (((op_c == 1) && (op_b != 2)) || ((op_b & 1) != 0)))
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
                    else if (((op_b == 0) && (op_a < 4)) || ((op_b == 3) && (op_a > 1) && (op_a < 4)))
                    {
                        // Movement
                        Move_Instr();
                    }
                    else if ((op_b == 2) && (op_a < 4))
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

            // In case of failure.
            if (t > 20)
            {
                CPU_Halted = true;
                Console.WriteLine($"Opcode ${opcode:X}({op_a:X}, {op_b:X}, {op_c:X}) did not finish; t register exceeded 20.");
                MessageBox.Show($"Opcode ${opcode:X}({op_a:X}, {op_b:X}, {op_c:X}) did not finish; t register exceeded 20.\nThis error should not occur, and should be reported to the developer.", 
                    "CPU Emulation Error: Instruction Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // The PPU code is temporary
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
