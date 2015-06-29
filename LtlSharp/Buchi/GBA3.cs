using System;
using System.Collections.Generic;

namespace LtlSharp.Buchi
{
    public class GBA3Node {
        public int id;
        public string name;
        public bool initial;
        public GBA3Node (int id, string name, bool initial)
        {
            this.id = id;
            this.name = name;
            this.initial = initial;
        }
    }
    
    public class AcceptanceSet {
        public int id;
        public int[] nodes;
        public AcceptanceSet (int id, int[] acceptance_nodes)
        {
            this.id = id;
            this.nodes = acceptance_nodes;
        }
        
    }
    
    public class GBA3
    {
        public GBA3 (int nnodes)
        {
            Nodes = new GBA3Node[nnodes];
            Transitions = new List<int>[nnodes];
            Labels = new List<ILiteral>[nnodes];
        }
        
        public GBA3Node[] Nodes;
        public List<int>[] Transitions; // probably better to use a sparse array instead.
        public List<ILiteral>[] Labels;
        public AcceptanceSet[] AcceptanceSets;
    }
}

