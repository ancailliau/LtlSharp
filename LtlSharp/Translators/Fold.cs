using System;
using QuickGraph;
using LtlSharp.Buchi.Automata;
using System.Linq;
using LtlSharp.Language;
using LtlSharp.Automata;

namespace LtlSharp.Translators
{
    public class Fold<T> : Transformer<T,T>
        where T : OmegaAutomaton
    {
        public Fold ()
        {
        }
        
        public override T Transform (T target)
        {
            foreach (var node in target.Vertices) {
                var transitions = target.OutTransitions (node);
                foreach (var trans in transitions.ToList ()) {
                    var sameTarget = transitions.Where (t => t.Target.Equals (trans.Target)).ToList ();
                    var labels = sameTarget.Select (x => x.Labels);
                    var lf = new LiteralFormula (labels);
                    var newLabels = lf.Simplify ();
                    foreach (var e in sameTarget) {
                        target.RemoveEdge (e);
                    }
                    foreach (var nl in newLabels) {
                        target.AddEdge (new LabeledAutomataTransition<AutomataNode> (trans.Source, trans.Target, nl));
                    }
                }
            }

            return target;
        }
        
    }
}

