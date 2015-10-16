using System;
using LtlSharp;
using NUnit.Framework;
using LtlSharp.ModelCheckers;
using LtlSharp.Tests.Models;
using LtlSharp.Models;
using LtlSharp.Automata;

namespace LtlSharp.Tests.TestProbabilisticSystems
{
    [TestFixture()]
    public class TestPCTL
    {
        [Test ()]
        public void TestCraps ()
        {
            var mc = TestMarkovChain.GetExample ("craps");

            var s8 = new Proposition ("8");
            var s9 = new Proposition ("9");
            var s10 = new Proposition ("10");
            var disj = new Disjunction (s8, new Disjunction (s9, s10));
            var neg = new Negation (disj);
            var won = new Proposition ("won");

            var p1 = new ProbabilisticOperator (new Until (neg, won), .32, 1);
            var p2 = new ProbabilisticOperator (new Until (neg, won, 5), .32, 1);
            var p3 = new ProbabilisticOperator (new Until (neg, new ProbabilisticOperator (new Globally (won), 1), 5), .32, 1);

            var checker = new PCTLModelChecker<AutomatonNode> (mc, p1, 1e-10);
            var sat = checker.Check ();
            Assert.That (sat.Contains (mc.GetVertex ("start")));
            
            checker = new PCTLModelChecker<AutomatonNode> (mc, p2, 1e-10);
            sat = checker.Check ();
            Assert.That (sat.Contains (mc.GetVertex ("start")));
            
            checker = new PCTLModelChecker<AutomatonNode> (mc, p3, 1e-10);
            sat = checker.Check ();
            Assert.That (sat.Contains (mc.GetVertex ("start")));
        }
        
        [Test ()]
        public void TestCommunicationProtocol () {
            var formula = new ProbabilisticOperator (
                new Globally (
                    new ProbabilisticOperator (
                        new Implication (
                            new Proposition ("try"), 
                            new Next (
                                new Proposition ("delivered")
                            )
                        ), 
                        .9, 1
                    )
                ), 
                1, 1
            );
    
            var mc = TestMarkovChain.GetExampleFig101 ();
            var mchecker = new PCTLModelChecker<AutomatonNode> (mc, formula, 1e-5);
    
            var result = mchecker.Check ();
            Console.WriteLine ("{"+string.Join (",", result)+"}");
            Assert.That (result.SetEquals (mc.Nodes));
        }
    }
}

