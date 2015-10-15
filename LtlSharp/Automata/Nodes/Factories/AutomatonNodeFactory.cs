using System;
using System.Collections.Generic;

namespace LtlSharp.Automata.Nodes.Factories
{
    public class AutomatonNodeDefaultFactory : IAutomatonNodeFactory<AutomatonNode>
    {
        public AutomatonNode Create ()
        {
            return new AutomatonNode ();
        }
        public AutomatonNode Create (string name)
        {
            return new AutomatonNode (name);
        }
        public AutomatonNode Create (string name, IEnumerable<ILiteral> labels)
        {
            return new AutomatonNode (name, labels);
        }
    }
}

