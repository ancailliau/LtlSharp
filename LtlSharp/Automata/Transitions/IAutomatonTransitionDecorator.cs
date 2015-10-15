using System;
using System.Collections.Generic;
using LtlSharp.Automata;
using LtlSharp.Automata.Transitions;

namespace LtlSharp.Automata
{
    /// <summary>
    /// Defines a generic structure for decoration on transitions
    /// </summary>
    public interface IAutomatonTransitionDecorator<T>
        where T : IAutomatonTransitionDecorator<T>
    {
        IEnumerable<ILiteral> GetAlphabet ();
        LiteralsSet ToLiteralSet ();
        
        bool Entails (T l);
        IEnumerable<T> UnfoldLabels (IEnumerable<ILiteral> enumerable);
    }
}

