using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LtlSharp.Buchi.Automata;

namespace LtlSharp.Buchi
{
    public class BuchiAutomata
    {
        public IEnumerable<AutomataNode> Nodes {
            get {
                return Transitions.Keys;
            }
        }
        
        public Dictionary<AutomataNode, HashSet<AutomataTransition>> Transitions;
        
        public HashSet<AutomataNode> AcceptanceSet;
        
        public BuchiAutomata (int n_nodes)
        {
            Transitions = new Dictionary<AutomataNode, HashSet<AutomataTransition>> ();
            AcceptanceSet = new HashSet<AutomataNode> ();
        }
    }
}

