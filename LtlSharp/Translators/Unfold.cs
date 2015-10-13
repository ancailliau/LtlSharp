using System;
using QuickGraph;
using LtlSharp.Buchi.Automata;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;
using LtlSharp.Automata;

namespace LtlSharp.Translators
{
    public class Unfold : Transformer<BuchiAutomata, BuchiAutomata>
    {
        public Unfold ()
        {
        }
        
        public override BuchiAutomata Transform (BuchiAutomata t)
        {
            var alphabet = t.Edges.SelectMany (x => x.Labels).Select (x => x is Negation ? ((ILiteral)((Negation)x).Enclosed) : x).Distinct ();
            return Transform (t, alphabet);
        }
        
        public BuchiAutomata Transform (BuchiAutomata t, IEnumerable<ILiteral> alphabet)
        {        
            var automata = new BuchiAutomata ();
            automata.AddNodes (t.Vertices);
            automata.SetAcceptanceCondition (new BuchiAcceptance<AutomataNode> ((BuchiAcceptance<AutomataNode>) t.AcceptanceCondition));
            automata.SetInitialNode (t.InitialNode);

            foreach (var trans in t.Edges) {
                var labels = UnfoldLabels (trans.Labels, alphabet);
                foreach (var label in labels) {
                    automata.AddTransition (new LabeledAutomataTransition<AutomataNode> (trans.Source, trans.Target, label));
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

