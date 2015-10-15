using System;
using System.Collections.Generic;
namespace LtlSharp.Automata.Nodes.Factories
{
    /// <summary>
    /// Defines a generic interface for factories creating automaton nodes.
    /// </summary>
    public interface IAutomatonNodeFactory<T>
        where T : IAutomatonNode
    {
        /// <summary>
        /// Creates a new automaton node with default name and an empty set of labels
        /// </summary>
        T Create ();
        
        /// <summary>
        /// Creates a new automaton node with the specified name and an empty set of labels.
        /// </summary>
        /// <param name="name">Name.</param>
        T Create (string name);
        
        /// <summary>
        /// Creates a new automaton node with the specified name and labels.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="labels">Labels.</param>
        T Create (string name, IEnumerable<ILiteral> labels);
    }
}

