using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LtlSharp.Buchi
{
    public class GBANode {
        public int Id;
        public string Name;
        public bool Initial;
        public GBANode (int id, string name, bool initial)
        {
            Id = id;
            Name = name;
            Initial = initial;
        }
    }
    
    public class GBATransition {
        public int To;
        public HashSet<ILiteral> Labels;
        public GBATransition (int to, HashSet<ILiteral> labels)
        {
            To = to;
            Labels = labels;
        }
    }
    
    public class GBAAcceptanceSet {
        public int Id;
        public int[] Nodes;
        public GBAAcceptanceSet (int id, int[] acceptance_nodes)
        {
            Id = id;
            Nodes = acceptance_nodes;
        }
        
    }
    
    public class GeneralizedBuchiAutomata
    {
        public GBANode[] Nodes;
        public List<GBATransition>[] Transitions; // probably better to use a sparse array instead.
        public GBAAcceptanceSet[] AcceptanceSets;
        
        public GeneralizedBuchiAutomata (int n_nodes)
        {
            Nodes = new GBANode[n_nodes];
            Transitions = new List<GBATransition>[n_nodes];
        }
        
        public string ToDot () 
        {
            var str = new StringWriter ();
            var dict = new Dictionary<GBANode, string> ();
            int i = 0;
            str.WriteLine ("digraph G {");
            foreach (var n in Nodes) {
                dict.Add (n, "s" + (i++));
                str.WriteLine ("\t" + dict[n] + "[label=\""+n.Name+"\""
                    +(AcceptanceSets.Any(a => a.Nodes.Contains(n.Id)) ? ",shape=doublecircle" : "")
                    +(n.Initial ? ",penwidth=3" : "")+
                    "];");
            } 
            for (int j = 0; j < Transitions.Length; j++) {
                var node = Transitions [j];
                foreach (var t in node) {
                    str.WriteLine ("\t" + dict [Nodes [j]] + " -> " + dict [Nodes [t.To]] + " [label=\"" + string.Join (",", t.Labels) + "\"];");
                }
            }
            str.WriteLine ("}");
            
            return str.ToString ();
        }
    }
}

