using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Buchi.Automata;
using LtlSharp.Models;
using LtlSharp.Utils;

namespace LtlSharp.Automata.AcceptanceConditions
{
    public interface IAcceptanceCondition<T>
    {
        /// <summary>
        /// Returns whether the specified nodes is accepted by the w-automaton when repeatedly reached.
        /// </summary>
        /// <param name="nodes">Node repeatedly reached.</param>
        bool Accept (T nodes);
        
        /// <summary>
        /// Returns whether the specified set of nodes is accepted by the w-automaton when repeatedly reached.
        /// </summary>
        /// <param name="nodes">Nodes repeatedly reached.</param>
        bool Accept (IEnumerable<T> nodes);

        /// <summary>
        /// Returns whether the condition might be satisfied.
        /// </summary>
        /// <returns><c>True</c> if the condition can be satisfied, <c>False</c> otherwise.</returns>
        bool IsSatisfiable { get; }

        /// <summary>
        /// Returns the subset of nodes that are accepted by the acceptance condition when repeatedly reached.
        /// </summary>
        /// <returns>The accepting nodes.</returns>
        /// <param name="nodes">Nodes.</param>
        IEnumerable<T> GetAcceptingNodes (HashSet<T> nodes);

        /// <summary>
        /// Returns a new acceptance condition where nodes are transformed using the provided <c>map</c> function.
        /// </summary>
        /// <param name="map">Mapping function.</param>
        /// <typeparam name="T1">The generic type for the new acceptance condition.</typeparam>
        IAcceptanceCondition<T1> Map<T1>(Func<T, IEnumerable<T1>> map);
    }
}

