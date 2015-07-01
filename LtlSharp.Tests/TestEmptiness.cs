using System;
using NUnit.Framework;
using LtlSharp.Buchi.LTL2Buchi;
using LtlSharp;
using LtlSharp.Buchi.Translators;
using LtlSharp.Buchi;
using LittleSharp.Buchi;
using System.Linq;

namespace CheckMyModels.Tests
{
    [TestFixture()]
    public class TestEmptiness
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
            ILTLFormula f0 = new StrongImplication (new Proposition ("allocated"), new Proposition ("allocated"));
            AssertEquivalentEmptiness (f0.Negate ());
        }
        
        [Test()]
        public void TestF0 ()
        {
            ILTLFormula f0 = new False ();
            AssertEquivalentEmptiness (f0);
        }
        
        [Test()]
        public void TestF1 ()
        {
            ILTLFormula f = new Until (p1, p2);
            AssertEquivalentEmptiness (f);
        }
        
        [Test()]
        public void TestF2 ()
        {
            ILTLFormula f = new Until (p1, new Until (p2, p3));
            AssertEquivalentEmptiness (f);
        }
        
        [Test()]
        public void TestF3 ()
        {
            ILTLFormula f = new Negation (new Until (p1, new Until (p2, p3))); // problem
            AssertEquivalentEmptiness (f);
        }
        
        [Test()]
        public void TestF4 ()
        {
            ILTLFormula f = new Implication (new Globally (new Finally (p1)), new Globally (new Finally (p2))); // problem
            AssertEquivalentEmptiness (f);
        }
        
        [Test()]
        public void TestF5 ()
        {
            ILTLFormula f = new Until (new Finally (p1), new Globally (p2));
            AssertEquivalentEmptiness (f);
        }
        
        [Test()]
        public void TestF6 ()
        {
            ILTLFormula f = new Until (new Globally (p1), p2);
            AssertEquivalentEmptiness (f);
        }

        [Test()]
        public void TestF7 ()
        {
            ILTLFormula f = new Negation (new Equivalence (new Finally (new Finally (p1)), new Finally (p1)));
            AssertEquivalentEmptiness (f);
        }
        
        [Test()]
        public void TestF7bis ()
        {
            ILTLFormula f = new Equivalence (new Finally (new Finally (p1)), new Finally (p1));
            AssertEquivalentEmptiness (f);
        }

        [Test()]
        public void TestF7tris ()
        {
            ILTLFormula f = new Negation (new Implication (new Finally (new Finally (p1)), new Finally (p1)));
            AssertEquivalentEmptiness (f);
        }
        
        [Test()]
        public void TestF8 ()
        {
            ILTLFormula f = new Release (p1, p2);
            AssertEquivalentEmptiness (f);
        }
        
        [Test()]
        public void TestF9 ()
        {
            ILTLFormula f = new Release (new Negation (p1), new Negation(p2));
            AssertEquivalentEmptiness (f);
        }
        
        [Test()]
        public void TestF10 ()
        {
            ILTLFormula f = new Release (p1, new Release (p2, p3));
            AssertEquivalentEmptiness (f);
        }
    }
}

