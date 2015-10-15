using System;
using QuickGraph;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;
using LtlSharp.Automata;
using LtlSharp.Automata.AcceptanceConditions;
using LtlSharp.Automata.OmegaAutomata;
using LtlSharp.Automata.Transitions;
using System.ComponentModel;
using LtlSharp.Automata.Nodes.Factories;
using LtlSharp.Automata.Transitions.Factories;

namespace LtlSharp.Translators
{
    [Obsolete]
    public class Unfold : Transformer<BuchiAutomaton<AutomatonNode>, BuchiAutomaton<AutomatonNode>>
    {
        [Obsolete]
        public Unfold ()
        {
        }
        
        [Obsolete]
        public override BuchiAutomaton<AutomatonNode> Transform (BuchiAutomaton<AutomatonNode> t)
        {
            var alphabet = t.Alphabet ();
            return Transform (t, alphabet);
        }
        
        [Obsolete()]
        public BuchiAutomaton<AutomatonNode> Transform (BuchiAutomaton<AutomatonNode> t, IEnumerable<ILiteral> alphabet)
        {        
            var automata = new BuchiAutomaton<AutomatonNode> (new AutomatonNodeFactory (), new LiteralSetFactory ());
            automata.AddNodes (t.Nodes);
            automata.SetAcceptanceCondition (new BuchiAcceptance<AutomatonNode>((BuchiAcceptance<AutomatonNode>) t.AcceptanceCondition));
            automata.SetInitialNode (t.InitialNode);

            automata.MapLabel ((arg) => UnfoldLabels (arg, alphabet));
            
            return automata;
        }
        
        [Obsolete()]
        HashSet<LiteralsSet> UnfoldLabels (LiteralsSet trans, IEnumerable<ILiteral> alphabet)
        {
            return null;
        }
    }
}

