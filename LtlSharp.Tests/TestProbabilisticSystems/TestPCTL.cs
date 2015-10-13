using System;
using LtlSharp;
using NUnit.Framework;
using LtlSharp.ModelCheckers;
using CheckMyModels.Tests.Models;
using LtlSharp.Models;

namespace CheckMyModels.Tests.TestProbabilisticSystems
{
    [TestFixture()]
    public class TestPCTL
    {
        [Test ()]
        public void TestPCTL01 ()
        {
            var s1 = new Finally (new Proposition ("1"));
            var s2 = new Finally (new Proposition ("2"));
            var s3 = new Finally (new Proposition ("3"));
            var s4 = new Finally (new Proposition ("4"));
            var s5 = new Finally (new Proposition ("5"));
            var s6 = new Finally (new Proposition ("6"));
            
            var p = new Conjunction (s1, new Conjunction (s2, new Conjunction (s3, new Conjunction (s4, new Conjunction (s5, s6)))));
        }
        
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

            var checker = new PCTLModelChecker<MarkovNode> (mc, p1, 1e-10);
            var sat = checker.Check ();
            Assert.That (sat.Contains (mc.GetVertex ("start")));
            
            checker = new PCTLModelChecker<MarkovNode> (mc, p2, 1e-10);
            sat = checker.Check ();
            Assert.That (sat.Contains (mc.GetVertex ("start")));
            
            checker = new PCTLModelChecker<MarkovNode> (mc, p3, 1e-10);
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
            var mchecker = new PCTLModelChecker<MarkovNode> (mc, formula, 1e-5);
    
            var result = mchecker.Check ();
            Assert.That (result.SetEquals (mc.Nodes));
        }
    }
}

