using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Buchi;
using LtlSharp.Buchi.Automata;
using LtlSharp.Buchi.LTL2Buchi;
using LtlSharp.Buchi.Translators;
using QuickGraph.Graphviz;

namespace LtlSharp.Monitoring
{
    public enum MonitorStatus {
        True, False, Inconclusive
    }
    
    public class LTLMonitor
    {
        public AutomataNode currentNegative;
        public AutomataNode currentPositive;
        public NFA negativeNFA;
        public NFA positiveNFA;
        ILTLTranslator translator = new Gia02 ();

        public MonitorStatus Status { get; private set; }
        
        public LTLMonitor (ILTLFormula formula)
        {
            positiveNFA = BA2NFA.Transform (translator.GetAutomaton (formula));
            negativeNFA = BA2NFA.Transform (translator.GetAutomaton (formula.Negate ()));

            positiveNFA = positiveNFA.Determinize ();
            negativeNFA = negativeNFA.Determinize ();
            
            currentNegative = negativeNFA.InitialNodes.Single ();
            currentPositive = positiveNFA.InitialNodes.Single ();
            
            UpdateStatus ();
        }

        public void Step (MonitoredState state)
        {
            if (currentNegative == null | currentPositive == null)
                return;

            var transitions = positiveNFA.OutEdges (currentPositive).Where (t => state.Evaluate (t.Labels));
            //Console.WriteLine ("--> " + state);
            //Console.WriteLine (string.Join ("\n", transitions));
            //Console.WriteLine ("<--");
            if (transitions.Count () == 1) {
                currentPositive = transitions.Single ().Target;

            } else if (transitions.Count () == 0) {
                // There is no way to satisfy the formula
                Console.WriteLine ("No out transition in positive NFA");
                currentPositive = null;
                Status = MonitorStatus.False;
                return;

            } else {
                throw new NotImplementedException ("Non deterministic automata not supported.");
            }

            transitions = negativeNFA.OutEdges (currentNegative).Where (t => state.Evaluate (t.Labels));
            if (transitions.Count () == 1) {
                currentNegative = transitions.Single ().Target;

            } else if (transitions.Count () == 0) {
                Console.WriteLine ("No out transition in negative NFA");
                // There is no way to dissatisfy the formula
                currentNegative = null;
                Status = MonitorStatus.True;
                return;

            } else {
                throw new NotImplementedException ("Non deterministic automata not supported.");
            }

            UpdateStatus ();
        }

        void UpdateStatus ()
        {
            var negativeAcceptance = negativeNFA.AcceptanceSet.Contains (currentNegative);
            var positiveAcceptance = positiveNFA.AcceptanceSet.Contains (currentPositive);
            if (negativeAcceptance & positiveAcceptance) {
                Status = MonitorStatus.Inconclusive;
            } else if (!negativeAcceptance) {
                Status = MonitorStatus.True;
            } else if (!positiveAcceptance) {
                Status = MonitorStatus.False;
            } else {
                throw new NotImplementedException ();
            }
        }

        public void PrintDot ()
        {
            var product = NFA.Product (positiveNFA, negativeNFA);

            var graphviz = GetEngine (product);
            var output = graphviz.Generate ();
            Console.WriteLine (output);
        }

        GraphvizAlgorithm<MonitorNode, LabeledAutomataTransition<MonitorNode>> GetEngine (NFAProduct automata)
        {
            var graphviz = new GraphvizAlgorithm<MonitorNode, LabeledAutomataTransition<MonitorNode>> (automata);
            graphviz.FormatVertex += (object sender, FormatVertexEventArgs<MonitorNode> e) => {
                e.VertexFormatter.Label = e.Vertex.Status.ToString ();
                if (automata.InitialNodes.Contains (e.Vertex)) {
                    e.VertexFormatter.Style = QuickGraph.Graphviz.Dot.GraphvizVertexStyle.Bold;
                }
            };
            graphviz.FormatEdge += (object sender, FormatEdgeEventArgs<MonitorNode, LabeledAutomataTransition<MonitorNode>> e) => {
                e.EdgeFormatter.Label.Value = string.Join (",", e.Edge.Labels);
            };
            return graphviz;
        }
    }
}

