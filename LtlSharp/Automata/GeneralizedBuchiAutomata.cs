using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LtlSharp.Automata;
using LtlSharp.Buchi.Automata;
using QuickGraph;

namespace LtlSharp.Buchi
{
    public class GBAAcceptanceSet {
        public int Id;
        public AutomatonNode[] Nodes;
        public GBAAcceptanceSet (int id, AutomatonNode[] acceptance_nodes)
        {
            Id = id;
            Nodes = acceptance_nodes;
        }
        
    }
    
    public class GeneralizedBuchiAutomata : AdjacencyGraph<LabelledAutomataNode, AutomatonTransition<LabelledAutomataNode>>
    {
        public GBAAcceptanceSet[] AcceptanceSets;
        public HashSet<AutomatonNode> InitialNodes;
        
        public GeneralizedBuchiAutomata (int n_nodes)
        {
            InitialNodes = new HashSet<AutomatonNode> ();
        }
    }

    public class TransitionGeneralizedBuchiAutomata : AdjacencyGraph<AutomatonNode, LabeledAutomataTransition<AutomatonNode>>
    {
        public GBAAcceptanceSet[] AcceptanceSets;
        public HashSet<AutomatonNode> InitialNodes;

        public TransitionGeneralizedBuchiAutomata ()
        {
            InitialNodes = new HashSet<AutomatonNode> ();
        }
    }
    
    
    public class TGBA : AdjacencyGraph<AutomatonNode, LabeledAutomataTransition<AutomatonNode>>
    {
        public TGBAAcceptanceSet[] AcceptanceSets;
        public HashSet<AutomatonNode> InitialNodes;

        public TGBA ()
        {
            InitialNodes = new HashSet<AutomatonNode> ();
        }
    }
    
    public class TGBAAcceptanceSet {
        public int Id;
        public HashSet<LabeledAutomataTransition<AutomatonNode>> Transitions;
        public TGBAAcceptanceSet (int id)
        {
            Id = id;
            Transitions = new HashSet<LabeledAutomataTransition<AutomatonNode>> ();
        }
        public void Add (LabeledAutomataTransition<AutomatonNode> n)
        {
            Transitions.Add (n);
        }
    }
}

