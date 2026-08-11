using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anode.Base
{
    internal interface EmuCore
    {
        void AdvanceFrame();

        void SetConfig(byte Region);

        void Reset();
    }
}
