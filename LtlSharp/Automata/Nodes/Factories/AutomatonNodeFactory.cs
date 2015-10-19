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
        int currentId ;
        
        public AutomatonNodeFactory ()
        {
            currentId = 0;
        }
        
        #region IAutomatonNodeFactory<T> Members
        
        AutomatonNode IAutomatonNodeFactory<AutomatonNode>.Create ()
        {
            return new AutomatonNode (currentId++);
        }
        AutomatonNode IAutomatonNodeFactory<AutomatonNode>.Create (string name)
        {
            return new AutomatonNode (currentId++, name);
        }
        AutomatonNode IAutomatonNodeFactory<AutomatonNode>.Create (string name, IEnumerable<ILiteral> labels)
        {
            return new AutomatonNode (currentId++, name, labels);
        }
        
        #endregion
    }
}

