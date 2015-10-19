using System;
using System.Collections.Generic;
using System.Linq;

namespace LtlSharp.Automata.AcceptanceConditions
{
    /// <summary>
    /// Defines a Buchï acceptance condition.
    /// </summary>
    /// <remarks>
    /// An omega automaton with a Buchï condition accepts the words where at least a node in the accepting set
    /// of the Buchï condition is met infinitely often.
    /// </remarks>
    /// <typeparam name="T">Types of the nodes in omega automaton.</typeparam>
    public class BuchiAcceptance<T> : IAcceptanceCondition<T>
    {
        /// <summary>
        /// Gets the set of nodes that are accepting.
        /// </summary>
        /// <value>The accepting set.</value>
        public HashSet<T> AcceptingSet {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LtlSharp.Automata.AcceptanceConditions.BuchiAcceptance`1"/> 
        /// class with an empty accepting set.
        /// </summary>
        public BuchiAcceptance ()
        {
            AcceptingSet = new HashSet<T> ();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LtlSharp.Automata.AcceptanceConditions.BuchiAcceptance`1"/> 
        /// class with the same accepting set that one in the specified condition
        /// </summary>
        /// <param name="acceptanceCondition">Buchï Acceptance Condition.</param>
        public BuchiAcceptance (BuchiAcceptance<T> acceptanceCondition)
        {
            AcceptingSet = new HashSet<T> (acceptanceCondition.AcceptingSet);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LtlSharp.Automata.AcceptanceConditions.BuchiAcceptance`1"/> 
        /// class with the specified accepting set of nodes.
        /// </summary>
        /// <param name="nodes">Nodes.</param>
        public BuchiAcceptance (IEnumerable<T> nodes)
        {
            AcceptingSet = new HashSet<T> (nodes);
        }

        /// <summary>
        /// Add the specified node to the accepting set.
        /// </summary>
        /// <param name="node">Node.</param>
        public void Add (T node)
        {
            AcceptingSet.Add (node);
        }
        
        /// <summary>
        /// Returns the subset of nodes that are accepted by the acceptance condition when repeatedly reached.
        /// </summary>
        /// <returns>The accepting nodes.</returns>
        public IEnumerable<T> GetAcceptingNodes ()
        {
            return AcceptingSet;
        }
        
        /// <summary>
        /// Returns the subset of nodes that are accepted by the acceptance condition when repeatedly reached.
        /// </summary>
        /// <returns>The accepting nodes.</returns>
        /// <param name="nodes">Nodes.</param>
        public IEnumerable<T> GetAcceptingNodes (HashSet<T> nodes)
        {
            return AcceptingSet.Intersect (nodes);
        }
        
        #region IAcceptanceCondition<T> Members

        public bool IsSatisfiable {
            get {
                return AcceptingSet.Count > 0;
            }
        }

        public bool Accept (T node)
        {
            return AcceptingSet.Contains (node);
        }

        public bool Accept (IEnumerable<T> nodes)
        {
            return AcceptingSet.Intersect (nodes).Any ();
        }

        public IAcceptanceCondition<T1> Map<T1>(Func<T, IEnumerable<T1>> map)
        {
            return new BuchiAcceptance<T1> (AcceptingSet.SelectMany (map));
        }
        
        #endregion

        public override string ToString ()
        {
            return string.Format ("[BuchiAcceptance: AcceptingSet={{{0}}}]", 
                                  string.Join (",", AcceptingSet.Select (x => x.ToString ())));
        }
    }
}

