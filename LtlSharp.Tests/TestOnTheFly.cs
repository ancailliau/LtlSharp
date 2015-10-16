using System;
using NUnit.Framework;
using LtlSharp.Buchi.LTL2Buchi;
using LtlSharp;
using LtlSharp.Buchi.Translators;
using LtlSharp.Buchi;
using LittleSharp.Buchi;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Automata;
using LtlSharp.Automata.AcceptanceConditions;
using LtlSharp.Automata.OmegaAutomata;
using LtlSharp.Automata.Nodes.Factories;
using LtlSharp.Automata.Transitions.Factories;

namespace LtlSharp.Tests
{
    [TestFixture()]
    public class TestOnTheFly
    {
        private void AssertEquivalentEmptiness (ITLFormula f)
        {
            var t = new GPVW ();

            var gba = t.GetGBA (f);
            var ba = GBA2BA.Transform (gba);

            var ec = new GBAEmptinessChecker ();
            var e1 = ec.EmptinessSearch (gba);
//            Console.WriteLine (e1 ? "Empty" : "Not empty");

            var ec2 = new EmptinessChecker<AutomatonNode> (ba);
            var e2 = ec2.Emptiness ();
//            Console.WriteLine (e2 ? "Empty" : "Not empty");
            
            //Console.WriteLine (gba.AcceptanceSets.Length);
            //foreach (var aset in gba.AcceptanceSets) {
            //    Console.WriteLine (aset.Id + " : " + string.Join (",", aset.Nodes));
            //}
            
            //Console.WriteLine ("---");
            //Console.WriteLine (gba.ToDot ());
            //Console.WriteLine ("---");
            //Console.WriteLine (ba.ToDot ());
            //Console.WriteLine ("---");
            
            //if (e1) {
            
            //    foreach (var i in ec2.dfsStack1.Reverse ()) {
            //        Console.WriteLine (ba.Nodes [i].Name);
            //    }
            //    Console.WriteLine ("-- loop starts here --");
            //    foreach (var i in ec2.dfsStack2.Reverse ()) {
            //        Console.WriteLine (ba.Nodes [i].Name);
            //    }
            //    Console.WriteLine ("-- loop ends here --");
            //}
                
            Assert.That (e1 == e2);
        }
        
        Proposition p1 = new Proposition ("p1");
        Proposition p2 = new Proposition ("p2");
        Proposition p3 = new Proposition ("p3");
        
        [Test()]
        public void TestMobilizedWhenAlloc ()
        {
            ILiteral alloc = new Proposition ("allocated");
            ILiteral mob = new Proposition ("mobilized");
            ILiteral nalloc = new Negation (alloc);
            ILiteral nmob = new Negation (mob);
            
            var lts = new BuchiAutomaton<AutomatonNode> (new AutomatonNodeFactory (), new LiteralSetDecorationFactory ());
            var n0 = new AutomatonNode ("i"); lts.AddNode (n0);
            var n1 = new AutomatonNode ("s0");lts.AddNode (n1);
            var n2 = new AutomatonNode ("s1");lts.AddNode (n2);
            var n3 = new AutomatonNode ("s2");lts.AddNode (n3);

            lts.SetInitialNode (n0);
               
            lts.AddTransition (n0, n1, new ILiteral [] { nalloc, nmob });
            lts.AddTransition (n1, n2, new ILiteral [] { alloc, nmob });
            lts.AddTransition (n2, n3, new ILiteral [] { alloc, mob });
            lts.AddTransition (n3, n0, new ILiteral [] { nalloc, nmob });
            
            lts.SetAcceptanceCondition (new BuchiAcceptance<AutomatonNode> (lts.Nodes));
            
            var f = new StrongImplication (alloc, new Next (mob)).Negate ();
            
            var t = new GPVW ();
            var gba = t.GetGBA (f);
            var ba = GBA2BA.Transform (gba);
            
            var otfec = new OnTheFlyEmptinessChecker<AutomatonNode,AutomatonNode> (ba, lts);
            var e1 = otfec.Emptiness ();
            
            if (e1) {

                foreach (var i in otfec.counterexample_prefix) {
                    Console.WriteLine (i.Name);
                }
                Console.WriteLine ("-- loop starts here --");
                foreach (var i in otfec.counterexample_loop) {
                    Console.WriteLine (i.Name);
                }
                Console.WriteLine ("-- loop ends here --");
            } else {
                Console.WriteLine ("No trace found");
            }
        }
        
        
        [Test()]
        public void TestMobilizedWhenAllocGBA ()
        {
            ILiteral alloc = new Proposition ("allocated");
            ILiteral mob = new Proposition ("mobilized");
            ILiteral nalloc = new Negation (alloc);
            ILiteral nmob = new Negation (mob);
            
            var lts = new BuchiAutomaton<AutomatonNode> (new AutomatonNodeFactory (), new LiteralSetDecorationFactory ());
            var n0 = new AutomatonNode ("i"); lts.AddNode (n0);
            var n1 = new AutomatonNode ("s0");lts.AddNode (n1);
            var n2 = new AutomatonNode ("s1");lts.AddNode (n2);
            var n3 = new AutomatonNode ("s2");lts.AddNode (n3);

            lts.SetInitialNode (n0);

            lts.AddTransition (n0, n1, new ILiteral [] { nalloc, nmob });
            lts.AddTransition (n1, n2, new ILiteral [] { alloc, nmob });
            lts.AddTransition (n2, n3, new ILiteral [] { alloc, mob });
            lts.AddTransition (n3, n0, new ILiteral [] { nalloc, nmob });

            lts.SetAcceptanceCondition (new BuchiAcceptance<AutomatonNode> (lts.Nodes));

            var f = new StrongImplication (alloc, new Next (mob)).Negate ();

            var t = new GPVW ();
            var gba = t.GetGBA (f);

            var otfec = new OnTheFlyGBAEmptinessChecker (gba, lts);
            var e1 = otfec.EmptinessSearch ();

            Console.WriteLine (e1);
        }
        
    }
}

