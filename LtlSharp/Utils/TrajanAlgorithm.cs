using System;
using LtlSharp.Models;
using System.Collections.Generic;
using System.Linq;

namespace LtlSharp.Utils
{
    public static class TrajanAlgorithm
    {
        public static IEnumerable<HashSet<MarkovNode>> GetBSCC (this MarkovChain mc)
        {
            // A SCC is a Bottom SCC if all successors of the nodes in the SCC are also in the SCC
            // (or no nodes outside the scc can be reached)
            return GetSCC (mc).Where (scc => scc.IsSupersetOf (mc.Post (scc)));
        }
        
        public static IEnumerable<HashSet<MarkovNode>> GetSCC (this MarkovChain mc)
        {
            // Implements Trajan algorithm
            var sccs = new List<HashSet<MarkovNode>> ();
            var index = new Dictionary<MarkovNode, int> ();
            var lowlink = new Dictionary<MarkovNode, int> ();
            var onstack = new HashSet<MarkovNode> ();
            var S = new Stack<MarkovNode> ();
            int currentIndex = 0;

            foreach (var v in mc.Nodes) {
                if (!index.ContainsKey(v)) {
                    StrongConnect (v, mc, index, lowlink, onstack, S, ref currentIndex, sccs);
                }
            }

            return sccs;
        }

        static void StrongConnect (MarkovNode v, 
            MarkovChain mc,
            Dictionary<MarkovNode, int> index,
            Dictionary<MarkovNode, int> lowlink,
            HashSet<MarkovNode> onstack,
            Stack<MarkovNode> S,
            ref int currentIndex,
            List<HashSet<MarkovNode>> sccs) {
            
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
                var scc = new HashSet<MarkovNode> ();
                MarkovNode w;
                do {
                    w = S.Pop ();
                    onstack.Remove (w);
                    scc.Add (w);
                } while (w != v);
                sccs.Add (scc);
            }
        }
        
    }
}

