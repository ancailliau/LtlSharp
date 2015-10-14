using System;
using QuickGraph;
using LtlSharp.Buchi.Automata;
using System.Linq;
using LtlSharp.Language;
using LtlSharp.Automata;
using LtlSharp.Automata.OmegaAutomata;

namespace LtlSharp.Translators
{
    public class Fold<T,T2> : Transformer<T,T>
        where T : OmegaAutomaton<T2>
        where T2 : IAutomatonNode
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
                        target.RemoveTransition (e);
                    }
                    foreach (var nl in newLabels) {
                        target.AddTransition (new AutomatonTransition<T2> (trans.Source, trans.Target, nl));
                    }
                }
            }

            return target;
        }
        
    }
}

