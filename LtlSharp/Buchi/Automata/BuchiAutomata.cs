using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LtlSharp.Buchi.Automata;
using QuickGraph;

namespace LtlSharp.Buchi
{
    public class BuchiAutomata : AdjacencyGraph<AutomataNode, LabeledAutomataTransition<AutomataNode>> 
    {
        public HashSet<AutomataNode> InitialNodes;
        public HashSet<AutomataNode> AcceptanceSet;
        
        public BuchiAutomata () : base ()
        {
            InitialNodes = new HashSet<AutomataNode> ();
            AcceptanceSet = new HashSet<AutomataNode> ();
        }
    }
    
    public class DegeneralizerBuchiAutomata : AdjacencyGraph<AutomataNode, DegeneralizerAutomataTransition<AutomataNode>> 
    {
        public HashSet<AutomataNode> InitialNodes;
        public HashSet<AutomataNode> AcceptanceSet;

        public DegeneralizerBuchiAutomata () : base ()
        {
            InitialNodes = new HashSet<AutomataNode> ();
            AcceptanceSet = new HashSet<AutomataNode> ();
        }
    }
}

