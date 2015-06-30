using System;
using System.Collections.Generic;
using System.Linq;

namespace LtlSharp.Buchi
{
    public class GBAEmptinessChecker
    {
        Stack<int> path;
        // stack of nodes
        
        HashSet<int> processed;
        // set of nodes
        
        HashSet<int>[] label; 
        // index of the node, index of the acceptance set
        
        public GBAEmptinessChecker ()
        {
            
        }
        
        public bool EmptinessSearch (GeneralizedBuchiAutomata a)
        {
            if (a.AcceptanceSets.Length == 0) {
                throw new NotImplementedException ("");
            }
            
            path = new Stack<int> ();
            processed = new HashSet<int> ();
            label = new HashSet<int>[a.Nodes.Length];
            
            foreach (var n in a.Nodes.Where (x => x.Initial)) {
                Console.WriteLine ("******");
                if (EmptinessSearch (a, n)) {
                    return true;
                }
                Console.WriteLine ("******");
            }
            
            return false;
        }
        
        public bool EmptinessSearch (GeneralizedBuchiAutomata a, GBANode qi)
        {
            
            for (int i = 0, length = a.Nodes.Length; i < length; i++) {
                label[i] = new HashSet<int> ();
            }
            
            path.Push (qi.Id);
            while (path.Count > 0) {
                var q = path.Peek ();
                Console.WriteLine ("q={0}", a.Nodes[q].Name);
            
                var succToProcess = a.Transitions[q].Select (x => x.To).Except (path.Union (processed));
                while (succToProcess.Count () > 0) {
                    var succ = succToProcess.First ();
                    path.Push (succ);
                    label [succ] = new HashSet<int> ();
                    q = succ;
                    succToProcess = a.Transitions[q].Select (x => x.To).Except (path.Union (processed));
                    Console.WriteLine ("q={0}", a.Nodes[q].Name);
                }
                Console.WriteLine ("----");
                if (label[q].Count == 0 | a.AcceptanceSets.Any (x => x.Nodes.Contains (q))) {
                    var labelsToPropagate = label [q].Union ((from x in a.AcceptanceSets
                                                                             where x.Nodes.Contains (q)
                                                                             select x.Id));
                    Console.WriteLine ("labelsToPropagate={0}", string.Join (",", labelsToPropagate));
            
                    propagate (a, new [] { q }, labelsToPropagate);
                    if (label[q].SetEquals (a.AcceptanceSets.Select (set => set.Id))) {
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
        

        void propagate (GeneralizedBuchiAutomata a, IEnumerable<int> nodes, IEnumerable<int> labelsToPropagate)
        {
            
            Console.WriteLine ("path : " + string.Join(",", path));
            Console.WriteLine ("processed : " + string.Join(",", processed));
            
            var toProp = labelsToPropagate.ToArray ();
            Stack<int> nodesToProcess = new Stack<int> (nodes);
            while (nodesToProcess.Count > 0) {
                var q = nodesToProcess.Pop ();
                var successors = a.Transitions [q].Select (x => x.To).Intersect (path.Union (processed)).ToArray ();
                Console.WriteLine ("successors : " + string.Join(",", a.Transitions [q].Select (x => a.Nodes[x.To].Name)));
                Console.WriteLine ("successors : " + string.Join(",", successors));
                foreach (var succ in successors) {
                    if (!label[succ].IsSupersetOf (labelsToPropagate)) {
                        nodesToProcess.Push (succ);
                        for (int i = 0, toPropLength = toProp.Length; i < toPropLength; i++) {
                            label [succ].Add (toProp [i]);
                        }
                    }
                }
            }
        } 
    }
}

