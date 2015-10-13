using System;
using System.Collections.Generic;
using LtlSharp.Automata;

namespace LtlSharp.Buchi.LTL2Buchi
{
    public interface ILTLTranslator
    {
        BuchiAutomata GetAutomaton (ITLFormula phi);
    }
}

