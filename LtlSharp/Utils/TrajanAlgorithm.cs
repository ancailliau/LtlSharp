using System;
using LtlSharp.Models;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Automata;

namespace LtlSharp.Utils
{
    public static class TrajanAlgorithm
    {
        public static IEnumerable<HashSet<T>> GetBSCC<T> (this MarkovChain<T> mc) where T : IAutomatonNode
        {
            // A SCC is a Bottom SCC if all successors of the nodes in the SCC are also in the SCC
            // (or no nodes outside the scc can be reached)
            return GetSCC (mc).Where (scc => scc.IsSupersetOf (mc.Post (scc)));
        }
        
        public static IEnumerable<HashSet<T>> GetSCC<T> (this MarkovChain<T> mc) where T : IAutomatonNode
        {
            // Implements Trajan algorithm
            var sccs = new List<HashSet<T>> ();
            var index = new Dictionary<T, int> ();
            var lowlink = new Dictionary<T, int> ();
            var onstack = new HashSet<T> ();
            var S = new Stack<T> ();
            int currentIndex = 0;

            foreach (var v in mc.Nodes) {
                if (!index.ContainsKey(v)) {
                    StrongConnect (v, mc, index, lowlink, onstack, S, ref currentIndex, sccs);
                }
            }

            return sccs;
        }

        static void StrongConnect<T> (T v, 
            MarkovChain<T> mc,
            Dictionary<T, int> index,
            Dictionary<T, int> lowlink,
            HashSet<T> onstack,
            Stack<T> S,
            ref int currentIndex,
                                      List<HashSet<T>> sccs)  where T : IAutomatonNode {
            
            if (index.ContainsKey (v))
                index[v] = currentIndex;
            else
                index.Add (v, currentIndex);

            if (lowlink.ContainsKey (v))
                lowlink[v] = currentIndex;
            else
                lowlink.Add (v, currentIndex);

            currentIndex++;

            S.Push(v);
            onstack.Add (v);

            foreach (var w in mc.Post (v)) {
                if (!index.ContainsKey(w)) {
                    StrongConnect (w, mc, index, lowlink, onstack, S, ref currentIndex, sccs);
                    // lowlink[v] = min (lowlink[v], lowlink[w])
                    lowlink[v] = (lowlink[v] > lowlink[w]) ? lowlink[w] : lowlink[v];
                } else if (onstack.Contains(w)) {
                    // lowlink[v] = min (lowlink[v], index[w])
                    lowlink[v] = (lowlink[v] > index[w]) ? index[w] : lowlink[v];
                }
            }
            
            if (lowlink[v] == index[v]) {
                var scc = new HashSet<T> ();
                T w;
                do {
                    w = S.Pop ();
                    onstack.Remove (w);
                    scc.Add (w);
                } while (!w.Equals (v));
                sccs.Add (scc);
            }
        }
        
    }
}

