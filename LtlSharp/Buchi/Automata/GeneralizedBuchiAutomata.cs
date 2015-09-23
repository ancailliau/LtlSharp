using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LtlSharp.Buchi.Automata;

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
    
    public class GeneralizedBuchiAutomata
    {
        public IEnumerable<AutomataNode> Nodes {
            get {
                return Transitions.Keys;
            }
        }
        
        public Dictionary<AutomataNode, HashSet<AutomataTransition>> Transitions; // probably better to use a sparse array instead.
        public GBAAcceptanceSet[] AcceptanceSets;
        
        public GeneralizedBuchiAutomata (int n_nodes)
        {
            //Nodes = new AutomataNode[n_nodes];
            Transitions = new Dictionary<AutomataNode, HashSet<AutomataTransition>> ();
        }
    }
}

