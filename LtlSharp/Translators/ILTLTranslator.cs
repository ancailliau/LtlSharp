using System;
using System.Collections.Generic;

namespace LtlSharp.Buchi.LTL2Buchi
{
    public interface ILTLTranslator
    {
        BuchiAutomata GetAutomaton (ITLFormula phi);
    }
}

