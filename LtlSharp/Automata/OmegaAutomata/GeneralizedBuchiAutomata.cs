using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LtlSharp.Automata;
using QuickGraph;
using LtlSharp.Automata.Nodes.Factories;
using LtlSharp.Automata.Transitions;
using LtlSharp.Utils;
using LtlSharp.Automata.Transitions.Factories;

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
        public override string ToString ()
        {
            return string.Format ("[GBAAcceptanceSet {{{0}}}]", string.Join (",", Nodes.AsEnumerable ()));
        }
    }
    
    public class GeneralizedBuchiAutomata : OmegaAutomaton<AutomatonNode, LiteralsSet>
    {
        public override AcceptanceConditions.IAcceptanceCondition<AutomatonNode> AcceptanceCondition {
            get {
                throw new NotImplementedException ();
            }
        }
        
        public GBAAcceptanceSet[] AcceptanceSets;
        public HashSet<AutomatonNode> InitialNodes;
        
        public GeneralizedBuchiAutomata (int n_nodes)
            : base (new AutomatonNodeFactory (), new LiteralSetFactory ())
        {
            InitialNodes = new HashSet<AutomatonNode> ();
        }
    }

    public class TransitionGeneralizedBuchiAutomata : OmegaAutomaton<AutomatonNode, LiteralsSet>
    {
        public override AcceptanceConditions.IAcceptanceCondition<AutomatonNode> AcceptanceCondition {
            get {
                throw new NotImplementedException ();
            }
        }
        public GBAAcceptanceSet[] AcceptanceSets;
        public HashSet<AutomatonNode> InitialNodes;

        public TransitionGeneralizedBuchiAutomata ()
            : base (new AutomatonNodeFactory (), new LiteralSetFactory ())
        {
            InitialNodes = new HashSet<AutomatonNode> ();
        }

        public IEnumerable<Tuple<LiteralsSet, AutomatonNode>> OutTransitions (AutomatonNode root)
        {
            return graph.OutEdges (root).Select (x => new Tuple<LiteralsSet, AutomatonNode> (x.Value, x.Target));
        }
    }


    public class TGBA : OmegaAutomaton<AutomatonNode, LiteralsSet>
    {
        public override AcceptanceConditions.IAcceptanceCondition<AutomatonNode> AcceptanceCondition {
            get {
                throw new NotImplementedException ();
            }
        }
        public TGBAAcceptanceSet [] AcceptanceSets;
        public HashSet<AutomatonNode> InitialNodes;

        public TGBA ()
            : base (new AutomatonNodeFactory (), new LiteralSetFactory ())
        {
            InitialNodes = new HashSet<AutomatonNode> ();
        }

        internal IEnumerable<Tuple<AutomatonNode, AutomatonNode, LiteralsSet>> OutEdges (AutomatonNode n0)
        {
            return graph.OutEdges (n0).Select (x => new Tuple<AutomatonNode, AutomatonNode, LiteralsSet> (n0, x.Target, x.Value));
        }
    }
    
    public class TGBAAcceptanceSet {
        public int Id;
        public HashSet<Tuple<AutomatonNode, AutomatonNode, LiteralsSet>> Transitions;
        public TGBAAcceptanceSet (int id)
        {
            Id = id;
            Transitions = new HashSet<Tuple<AutomatonNode, AutomatonNode, LiteralsSet>> ();
        }
        public void Add (Tuple<AutomatonNode, AutomatonNode, LiteralsSet> n)
        {
            Transitions.Add (n);
        }
    }
}

