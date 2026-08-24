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
        ushort AddressBus;
        ushort DataLatch; // Internal bus
        ushort DataBus;
    }
}
