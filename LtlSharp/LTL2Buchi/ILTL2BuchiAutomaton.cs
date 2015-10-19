using System;
using System.Collections.Generic;
using LtlSharp.Automata;
using LtlSharp.Automata.OmegaAutomata;

namespace LtlSharp.Buchi.LTL2Buchi
{
    public interface ILTL2BuchiAutomaton<T>
        where T : IAutomatonNode
    {
        BuchiAutomaton<T> GetBuchiAutomaton (ITLFormula phi);
    }
}

