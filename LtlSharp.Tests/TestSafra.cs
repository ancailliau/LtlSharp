using System;
using NUnit.Framework;
using LtlSharp.Buchi;
using LtlSharp;
using System.Collections.Generic;
using LtlSharp.Translators;
using LtlSharp.Automata;
using LtlSharp.Automata.AcceptanceConditions;
using LtlSharp.Automata.OmegaAutomata;
using LtlSharp.Automata.Nodes.Factories;

using LtlSharp.Automata.Utils;

namespace LtlSharp.Tests
{
    [TestFixture()]
    public class TestSafra
    {
        [Test()]
        public void TestSafraExampleInPaper ()
        {
            var ba = new BuchiAutomaton<AutomatonNode> (new AutomatonNodeFactory ());
            var q = new AutomatonNode ("qI"); ba.AddNode (q);
            var f = new AutomatonNode ("f"); ba.AddNode (f);
            var g = new AutomatonNode ("g"); ba.AddNode (g);

            var pa = new Proposition ("a");
            var pb = new Proposition ("b");
            var pc = new Proposition ("c");

            var a = new ILiteral[] { pa };
            var b = new ILiteral[] { pb };
            var c = new ILiteral [] { pc };

            // a,b from qI to qI
            ba.AddTransition (q, q, a);
            ba.AddTransition (q, q, b);

            // c from f to f
            ba.AddTransition (f, f, c);

            // a from qI to f
            ba.AddTransition (q, f, a);

            // a from f to g
            ba.AddTransition (f, g, a);

            // a from g to g
            ba.AddTransition (g, g, a);

            // a from g to f
            ba.AddTransition (g, f, a);

            ba.SetInitialNode (q);
            ba.SetAcceptanceCondition (new BuchiAcceptance<AutomatonNode> (new [] { f, g }));

            var safra = new SafraDeterminization ();
            var rabin = safra.Transform (ba);

            //var folder = new Fold<RabinAutomaton<AutomatonNode>, AutomatonNode> ();
            //rabin = folder.Transform (rabin);
            rabin.UnfoldTransitions ();

            Console.WriteLine (rabin.ToDot ());
        }
    }
}

