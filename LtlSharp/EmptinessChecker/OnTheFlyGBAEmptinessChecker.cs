﻿using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Automata;
using LtlSharp.Automata.OmegaAutomata;
using LtlSharp.Automata.Utils;

namespace LtlSharp.Buchi
{
    public class OnTheFlyGBAEmptinessChecker
    {
        public Stack<Tuple<AutomatonNode, AutomatonNode>> path;
        // stack of nodes
        
        public HashSet<Tuple<AutomatonNode, AutomatonNode>> processed;
        // set of nodes
        
        public Dictionary<AutomatonNode, HashSet<int>> label; 
        // index of the node, index of the acceptance set
        
        GeneralizedBuchiAutomaton<AutomatonNode> a;
        BuchiAutomaton<AutomatonNode> ba;
        
        public OnTheFlyGBAEmptinessChecker (GeneralizedBuchiAutomaton<AutomatonNode> a, BuchiAutomaton<AutomatonNode> ba)
        {
            this.a = a;
            this.ba = ba;
        }
        
        public bool EmptinessSearch ()
        {
            if (!a.AcceptanceCondition.IsSatisfiable)
                return false;
            
            label = new Dictionary<AutomatonNode, HashSet<int>> ();
            processed = new HashSet<Tuple<AutomatonNode,AutomatonNode>> ();
            path = new Stack<Tuple<AutomatonNode,AutomatonNode>> ();

            return EmptinessSearch (a.InitialNode, ba.InitialNode);
        }
        
        public bool EmptinessSearch (AutomatonNode qi, AutomatonNode bi)
        {   
            foreach (var n in a.Nodes) {
                label.Add (n, new HashSet<int> ());
            }
            
            path.Push (new Tuple<AutomatonNode, AutomatonNode> (qi, bi));
            while (path.Count > 0) {
                var q = path.Peek ();
            
                var succToProcess = new Stack<Tuple<AutomatonNode, AutomatonNode>> ();
                
                var pup = new HashSet<Tuple<AutomatonNode, AutomatonNode>> (path.Union (processed));
                foreach (var t1 in a.GetTransitions (q.Item1)) {
                    foreach (var t2 in ba.Post (q.Item2, t1.Decoration)) {
                        //if (t1.Labels.IsSubsetOf (t2.Labels)) { fixme
                        var nt = new Tuple<AutomatonNode, AutomatonNode> (t1.Target, t2);
                            if (!pup.Contains (nt)) {
                                ////Console.WriteLine ("pushing from (" + a.Nodes[q.Item1].Name + ", " + q.Item2.Name + ") to (" + a.Nodes[nt.Item1].Name + ", " + nt.Item2.Name + ")");
                                succToProcess.Push (nt);
                            }
                        //}
                    }
                }
                
                while (succToProcess.Count () > 0) {
                    var succ = succToProcess.Pop ();
                    path.Push (succ);
                    label [succ.Item1] = new HashSet<int> ();
                    q = succ;
                    
                    succToProcess = new Stack<Tuple<AutomatonNode, AutomatonNode>> ();
                    pup = new HashSet<Tuple<AutomatonNode,AutomatonNode>> (path.Union (processed));
                    foreach (var t1 in a.GetTransitions (q.Item1)) {
                        foreach (var t2 in ba.Post (q.Item2, t1.Decoration)) {
                            //if (t1.Labels.IsSubsetOf (t2.Labels)) {
                            var nt = new Tuple<AutomatonNode,AutomatonNode> (t1.Target, t2);
                                if (!pup.Contains (nt)) {
                                    ////Console.WriteLine ("pushing from (" + a.Nodes[q.Item1].Name + ", " + ba.Nodes[q.Item2].Name + ") to (" + a.Nodes[nt.Item1].Name + ", " + ba.Nodes[nt.Item2].Name + ")");
                                    succToProcess.Push (nt);
                                }
                            //}
                        }
                    }
                    
                }
                ////Console.WriteLine ("----");
                if (label[q.Item1].Count == 0 | a.AcceptanceCondition.Accept(q.Item1)) {
                    var labelsToPropagate = label [q.Item1].Union (a.AcceptanceCondition.GetAcceptingConditions(q.Item1));
                    
                    propagate (new [] { q }, labelsToPropagate);
                    if (label[q.Item1].SetEquals (a.AcceptanceCondition.AllKeys ())) {
                        goto ExitTrue;
                        // return true;
                    }
                }
                processed.Add (q);
                path.Pop ();
            }

            return false;
            
            ExitTrue : return true;
        }
        

        void propagate (IEnumerable<Tuple<AutomatonNode, AutomatonNode>> nodes, IEnumerable<int> labelsToPropagate)
        {
            
            //Console.WriteLine ("path : " + string.Join(",", path));
            //Console.WriteLine ("processed : " + string.Join(",", processed));
            
            var toProp = labelsToPropagate.ToArray ();
            var nodesToProcess = new Stack<Tuple<AutomatonNode,AutomatonNode>> (nodes);
            while (nodesToProcess.Count > 0) {
                var q = nodesToProcess.Pop ();
                
                var successors = new Stack<Tuple<AutomatonNode,AutomatonNode>> ();
                // a.Transitions [q].Select (x => x.To).Intersect (path.Union (processed)).ToArray ();
                
                var pup = new HashSet<Tuple<AutomatonNode,AutomatonNode>> (path.Union (processed));
                foreach (var t1 in a.GetTransitions (q.Item1)) {
                    foreach (var t2 in ba.Post (q.Item2, t1.Decoration)) {
                        //if (t1.Labels.IsSubsetOf (t2.Labels)) {
                        var nt = new Tuple<AutomatonNode,AutomatonNode> (t1.Target, t2);
                            if (pup.Contains (nt)) {
                                successors.Push (nt);
                            }
                        //}
                    }
                }
                
//                //Console.WriteLine ("successors : " + string.Join(",", a.Transitions [q].Select (x => a.Nodes[x.To].Name)));
//                //Console.WriteLine ("successors : " + string.Join(",", successors));
                foreach (var succ in successors) {
                    if (!label[succ.Item1].IsSupersetOf (labelsToPropagate)) {
                        nodesToProcess.Push (succ);
                        for (int i = 0, toPropLength = toProp.Length; i < toPropLength; i++) {
                            label [succ.Item1].Add (toProp [i]);
                        }
                    }
                }
            }
        } 
    }
}

