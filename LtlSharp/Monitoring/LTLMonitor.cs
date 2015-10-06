using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Buchi;
using LtlSharp.Buchi.Automata;
using LtlSharp.Buchi.LTL2Buchi;
using LtlSharp.Buchi.Translators;
using QuickGraph.Graphviz;
using QuickGraph;
using LtlSharp.Language;

namespace LtlSharp.Monitoring
{
    public enum MonitorStatus {
        True, False, Inconclusive
    }
    
    public class LTLMonitor
    {
        
        private class NFAProduct : AdjacencyGraph<MonitorNode, LabeledAutomataTransition<MonitorNode>>
        {
            public HashSet<MonitorNode> InitialNodes;

            public NFAProduct () : base ()
            {
                InitialNodes = new HashSet<MonitorNode> ();
            }

            public NFAProduct Fold ()
            {
                foreach (var node in Vertices) {
                    var transitions = OutEdges (node);
                    foreach (var trans in transitions.ToList ()) {
                        var sameTarget = transitions.Where (t => t.Target.Equals (trans.Target)).ToList ();
                        var labels = sameTarget.Select (x => x.Labels);
                        var lf = new LiteralFormula (labels);
                        var newLabels = lf.Simplify ();
                        foreach (var e in sameTarget) {
                            RemoveEdge (e);
                        }
                        foreach (var nl in newLabels) {
                            AddEdge (new LabeledAutomataTransition<MonitorNode> (trans.Source, trans.Target, nl));
                        }
                    }
                }

                return this;
            }
        }
        
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
//                Console.WriteLine ("No out transition in positive NFA");
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
//                Console.WriteLine ("No out transition in negative NFA");
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
            var product = Product (positiveNFA, negativeNFA);

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
        
        
        // TODO This should not be in that fucking class!

        static MonitorStatus GetStatus (NFA positive, NFA negative, AutomataNode s0, AutomataNode s1)
        {
            var positiveAcceptance = positive.AcceptanceSet.Contains (s0);
            var negativeAcceptance = negative.AcceptanceSet.Contains (s1);

            if (negativeAcceptance & positiveAcceptance) {
                return MonitorStatus.Inconclusive;
            } else if (!negativeAcceptance) {
                return MonitorStatus.True;
            } else if (!positiveAcceptance) {
                return MonitorStatus.False;
            } else {
                throw new NotImplementedException ();
            }
        }

        static NFAProduct Product (NFA positive, NFA negative) {
            var product = new NFAProduct ();

            positive = positive.Unfold ();
            negative = negative.Unfold ();

            //            var graphviz = new GraphvizAlgorithm<AutomataNode, LabeledAutomataTransition<AutomataNode>> (positive);
            //            graphviz.FormatVertex += (object sender, FormatVertexEventArgs<AutomataNode> e) => {
            //                e.VertexFormatter.Label = e.Vertex.Name;
            //                if (positive.InitialNodes.Contains (e.Vertex))
            //                    e.VertexFormatter.Style = GraphvizVertexStyle.Bold;
            //                if (positive.AcceptanceSet.Contains (e.Vertex))
            //                    e.VertexFormatter.Shape = GraphvizVertexShape.DoubleCircle;
            //            };
            //            graphviz.FormatEdge += (object sender, FormatEdgeEventArgs<AutomataNode, LabeledAutomataTransition<AutomataNode>> e) => {
            //                e.EdgeFormatter.Label.Value = string.Join (",", e.Edge.Labels);
            //            };
            //            Console.WriteLine (graphviz.Generate ());
            //
            //            graphviz = new GraphvizAlgorithm<AutomataNode, LabeledAutomataTransition<AutomataNode>> (negative);
            //            graphviz.FormatVertex += (object sender, FormatVertexEventArgs<AutomataNode> e) => {
            //                e.VertexFormatter.Label = e.Vertex.Name;
            //                if (negative.InitialNodes.Contains (e.Vertex))
            //                    e.VertexFormatter.Style = GraphvizVertexStyle.Bold;
            //                if (negative.AcceptanceSet.Contains (e.Vertex))
            //                    e.VertexFormatter.Shape = GraphvizVertexShape.DoubleCircle;
            //            };
            //            graphviz.FormatEdge += (object sender, FormatEdgeEventArgs<AutomataNode, LabeledAutomataTransition<AutomataNode>> e) => {
            //                e.EdgeFormatter.Label.Value = string.Join (",", e.Edge.Labels);
            //            };
            //            Console.WriteLine (graphviz.Generate ());

            var initA = positive.InitialNodes.Single ();
            var initB = negative.InitialNodes.Single ();

            var init = new MonitorNode ("init", GetStatus (positive, negative, initA, initB));
            product.AddVertex (init);
            product.InitialNodes.Add (init);

            var pending = new Stack<Tuple<AutomataNode, AutomataNode>> ();

            var tuple = new Tuple<AutomataNode, AutomataNode> (initA, initB);
            pending.Push (tuple);

            var mapping = new Dictionary<Tuple<AutomataNode, AutomataNode>, MonitorNode> ();
            mapping.Add (tuple, init);

            var acceptingNode = new MonitorNode ("accept", MonitorStatus.True);
            var rejectingNode = new MonitorNode ("reject", MonitorStatus.False);
            product.AddVertex (acceptingNode);
            product.AddVertex (rejectingNode);

            int i = 0;
            while (pending.Count > 0) 
            {
                var current = pending.Pop ();
                var commonTransitions = from t1 in positive.OutEdges (current.Item1)
                    from t2 in negative.OutEdges (current.Item2)
                        where t1.Labels.SetEquals (t2.Labels)
                    select new Tuple<LabeledAutomataTransition<AutomataNode>, LabeledAutomataTransition<AutomataNode>> (t1, t2);

                foreach (var t in commonTransitions) {
                    //                    Console.WriteLine ("Transition : " + t);
                    tuple = new Tuple<AutomataNode, AutomataNode> (t.Item1.Target, t.Item2.Target);
                    if (!mapping.ContainsKey (tuple)) {
                        mapping.Add (tuple, new MonitorNode ("s" + (i++), GetStatus (positive, negative, t.Item1.Target, t.Item2.Target)));
                        product.AddVertex (mapping [tuple]);
                        pending.Push (tuple);
                    }
                    var target = mapping[tuple];

                    //                    Console.WriteLine ("Transition from " + mapping[current] + " to " + target + " with " + string.Join (",", t.Item1.Labels));
                    product.AddEdge (new LabeledAutomataTransition<MonitorNode> (mapping[current], target, t.Item1.Labels));
                }

                foreach (var t in positive.OutEdges (current.Item1).Except (commonTransitions.Select (tt => tt.Item1))) {
                    product.AddEdge (new LabeledAutomataTransition<MonitorNode> (mapping[current], acceptingNode, t.Labels));
                }

                foreach (var t in negative.OutEdges (current.Item2).Except (commonTransitions.Select (tt => tt.Item2))) {
                    product.AddEdge (new LabeledAutomataTransition<MonitorNode> (mapping[current], rejectingNode, t.Labels));
                }
            }

            // If no edge target the accepting node, it can be removed.
            if (product.Edges.All (t => !t.Target.Equals (acceptingNode))) {
                product.RemoveVertex (acceptingNode);
            }

            // If no edge target the rejecting node, it can be removed.
            if (product.Edges.All (t => !t.Target.Equals (rejectingNode))) {
                product.RemoveVertex (rejectingNode);
            }

            product.Fold ();

            return product;
        }
    }
}

