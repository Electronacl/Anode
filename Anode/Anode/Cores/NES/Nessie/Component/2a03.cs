using Anode.Common;
using System;
using System.IO;

namespace Anode.Cores.NES.Nessie
{
    [Serializable]
    internal class _2a03
    {
        readonly byte unstable_magic = 0xEC;

        byte A; // Accumulator
        byte X;
        byte Y;

        byte SP; // Stack Pointer
        public ushort PC; // Program Counter

        byte ADD;
        int signedTemp;

        bool flag_Carry;
        bool flag_Zero;
        bool flag_InterruptDisable;
        bool flag_Decimal;
        bool flag_Overflow;
        bool flag_Negative;

        bool FutureFlag_Carry;

        byte op_t;
        byte t;
        bool resetInstr;
        bool finishedOp;
        bool quickOpComplete;

        public ushort AddressBus;
        public byte DataLatch; // Internal bus
        public byte DataBus;

        byte preIndex_hi;
        bool changedBoundary;

        public ushort DelayedAddr;

        public bool halt;

        public bool getRequired;

        byte opcode;
        byte op_a;
        byte op_b;
        byte op_c;

        // On reading and writing - important!
        // Set the read/write address *before* the cycle you want to access memory on.
        // This means that for push/pull, use it before the frame to push/pull on.

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

        // Temp values for debug
        public bool logging = false;
        public StreamWriter tracelog;
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
            DataLatch = DataBus;
            // Why set the data latch instead of using the data bus?
            // Well, it hasn't been implemented yet, but there's a separate internal and
            // external bus - some audio registers don't update the external one
            // So, in this case, DL is the internal and DB is external

            if (op_t == 0 && t == 0)
            {
                // Read the opcode
                opcode = DataLatch;
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

                if (opcode == 0xCa)
                {

                }

                
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
                else if (op_c == 2 && ((op_a & 0b100) == 0 || ((op_a & 0b110) != 0b100 && (op_b & 0b11) != 0b10)))
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
                        if (op_c == 0 && (op_a & 0b110) == 0b010)
                        {
                            // JMP is weird
                            opcode_type = 0x80;
                        }
                        else
                        {
                            // Internal memory execution
                            opcode_type = 2;
                        }
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
                    finishedOp = true;
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
                            Zero_Page();
                            break;
                        case 0x11:
                        case 0x12:
                            Zero_Page_Indexed();
                            break;
                        case 0x20:
                            Absolute();
                            break;
                        case 0x21:
                        case 0x22:
                            Absolute_Indexed();
                            break;
                        case 0x31:
                            X_Indirect();
                            break;
                        case 0x32:
                            Y_Indirect();
                            break;
                    }
                    if (finishedOp)
                    {
                        switch (opcode_type)
                        {
                            case 1:
                                getRequired = false;
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
                            DataLatch = A;
                            t = 3;
                            // A instructions take 2 cycles and only do the first dummy read
                        }
                        else
                        {
                            PC++;
                        }
                        quickOpComplete = false;
                    }

                    switch (opcode_type)
                    {
                        case 0:
                            RMW();
                            break;
                        case 1:
                            Store();
                            break;
                        case 2:
                            Internal_Mem();
                            break;
                        case 0x80:
                            Move();
                            break;
                        case 0x81:
                            Stack();
                            break;
                        case 0x82:
                            Single_Byte();
                            break;
                        case 0x83:
                            Branch();
                            break;
                        case 0x84:
                            Unofficial_Immediate();
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
                changedBoundary = false;

                op_t = 0;
                t = 0;

                DelayedAddr = PC;
            }

            DataBus = DataLatch;
        }

        void Internal_Mem()
        {
            switch (op_a)
            {
                case 0:
                    // c=0 is NOP, N/A or NOP for c=2
                    if (op_c == 1)
                    {
                        // ORA
                        // Binary ORs accumulator with value
                        A |= DataLatch;
                        flag_Negative = A >= 0x80;
                        flag_Zero = A == 0;
                    }
                    break;
                case 1:
                    // N/A or NOP for c=2
                    switch (op_c)
                    {
                        case 0:
                            if (op_b < 4)
                            {
                                // BIT
                                // This isn't commonly used, but still important to implement
                                flag_Zero = (A & DataLatch) == 0;
                                flag_Negative = (DataLatch & 0x80) != 0;
                                flag_Overflow = (DataLatch & 0x40) != 0;
                            }
                            // In other cases, NOP
                            break;
                        case 1:
                            // AND
                            // Binary ANDs accumulator with value
                            A &= DataLatch;
                            flag_Negative = A >= 0x80;
                            flag_Zero = A == 0;
                            break;
                    }
                    break;
                case 2:
                    // N/A or NOP for C=0 and C=2
                    if (op_c == 1)
                    {
                        // EOR
                        // Binary exclusive ORs with accumulator, aka XOR
                        A ^= DataLatch;
                        flag_Negative = A >= 0x80;
                        flag_Zero = A == 0;
                    }
                    break;
                case 3:
                    if (op_c == 1)
                    {
                        // ADC
                        // Add with carry - this one is kinda complex...

                        // Find the result of the calculation
                        signedTemp = DataLatch + A + (flag_Carry ? 1 : 0);

                        // Figure out whether it causes an overflow (2 positives
                        // ends up being negative, or 2 negatives is positive)
                        // This is used in signed calculations
                        flag_Overflow = (~(A ^ DataLatch) & (A ^ signedTemp) & 0x80) != 0;

                        // Find whether it carries over to the next bit
                        flag_Carry = signedTemp > 0xFF;

                        // Now store in A and set other flags as normal
                        A = (byte)signedTemp;
                        flag_Negative = A >= 0x80;
                        flag_Zero = A == 0;
                    }
                    break;
                case 4:
                    // Only NOP
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
                    switch (op_c)
                    {
                        case 0:
                            if (op_b < 4)
                            {
                                // CPY
                                // Compares the Y register with the data bus to set flags
                                flag_Carry = Y >= DataLatch;
                                flag_Zero = DataLatch == Y;
                                flag_Negative = (byte)(Y - DataLatch) >= 0x80;
                            }
                            // In other cases, NOP
                            break;
                        case 1:
                            // CMP
                            // Compares the accumulator with the data bus to set flags
                            flag_Carry = A >= DataLatch;
                            flag_Zero = DataLatch == A;
                            flag_Negative = (byte)(A - DataLatch) >= 0x80;
                            break;
                        // Case 2 is NOP
                    }
                    break;
                case 7:
                    switch (op_c)
                    {
                        case 0:
                            // CPX
                            // Compares the x register with the data bus to set flags
                            if (op_b < 4)
                            {
                                flag_Carry = X >= DataLatch;
                                flag_Zero = DataLatch == X;
                                flag_Negative = (byte)(X - DataLatch) >= 0x80;
                            }
                            // In other cases, NOP
                            break;
                        case 1:
                            // SBC
                            // Ahh, another complex one

                            // Get the result of the calculation
                            signedTemp = A - DataLatch - (flag_Carry ? 0 : 1);
                            // In case of the signed overflow
                            flag_Overflow = ((A ^ DataLatch) & (A ^ signedTemp) & 0x80) != 0;
                            // Unsigned overflow
                            flag_Carry = signedTemp >= 0;
                            // Transfer to A and set regular flags
                            A = (byte)signedTemp;
                            flag_Negative = A >= 0x80;
                            flag_Zero = A == 0;
                            break;
                            // Case 2 is NOP
                    }
                    break;
            }
            resetInstr = true;

            if (halt && logging)
            {
                tracelog.Close();
            }
        }

        void Store()
        {
            switch (op_c)
            {
                case 0:
                    // STY
                    DataLatch = Y;
                    break;
                case 1:
                    // STA
                    DataLatch = A;
                    break;
                case 2:
                    // STX
                    DataLatch = X;
                    break;
                case 3:
                    // SHA, SHS
                    DataLatch = (byte)(A & X);
                    break;
            }
            if ((op_c != 1) && (op_b == 4 || op_b == 7 || op_b == 6))
            {
                if (op_b == 6)
                {
                    // SHS (TAS)
                    SP = DataLatch;
                }

                // SHX, SHA, SHY
                Unstable_Cross(DataLatch);
                DataLatch = (byte)(DataLatch & (preIndex_hi + 1));
            }
            resetInstr = true;
        }

        void Branch()
        {
            switch(t)
            {
                case 1:
                    PC++;
                    DelayedAddr++;
                    bool branch_condition = false;
                    switch (op_a)
                    {
                        case 0:
                        case 1:
                            // BPL, BMI
                            branch_condition = flag_Negative;
                            break;
                        case 2:
                        case 3:
                            // BVC, BVS
                            branch_condition = flag_Overflow;
                            break;
                        case 4:
                        case 5:
                            // BCC, BCS
                            branch_condition = flag_Carry;
                            break;
                        case 6:
                        case 7:
                            // BNE, BEQ
                            branch_condition = flag_Zero;
                            break;
                    }
                    if ((!branch_condition && ((op_a & 1) != 0)) || (branch_condition && ((op_a & 1) == 0)))
                    {
                        // Don't take the branch
                        resetInstr = true;
                    }
                    signedTemp = DataLatch;
                    break;
                case 2:
                    if ((signedTemp & 0x80) != 0)
                    {
                        signedTemp -= 256;
                    }

                    // Change the lower byte only...
                    ushort BranchTemp = (ushort)(((PC + signedTemp) & 0xFF) | (PC & 0xFF00));
                    // ... and use it to figure out whether it crosses a page boundary
                    signedTemp = PC + signedTemp - BranchTemp;

                    // Update the PC
                    PC = BranchTemp;
                    DelayedAddr = PC;

                    // Check for a boundary cross
                    if (signedTemp == 0)
                    {
                        // The next cycle is therefore skipped
                        resetInstr = true;
                    }
                    break;
                case 3:
                    DelayedAddr = (ushort)(AddressBus + signedTemp);
                    PC = DelayedAddr;
                    break;
            }
        }

        void Stack()
        {
            if ((op_a & 1) != 0)
            {
                // Pull
                switch (t)
                {
                    case 1:
                        DelayedAddr = (ushort)(0x100 + SP);
                        break;
                    case 2:
                        SP++;
                        DelayedAddr = (ushort)(0x100 + SP);
                        break;
                    case 3:
                        if ((op_a & 2) == 0)
                        {
                            // PLP
                            flag_Carry = (DataLatch & 1) != 0;
                            flag_Zero = (DataLatch & 2) != 0;
                            flag_InterruptDisable = (DataLatch & 4) != 0;
                            flag_Decimal = (DataLatch & 8) != 0;
                            flag_Overflow = (DataLatch & 0x40) != 0;
                            flag_Negative = (DataLatch & 0x80) != 0;
                        }
                        else
                        {
                            // PLA
                            A = DataLatch;
                            flag_Zero = A == 0;
                            flag_Negative = A >= 0x80;
                        }
                        resetInstr = true;
                        break;
                }
            }
            else
            {
                // Push
                switch (t)
                {
                    case 1:
                        getRequired = false;
                        Push();
                        break;
                    case 2:
                        if ((op_a & 2) == 0)
                        {
                            // PHP
                            DataLatch = 0;
                            DataLatch |= (byte)(flag_Carry ? 1 : 0);
                            DataLatch |= (byte)(flag_Zero ? 2 : 0);
                            DataLatch |= (byte)(flag_InterruptDisable ? 4 : 0);
                            DataLatch |= (byte)(flag_Decimal ? 8 : 0);
                            DataLatch |= 0x10; // Always set
                            DataLatch |= 0x20; // Always set
                            DataLatch |= (byte)(flag_Overflow ? 0x40 : 0);
                            DataLatch |= (byte)(flag_Negative ? 0x80 : 0);
                        }
                        else
                        {
                            // PHA
                            DataLatch = A;
                        }
                        resetInstr = true;
                        break;
                }
            }
        }

        void Move()
        {
            if(op_b == 0)
            {
                switch (op_a)
                {
                    case 0:
                        // BRK
                        switch (t)
                        {
                            case 1:
                                PC++;
                                getRequired = false;
                                break;
                            case 2:
                                DataLatch = (byte)(PC >> 8);

                                // Potentially update the push method to use this, as it's more accurate
                                AddressBus = (ushort)(0x100 + SP);
                                SP--;
                                break;
                            case 3:
                                DataLatch = (byte)PC;
                                AddressBus = (ushort)(0x100 + SP);
                                SP--;
                                break;
                            case 4:
                                DataLatch =  (byte)(flag_Carry ? 1 : 0);
                                DataLatch |= (byte)(flag_Zero ? 2 : 0);
                                DataLatch |= (byte)(flag_InterruptDisable ? 4 : 0);
                                DataLatch |= (byte)(flag_Decimal ? 8 : 0);
                                //DataLatch |= (byte)((inNMI || inIRQ) ? 0 : 0x10); // NMI has no B flag
                                DataLatch |= 0x10;
                                DataLatch |= 0x20; // Always set
                                DataLatch |= (byte)(flag_Overflow ? 0x40 : 0);
                                DataLatch |= (byte)(flag_Negative ? 0x80 : 0);

                                AddressBus = (ushort)(0x100 + SP);
                                SP--;

                                getRequired = true;
                                //DelayedAddr = (ushort)(inNMI ? 0xFFFA : 0xFFFE);
                                DelayedAddr = 0xFFFE;
                                break;
                            case 5:
                                ADD = DataLatch;
                                DelayedAddr++;
                                break;
                            case 6:
                                DelayedAddr = (ushort)((DataLatch << 8) | ADD);
                                PC = DelayedAddr;

                                resetInstr = true;
                                break;
                        }
                        break;
                    case 1:
                        // JSR
                        switch (t)
                        {
                            case 1:
                                PC++;
                                ADD = DataLatch;
                                DelayedAddr = (ushort)(0x100 + SP);
                                break;
                            case 2:
                                getRequired = false;
                                Push();
                                break;
                            case 3:
                                DataLatch = (byte)(PC >> 8);
                                Push();
                                break;
                            case 4:
                                DataLatch = (byte)PC;
                                getRequired = true;
                                DelayedAddr = PC;
                                break;
                            case 5:
                                PC = (ushort)((DataLatch << 8) | ADD);
                                resetInstr = true;
                                break;
                        }
                        break;
                    case 2:
                        // RTI
                        switch (t)
                        {
                            case 1:
                                DelayedAddr = (ushort)(SP + 0x100);
                                break;
                            case 2:
                                Pull();
                                break;
                            case 3:
                                flag_Carry = (DataLatch & 1) != 0;
                                flag_Zero = (DataLatch & 2) != 0;
                                flag_InterruptDisable = (DataLatch & 4) != 0;
                                flag_Decimal = (DataLatch & 8) != 0;
                                flag_Overflow = (DataLatch & 0x40) != 0;
                                flag_Negative = (DataLatch & 0x80) != 0;

                                Pull();
                                break;
                            case 4:
                                ADD = DataLatch;
                                Pull();
                                break;
                            case 5:
                                PC = (ushort)((DataLatch << 8) | ADD);
                                resetInstr = true;
                                break;
                        }
                        break;
                    case 3:
                        // RTS
                        switch (t)
                        {
                            case 1:
                                // rd
                                DelayedAddr = (ushort)(0x100 + SP);
                                break;
                            case 2:
                                // rd
                                Pull();
                                break;
                            case 3:
                                // rd
                                ADD = DataLatch;
                                Pull();
                                break;
                            case 4:
                                // rd
                                PC = (ushort)((DataLatch << 8) | ADD);
                                PC++;
                                DelayedAddr = PC;
                                break;
                            case 5:
                                // rd
                                resetInstr = true;
                                break;
                        }
                        break;
                }
            }
            else
            {
                if (op_a != 3 && t == 1)
                {
                    // For absolute instead of indirect, skip
                    t = 3;
                }
                switch (t)
                {
                    case 1:
                        ADD = DataLatch;
                        DelayedAddr++;
                        break;
                    case 2:
                        DelayedAddr = (ushort)((DataLatch << 8) | ADD);
                        break;
                    case 3:
                        ADD = DataLatch;
                        if (op_a == 3)
                        {
                            DelayedAddr = (ushort)((AddressBus & 0xFF00) | ((AddressBus + 1) & 0xFF));
                        }
                        else
                        {
                            DelayedAddr++;
                        }
                        break;
                    case 4:
                        PC = (ushort)((DataLatch << 8) | ADD);

                        resetInstr = true;
                        break;
                }
            }
        }

        void RMW()
        {
            switch (t)
            {
                case 1:
                    // Read
                    getRequired = false;
                    break;
                case 2:
                    // Dummy write
                    break;
                case 3:
                    // Do stuff
                    if (op_c != 3)
                    {
                        switch (op_a)
                        {
                            case 0:
                                // ASL - Arithmetic Shift Left
                                flag_Carry = DataLatch >= 0x80;
                                DataLatch <<= 1;
                                flag_Zero = DataLatch == 0;
                                flag_Negative = DataLatch >= 0x80;
                                break;
                            case 1:
                                // ROL - Rotate Left
                                bool Futureflag_Carry = DataLatch >= 0x80;
                                DataLatch <<= 1;
                                if (flag_Carry)
                                {
                                    DataLatch |= 1;
                                }
                                flag_Carry = Futureflag_Carry;
                                flag_Negative = DataLatch >= 0x80;
                                flag_Zero = DataLatch == 0;
                                break;
                            case 2:
                                // LSR - Logical Shift Right
                                flag_Carry = (DataLatch & 1) != 0;
                                DataLatch >>= 1;
                                flag_Negative = DataLatch >= 0x80;
                                flag_Zero = DataLatch == 0;
                                break;
                            case 3:
                                // ROR - Rotate Right
                                bool FutureFlag_Carry = (DataLatch & 1) != 0;
                                DataLatch >>= 1;
                                if (flag_Carry)
                                {
                                    DataLatch |= 0x80;
                                }
                                flag_Carry = FutureFlag_Carry;
                                flag_Negative = DataLatch >= 0x80;
                                flag_Zero = DataLatch == 0;
                                break;
                            case 6:
                                // DEC - Decrement
                                DataLatch--;
                                flag_Negative = DataLatch >= 0x80;
                                flag_Zero = DataLatch == 0;
                                break;
                            case 7:
                                // INC - Increment
                                DataLatch++;
                                flag_Negative = DataLatch >= 0x80;
                                flag_Zero = DataLatch == 0;
                                break;
                        }
                    }
                    else
                    {
                        switch (op_a)
                        {
                            case 0:
                                // SLO (Unofficial)
                                flag_Carry = DataLatch >= 0x80;
                                DataLatch <<= 1;
                                A |= DataLatch;
                                flag_Negative = A >= 0x80;
                                flag_Zero = A == 0;
                                break;
                            case 1:
                                // RLA (Unofficial)
                                FutureFlag_Carry = DataLatch >= 0x80;
                                DataLatch <<= 1;
                                if (flag_Carry)
                                {
                                    DataLatch |= 1;
                                }
                                flag_Carry = FutureFlag_Carry;
                                A &= DataLatch;
                                flag_Negative = A >= 0x80;
                                flag_Zero = A == 0;
                                break;
                            case 2:
                                // SRE (Unofficial)
                                flag_Carry = (DataLatch & 1) != 0;
                                DataLatch >>= 1;

                                A ^= DataLatch;
                                flag_Negative = A >= 0x80;
                                flag_Zero = A == 0;
                                break;
                            case 3:
                                // RRA
                                FutureFlag_Carry = (DataLatch & 1) != 0;
                                DataLatch >>= 1;
                                if (flag_Carry)
                                {
                                    DataLatch |= 0x80;
                                }
                                flag_Carry = FutureFlag_Carry;

                                signedTemp = DataLatch + A + (flag_Carry ? 1 : 0);
                                flag_Overflow = (~(A ^ DataLatch) & (A ^ signedTemp) & 0x80) != 0; // Signed overflow
                                flag_Carry = signedTemp > 0xFF; // Unsigned overflow
                                A = (byte)signedTemp;

                                flag_Negative = A >= 0x80;
                                flag_Zero = A == 0;
                                break;
                            case 6:
                                // DCP (Unofficial)
                                DataLatch--;
                                flag_Carry = A >= DataLatch;
                                flag_Zero = DataLatch == A;
                                flag_Negative = (byte)(A - DataLatch) >= 0x80;
                                break;
                            case 7:
                                // ISC (Unofficial)
                                DataLatch++;
                                // Get the result of the calculation
                                signedTemp = A - DataLatch - (flag_Carry ? 0 : 1);
                                // In case of the signed overflow
                                flag_Overflow = ((A ^ DataLatch) & (A ^ signedTemp) & 0x80) != 0;
                                // Unsigned overflow
                                flag_Carry = signedTemp >= 0;
                                // Transfer to A and set regular flags
                                A = (byte)signedTemp;
                                flag_Negative = A >= 0x80;
                                flag_Zero = A == 0;
                                break;
                        }
                    }
                    if (operand_type == 2)
                    {
                        A = DataLatch;
                    }
                    resetInstr = true;
                    break;
            }
        }

        void Unofficial_Immediate()
        {
            PC++;
            switch (op_a)
            {
                case 0:
                case 1:
                    // ANC
                    // First AND
                    A &= DataLatch;
                    flag_Negative = A >= 0x80;
                    flag_Zero = A == 0;

                    // Then carry flag is set
                    flag_Carry = A >= 0x80;
                    break;
                case 2:
                    // ALR / ASR
                    // First AND
                    A &= DataLatch;

                    DataLatch = A;

                    // Then LSR
                    flag_Carry = (DataLatch & 1) != 0;
                    DataLatch >>= 1;
                    flag_Negative = DataLatch >= 0x80;
                    flag_Zero = DataLatch == 0;

                    // Affects A
                    A = DataLatch;
                    break;
                case 3:
                    // ARR
                    // Shiver me timbers!
                    // First AND
                    A &= DataLatch;
                    flag_Negative = A >= 0x80;
                    flag_Zero = A == 0;

                    DataLatch = A;

                    // Then ROR
                    bool FutureFlag_Carry = (DataLatch & 1) != 0;
                    DataLatch >>= 1;
                    if (flag_Carry)
                    {
                        DataLatch |= 0x80;
                    }
                    flag_Carry = FutureFlag_Carry;
                    flag_Negative = DataLatch >= 0x80;
                    flag_Zero = DataLatch == 0;

                    // Affects A
                    A = DataLatch;

                    // Sets flags based on certain conditions
                    flag_Carry = (A & 0b01000000) != 0; // Flag is set if bit 6 is set
                    flag_Overflow = flag_Carry != ((A & 0b00100000) != 0); // Flag is set if either bit 6 OR 5 is set but NOT both
                    break;
                case 4:
                    // ANE (XXA)
                    // Unstable opcode using a magic value. The magic value fluctuates on real
                    // hardware based on temperature and other factors, but this isn't done in emulation,
                    // as it prevents repeatability. The constant is currently chosen based on a realistic
                    // value, albeit I don't have a flash cart to check real hardware.
                    A = (byte)((unstable_magic | A) & X & DataLatch);
                    flag_Zero = A == 0;
                    flag_Negative = A >= 0x80;
                    break;
                case 5:
                    // LXA
                    // immediate version of LAX, but it's unstable
                    DataLatch = (byte)((A | unstable_magic) & DataLatch);
                    A = DataLatch;
                    X = DataLatch;
                    flag_Negative = DataLatch >= 0x80;
                    flag_Zero = DataLatch == 0;
                    break;
                case 6:
                    // SBX (AXS, SAX)
                    DataLatch = (byte)((A & X) - DataLatch);
                    flag_Zero = DataLatch == 0;
                    flag_Negative = DataLatch >= 0x80;
                    flag_Carry = DataLatch <= (A & X);
                    X = DataLatch;
                    break;
                case 7:
                    // USBC (Wow, USB-C! The NES was really ahead of it's time /j)
                    // Literally just SBC #

                    // Get the result of the calculation
                    signedTemp = A - DataLatch - (flag_Carry ? 0 : 1);
                    // In case of the signed overflow
                    flag_Overflow = ((A ^ DataLatch) & (A ^ signedTemp) & 0x80) != 0;
                    // Unsigned overflow
                    flag_Carry = signedTemp >= 0;
                    // Transfer to A and set regular flags
                    A = (byte)signedTemp;
                    flag_Negative = A >= 0x80;
                    flag_Zero = A == 0;
                    break;
            }
            resetInstr = true;
        }

        void Single_Byte()
        {
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
                            flag_Negative = Y >= 0x80;
                            break;
                        case 5:
                            // TAY
                            // Sets Y to A and then updates flags
                            Y = A;
                            flag_Zero = Y == 0;
                            flag_Negative = Y >= 0x80;
                            break;
                        case 6:
                            // INY
                            // Increments 1 on Y and update flags
                            Y++;
                            flag_Zero = Y == 0;
                            flag_Negative = Y >= 0x80;
                            break;
                        case 7:
                            // INX
                            // Increments 1 on X and updates flags
                            X++;
                            flag_Zero = X == 0;
                            flag_Negative = X >= 0x80;
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
                            flag_Negative = A >= 0x80;
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
                            flag_Negative = A >= 0x80;
                            break;
                        case 5:
                            // TAX
                            // Copies the A register to the X register and sets flags
                            X = A;
                            flag_Zero = X == 0;
                            flag_Negative = X >= 0x80;
                            break;
                        case 6:
                            // DEX
                            // Subtracts 1 from the X register and sets flags
                            X--;
                            flag_Zero = X == 0;
                            flag_Negative = X >= 0x80;
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
                            flag_Negative = X >= 0x80;
                            break;
                        // Others are unofficial NOPs
                    }
                }
            }
            resetInstr = true;
        }

        void Zero_Page()
        {
            DelayedAddr = DataLatch;
            finishedOp = true;
            PC++;
        }

        void Zero_Page_Indexed()
        {
            switch (op_t)
            {
                case 1:
                    DelayedAddr = DataLatch;
                    PC++;
                    break;
                case 2:
                    if (operand_type == 0x11)
                    {
                        // Zero Page, X
                        DelayedAddr = (byte)(DelayedAddr + X);
                    }
                    else
                    {
                        // Zero Page, Y
                        DelayedAddr = (byte)(DelayedAddr + Y);
                    }
                    finishedOp = true;
                    break;
            }
        }

        void Absolute()
        {
            switch (op_t)
            {
                case 1:
                    ADD = DataLatch;
                    PC++;
                    DelayedAddr++;
                    break;
                case 2:
                    DelayedAddr = (ushort)((DataLatch << 8) | ADD);
                    PC++;
                    finishedOp = true;
                    break;
            }
        }

        void Absolute_Indexed()
        {
            switch (op_t)
            {
                case 1:
                    ADD = DataLatch;
                    PC++;
                    DelayedAddr++;
                    break;
                case 2:
                    DelayedAddr = (ushort)((DataLatch << 8) | ADD);
                    PC++;
                    preIndex_hi = DataLatch;

                    ushort AddressTemp;
                    if (operand_type == 0x21)
                    {
                        // Absolute, X
                        AddressTemp = (ushort)(DelayedAddr + X);
                    }
                    else
                    {
                        // Absolute, Y
                        AddressTemp = (ushort)(DelayedAddr + Y);
                    }

                    DelayedAddr = (ushort)((DelayedAddr & 0xFF00) | (byte)AddressTemp);
                    if (AddressTemp != DelayedAddr)
                    {
                        // Page boundary crossed, wait
                        signedTemp = (AddressTemp - DelayedAddr);
                        changedBoundary = true;
                    }
                    else
                    {
                        // No boundary crossed
                        if (opcode_type != 1)
                        {
                            finishedOp = true;
                        }
                        else
                        {
                            // Store instructions don't skip the next step if not rquired
                            signedTemp = 0;
                        }
                    }
                    break;
                case 3:
                    DelayedAddr = (ushort)(AddressBus + signedTemp);
                    break;
            }
        }

        void X_Indirect()
        {
            switch (op_t)
            {
                case 1:
                    PC++;
                    DelayedAddr = DataLatch;
                    break;
                case 2:
                    DelayedAddr = (byte)(AddressBus + X);
                    break;
                case 3:
                    ADD = DataLatch;
                    DelayedAddr = (byte)(AddressBus + 1);
                    break;
                case 4:
                    DelayedAddr = (ushort)((DataLatch << 8) | ADD);
                    finishedOp = true;
                    break;
            }
        }

        void Y_Indirect()
        {
            switch (op_t)
            {
                case 1:
                    PC++;
                    DelayedAddr = DataLatch;
                    break;
                case 2:
                    DelayedAddr = (byte)(AddressBus + 1);
                    ADD = DataLatch;
                    break;
                case 3:
                    preIndex_hi = DataLatch;
                    DelayedAddr = (ushort)((DataLatch << 8) | ADD);

                    ushort AddrTemp = (ushort)(((DelayedAddr + Y) & 0xFF) | (DelayedAddr & 0xFF00));
                    signedTemp = DelayedAddr + Y - AddrTemp;
                    DelayedAddr = AddrTemp;

                    if (signedTemp == 0 && opcode_type != 1)
                    {
                        finishedOp = true;
                    }
                    break;
                case 4:
                    changedBoundary = signedTemp != 0;
                    DelayedAddr = (ushort)(DelayedAddr + signedTemp);
                    finishedOp = true;
                    break;
            }
        }

        void Unstable_Cross(byte CrossVal)
        {
            // The edge case might not be applicable at the moment as DMA is not working correctly.
            // DON'T change the address bus like this, but ig it works for this case as the use of it occurs after the cycle
            if (changedBoundary)
            {
                AddressBus = (ushort)((AddressBus & 0xFF00 & (CrossVal << 8)) | (AddressBus & 0xFF));
            }
        }

        void Push()
        {
            // Always use a "put" cycle.
            DelayedAddr = (ushort)(0x100 + SP);
            SP--;
        }

        void Pull()
        {
            // A bit redundant, but I think the function should be called at reset IIRC
            SP++;
            DelayedAddr = (ushort)(0x100 + SP);
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
        }
    }
}
