using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LtlSharp.Buchi.Automata;
using QuickGraph;

namespace LtlSharp.Buchi
{
    public class BuchiAutomata : AdjacencyGraph<AutomataNode, AutomataTransition> 
    {
        public HashSet<AutomataNode> InitialNodes;
        public HashSet<AutomataNode> AcceptanceSet;
        
        public BuchiAutomata () : base ()
        {
            InitialNodes = new HashSet<AutomataNode> ();
            AcceptanceSet = new HashSet<AutomataNode> ();
        }
    }
}

