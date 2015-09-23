using System;
using LittleSharp.Buchi;
using LtlSharp.Buchi.LTL2Buchi;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;
using LtlSharp.Buchi.Translators;
using LtlSharp.Buchi.Automata;

namespace LtlSharp.Temp
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            Console.WriteLine ("Hello World!");

            ILiteral notspawn = new Negation (new Proposition ("spawn"));
            ILiteral init = new Proposition ("init");
            var formula = new Until (notspawn, init);
         
            ILTL2Buchi t = new GPVW ();
            
            var ba1 = GBA2BA.Transform (t.GetAutomaton (formula));
            var ba2 = GBA2BA.Transform (t.GetAutomaton (formula.Negate ()));
            
            
            /*Console.WriteLine (string.Join (",", ba.AcceptanceSet));
            */
            Console.WriteLine (string.Join (",", ba1.Nodes.Select (x => x.ToString ())));
            /*Console.WriteLine (string.Join ("\n", ba.Transitions.Select (x => "{" + string.Join(",", x.Select (tr => "("+string.Join(",", tr.Labels)+")" + tr.To)) + "}")));
            */
            
            var ec = new EmptinessChecker (ba1);
            var fhat = new List<int> ();
            foreach (var n in ba1.Nodes) {
                var b = ec.Emptiness (n);
                if (b)
                    fhat.Add (n.Id);
                Console.WriteLine (n.Name + " ? " + b);
            }
            ba1.AcceptanceSet = fhat.ToArray ();
            
            var ec2 = new EmptinessChecker (ba2);
            fhat = new List<int> ();
            foreach (var n in ba2.Nodes) {
                var b = ec2.Emptiness (n);
                if (b)
                    fhat.Add (n.Id);
                Console.WriteLine (n.Name + " ? " + b);
            }
            ba2.AcceptanceSet = fhat.ToArray ();
            
        }
        
        public static void Main2 (string[] args)
        {
            Console.WriteLine ("Hello World!");
            
            ILiteral alloc = new Proposition ("allocated");
            ILiteral mob = new Proposition ("mobilized");
            ILiteral nalloc = new Negation (alloc);
            ILiteral nmob = new Negation (mob);

            var lts = new BuchiAutomata (4);
            lts.Nodes [0] = new AutomataNode (0, "i", true);
            lts.Nodes [1] = new AutomataNode (1, "s0", false);
            lts.Nodes [2] = new AutomataNode (2, "s1", false);
            lts.Nodes [3] = new AutomataNode (3, "s2", false);

            lts.Transitions [0] = new List<AutomataTransition> (new [] {
                new AutomataTransition (1, new HashSet<ILiteral> (new ILiteral[] { nalloc, nmob }))
            });

            lts.Transitions [1] = new List<AutomataTransition> (new [] {
                new AutomataTransition (2, new HashSet<ILiteral> (new ILiteral[] { alloc, nmob }))
            });

            lts.Transitions [2] = new List<AutomataTransition> (new [] {
                new AutomataTransition (3, new HashSet<ILiteral> (new ILiteral[] { alloc, mob }))
            });

            lts.Transitions [3] = new List<AutomataTransition> (new [] {
                new AutomataTransition (1, new HashSet<ILiteral> (new ILiteral[] { nalloc, nmob }))
            });

            lts.AcceptanceSet = lts.Nodes.Select (x => x.Id).ToArray ();

            var f = new StrongImplication (alloc, new Next (mob)).Negate ();

            ILTL2Buchi t = new GPVW ();
            var gba = t.GetAutomaton (f);

            var otfec = new OnTheFlyGBAEmptinessChecker (gba, lts);
            var e1 = otfec.EmptinessSearch ();

            Console.WriteLine (e1);
            
            Console.WriteLine ("****");
            foreach (var i in otfec.path.Reverse ()) {
                Console.WriteLine (lts.Nodes[i.Item2].Name  + " x " +  gba.Nodes[i.Item1].Name);
            }
            Console.WriteLine ("****");
            
        }
    }
}
