using System;
using System.Collections.Generic;

namespace LtlSharp.Automata.Transitions.Factories
{
    public class LiteralSetFactory
        : IAutomatonTransitionFactory<LiteralsSet>
    {
        LiteralsSet IAutomatonTransitionFactory<LiteralsSet>.Create (IEnumerable<ILiteral> literals)
        {
            return new LiteralsSet (literals);
        }
    }
}

