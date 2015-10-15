using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Automata.Transitions;

namespace LtlSharp.Automata
{
    /// <summary>
    /// Represents a node of a product automaton between two automaton
    /// </summary>
    public class ProductAutomatonNode<T1, T2>
        : AutomatonNode 
        where T1 : IAutomatonNode 
        where T2 : IAutomatonNode
    {
        
        /// <summary>
        /// Gets or sets the automaton node of the first automaton
        /// </summary>
        /// <value>The markov node.</value>
        public T1 Node1 {
            get;
            protected set;
        }
        
        /// <summary>
        /// Gets or sets the automaton node of the second automaton
        /// </summary>
        /// <value>The omega automaton node.</value>
        public T2 Node2 {
            get;
            protected set;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LtlSharp.Automata.ProductAutomatonNode`1"/> class with
        /// both null automaton nodes.
        /// </summary>
        public ProductAutomatonNode () 
            : base ()
        {}
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LtlSharp.Automata.ProductAutomatonNode`1"/> class with the 
        /// specified name, and both null automaton nodes.
        /// </summary>
        /// <param name="name">Name.</param>
        public ProductAutomatonNode (string name) 
            : base(name)
        {}
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LtlSharp.Automata.ProductAutomatonNode`1"/> class with the
        /// specified name and label, and both null automaton nodes.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="labels">Labels.</param>
        public ProductAutomatonNode (string name, IEnumerable<ILiteral> labels)
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
        public ProductAutomatonNode (string name, IEnumerable<ILiteral> labels, T1 node1, T2 node2)
            : base (name, labels)
        {
            Node1 = node1;
            Node2 = node2;
        }

        /// <summary>
        /// Sets the nodes.
        /// </summary>
        /// <returns>The nodes.</returns>
        /// <param name="node1">Node 1.</param>
        /// <param name="node2">Node 2.</param>
        public void SetNodes (T1 node1, T2 node2)
        {
            Node1 = node1;
            Node2 = node2;
        }
        
        public override string ToString ()
        {
            return string.Format ("[ProductAutomatonNode: Node1={0}, Node2={1}]", Node1.Name, Node2.Name);
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(ProductAutomatonNode<T1,T2>))
                return false;
            var other = (ProductAutomatonNode<T1,T2>)obj;
            return base.Equals (other) & Node1.Equals (other.Node1) & Node2.Equals (other.Node2);
        }

        public override int GetHashCode ()
        {
            return 17 + base.GetHashCode () + 32 * (
                ((Node1 != null) ? Node1.GetHashCode () : 0) 
                + 32 * ((Node2 != null) ? Node2.GetHashCode () : 0));
        }
    }
}

