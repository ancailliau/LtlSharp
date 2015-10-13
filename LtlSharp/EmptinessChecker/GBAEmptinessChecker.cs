using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Automata;
using LtlSharp.Automata.OmegaAutomata;
using LtlSharp.Buchi.Automata;

namespace LtlSharp.Buchi
{
    public class GBAEmptinessChecker
    {
        Stack<AutomatonNode> path;
        // stack of nodes
        
        HashSet<AutomatonNode> processed;
        // set of nodes
        
        Dictionary<AutomatonNode, HashSet<int>> label; 
        // index of the node, index of the acceptance set
        
        public GBAEmptinessChecker ()
        {
            
        }
        
        public bool EmptinessSearch (TransitionGeneralizedBuchiAutomata a)
        {
            if (a.AcceptanceSets.Length == 0) {
                throw new NotImplementedException ("EmptinessSearch (GeneralizedBuchiAutomata a)");
            }
            
            path = new Stack<AutomatonNode> ();
            processed = new HashSet<AutomatonNode> ();
            label = new Dictionary<AutomatonNode, HashSet<int>> ();
            
            foreach (var n in a.InitialNodes) {
                //Console.WriteLine ("******");
                if (EmptinessSearch (a, n)) {
                    return true;
                }
                //Console.WriteLine ("******");
            }
            
            return false;
        }
        
        public bool EmptinessSearch (TransitionGeneralizedBuchiAutomata a, AutomatonNode qi)
        {
            label = new Dictionary<AutomatonNode, HashSet<int>> ();
            foreach (var n in a.Vertices) {
                label.Add (n, new HashSet<int> ());
            }
            //for (int i = 0, length = a.Transitions.Count; i < length; i++) {
            //    label[i] = new HashSet<int> ();
            //}
            
            path.Push (qi);
            while (path.Count > 0) {
                var q = path.Peek ();
                //Console.WriteLine ("q={0}", q.Name);
            
                var succToProcess = a.OutEdges(q).Select (x => x.Target).Except (path.Union (processed));
                while (succToProcess.Count () > 0) {
                    var succ = succToProcess.First ();
                    path.Push (succ);
                    label [succ] = new HashSet<int> ();
                    q = succ;
                    succToProcess = a.OutEdges(q).Select (x => x.Target).Except (path.Union (processed));
                    //Console.WriteLine ("q={0}", q.Name);
                }
                //Console.WriteLine ("----");
                if (label[q].Count == 0 | a.AcceptanceSets.Any (x => x.Nodes.Contains (q))) {
                    var labelsToPropagate = label [q].Union ((from x in a.AcceptanceSets
                                                                             where x.Nodes.Contains (q)
                                                                             select x.Id));
                    //Console.WriteLine ("labelsToPropagate={0}", string.Join (",", labelsToPropagate));
            
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
        

        void propagate (TransitionGeneralizedBuchiAutomata a, IEnumerable<AutomatonNode> nodes, IEnumerable<int> labelsToPropagate)
        {
            
            //Console.WriteLine ("path : " + string.Join(",", path));
            //Console.WriteLine ("processed : " + string.Join(",", processed));
            
            var toProp = labelsToPropagate.ToArray ();
            var nodesToProcess = new Stack<AutomatonNode> (nodes);
            while (nodesToProcess.Count > 0) {
                var q = nodesToProcess.Pop ();
                var successors = a.OutEdges (q).Select (x => x.Target).Intersect (path.Union (processed));
                //Console.WriteLine ("successors : " + string.Join(",", a.OutEdges (q).Select (x => x.Target.Name)));
                //Console.WriteLine ("successors : " + string.Join(",", successors));
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

