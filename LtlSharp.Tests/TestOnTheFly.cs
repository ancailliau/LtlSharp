using System;
using NUnit.Framework;
using LtlSharp.Buchi.LTL2Buchi;
using LtlSharp;
using LtlSharp.Buchi.Translators;
using LtlSharp.Buchi;
using LittleSharp.Buchi;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi.Automata;

namespace CheckMyModels.Tests
{
    [TestFixture()]
    public class TestOnTheFly
    {
        private void AssertEquivalentEmptiness (ILTLFormula f)
        {
            ILTL2Buchi t = new GPVW ();

            var gba = t.GetAutomaton (f);
            var ba = GBA2BA.Transform (gba);

            var ec = new GBAEmptinessChecker ();
            var e1 = ec.EmptinessSearch (gba);
//            Console.WriteLine (e1 ? "Empty" : "Not empty");

            var ec2 = new EmptinessChecker (ba);
            var e2 = ec2.Emptiness ();
//            Console.WriteLine (e2 ? "Empty" : "Not empty");
            
            Console.WriteLine (gba.AcceptanceSets.Length);
            foreach (var aset in gba.AcceptanceSets) {
                Console.WriteLine (aset.Id + " : " + string.Join (",", aset.Nodes));
            }
            
            Console.WriteLine ("---");
            Console.WriteLine (gba.ToDot ());
            Console.WriteLine ("---");
            Console.WriteLine (ba.ToDot ());
            Console.WriteLine ("---");
            
            if (e1) {
            
                foreach (var i in ec2.dfsStack1.Reverse ()) {
                    Console.WriteLine (ba.Nodes [i].Name);
                }
                Console.WriteLine ("-- loop starts here --");
                foreach (var i in ec2.dfsStack2.Reverse ()) {
                    Console.WriteLine (ba.Nodes [i].Name);
                }
                Console.WriteLine ("-- loop ends here --");
            }
                
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
            var ba = GBA2BA.Transform (gba);
            
            var otfec = new OnTheFlyEmptinessChecker (ba, lts);
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
        }
        
    }
}

