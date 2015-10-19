using System;
using System.Collections.Generic;
using LtlSharp.Automata;
using LtlSharp.Automata.Transitions;

namespace LtlSharp.Automata.Transitions.Decorations
{
    /// <summary>
    /// Defines a generic structure for decoration on transitions
    /// </summary>
    public interface ITransitionDecoration<T>
        where T : ITransitionDecoration<T>
    {
    }
}

