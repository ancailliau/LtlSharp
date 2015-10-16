using System;
using System.Collections.Generic;

namespace LtlSharp.Automata.Transitions.Factories
{
    public class LiteralSetDecorationFactory
        : IAutomatonTransitionFactory<LiteralSetDecoration>
    {
        LiteralSetDecoration IAutomatonTransitionFactory<LiteralSetDecoration>.Create (IEnumerable<ILiteral> literals)
        {
            return new LiteralSetDecoration (literals);
        }
    }
}

