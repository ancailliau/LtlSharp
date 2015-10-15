using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LtlSharp.Automata;
using QuickGraph;

namespace LtlSharp.Automata.OmegaAutomata
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
    
    public class GeneralizedBuchiAutomata : AdjacencyGraph<AutomatonNode, AutomatonTransition<AutomatonNode>>
    {
        public GBAAcceptanceSet[] AcceptanceSets;
        public HashSet<AutomatonNode> InitialNodes;
        
        public GeneralizedBuchiAutomata (int n_nodes)
        {
            InitialNodes = new HashSet<AutomatonNode> ();
        }
    }

    public class TransitionGeneralizedBuchiAutomata : AdjacencyGraph<AutomatonNode, AutomatonTransition<AutomatonNode>>
    {
        public GBAAcceptanceSet[] AcceptanceSets;
        public HashSet<AutomatonNode> InitialNodes;

        public TransitionGeneralizedBuchiAutomata ()
        {
            InitialNodes = new HashSet<AutomatonNode> ();
        }
    }
    
    
    public class TGBA : AdjacencyGraph<AutomatonNode, AutomatonTransition<AutomatonNode>>
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
        public HashSet<AutomatonTransition<AutomatonNode>> Transitions;
        public TGBAAcceptanceSet (int id)
        {
            Id = id;
            Transitions = new HashSet<AutomatonTransition<AutomatonNode>> ();
        }
        public void Add (AutomatonTransition<AutomatonNode> n)
        {
            Transitions.Add (n);
        }
    }
}

