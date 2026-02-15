using System;
using System.Collections.Generic;
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

        bool a_indexed = false;

        public string filepath;
        public string tracepath;
        public bool logging;

        string this_trace = "";
        StreamWriter tracelog;

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

            byte PC_Lo = Read_Raw(0xFFFC);
            byte PC_Hi = Read_Raw(0xFFFD);
            ProgramCounter = (ushort)((PC_Hi * 0x100) + PC_Lo);

            SP = 0xFD;
            flag_InterruptDisable = true;

            if (logging)
            {
                tracelog = new StreamWriter(tracepath);
            }
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
                if (logging)
                {
                    tracelog.Close();
                    Console.WriteLine("Tracelog saved!");
                }
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
                                break;
                            case 2:
                                ProgramCounter++;
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
                                DataBus |= 0x10;
                                DataBus |= 0x20;
                                DataBus |= (byte)(flag_Overflow ? 0x40 : 0);
                                DataBus |= (byte)(flag_Negative ? 0x80 : 0);
                                Push();
                                break;
                            case 5:
                                AddressBus = 0xFFFE;
                                Read();
                                ADD = DataBus;
                                break;
                            case 6:
                                AddressBus++;
                                Read();
                                ProgramCounter = (ushort)((DataBus << 8) | ADD);
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
                        // Implied
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
                if (logging)
                {
                    Tracelogger(opcode);
                }
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
                Console.WriteLine($"Opcode {opcode:X}({op_a:X}, {op_b:X}, {op_c:X}) did not finish; t exceeded 20.");
            }
        }
    }
}
