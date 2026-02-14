using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

namespace Anode
{
    internal class Emulator
    {
        // CPU Regisers
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

        // Clock
        byte Master_Clock = 1;

        // Storage
        public byte[] RAM = new byte[0x800];
        byte[] ROM = new byte[0x8000];
        byte[] Header = new byte[0x10];

        // Emulator specific
        byte op_t;
        bool inc_op_t;
        int signedTemp;

        public bool CPU_Halted;

        byte op_a;
        byte op_b;
        byte op_c;

        public string filepath;

        public void Reset()
        {
            byte[] HeaderedROM = File.ReadAllBytes(filepath);
            Array.Copy(HeaderedROM, Header, 0x10);
            byte size = Header[4];
            Array.Copy(HeaderedROM, 0x10, ROM, 0, 0x4000 * size);

            byte PC_Lo = Read_Raw(0xFFFC);
            byte PC_Hi = Read_Raw(0xFFFD);
            ProgramCounter = (ushort)((PC_Hi * 0x100) + PC_Lo);

            SP = 0xFD;
        }

        public void Run()
        {
            // Clocking
            if (!CPU_Halted)
            {
                if ((Master_Clock - 1) % 4 == 0)
                {
                    // Emulate_PPU();
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
            }
        }

        byte Read_Raw(ushort Address)
        {
            if (Address < 0x2000)
            {
                return RAM[Address & 0x7FF];
            }
            if (Address >= 0x8000)
            {
                return ROM[Address - 0x8000];
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
        }

        void Write()
        {
            Write_Raw(AddressBus, DataBus);
        }

        void Read_Operand()
        {
            if (op_b == 2 || (op_b == 0 && op_c != 1))
            {
                if (op_c == 2 && op_a < 4)
                {
                    CPU_Halted = true;
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
                if (t == 1)
                {
                    Read();
                    ProgramCounter++;
                }
                else if (t == 2)
                {
                    AddressBus = DataBus;
                    Read();
                    inc_op_t = true;
                }
            }
            else if (op_b == 3)
            {
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
                }
                else if (t == 3)
                {
                    AddressBus = (ushort)((DataBus << 8) | ADD);
                    Read();
                    inc_op_t = true;
                }
            }
            else if (op_b == 4)
            {
                // Ind, Y
                if (op_c == 2)
                {
                    // HLT
                    CPU_Halted = true;
                }
            }
        }

        void RMW_Instr()
        {
            if (op_c != 1 && op_b == 2 && t == 1)
            {
                // Accumulator instruction
                DataBus = A;
                inc_op_t = true;
                op_t = 1;
            }
            
            if (!inc_op_t)
            {
                Read_Operand();
            }
            else
            {

            }
        }

        void Store_Instr()
        {
            // Read addresses, this is different
            if (!inc_op_t)
            {
                if (op_b == 1)
                {
                    // Zero page
                    Read();
                    ProgramCounter++;
                    AddressBus = DataBus;
                    inc_op_t = true;
                }
                else if (op_b == 3)
                {
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
                }
            }
            else
            {
                if (op_c == 0)
                {
                    // STY
                    DataBus = Y;
                    Write();
                    t = 255;
                }
                else if (op_c == 1)
                {
                    // STA
                    DataBus = A;
                    Write();
                    t = 255;
                }
                else if (op_c == 2)
                {
                    // STX
                    DataBus = X;
                    Write();
                    t = 255;
                }
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

        }

        void Stack_Instr()
        {

        }

        void Single_Byte_Instr()
        {
            Read(); // Dummy Read
        }

        void Internal_Mem_Instr()
        {
            if (!inc_op_t)
            {
                Read_Operand();
            }
            else
            {
                if (op_a == 5)
                {
                    // Load instruction
                    if (op_c == 0)
                    {
                        // LDY
                        Y = DataBus;
                        t = 255;
                    }
                    else if (op_c == 1)
                    {
                        // LDA
                        A = DataBus;
                        t = 255;
                    }
                    else if (op_c == 2)
                    {
                        // LDX
                        X = DataBus;
                        t = 255;
                    }
                    flag_Zero = DataBus == 0;
                    flag_Negative = DataBus > 127;
                }
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
            if (t == 0)
            {
                op_t = 0; // 1st instr on 1
                inc_op_t = false;
                // Read next opcode
                AddressBus = ProgramCounter;
                Read();
                opcode = DataBus;
                // Increment addresses
                ProgramCounter++;
                AddressBus++;
                // Split it up, as this can be used to determine what to do
                op_a = (byte)(opcode >> 5);
                op_b = (byte)((opcode & 0x1C) >> 2);
                op_c = (byte)(opcode & 0x3);
            }
            else
            {
                if ((op_c == 2 && (op_a < 4 || (op_a > 5 && (op_b & 1) == 1))) || (op_c == 1 && op_a == 7))
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
                Console.WriteLine($"Opcode {opcode:X}({op_a:X}, {op_b:X}, {op_c:X}) did not finish; t exceeded 20.");
            }
        }
    }
}
