using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LtlSharp.Buchi.Automata
{
    /// <summary>
    /// This class represents a non-deterministic finite automata.
    /// </summary>
    /// <description>
    /// See Andreas Bauer et al, Runtime Verification for LTL and TLTL, TOSEM.
    /// </description>
    public class NFA
    {
        public AutomataNode [] Nodes { get; set; }
        public List<AutomataTransition>[] Transitions { get; set; }
        public int[] AcceptanceSet { get; set; }

        public NFA (int n_nodes)
        {
            Nodes = new AutomataNode[n_nodes];
            Transitions = new List<AutomataTransition>[n_nodes];
            AcceptanceSet = new int[0];
        }

        public string ToDot () 
        {
            var str = new StringWriter ();
            var dict = new Dictionary<AutomataNode, string> ();
            int i = 0;
            str.WriteLine ("digraph G {");
            foreach (var n in Nodes.Where (x => x != null)) {
                dict.Add (n, "s" + (i++));
                str.WriteLine ("\t" + dict[n] + "[label=\""+n.Name+"\""
                               +(AcceptanceSet.Contains(n.Id) ? ",shape=doublecircle" : "")
                               +(n.Initial ? ",penwidth=3" : "")+
                               "];");
            } 
            for (int j = 0; j < Transitions.Length; j++) {
                var node = Transitions [j];
                if (node == null)
                    continue;
                foreach (var t in node.Where (x => x != null)) {
                    if (Nodes[j] == null | Nodes[t.To] == null) {
                        continue;
                    }
                    str.WriteLine ("\t" + dict [Nodes [j]] + " -> " + dict [Nodes [t.To]] + " [label=\"" + string.Join (",", t.Labels) + "\"];");
                }
            }
            str.WriteLine ("}");

            return str.ToString ();
        }
    }
}

