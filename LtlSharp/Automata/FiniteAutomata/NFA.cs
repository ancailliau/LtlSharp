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
using LtlSharp.Automata;

namespace LtlSharp.Buchi.Automata
{
    /// <summary>
    /// This class represents a non-deterministic finite automata.
    /// </summary>
    /// <description>
    /// See Andreas Bauer et al, Runtime Verification for LTL and TLTL, TOSEM.
    /// </description>
    public class NFA : AdjacencyGraph<AutomatonNode, AutomatonTransition<AutomatonNode>>
    {
        public HashSet<AutomatonNode> AcceptanceSet;
        public HashSet<AutomatonNode> InitialNodes;
        
        public NFA ()
        {
            AcceptanceSet = new HashSet<AutomatonNode> ();
            InitialNodes = new HashSet<AutomatonNode> ();
        }
        
        public void ToSingleInitialState ()
        {
            Console.WriteLine (InitialNodes.Count);
            if (InitialNodes.Count == 1)
                return;
            
            var newInitialState = new AutomatonNode ("init");
            this.AddVertex (newInitialState);
            
            foreach (var initialState in InitialNodes.ToList ()) {
                //Console.WriteLine (OutDegree (initialState));
                foreach (var otransition in OutEdges (initialState)) {
                    var newTransition = new AutomatonTransition<AutomatonNode> (
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
            var pending = new Stack<AutomatonNode> (InitialNodes);
            var visited = new HashSet<AutomatonNode> ();
            
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
                        AddEdge (new AutomatonTransition<AutomatonNode> (trans.Source, trans.Target, nl));
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
            nnfa.AcceptanceSet = new HashSet<AutomatonNode> (AcceptanceSet);
            nnfa.InitialNodes = new HashSet<AutomatonNode> (InitialNodes);
            
            foreach (var trans in Edges) {
                var labels = UnfoldLabels (trans.Labels, alphabet);
                foreach (var label in labels) {
                    nnfa.AddEdge (new AutomatonTransition<AutomatonNode> (trans.Source, trans.Target, label));
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
            
            var ss0 = new HashSet<AutomatonNode> (new [] { initialNode });

            var pending = new Stack<HashSet<AutomatonNode>> ();
            pending.Push (ss0);

            var mapping = new Dictionary<HashSet<AutomatonNode>, AutomatonNode> (HashSet<AutomatonNode>.CreateSetComparer ());
            var node = new AutomatonNode (string.Join (",", ss0.Select (s => s.Name)));
            mapping.Add (ss0, node);
            dfa.AddVertex (node);
            dfa.InitialNodes.Add (node);
            
            if (unfoldedAutomata.AcceptanceSet.Contains (initialNode)) {
                dfa.AcceptanceSet.Add (initialNode);
            }

            var transitions = new Dictionary<Tuple<HashSet<AutomatonNode>, HashSet<AutomatonNode>>, HashSet<HashSet<ILiteral>>> ();
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

                    var succs = new HashSet<AutomatonNode> ();
                    
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
                        node = new AutomatonNode (string.Join (",", succs.Select (s => s.Name)));
                        var vs = new HashSet<AutomatonNode> (succs);
                        mapping.Add (vs, node);
                        dfa.AddVertex (node);

                        pending.Push (succs);

                        if (succs.Any (succ => unfoldedAutomata.AcceptanceSet.Contains (succ))) {
                            dfa.AcceptanceSet.Add (node);
                        }
                    }

                    var skey = new Tuple<HashSet<AutomatonNode>, HashSet<AutomatonNode>> (ss, succs);
                    if (!transitions.ContainsKey (skey)) {
                        transitions.Add (skey, new HashSet<HashSet<ILiteral>> ());
                    }
                    transitions[skey].Add (c);
                }   
            }
            
            foreach (var key in transitions.Keys) {
                foreach (var tadam in transitions [key]) {
                    dfa.AddEdge (new AutomatonTransition<AutomatonNode> (mapping [key.Item1], 
                                                                              mapping [key.Item2],
                                                                              tadam)
                                );
                }
            }

            dfa.Fold ();
            
            return dfa;
        }
        
    }
    
}

