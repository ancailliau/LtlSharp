using System;
using System.Collections.Generic;
namespace LtlSharp.Automata.Transitions.Factories
{
    public interface IAutomatonTransitionFactory<T>
    {
        T Create (IEnumerable<ILiteral> literals);
    }
}

