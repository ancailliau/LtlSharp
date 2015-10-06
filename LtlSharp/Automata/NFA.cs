using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LtlSharp.Buchi.Automata;
using LtlSharp.Language;
using QuickGraph;
using LtlSharp.Monitoring;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace LtlSharp.Buchi.Automata
{
    /// <summary>
    /// This class represents a non-deterministic finite automata.
    /// </summary>
    /// <description>
    /// See Andreas Bauer et al, Runtime Verification for LTL and TLTL, TOSEM.
    /// </description>
    public class NFA : AdjacencyGraph<AutomataNode, LabeledAutomataTransition<AutomataNode>>
    {
        public HashSet<AutomataNode> AcceptanceSet;
        public HashSet<AutomataNode> InitialNodes;
        
        public NFA ()
        {
            AcceptanceSet = new HashSet<AutomataNode> ();
            InitialNodes = new HashSet<AutomataNode> ();
        }
        
        public void ToSingleInitialState ()
        {
            Console.WriteLine (InitialNodes.Count);
            if (InitialNodes.Count == 1)
                return;
            
            var newInitialState = new AutomataNode ("init");
            this.AddVertex (newInitialState);
            
            foreach (var initialState in InitialNodes.ToList ()) {
                //Console.WriteLine (OutDegree (initialState));
                foreach (var otransition in OutEdges (initialState)) {
                    var newTransition = new LabeledAutomataTransition<AutomataNode> (
                        newInitialState, 
                        otransition.Target, 
                        otransition.Labels
                    );
                    //Console.WriteLine (newTransition);
                    this.AddEdge (newTransition);
                }
                InitialNodes.Remove (initialState);
                //Console.WriteLine ("*>" + this.Edges.All (e => this.ContainsVertex(e.Target)));
            }

            InitialNodes.Add (newInitialState);
            //Console.WriteLine ("^^^^^^^^^^^^");
        }
        
        public bool IsDeterministic ()
        {
            var pending = new Stack<AutomataNode> (InitialNodes);
            var visited = new HashSet<AutomataNode> ();
            
            while (pending.Count > 0) {
                var s0 = pending.Pop ();
                visited.Add (s0);
                
                //Console.WriteLine ("*" + s0);
                //Console.WriteLine (string.Join("\n--", Edges));
                
                var transitions = OutEdges (s0);
                
                foreach (var c in transitions.SelectMany (x => x.Labels)) {
                    var succ = transitions.Where (t => t.Labels.Contains (c)).Select (t => t.Target);
                    if (succ.Count () > 1) {
                        return false;
                    } else {
                        foreach (var s in succ.Where (node => !visited.Contains (node))) {
                            //Console.WriteLine (ContainsVertex (s));
                            pending.Push (s);
                        }
                    }
                }
            }

            return true;
        }
        
        public NFA Fold ()
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
                        AddEdge (new LabeledAutomataTransition<AutomataNode> (trans.Source, trans.Target, nl));
                    }
                }
            }

            return this;
        }
        
        public void ResetNames () 
        {
            // Don't
            // Changing names changes hashcode and nothing is found again in hashtables :-)
        }
        
        public NFA Unfold ()
        {
            var alphabet = Edges.SelectMany (x => x.Labels).Select (x => x is Negation ? ((ILiteral)((Negation)x).Enclosed) : x).Distinct ();
            
            var nnfa = new NFA ();
            nnfa.AddVertexRange (Vertices);
            nnfa.AcceptanceSet = new HashSet<AutomataNode> (AcceptanceSet);
            nnfa.InitialNodes = new HashSet<AutomataNode> (InitialNodes);
            
            foreach (var trans in Edges) {
                var labels = UnfoldLabels (trans.Labels, alphabet);
                foreach (var label in labels) {
                    nnfa.AddEdge (new LabeledAutomataTransition<AutomataNode> (trans.Source, trans.Target, label));
                }
            }
            return nnfa;
        }
        
        private HashSet<HashSet<ILiteral>> UnfoldLabels (HashSet<ILiteral> trans, IEnumerable<ILiteral> alphabet)
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
        
        public NFA Determinize ()
        {
            if (InitialNodes.Count > 1) {
                throw new NotImplementedException ();
            }

            var unfoldedAutomata = Unfold ();
            var initialNode = unfoldedAutomata.InitialNodes.Single ();
            
            var dfa = new NFA ();
            
            var ss0 = new HashSet<AutomataNode> (new [] { initialNode });

            var pending = new Stack<HashSet<AutomataNode>> ();
            pending.Push (ss0);

            var mapping = new Dictionary<HashSet<AutomataNode>, AutomataNode> (HashSet<AutomataNode>.CreateSetComparer ());
            var node = new AutomataNode (string.Join (",", ss0.Select (s => s.Name)));
            mapping.Add (ss0, node);
            dfa.AddVertex (node);
            dfa.InitialNodes.Add (node);
            
            if (unfoldedAutomata.AcceptanceSet.Contains (initialNode)) {
                dfa.AcceptanceSet.Add (initialNode);
            }

            var transitions = new Dictionary<Tuple<HashSet<AutomataNode>, HashSet<AutomataNode>>, HashSet<HashSet<ILiteral>>> ();
            while (pending.Count > 0) {
                var ss = pending.Pop ();
                var cc = new HashSet<HashSet<ILiteral>> (ss.SelectMany (s => unfoldedAutomata.OutEdges (s).Select (x => x.Labels)),
                                                         HashSet<ILiteral>.CreateSetComparer ());
                
//                Console.WriteLine (string.Join (",", ss));
//                foreach (var c in cc) {
//                    Console.WriteLine (string.Join (",", c));
//                }
//                Console.WriteLine ("--");
                
                foreach (var c in cc) {

                    var succs = new HashSet<AutomataNode> ();
                    
                    foreach (var s in ss) {
//                        Console.WriteLine ("Successor of " + s);
                        var enumerable = unfoldedAutomata.OutEdges (s).Where (x => {
//                            Console.WriteLine (string.Join (",", x.Labels) + " == " + string.Join (",", c) + " = " + x.Labels.SetEquals (c));
                            return x.Labels.SetEquals (c);   
                        });
//                        Console.WriteLine (string.Join (",", enumerable));
                        foreach (var t in enumerable) {
                            succs.Add (t.Target);
                        }
                    }
                    
//                    Console.WriteLine ("Successors with [" + string.Join (",", c) + "] : " + string.Join (",", succs));
                    
                    if (!mapping.ContainsKey (succs)) {
                        node = new AutomataNode (string.Join (",", succs.Select (s => s.Name)));
                        var vs = new HashSet<AutomataNode> (succs);
                        mapping.Add (vs, node);
                        dfa.AddVertex (node);

                        pending.Push (succs);

                        if (succs.Any (succ => unfoldedAutomata.AcceptanceSet.Contains (succ))) {
                            dfa.AcceptanceSet.Add (node);
                        }
                    }

                    var skey = new Tuple<HashSet<AutomataNode>, HashSet<AutomataNode>> (ss, succs);
                    if (!transitions.ContainsKey (skey)) {
                        transitions.Add (skey, new HashSet<HashSet<ILiteral>> ());
                    }
                    transitions[skey].Add (c);
                }   
            }
            
            foreach (var key in transitions.Keys) {
                foreach (var tadam in transitions [key]) {
                    dfa.AddEdge (new LabeledAutomataTransition<AutomataNode> (mapping [key.Item1], 
                                                                              mapping [key.Item2],
                                                                              tadam)
                                );
                }
            }

            dfa.Fold ();
            
            return dfa;
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
        
        public static NFAProduct Product (NFA positive, NFA negative) {
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
    
    public class NFAProduct : AdjacencyGraph<MonitorNode, LabeledAutomataTransition<MonitorNode>>
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
}

