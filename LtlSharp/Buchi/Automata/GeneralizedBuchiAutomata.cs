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
    
    public class GeneralizedBuchiAutomata : AdjacencyGraph<LabelledAutomataNode, AutomataTransition<LabelledAutomataNode>>
    {
        public GBAAcceptanceSet[] AcceptanceSets;
        public HashSet<AutomataNode> InitialNodes;
        
        public GeneralizedBuchiAutomata (int n_nodes)
        {
            InitialNodes = new HashSet<AutomataNode> ();
        }
    }

    public class TransitionGeneralizedBuchiAutomata : AdjacencyGraph<AutomataNode, LabeledAutomataTransition<AutomataNode>>
    {
        public GBAAcceptanceSet[] AcceptanceSets;
        public HashSet<AutomataNode> InitialNodes;

        public TransitionGeneralizedBuchiAutomata ()
        {
            InitialNodes = new HashSet<AutomataNode> ();
        }
    }
    
    
    public class TGBA : AdjacencyGraph<AutomataNode, LabeledAutomataTransition<AutomataNode>>
    {
        public TGBAAcceptanceSet[] AcceptanceSets;
        public HashSet<AutomataNode> InitialNodes;

        public TGBA ()
        {
            InitialNodes = new HashSet<AutomataNode> ();
        }
    }
    
    public class TGBAAcceptanceSet {
        public int Id;
        public HashSet<LabeledAutomataTransition<AutomataNode>> Transitions;
        public TGBAAcceptanceSet (int id)
        {
            Id = id;
            Transitions = new HashSet<LabeledAutomataTransition<AutomataNode>> ();
        }
        public void Add (LabeledAutomataTransition<AutomataNode> n)
        {
            Transitions.Add (n);
        }
    }
}

