using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Buchi.Automata;
using LtlSharp.Buchi.LTL2Buchi;
using LtlSharp.Buchi.Translators;
using QuickGraph.Graphviz;

namespace LtlSharp.Monitoring
{
    
    public class LTLMonitorHard
    {
        HashSet<AutomataNode> currentPositive;
        HashSet<AutomataNode> currentNegative;
        
        NFA positiveNFA;
        NFA negativeNFA;

        public MonitorStatus Status;

        public LTLMonitorHard (ILTLFormula formula)
        {
            /*
            var ltl2buchi = new GPVW ();
            var positiveGBA = ltl2buchi.GetAutomaton (formula);
            var negativeGBA = ltl2buchi.GetAutomaton (formula.Negate ());
            
            var positiveBA = GBA2BA.Transform (positiveGBA);
            var negativeBA = GBA2BA.Transform (negativeGBA);

            positiveNFA = BA2NFA.Transform (positiveBA);
            negativeNFA = BA2NFA.Transform (negativeBA);

            positiveNFA.ToSingleInitialState ();
            negativeNFA.ToSingleInitialState ();

            currentNegative = new HashSet<AutomataNode> (negativeNFA.InitialNodes);
            currentPositive = new HashSet<AutomataNode> (positiveNFA.InitialNodes);
            
            var graphviz = new GraphvizAlgorithm<AutomataNode,AutomataTransition>(positiveNFA);
            graphviz.FormatVertex += (object sender, FormatVertexEventArgs<AutomataNode> e) => {
                e.VertexFormatter.Label = e.Vertex.Name + "(" + string.Join (",", e.Vertex.Labels) + ")";;
                if (positiveNFA.InitialNodes.Contains (e.Vertex)) {
                    e.VertexFormatter.FillColor = QuickGraph.Graphviz.Dot.GraphvizColor.LightYellow;
                }
            };
            graphviz.FormatEdge += (object sender, FormatEdgeEventArgs<AutomataNode, AutomataTransition> e) => {
                e.EdgeFormatter.Label.Value = string.Join (",", e.Edge.Labels);
            };
            string output = graphviz.Generate();
            Console.WriteLine (output);
            
            
            graphviz = new GraphvizAlgorithm<AutomataNode,AutomataTransition>(negativeNFA);
            graphviz.FormatVertex += (object sender, FormatVertexEventArgs<AutomataNode> e) => {
                e.VertexFormatter.Label = e.Vertex.Name + "(" + string.Join (",", e.Vertex.Labels) + ")";
                if (positiveNFA.InitialNodes.Contains (e.Vertex)) {
                    e.VertexFormatter.FillColor = QuickGraph.Graphviz.Dot.GraphvizColor.LightYellow;
                }
            };
            graphviz.FormatEdge += (object sender, FormatEdgeEventArgs<AutomataNode, AutomataTransition> e) => {
                e.EdgeFormatter.Label.Value = string.Join (",", e.Edge.Labels);
            };
            output = graphviz.Generate();
            Console.WriteLine (output);
            
            Console.WriteLine (string.Join ("/", positiveNFA.AcceptanceSet.Select (x => x.Name)));
            Console.WriteLine (string.Join ("/", negativeNFA.AcceptanceSet.Select (x => x.Name)));
            
            var negativeAcceptance = currentNegative.All (t => !negativeNFA.AcceptanceSet.Contains (t));
            var positiveAcceptance = currentPositive.All (t => !positiveNFA.AcceptanceSet.Contains (t));
            Status = MonitorStatus.Inconclusive;
            if (negativeAcceptance)
                Status = MonitorStatus.False;
            if (positiveAcceptance)
                Status = MonitorStatus.True;
                */
        }
        
        public void Consume (MonitoredState state)
        {
            foreach (var current in currentPositive.ToList ()) {
                var transitions = positiveNFA.OutEdges (current).Where (t => state.Evaluate (t.Labels));
                currentPositive.Remove (current);
                foreach (var nt in transitions.Select (t => t.Target)) {
                    currentPositive.Add (nt);
                }
            }
            Console.WriteLine ("Positive: " + string.Join (",", currentPositive.Select (t => t.Name)));
            
            foreach (var current in currentNegative.ToList ()) {
                var transitions = negativeNFA.OutEdges (current).Where (t => state.Evaluate (t.Labels));
                currentNegative.Remove (current);
                foreach (var nt in transitions.Select (t => t.Target)) {
                    currentNegative.Add (nt);
                }
            }
            Console.WriteLine ("Negative: " + string.Join (",", currentNegative.Select (t => t.Name)));
            
            
            var negativeAcceptance = currentNegative.All (t => !negativeNFA.AcceptanceSet.Contains (t));
            var positiveAcceptance = currentPositive.All (t => !positiveNFA.AcceptanceSet.Contains (t));
            
            Status = MonitorStatus.Inconclusive;
            if (negativeAcceptance | currentPositive.Count == 0)
                Status = MonitorStatus.False;
            if (positiveAcceptance | currentNegative.Count == 0)
                Status = MonitorStatus.True;   
        }
    }
}

