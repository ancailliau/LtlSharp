using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LtlSharp.Buchi.Automata;
using QuickGraph;

namespace LtlSharp.Buchi
{
    public class GBAAcceptanceSet {
        public int Id;
        public AutomataNode[] Nodes;
        public GBAAcceptanceSet (int id, AutomataNode[] acceptance_nodes)
        {
            Id = id;
            Nodes = acceptance_nodes;
        }
        
    }
    
    public class GeneralizedBuchiAutomata : AdjacencyGraph<AutomataNode, AutomataTransition>
    {
        public GBAAcceptanceSet[] AcceptanceSets;
        
        public GeneralizedBuchiAutomata (int n_nodes)
        {
        }
    }
}

