using System;
using QuickGraph;
using LtlSharp.Buchi.Automata;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;
using LtlSharp.Automata;
using LtlSharp.Automata.AcceptanceConditions;
using LtlSharp.Automata.OmegaAutomata;

namespace LtlSharp.Translators
{
    public class Unfold : Transformer<BuchiAutomaton<AutomatonNode>, BuchiAutomaton<AutomatonNode>>
    {
        public Unfold ()
        {
        }
        
        public override BuchiAutomaton<AutomatonNode> Transform (BuchiAutomaton<AutomatonNode> t)
        {
            var alphabet = t.Edges.SelectMany (x => x.Labels).Select (x => x is Negation ? ((ILiteral)((Negation)x).Enclosed) : x).Distinct ();
            return Transform (t, alphabet);
        }
        
        public BuchiAutomaton<AutomatonNode> Transform (BuchiAutomaton<AutomatonNode> t, IEnumerable<ILiteral> alphabet)
        {        
            var automata = new BuchiAutomaton<AutomatonNode> ();
            automata.AddNodes (t.Vertices);
            automata.SetAcceptanceCondition (new BuchiAcceptance<AutomatonNode>((BuchiAcceptance<AutomatonNode>) t.AcceptanceCondition));
            automata.SetInitialNode (t.InitialNode);

            foreach (var trans in t.Edges) {
                var labels = UnfoldLabels (trans.Labels, alphabet);
                foreach (var label in labels) {
                    automata.AddTransition (new AutomatonTransition<AutomatonNode>(trans.Source, trans.Target, label));
                }
            }
            return automata;
        }
        
        HashSet<HashSet<ILiteral>> UnfoldLabels (HashSet<ILiteral> trans, IEnumerable<ILiteral> alphabet)
        {
            var s = new HashSet<HashSet<ILiteral>> ();
            s.Add (new HashSet<ILiteral> ());

            var pending = new Stack<ILiteral> (alphabet);
            while (pending.Count > 0) {
                var current = pending.Pop ();
                if (trans.Contains (current)) {
                    foreach (var e in s) {
                        e.Add (current);
                    }

                } else if (trans.Contains (current.Negate ())) {
                    s = new HashSet<HashSet<ILiteral>> (s.Where (l => !l.Contains (current)));

                } else {
                    foreach (var e in s.ToList ()) {
                        var ns = new HashSet<ILiteral> (e);
                        ns.Add (current);
                        s.Add (ns);
                    }
                }
            }

            foreach (var a in alphabet) {
                foreach (var ss in s) {
                    if (!ss.Contains (a)) {
                        ss.Add ((ILiteral)a.Negate ());
                    }
                }
            }

            return s;
        }
    }
}

