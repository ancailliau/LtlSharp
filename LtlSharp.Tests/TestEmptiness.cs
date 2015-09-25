using System;
using NUnit.Framework;
using LtlSharp.Buchi.LTL2Buchi;
using LtlSharp;
using LtlSharp.Buchi.Translators;
using LtlSharp.Buchi;
using LittleSharp.Buchi;
using System.Linq;
using LtlSharp.Buchi.Automata;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace CheckMyModels.Tests
{
    [TestFixture()]
    public class TestEmptiness
    {
        private void AssertEquivalentEmptiness (ILTLFormula f)
        {
            var gpvw = new GPVW ();
            var ltl2buchi = new Gia02 ();

            var gba = gpvw.GetGBA (f);
            var ba = GBA2BA.Transform (gba);
            var ba2 = ltl2buchi.GetAutomaton (f);

            var ec = new GBAEmptinessChecker ();
            var ec2 = new EmptinessChecker (ba);
            var ec3 = new EmptinessChecker (ba2);
            
            var e1 = ec.EmptinessSearch (gba);
            var e2 = ec2.Emptiness ();
            var e3 = ec3.Emptiness ();

            // Display Buchi Automata
            
            var graphviz = new GraphvizAlgorithm<AutomataNode, LabeledAutomataTransition<AutomataNode>> (ba);
            graphviz.FormatVertex += (object sender, FormatVertexEventArgs<AutomataNode> e) => {
                e.VertexFormatter.Label = e.Vertex.Name;
                if (ba.InitialNodes.Contains (e.Vertex))
                    e.VertexFormatter.Style = GraphvizVertexStyle.Bold;
                if (ba.AcceptanceSet.Contains (e.Vertex))
                    e.VertexFormatter.Shape = QuickGraph.Graphviz.Dot.GraphvizVertexShape.DoubleCircle;
            };
            graphviz.FormatEdge += (object sender, FormatEdgeEventArgs<AutomataNode, LabeledAutomataTransition<AutomataNode>> e) => {
                e.EdgeFormatter.Label.Value = string.Join (",", e.Edge.Labels);
            };
            Console.WriteLine (graphviz.Generate ());
            
            
            Assert.That (e1 == e2);
            Assert.That (e2 == e3);
            Assert.That (e1 == e3);
        }
        
        
        Proposition p1 = new Proposition ("p1");
        Proposition p2 = new Proposition ("p2");
        Proposition p3 = new Proposition ("p3");
        
        [Test()]
        public void TestNoSpawnUntilInit ()
        {
            ILTLFormula f0 = new Until (new Proposition ("spawn").Negate (), new Proposition ("init"));
            AssertEquivalentEmptiness (f0.Negate ());
        }
        
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
            // (G (F (p1))) -> (G (F (p2)))
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

