using System;
using NUnit.Framework;
using LtlSharp.Buchi;
using LtlSharp;
using LtlSharp.Buchi.Automata;
using System.Collections.Generic;
using LtlSharp.Translators;
using LtlSharp.Automata;

namespace CheckMyModels.Tests
{
    [TestFixture()]
    public class TestSafra
    {
        [Test()]
        public void TestSafraExampleInPaper ()
        {
            var ba = new BuchiAutomata ();
            var q = new AutomataNode ("qI"); ba.AddVertex (q);
            var f = new AutomataNode ("f"); ba.AddVertex (f);
            var g = new AutomataNode ("g"); ba.AddVertex (g);

            var pa = new Proposition ("a");
            var pb = new Proposition ("b");
            var pc = new Proposition ("c");

            var a = new HashSet<ILiteral> (new ILiteral[] { pa });
            var b = new HashSet<ILiteral> (new ILiteral[] { pb });
            var c = new HashSet<ILiteral> (new ILiteral[] { pc });

            // a,b from qI to qI
            ba.AddEdge (new LabeledAutomataTransition<AutomataNode> (q, q, a));
            ba.AddEdge (new LabeledAutomataTransition<AutomataNode> (q, q, b));

            // c from f to f
            ba.AddEdge (new LabeledAutomataTransition<AutomataNode> (f, f, c));

            // a from qI to f
            ba.AddEdge (new LabeledAutomataTransition<AutomataNode> (q, f, a));

            // a from f to g
            ba.AddEdge (new LabeledAutomataTransition<AutomataNode> (f, g, a));

            // a from g to g
            ba.AddEdge (new LabeledAutomataTransition<AutomataNode> (g, g, a));

            // a from g to f
            ba.AddEdge (new LabeledAutomataTransition<AutomataNode> (g, f, a));

            ba.InitialNodes.Add (q);
            ba.AcceptanceSet = new HashSet<AutomataNode> (new [] { f, g });

            var safra = new SafraDeterminization ();
            var rabin = safra.Transform (ba);

            var folder = new Fold<RabinAutomata> ();
            rabin = folder.Transform (rabin);

            Console.WriteLine (rabin.ToDot ());
        }
    }
}

