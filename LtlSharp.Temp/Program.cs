using System;
using LittleSharp.Buchi;
using LtlSharp.Buchi.LTL2Buchi;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;
using LtlSharp.Buchi.Translators;

namespace LtlSharp.Temp
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            Console.WriteLine ("Hello World!");
            
            var p1 = new Proposition ("p1");
            var p2 = new Proposition ("p2");
            var p3 = new Proposition ("p3");
            
            ILTLFormula f0 = new False ();
            ILTLFormula f1 = new Until (p1, p2);
            ILTLFormula f2 = new Until (p1, new Until (p2, p3));
            ILTLFormula f3 = new Negation (new Until (p1, new Until (p2, p3))); // problem
            ILTLFormula f4 = new Implication (new Globally (new Finally (p1)), new Globally (new Finally (p2))); // problem
            ILTLFormula f5 = new Until (new Finally (p1), new Globally (p2));
            ILTLFormula f6 = new Until (new Globally (p1), p2);
            ILTLFormula f7 = new Negation (new Equivalence (new Finally (new Finally (p1)), new Finally (p1)));
            
            ILTLFormula f8 = new Release (p1, p2);
            ILTLFormula f9 = new Release (new Negation (p1), new Negation(p2));
            ILTLFormula f10 = new Release (p1, new Release (p2, p3));
            
            var f = f2;
            
            Console.WriteLine (f);
            
            ILTL2Buchi t = new GPVW ();
            
            /*
            var normalizedFormula = f.Normalize ();
            Console.WriteLine (normalizedFormula);
            
            var nset = t.CreateGraph (normalizedFormula.Normalize ());
            
            foreach (var nn in nset) {
                Console.WriteLine (nn);
                Console.WriteLine ("  " + string.Join (",", nn.Incoming));
            }
            */
            
            var gba = t.GetAutomaton (f);
            Console.WriteLine (gba.AcceptanceSets.Length);
            foreach (var aset in gba.AcceptanceSets) {
                Console.WriteLine (aset.Id + " : " + string.Join (",", aset.Nodes));
            }
            
            Console.WriteLine ("---");
            
            Console.WriteLine (gba.ToDot ());
            
            Console.WriteLine ("---");
            
            var ba = GBA2BA.Transform (gba);
            
            Console.WriteLine (ba.ToDot ());
            
            
            /*
            var dict = new Dictionary<GBANode, string> ();
            int i = 0;
            Console.WriteLine ("digraph G {");
            foreach (var n in buchi.Nodes) {
                dict.Add (n.Value, "s" + (i++));
                Console.WriteLine ("\t" + dict[n.Value] + "[label=\""+dict[n.Value]+"\""
                    +(buchi.AcceptanceSet.Any(a => a.Contains(n.Value)) ? ",shape=doublecircle" : "")
                    +(n.Value.Initial ? ",penwidth=3" : "")+
                    "];");
            } 
            foreach (var n in buchi.Nodes.Values) {
                foreach (var n2 in n.Outgoing) {
                    Console.WriteLine ("\t" + dict [n] + " -> " + dict [n2.To] + " [label=\""+string.Join (",", n2.Literals)+"\"];");
                }
            }
            Console.WriteLine ("}");
            
            Console.WriteLine ("#AccSet: ");
            foreach (var ac in buchi.AcceptanceSet) {
                Console.WriteLine ("{{{0}}}", string.Join (",", ac.Select (x => dict[x])));
            }
            
            
            foreach (var n in buchi.Nodes.Values) {
                // Console.WriteLine (dict[n] + " --> {" + string.Join (",", n.) + "}");
            }
            */
            
            var ec = new GBAEmptinessChecker ();
            Console.WriteLine (ec.EmptinessSearch (gba) ? "Empty" : "Not empty");
            
            
            var ec2 = new EmptinessChecker (ba);
            Console.WriteLine (ec2.Emptiness () ? "Empty" : "Not empty");
            
            foreach (var i in ec2.dfsStack1.Reverse ()) {
                Console.WriteLine (ba.Nodes[i].Name);
            }
            Console.WriteLine ("-- loop starts here --");
            foreach (var i in ec2.dfsStack2.Reverse ()) {
                Console.WriteLine (ba.Nodes[i].Name);
            }
            Console.WriteLine ("-- loop starts here --");
        }
    }
}
