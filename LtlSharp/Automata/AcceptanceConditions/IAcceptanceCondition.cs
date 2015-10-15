using System;
using System.Collections.Generic;

namespace LtlSharp.Automata.AcceptanceConditions
{
    /// <summary>
    /// Defines a generalized acceptance condition for an omega automaton over nodes of type <c>T</c>.
    /// </summary>
    /// <typeparam name="T">The type of nodes in the condition.</typeparm>
    public interface IAcceptanceCondition<T>
    {
        /// <summary>
        /// Indicates whether the specified nodes is accepted by the w-automaton when repeatedly reached.
        /// </summary>
        /// <param name="nodes">Node repeatedly reached.</param>
        bool Accept (T nodes);
        
        /// <summary>
        /// Indicates whether the specified set of nodes is accepted by the w-automaton when repeatedly reached.
        /// </summary>
        /// <param name="nodes">Nodes repeatedly reached.</param>
        bool Accept (IEnumerable<T> nodes);

        /// <summary>
        /// Indicates whether the condition might be satisfied.
        /// </summary>
        /// <returns><c>True</c> if the condition can be satisfied, <c>False</c> otherwise.</returns>
        bool IsSatisfiable { get; }

        /// <summary>
        /// Returns a new acceptance condition where nodes are transformed using the provided <c>map</c> function.
        /// </summary>
        /// <remarks>
        /// If the mapping function returns more than one <c>T1</c>, the acceptance condition is expanded to the new
        /// nodes.
        /// </remarks>
        /// <param name="map">Mapping function.</param>
        /// <typeparam name="T1">The generic type for the new acceptance condition.</typeparam>
        IAcceptanceCondition<T1> Map<T1>(Func<T, IEnumerable<T1>> map);
    }
}

