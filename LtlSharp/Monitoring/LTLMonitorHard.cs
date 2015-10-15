using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Automata;
using LtlSharp.Automata.FiniteAutomata;
using LtlSharp.Buchi.LTL2Buchi;
using LtlSharp.Buchi.Translators;
using QuickGraph.Graphviz;

namespace LtlSharp.Monitoring
{
    
    public class LTLMonitorHard
    {
        HashSet<AutomatonNode> currentPositive;
        HashSet<AutomatonNode> currentNegative;
        
        NFA<AutomatonNode> positiveNFA;
        NFA<AutomatonNode> negativeNFA;

        public MonitorStatus Status;

        public LTLMonitorHard (ITLFormula formula)
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

            currentNegative = new HashSet<AutomatonNode> (negativeNFA.InitialNodes);
            currentPositive = new HashSet<AutomatonNode> (positiveNFA.InitialNodes);
            
            var graphviz = new GraphvizAlgorithm<AutomatonNode,AutomataTransition>(positiveNFA);
            graphviz.FormatVertex += (object sender, FormatVertexEventArgs<AutomatonNode> e) => {
                e.VertexFormatter.Label = e.Vertex.Name + "(" + string.Join (",", e.Vertex.Labels) + ")";;
                if (positiveNFA.InitialNodes.Contains (e.Vertex)) {
                    e.VertexFormatter.FillColor = QuickGraph.Graphviz.Dot.GraphvizColor.LightYellow;
                }
            };
            graphviz.FormatEdge += (object sender, FormatEdgeEventArgs<AutomatonNode, AutomataTransition> e) => {
                e.EdgeFormatter.Label.Value = string.Join (",", e.Edge.Labels);
            };
            string output = graphviz.Generate();
            Console.WriteLine (output);
            
            
            graphviz = new GraphvizAlgorithm<AutomatonNode,AutomataTransition>(negativeNFA);
            graphviz.FormatVertex += (object sender, FormatVertexEventArgs<AutomatonNode> e) => {
                e.VertexFormatter.Label = e.Vertex.Name + "(" + string.Join (",", e.Vertex.Labels) + ")";
                if (positiveNFA.InitialNodes.Contains (e.Vertex)) {
                    e.VertexFormatter.FillColor = QuickGraph.Graphviz.Dot.GraphvizColor.LightYellow;
                }
            };
            graphviz.FormatEdge += (object sender, FormatEdgeEventArgs<AutomatonNode, AutomataTransition> e) => {
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
                var successors = positiveNFA.Post (current, (l, t) => state.Evaluate (l));
                currentPositive.Remove (current);
                foreach (var nt in successors) {
                    currentPositive.Add (nt);
                }
            }
            
            foreach (var current in currentNegative.ToList ()) {
                var successors = negativeNFA.Post (current, (l, t) => state.Evaluate (l));
                currentNegative.Remove (current);
                foreach (var nt in successors) {
                    currentNegative.Add (nt);
                }
            }
            
            var negativeAcceptance = currentNegative.All (t => !negativeNFA.AcceptingNodes.Contains (t));
            var positiveAcceptance = currentPositive.All (t => !positiveNFA.AcceptingNodes.Contains (t));
            
            Status = MonitorStatus.Inconclusive;
            if (negativeAcceptance | currentPositive.Count == 0)
                Status = MonitorStatus.False;
            if (positiveAcceptance | currentNegative.Count == 0)
                Status = MonitorStatus.True;   
        }
    }
}

