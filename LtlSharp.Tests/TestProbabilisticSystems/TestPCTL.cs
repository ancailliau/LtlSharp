using System;
using LtlSharp;
using NUnit.Framework;

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
            var s8 = new Proposition ("8");
            var s9 = new Proposition ("9");
            var s10 = new Proposition ("10");
            var disj = new Disjunction (s8, new Disjunction (s9, s10));
            var neg = new Negation (disj);
            var won = new Proposition ("won");
            
            var p1 = new ProbabilisticOperator (new Until (neg, won), .32);
            var p2 = new ProbabilisticOperator (new Until (neg, won, 5), .32);
            var p3 = new ProbabilisticOperator (new Until (neg, new ProbabilisticOperator (new Globally (won), 1), 5), .32);
        }
    }
}

