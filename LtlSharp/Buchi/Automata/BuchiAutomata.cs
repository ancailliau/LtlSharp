using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LtlSharp.Buchi
{
    public class BANode {
        public int Id;
        public string Name;
        public bool Initial;
        public BANode (int id, string name, bool initial)
        {
            Id = id;
            Name = name;
            Initial = initial;
        }
        public override string ToString ()
        {
            return string.Format ("[BANode: Id={0}, Name=\"{1}\", Initial={2}]", Id, Name, Initial);
        }
    }

    public class BATransition {
        public int To;
        public HashSet<ILiteral> Labels;
        public BATransition (int to, HashSet<ILiteral> labels)
        {
            To = to;
            Labels = labels;
        }
        public override string ToString ()
        {
            return string.Format ("[BATransition: To={0}, Labels={1}]", To, string.Join (",", Labels));
        }
    }
        
    public class BuchiAutomata
    {
        public BANode[] Nodes;
        public List<BATransition>[] Transitions; // probably better to use a sparse array instead.
        public int[] AcceptanceSet;
        
        public BuchiAutomata (int n_nodes)
        {
            Nodes = new BANode[n_nodes];
            Transitions = new List<BATransition>[n_nodes];
            AcceptanceSet = new int[0];
        }
        
        public string ToDot () 
        {
            var str = new StringWriter ();
            var dict = new Dictionary<BANode, string> ();
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

