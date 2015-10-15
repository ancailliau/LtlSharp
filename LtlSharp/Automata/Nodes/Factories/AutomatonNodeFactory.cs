using System;
using System.Collections.Generic;

namespace LtlSharp.Automata.Nodes.Factories
{
    /// <summary>
    /// Defines a factory for creating automaton nodes of type <c>AutomatonNode</c>.
    /// </summary>
    public class AutomatonNodeFactory 
        : IAutomatonNodeFactory<AutomatonNode>
    {
        #region IAutomatonNodeFactory<T> Members
        
        AutomatonNode IAutomatonNodeFactory<AutomatonNode>.Create ()
        {
            return new AutomatonNode ();
        }
        AutomatonNode IAutomatonNodeFactory<AutomatonNode>.Create (string name)
        {
            return new AutomatonNode (name);
        }
        AutomatonNode IAutomatonNodeFactory<AutomatonNode>.Create (string name, IEnumerable<ILiteral> labels)
        {
            return new AutomatonNode (name, labels);
        }
        
        #endregion
    }
}

