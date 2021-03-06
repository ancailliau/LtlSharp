﻿using System;
using NUnit.Framework;
using LtlSharp.Buchi.LTL2Buchi;
using LtlSharp;

using LtlSharp.Buchi;
using LittleSharp.Buchi;
using System.Linq;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using LtlSharp.Automata;
using LtlSharp.Automata.Utils;

namespace LtlSharp.Tests
{
    [TestFixture()]
    public class TestEmptiness
    {
        private void AssertEquivalentEmptiness (ITLFormula f)
        {
            var gpvw = new GPVW ();
            var ltl2buchi = new Gia02 ();

            var gba = gpvw.GetGBA (f);
            var ba = gba.ToBA ();
            var ba2 = ltl2buchi.GetBuchiAutomaton (f);
            
            Console.WriteLine (ba.ToDot ());
            Console.WriteLine (ba2.ToDot ());
            
            var ec = new GBAEmptinessChecker ();
            var ec2 = new EmptinessChecker<AutomatonNode> (ba);
            var ec3 = new EmptinessChecker<AutomatonNode> (ba2);
            
            var e1 = ec.EmptinessSearch (gba);
            var e2 = ec2.Emptiness ();
            var e3 = ec3.Emptiness ();
            
            Console.WriteLine (e2);
            Console.WriteLine (e3);
            
            Assert.That (e1 == e2, "GBAEmptinessChecker and EmptinessChecker (on transformed ba) differs");
            Assert.That (e2 == e3, "EmptinessChecker and EmptinessChecker (on transformed ba) differs");
            Assert.That (e1 == e3, "GBAEmptinessChecker and EmptinessChecker differs");
        }
        
        
        Proposition p1 = new Proposition ("p1");
        Proposition p2 = new Proposition ("p2");
        Proposition p3 = new Proposition ("p3");
        
        [Test()]
        public void TestNoSpawnUntilInit ()
        {
            ITLFormula f0 = new Until (new Proposition ("spawn").Negate (), new Proposition ("init"));
            AssertEquivalentEmptiness (f0.Negate ());
        }
        
        [Test()]
        public void TestMobilizedWhenAlloc ()
        {
            ITLFormula f0 = new StrongImplication (new Proposition ("allocated"), new Proposition ("allocated"));
            AssertEquivalentEmptiness (f0.Negate ());
        }
        
        [Test()]
        public void TestF0 ()
        {
            ITLFormula f0 = new False ();
            AssertEquivalentEmptiness (f0);
        }
        
        [Test()]
        public void TestF1 ()
        {
            ITLFormula f = new Until (p1, p2);
            AssertEquivalentEmptiness (f);
        }
        
        [Test()]
        public void TestF2 ()
        {
            ITLFormula f = new Until (p1, new Until (p2, p3));
            AssertEquivalentEmptiness (f);
        }
        
        [Test()]
        public void TestF3 ()
        {
            ITLFormula f = new Negation (new Until (p1, new Until (p2, p3))); // problem
            AssertEquivalentEmptiness (f);
        }
        
        [Test()]
        public void TestF4 ()
        {
            // (G (F (p1))) -> (G (F (p2)))
            ITLFormula f = new Implication (new Globally (new Finally (p1)), new Globally (new Finally (p2))); // problem
            AssertEquivalentEmptiness (f);
        }
        
        [Test()]
        public void TestF5 ()
        {
            ITLFormula f = new Until (new Finally (p1), new Globally (p2));
            AssertEquivalentEmptiness (f);
        }
        
        [Test()]
        public void TestF6 ()
        {
            ITLFormula f = new Until (new Globally (p1), p2);
            AssertEquivalentEmptiness (f);
        }

        [Test()]
        public void TestF7 ()
        {
            ITLFormula f = new Negation (new Equivalence (new Finally (new Finally (p1)), new Finally (p1)));
            Console.WriteLine (f);
            AssertEquivalentEmptiness (f);
        }
        
        [Test()]
        public void TestF7bis ()
        {
            ITLFormula f = new Equivalence (new Finally (new Finally (p1)), new Finally (p1));
            AssertEquivalentEmptiness (f);
        }

        [Test()]
        public void TestF7tris ()
        {
            ITLFormula f = new Negation (new Implication (new Finally (new Finally (p1)), new Finally (p1)));
            AssertEquivalentEmptiness (f);
        }
        
        [Test()]
        public void TestF8 ()
        {
            ITLFormula f = new Release (p1, p2);
            AssertEquivalentEmptiness (f);
        }
        
        [Test()]
        public void TestF9 ()
        {
            ITLFormula f = new Release (new Negation (p1), new Negation(p2));
            AssertEquivalentEmptiness (f);
        }
        
        [Test()]
        public void TestF10 ()
        {
            ITLFormula f = new Release (p1, new Release (p2, p3));
            AssertEquivalentEmptiness (f);
        }
    }
}

