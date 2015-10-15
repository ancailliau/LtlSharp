using System;
using System.Collections.Generic;
using System.Linq;

namespace LtlSharp.Automata.Nodes.Factories
{
    /// <summary>
    /// Defines a generic interface for factories creating product automaton nodes.
    /// </summary>
    /// <typeparam name="T1">Type of the node of the first automaton</typeparam>
    /// <typeparam name="T2">Type of the node of the second automaton</typeparam>
    public class AutomatonNodeProductFactory<T1, T2> 
        : IAutomatonNodeFactory<ProductAutomatonNode<T1, T2>> 
        where T1 : IAutomatonNode
        where T2 : IAutomatonNode
    {
        #region IAutomatonNodeFactory<ProductAutomatonNode<T1, T2>> Members
        
        ProductAutomatonNode<T1, T2> IAutomatonNodeFactory<ProductAutomatonNode<T1, T2>>.Create ()
        {
            return new ProductAutomatonNode<T1, T2> ();
        }
        
        ProductAutomatonNode<T1, T2> IAutomatonNodeFactory<ProductAutomatonNode<T1, T2>>.Create (string name)
        {
            return new ProductAutomatonNode<T1, T2> (name);
        }
        
        ProductAutomatonNode<T1, T2> IAutomatonNodeFactory<ProductAutomatonNode<T1, T2>>.Create (string name, 
                                                                                                 IEnumerable<ILiteral> labels)
        {
            return new ProductAutomatonNode<T1, T2> (name, labels);
        }
        
        #endregion
        
        /// <summary>
        /// Creates a new automaton node with the specified nodes.
        /// </summary>
        /// <remarks>The name of the node is the name of the nodes separated by a cross; and the label of the node
        /// is union of the labels.</remarks>
        /// <param name="name">Name.</param>
        /// <param name="labels">Labels.</param>
        /// <param name="node1">Node of the first automaton.</param>
        /// <param name="node2">Node of the second automaton.</param>
        public ProductAutomatonNode<T1, T2> Create (T1 node1, T2 node2)
        {
            return new ProductAutomatonNode<T1, T2> (string.Format ("{0} x {1}", node1.Name, node2.Name),
                                                     node1.Labels.Union (node2.Labels).Distinct(), 
                                                     node1,
                                                     node2);
        }
    }
}

