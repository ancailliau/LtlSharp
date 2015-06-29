using System;
using System.Collections.Generic;

namespace LtlSharp.Buchi
{
    public class GBANode {
        public int Id;
        public string Name;
        public bool Initial;
        public GBANode (int id, string name, bool initial)
        {
            this.Id = id;
            this.Name = name;
            this.Initial = initial;
        }
    }
    
    public class GBAAcceptanceSet {
        public int Id;
        public int[] Nodes;
        public GBAAcceptanceSet (int id, int[] acceptance_nodes)
        {
            this.Id = id;
            this.Nodes = acceptance_nodes;
        }
        
    }
    
    public class GeneralizedBuchiAutomata
    {
        public GBANode[] Nodes;
        public List<int>[] Transitions; // probably better to use a sparse array instead.
        public List<ILiteral>[] Labels;
        public GBAAcceptanceSet[] AcceptanceSets;
        
        public GeneralizedBuchiAutomata (int n_nodes)
        {
            Nodes = new GBANode[n_nodes];
            Transitions = new List<int>[n_nodes];
            Labels = new List<ILiteral>[n_nodes];
        }
    }
}

