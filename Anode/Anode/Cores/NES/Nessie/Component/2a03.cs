using Anode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anode.Cores.NES.Nessie
{
    [Serializable]
    internal class _2a03
    {
        byte A; // Accumulator
        byte X;
        byte Y;

        byte SP; // Stack Pointer
        public ushort PC; // Program Counter

        bool flag_Carry;
        bool flag_Zero;
        bool flag_InterruptDisable;
        bool flag_Decimal;
        bool flag_Overflow;
        bool flag_Negative;

        byte op_t;
        byte t;
        bool resetInstr;
        bool finishedOp;
        bool quickOpComplete;

        public ushort AddressBus;
        public byte DataLatch; // Internal bus
        public byte DataBus;

        public ushort DelayedAddr;

        public bool halt;

        public bool getRequired;

        byte opcode;
        byte op_a;
        byte op_b;
        byte op_c;

        // Tracelogger code from 100th Coin's tutorial, I'll make my own one in the future. (it's been a while and I still haven't done this lol)
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

        readonly byte[] LengthTable =
        {
            10, 254, 20, 2, 40, 4, 80, 6, 160, 8, 60, 10, 14, 12, 26, 14,
            12, 16, 24, 18, 48, 20, 96, 22, 192, 24, 72, 26, 16, 28, 32, 30
        };

        // DMC Rate cycle diff, also from nesdev
        readonly ushort[] DMCRateNTSC =
        {
            428, 380, 340, 320, 286, 254, 226, 214, 190, 160, 142, 128, 106, 84, 72, 54
        };
        readonly ushort[] DMCRatePAL =
        {
            398, 354, 316, 298, 276, 236, 210, 198, 176, 148, 132, 118, 98, 78, 66, 50
        };

        byte opcode_type; // Values above 127 don't use standard operands
        byte operand_type;
        bool getAddrOnly;

        // Temp values for debug
        bool logging = false;
        StreamWriter tracelog;
        public string tracepath;
        void Tracelogger(byte opcode)
        {
            if (logging)
            {
                String line = "$" + PC.ToString("X4")
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

        public void Initialise()
        {
            getRequired = true;
        }

        public void RunCycle()
        {
            if (op_t == 0 && t == 0)
            {
                // Read the opcode
                opcode = DataBus;
                op_a = (byte)(opcode >> 5);
                op_b = (byte)((opcode & 0x1C) >> 2);
                op_c = (byte)(opcode & 0x3);

                getRequired = true;

                if (logging)
                {
                    Tracelogger(opcode);
                }

                PC++;
                DelayedAddr++;


                // Opcode types
                // 0x0x:
                // 0 - RMW
                // 1 - Store
                // 2 - Internal memory execution
                // 0x8x:
                // 0 - Movement
                // 1 - Push/Pull
                // 2 - Single byte
                // 3 - Branch
                // 4 - Unofficial immediate

                if (op_c == 0 && (op_a & 4) == 0 && (op_b & 0b101) == 0)
                {
                    // Movement or push/pull
                    opcode_type = op_b == 0 ? (byte)0x80 : (byte)0x81;
                }
                else if (op_c == 2 && (op_a & 0b110) != 0b100)
                {
                    // RMW
                    opcode_type = 0;
                }
                else if ((op_c & 0b01) == 0 && (op_b & 0b11) == 0b10)
                {
                    // Single byte (impl)
                    opcode_type = 0x82;
                }
                else if (op_c == 0 && op_b == 4)
                {
                    // Branch
                    opcode_type = 0x83;
                }
                else if (op_a == 4)
                {
                    // Store
                    opcode_type = 1;
                }
                else
                {
                    if (!(op_c == 3 && op_b == 2))
                    {
                        // Internal memory execution
                        opcode_type = 2;
                    }
                    else
                    {
                        // Unofficial immediates
                        opcode_type = 0x84;
                    }
                }

                // Operand types
                // First nybble is type, second is variant
                // 0b0000 0000 - N/A
                // 0b0000 0001 - Immediate
                // 0b0000 0010 - Accumulator

                // 0b0001 0000 - Zero Page
                // 0b0001 0001 - Zero Page, X
                // 0b0001 0010 - Zero page, Y

                // 0b0010 0000 - Absolute
                // 0b0010 0001 - Absolute, X
                // 0b0010 0010 - Absolute, Y

                // 0b0011 0001 - X, Indirect
                // 0b0011 0010 - Y, Indirect

                if ((opcode_type & 0x80) != 0)
                {
                    operand_type = 0x00;
                }
                else
                {
                    switch (op_b)
                    {
                        case 0:
                            if ((op_c & 1) == 0)
                            {
                                if ((op_a & 4) == 0)
                                {
                                    // HLT
                                    Halt();
                                }
                                else
                                {
                                    // Immediate (#)
                                    operand_type = 0x01;
                                }
                            }
                            else
                            {
                                // X, Indirect
                                operand_type = 0x31;
                            }
                            break;
                        case 1:
                            // Zero Page
                            operand_type = 0x10;
                            break;
                        case 2:
                            if ((op_c & 1) == 1)
                            {
                                // Immediate (#)
                                operand_type = 0x01;
                            }
                            else
                            {
                                // Accumulator (A)
                                operand_type = 0x02;
                            }
                            break;
                        case 3:
                            // Absolute
                            operand_type = 0x20;
                            break;
                        case 4:
                            if (op_c == 2)
                            {
                                // HLT
                                Halt();
                            }
                            else
                            {
                                // Indirect, Y
                                operand_type = 0x32;
                            }
                            break;
                        case 5:
                            if ((op_c & 2) == 2 && (op_a & 0b110) == 0b100)
                            {
                                // Zero Page, Y
                                operand_type = 0x12;
                            }
                            else
                            {
                                // Zero Page, X
                                operand_type = 0x11;
                            }
                            break;
                        case 6:
                            // Absolute, Y
                            operand_type = 0x22;
                            break;
                        case 7:
                            // Absolute, X
                            operand_type = 0x21;
                            break;
                    }

                    if ((operand_type & 0b11110000) == 0)
                    {
                        quickOpComplete = true;
                        finishedOp = true;
                    }
                }
            }
            else
            {
                if (!finishedOp)
                {
                    switch (operand_type)
                    {
                        case 0x10:
                        case 0x11:
                        case 0x12:
                            break;
                        case 0x20:
                        case 0x21:
                        case 0x22:
                            break;
                        case 0x31:
                            break;
                        case 0x32:
                            break;
                    }
                    if (finishedOp)
                    {
                        switch (opcode_type)
                        {
                            case 2:
                                getRequired = true;
                                break;
                        }
                    }
                }
                else
                {
                    if (quickOpComplete)
                    {
                        // Immediate and A
                        if (operand_type == 2)
                        {
                            DataBus = A;
                        }
                        else
                        {
                            PC++;
                        }
                        quickOpComplete = false;
                    }
                    DataLatch = DataBus;

                    switch (opcode_type)
                    {
                        case 0:
                            break;
                        case 1:
                            break;
                        case 2:
                            Internal_Mem();
                            break;
                        case 0x80:
                            break;
                        case 0x81:
                            break;
                        case 0x82:
                            break;
                        case 0x83:
                            break;
                        case 0x84:
                            break;
                    }
                }
            }

            if (finishedOp)
            {
                t++;
                if (t > 20)
                {
                    Util.ThrowError("CPU Error", $"Opcode {opcode:X} didn't reset. This is an emulation bug and should be reported to the developer.");
                    halt = true;
                }
            }
            else
            {
                op_t++;
                if (op_t > 20)
                {
                    Util.ThrowError("CPU Error", $"The operand on opcode {opcode:X} didn't reset. This is an emulation bug and should be reported to the developer.");
                    halt = true;
                }
            }

            if (resetInstr)
            {
                getRequired = true;
                resetInstr = false;
                finishedOp = false;
                quickOpComplete = false;
                op_t = 0;
                t = 0;

                DelayedAddr = PC;
            }
        }

        void Internal_Mem()
        {
            switch (op_a)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    // Load instruction
                    // Transfers into the correct register and then sets flags
                    switch (op_c)
                    {
                        case 0:
                            // LDY
                            Y = DataLatch;
                            break;
                        case 1:
                            // LDA
                            A = DataLatch;
                            break;
                        case 2:
                            // LDX
                            X = DataLatch;
                            break;
                        case 3:
                            // LAX (Unofficial)
                            if (op_b == 6)
                            {
                                // LAR/LAS
                                DataLatch &= SP;
                                SP = DataLatch;
                            }
                            A = X = DataLatch;
                            break;
                    }
                    flag_Zero = DataLatch == 0;
                    flag_Negative = DataLatch >= 0x80;
                    break;
                case 6:
                    break;
                case 7:
                    break;
            }
            resetInstr = true;

            if (halt && logging)
            {
                tracelog.Close();
            }
        }
        
        void Halt()
        {
            halt = true;
            Util.ThrowError("CPU Halted", $"Encountered a halt instruction: {opcode:X}");
        }

        public _2a03()
        {
            SP = 0xFD;
            flag_InterruptDisable = true;
            getRequired = true;
            resetInstr = false;
            finishedOp = false;
            op_t = 0;
            t = 0;
            getAddrOnly = false;

            tracepath = System.IO.Path.GetDirectoryName(System.Windows.Forms.Application.ExecutablePath) + "\\tracelog.txt";
            if (logging)
            {
                tracelog = new StreamWriter(tracepath);
            }
        }
    }
}
