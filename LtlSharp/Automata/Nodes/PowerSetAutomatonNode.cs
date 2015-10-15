using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Automata.Transitions;
using LtlSharp.Utils;

namespace LtlSharp.Automata
{
    /// <summary>
    /// Defines a generic node for a power set of nodes.
    /// </summary>
    public class PowerSetAutomatonNode<T> : AutomatonNode 
        where T : IAutomatonNode 
    {
        
        /// <summary>
        /// Gets or sets the nodes in the power set.
        /// </summary>
        /// <value>The nodes contained in the node.</value>
        public ISet<T> Nodes {
            get;
            protected set;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LtlSharp.Automata.ProductAutomatonNode`1"/> class with
        /// both null automaton nodes.
        /// </summary>
        public PowerSetAutomatonNode () 
            : base ()
        {}
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LtlSharp.Automata.ProductAutomatonNode`1"/> class with the 
        /// specified name, and both null automaton nodes.
        /// </summary>
        /// <param name="name">Name.</param>
        public PowerSetAutomatonNode (string name) 
            : base(name)
        {}
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LtlSharp.Automata.ProductAutomatonNode`1"/> class with the
        /// specified name and label, and both null automaton nodes.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="labels">Labels.</param>
        public PowerSetAutomatonNode (string name, IEnumerable<ILiteral> labels)
            : base (name, labels)
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LtlSharp.Automata.ProductAutomatonNode`2"/> class with the
        /// specified name, label and nodes.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="labels">Labels.</param>
        /// <param name="node1">Node of the first automaton.</param>
        /// <param name="node2">Node of the second automaton.</param>
        public PowerSetAutomatonNode (string name, IEnumerable<ILiteral> labels, IEnumerable<T> nodes)
            : base (name, labels)
        {
            Nodes = new HashSet<T> (nodes);
        }
        
        public override string ToString ()
        {
            return string.Format ("[PowerSetAutomatonNode: Nodes={{{0}}}]", 
                                  string.Join (",", Nodes.Select (n => n.ToString ())));
        }

        public void AddNode (T node)
        {
            Nodes.Add (node);
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(PowerSetAutomatonNode<T>))
                return false;
            var other = (PowerSetAutomatonNode<T>)obj;
            return base.Equals (other) & Nodes.SetEquals (other.Nodes);
        }

        public override int GetHashCode ()
        {
            return 17 + base.GetHashCode () + 32 * Nodes.GetHashCodeForElements ();
        }
    }
}

