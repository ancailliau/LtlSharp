using System;
using NUnit.Framework;
using LtlSharp.Buchi;
using LtlSharp;
using LtlSharp.Buchi.Automata;
using System.Collections.Generic;
using LtlSharp.Translators;
using LtlSharp.Automata;
using LtlSharp.Automata.AcceptanceConditions;

namespace CheckMyModels.Tests
{
    [TestFixture()]
    public class TestSafra
    {
        [Test()]
        public void TestSafraExampleInPaper ()
        {
            var ba = new BuchiAutomata ();
            var q = new AutomatonNode ("qI"); ba.AddNode (q);
            var f = new AutomatonNode ("f"); ba.AddNode (f);
            var g = new AutomatonNode ("g"); ba.AddNode (g);

            var pa = new Proposition ("a");
            var pb = new Proposition ("b");
            var pc = new Proposition ("c");

            var a = new HashSet<ILiteral> (new ILiteral[] { pa });
            var b = new HashSet<ILiteral> (new ILiteral[] { pb });
            var c = new HashSet<ILiteral> (new ILiteral[] { pc });

            // a,b from qI to qI
            ba.AddTransition (new LabeledAutomataTransition<AutomatonNode> (q, q, a));
            ba.AddTransition (new LabeledAutomataTransition<AutomatonNode> (q, q, b));

            // c from f to f
            ba.AddTransition (new LabeledAutomataTransition<AutomatonNode> (f, f, c));

            // a from qI to f
            ba.AddTransition (new LabeledAutomataTransition<AutomatonNode> (q, f, a));

            // a from f to g
            ba.AddTransition (new LabeledAutomataTransition<AutomatonNode> (f, g, a));

            // a from g to g
            ba.AddTransition (new LabeledAutomataTransition<AutomatonNode> (g, g, a));

            // a from g to f
            ba.AddTransition (new LabeledAutomataTransition<AutomatonNode> (g, f, a));

            ba.SetInitialNode (q);
            ba.SetAcceptanceCondition (new BuchiAcceptance<AutomatonNode> (new [] { f, g }));

            var safra = new SafraDeterminization ();
            var rabin = safra.Transform (ba);

            var folder = new Fold<RabinAutomata> ();
            rabin = folder.Transform (rabin);

            Console.WriteLine (rabin.ToDot ());
        }
    }
}

