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
    
    public class GeneralizedBuchiAutomata : OmegaAutomaton<AutomatonNode, LiteralSetDecoration>
    {
        public override AcceptanceConditions.IAcceptanceCondition<AutomatonNode> AcceptanceCondition {
            get {
                throw new NotImplementedException ();
            }
        }
        
        public GBAAcceptanceSet[] AcceptanceSets;
        public HashSet<AutomatonNode> InitialNodes;
        
        public GeneralizedBuchiAutomata (int n_nodes)
            : base (new AutomatonNodeFactory (), new LiteralSetDecorationFactory ())
        {
            InitialNodes = new HashSet<AutomatonNode> ();
        }

        public override Automata<AutomatonNode, LiteralSetDecoration> Clone ()
        {
            throw new NotImplementedException ();
        }
    }

    public class TransitionGeneralizedBuchiAutomata : OmegaAutomaton<AutomatonNode, LiteralSetDecoration>
    {
        public override AcceptanceConditions.IAcceptanceCondition<AutomatonNode> AcceptanceCondition {
            get {
                throw new NotImplementedException ();
            }
        }
        public GBAAcceptanceSet[] AcceptanceSets;
        public HashSet<AutomatonNode> InitialNodes;

        public TransitionGeneralizedBuchiAutomata ()
            : base (new AutomatonNodeFactory (), new LiteralSetDecorationFactory ())
        {
            InitialNodes = new HashSet<AutomatonNode> ();
        }

        public IEnumerable<Tuple<LiteralSetDecoration, AutomatonNode>> OutTransitions (AutomatonNode root)
        {
            return graph.OutEdges (root).Select (x => new Tuple<LiteralSetDecoration, AutomatonNode> (x.Value, x.Target));
        }

        public override Automata<AutomatonNode, LiteralSetDecoration> Clone ()
        {
            throw new NotImplementedException ();
        }
    }


    public class TGBA : OmegaAutomaton<AutomatonNode, LiteralSetDecoration>
    {
        public override AcceptanceConditions.IAcceptanceCondition<AutomatonNode> AcceptanceCondition {
            get {
                throw new NotImplementedException ();
            }
        }
        public TGBAAcceptanceSet [] AcceptanceSets;
        public HashSet<AutomatonNode> InitialNodes;

        public TGBA ()
            : base (new AutomatonNodeFactory (), new LiteralSetDecorationFactory ())
        {
            InitialNodes = new HashSet<AutomatonNode> ();
        }

        internal IEnumerable<Tuple<AutomatonNode, AutomatonNode, LiteralSetDecoration>> OutEdges (AutomatonNode n0)
        {
            return graph.OutEdges (n0).Select (x => new Tuple<AutomatonNode, AutomatonNode, LiteralSetDecoration> (n0, x.Target, x.Value));
        }

        public override Automata<AutomatonNode, LiteralSetDecoration> Clone ()
        {
            throw new NotImplementedException ();
        }
    }
    
    public class TGBAAcceptanceSet {
        public int Id;
        public HashSet<Tuple<AutomatonNode, AutomatonNode, LiteralSetDecoration>> Transitions;
        public TGBAAcceptanceSet (int id)
        {
            Id = id;
            Transitions = new HashSet<Tuple<AutomatonNode, AutomatonNode, LiteralSetDecoration>> ();
        }
        public void Add (Tuple<AutomatonNode, AutomatonNode, LiteralSetDecoration> n)
        {
            Transitions.Add (n);
        }
    }
}

