using System;
using System.Collections.Generic;

namespace LtlSharp.Buchi.LTL2Buchi
{
    public interface ILTL2Buchi
    {
        BuchiAutomata GetAutomaton (ILTLFormula phi);
    }
}

