using System;
using System.Collections.Generic;
using LtlSharp.Automata;
using LtlSharp.Automata.OmegaAutomata;

namespace LtlSharp.Buchi.LTL2Buchi
{
    public interface ILTLTranslator
    {
        BuchiAutomaton<AutomatonNode> GetAutomaton (ITLFormula phi);
    }
}

