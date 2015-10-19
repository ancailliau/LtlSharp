using System;
using System.Collections.Generic;
using System.Linq;

namespace LtlSharp.Automata.Nodes.Factories
{
    public class PowerSetAutomatonNodeFactory<T> 
        : IAutomatonNodeFactory<PowerSetAutomatonNode<T>> 
        where T : IAutomatonNode
    {
        int currentId ;

        public PowerSetAutomatonNodeFactory ()
        {
            currentId = 0;
        }
        
        #region IAutomatonNodeFactory<PowerSetAutomatonNode<T>> Members
        
        PowerSetAutomatonNode<T> IAutomatonNodeFactory<PowerSetAutomatonNode<T>>.Create ()
        {
            return new PowerSetAutomatonNode<T> (currentId++);
        }
        
        PowerSetAutomatonNode<T> IAutomatonNodeFactory<PowerSetAutomatonNode<T>>.Create (string name)
        {
            return new PowerSetAutomatonNode<T> (currentId++, name);
        }
        
        PowerSetAutomatonNode<T> IAutomatonNodeFactory<PowerSetAutomatonNode<T>>.Create (string name, 
                                                                                         IEnumerable<ILiteral> labels)
        {
            return new PowerSetAutomatonNode<T> (currentId++, name, labels);
        }
        
        #endregion
        
        /// <summary>
        /// Creates a new automaton node with the specified nodes.
        /// </summary>
        /// <remarks>The name of the node is the name of the nodes separated by a comma; and the label of the node
        /// is union of the labels.</remarks>
        /// <param name="name">Name.</param>
        /// <param name="labels">Labels.</param>
        /// <param name="node1">Node of the first automaton.</param>
        /// <param name="node2">Node of the second automaton.</param>
        public PowerSetAutomatonNode<T> Create (IEnumerable<T> nodes)
        {
            return new PowerSetAutomatonNode<T> (
                currentId++,
                string.Join (",", nodes.Select (x => x.ToString ())),
                nodes.SelectMany (n => n.Labels).Distinct (),
                nodes
            );
        }
    }
}

