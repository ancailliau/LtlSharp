using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Buchi.Automata;

namespace LtlSharp.Buchi
{
    public class OnTheFlyGBAEmptinessChecker
    {
        public Stack<Tuple<AutomataNode, AutomataNode>> path;
        // stack of nodes
        
        public HashSet<Tuple<AutomataNode, AutomataNode>> processed;
        // set of nodes
        
        public Dictionary<AutomataNode, HashSet<int>> label; 
        // index of the node, index of the acceptance set
        
        GeneralizedBuchiAutomata a;
        BuchiAutomata ba;
        
        public OnTheFlyGBAEmptinessChecker (GeneralizedBuchiAutomata a, BuchiAutomata ba)
        {
            this.a = a;
            this.ba = ba;
        }
        
        public bool EmptinessSearch ()
        {
            if (a.AcceptanceSets.Length == 0) {
                throw new NotImplementedException ("EmptinessSearch (GeneralizedBuchiAutomata a)");
            }
            
            label = new Dictionary<AutomataNode, HashSet<int>> ();
            processed = new HashSet<Tuple<AutomataNode,AutomataNode>> ();
            path = new Stack<Tuple<AutomataNode,AutomataNode>> ();
            
            foreach (var n1 in a.Nodes.Where (x => x.Initial)) {
                foreach (var n2 in ba.Nodes.Where (x => x.Initial)) {
                    if (EmptinessSearch (n1, n2)) {
                        return true;
                    }
                }
            }
            
            return false;
        }
        
        public bool EmptinessSearch (AutomataNode qi, AutomataNode bi)
        {   
            foreach (var n in a.Nodes) {
                label.Add (n, new HashSet<int> ());
            }
            
            path.Push (new Tuple<AutomataNode, AutomataNode> (qi, bi));
            while (path.Count > 0) {
                var q = path.Peek ();
            
                var succToProcess = new Stack<Tuple<AutomataNode, AutomataNode>> ();
                
                var pup = new HashSet<Tuple<AutomataNode, AutomataNode>> (path.Union (processed));
                foreach (var t1 in a.Transitions[q.Item1]) {
                    foreach (var t2 in ba.Transitions[q.Item2]) {
                        if (t1.Labels.IsSubsetOf (t2.Labels)) {
                            var nt = new Tuple<AutomataNode, AutomataNode> (t1.To, t2.To);
                            if (!pup.Contains (nt)) {
                                //Console.WriteLine ("pushing from (" + a.Nodes[q.Item1].Name + ", " + q.Item2.Name + ") to (" + a.Nodes[nt.Item1].Name + ", " + nt.Item2.Name + ")");
                                succToProcess.Push (nt);
                            }
                        }
                    }
                }
                
                while (succToProcess.Count () > 0) {
                    var succ = succToProcess.Pop ();
                    path.Push (succ);
                    label [succ.Item1] = new HashSet<int> ();
                    q = succ;
                    
                    succToProcess = new Stack<Tuple<AutomataNode, AutomataNode>> ();
                    pup = new HashSet<Tuple<AutomataNode,AutomataNode>> (path.Union (processed));
                    foreach (var t1 in a.Transitions[q.Item1]) {
                        foreach (var t2 in ba.Transitions[q.Item2]) {
                            if (t1.Labels.IsSubsetOf (t2.Labels)) {
                                var nt = new Tuple<AutomataNode,AutomataNode> (t1.To, t2.To);
                                if (!pup.Contains (nt)) {
                                    //Console.WriteLine ("pushing from (" + a.Nodes[q.Item1].Name + ", " + ba.Nodes[q.Item2].Name + ") to (" + a.Nodes[nt.Item1].Name + ", " + ba.Nodes[nt.Item2].Name + ")");
                                    succToProcess.Push (nt);
                                }
                            }
                        }
                    }
                    
                }
                //Console.WriteLine ("----");
                if (label[q.Item1].Count == 0 | a.AcceptanceSets.Any (x => x.Nodes.Contains (q.Item1))) {
                    var labelsToPropagate = label [q.Item1].Union ((from x in a.AcceptanceSets
                        where x.Nodes.Contains (q.Item1)
                                                                             select x.Id));
                    //Console.WriteLine ("labelsToPropagate={0}", string.Join (",", labelsToPropagate));
            
                    propagate (new [] { q }, labelsToPropagate);
                    if (label[q.Item1].SetEquals (a.AcceptanceSets.Select (set => set.Id))) {
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
        

        void propagate (IEnumerable<Tuple<AutomataNode, AutomataNode>> nodes, IEnumerable<int> labelsToPropagate)
        {
            
            Console.WriteLine ("path : " + string.Join(",", path));
            Console.WriteLine ("processed : " + string.Join(",", processed));
            
            var toProp = labelsToPropagate.ToArray ();
            var nodesToProcess = new Stack<Tuple<AutomataNode,AutomataNode>> (nodes);
            while (nodesToProcess.Count > 0) {
                var q = nodesToProcess.Pop ();
                
                var successors = new Stack<Tuple<AutomataNode,AutomataNode>> ();
                // a.Transitions [q].Select (x => x.To).Intersect (path.Union (processed)).ToArray ();
                
                var pup = new HashSet<Tuple<AutomataNode,AutomataNode>> (path.Union (processed));
                foreach (var t1 in a.Transitions[q.Item1]) {
                    foreach (var t2 in ba.Transitions[q.Item2]) {
                        if (t1.Labels.IsSubsetOf (t2.Labels)) {
                            var nt = new Tuple<AutomataNode,AutomataNode> (t1.To, t2.To);
                            if (pup.Contains (nt)) {
                                successors.Push (nt);
                            }
                        }
                    }
                }
                
//                Console.WriteLine ("successors : " + string.Join(",", a.Transitions [q].Select (x => a.Nodes[x.To].Name)));
//                Console.WriteLine ("successors : " + string.Join(",", successors));
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

