using System;
using System.Collections.Generic;
using LtlSharp.Buchi.LTL2Buchi;
namespace LtlSharp.Automata.Transitions.Factories
{
    public class DegeneralizerAutomataTransitionFactory
        : IAutomatonTransitionFactory<DegeneralizerAutomataTransition>
    {
        public DegeneralizerAutomataTransition Create (IEnumerable<ILiteral> literals)
        {
            return new DegeneralizerAutomataTransition ();
        }
    }
}

