using System;
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
    }
}

