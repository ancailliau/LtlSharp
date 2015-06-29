using System;
using System.Collections.Generic;

namespace LtlSharp.Buchi
{
    public class BANode {
        public int Id;
        public string Name;
        public bool Initial;
        public BANode (int id, string name, bool initial)
        {
            this.Id = id;
            this.Name = name;
            this.Initial = initial;
        }
    }
        
    public class BuchiAutomata
    {
        public BANode[] Nodes;
        public List<int>[] Transitions; // probably better to use a sparse array instead.
        public List<ILiteral>[] Labels;
        public int[] AcceptanceSet;
        
        public BuchiAutomata (int n_nodes)
        {
            Nodes = new BANode[n_nodes];
            Transitions = new List<int>[n_nodes];
            Labels = new List<ILiteral>[n_nodes];
        }
    }
}

