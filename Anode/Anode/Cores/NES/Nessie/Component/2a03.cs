using Anode.Common;
using System;
using System.Collections.Generic;
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
        ushort PC; // Program Counter

        byte op_t;
        byte t;
        bool resetInstr;
        bool finishedOp;

        public ushort AddressBus;
        byte DataLatch; // Internal bus
        public byte DataBus;

        public bool halt;

        public bool getRequired;

        byte opcode;
        byte op_a;
        byte op_b;
        byte op_c;

        public void Initialise()
        {
            getRequired = true;
        }

        public void RunCycle()
        {
            if (resetInstr)
            {
                getRequired = true;
                resetInstr = false;
                finishedOp = false;
                op_t = 0;
                t = 0;
            }
            else
            {
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
                    t++;
                    if (t > 20)
                    {
                        Util.ThrowError("CPU Error", $"The operand on opcode {opcode:X} didn't reset. This is an emulation bug and should be reported to the developer.");
                        halt = true;
                    }
                }
            }
        }
    }
}
