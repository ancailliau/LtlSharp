using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LtlSharp.Automata;
using QuickGraph;
using LtlSharp.Automata.Nodes.Factories;
using LtlSharp.Automata.Transitions;
using LtlSharp.Utils;


namespace LtlSharp.Automata.OmegaAutomata
{

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
            : base (new AutomatonNodeFactory ())
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

