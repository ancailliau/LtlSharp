using System;
using System.Collections.Generic;
using LtlSharp.Automata.Transitions;

namespace LtlSharp.Automata
{
    /// <summary>
    /// Defines a generic automaton node
    /// </summary>
    public interface IAutomatonNode
    {
        /// <summary>
        /// Gets the identifier of the node.
        /// </summary>
        /// <value>The identifier.</value>
        int Id { 
            get;
        }
        
        /// <summary>
        /// Gets or sets the name of the node.
        /// </summary>
        /// <value>The name.</value>
        string Name { 
            get; 
            set;
        }
        
        /// <summary>
        /// Gets the labels of the node.
        /// </summary>
        /// <value>The labels.</value>
        LiteralSet Labels {
            get;
        }
    }
}

