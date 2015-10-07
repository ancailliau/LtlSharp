using System;
using NUnit.Framework;
using LtlSharp.Models;
using System.Linq;
using LtlSharp.ProbabilisticSystems;
using System.Collections.Generic;

namespace CheckMyModels.Tests.Models
{
    [TestFixture()]
    public class TestReachability
    {
        [Test ()]
        public void ReachabilityLinearSystem ()
        {
            var mc = TestMarkovChain.GetExample ("101");
            var delivered = mc.GetVertex ("delivered");
            var start = mc.GetVertex ("start");
            var ntry = mc.GetVertex ("try");
            var lost = mc.GetVertex ("lost");

            var dict = mc.ReachabilityLinearSystem (new [] { delivered });
            Assert.NotNull (dict);
            
            Assert.That (dict.ContainsKey (start), "Node 'start' is not detected as reachable.");
            Assert.That (dict.ContainsKey (ntry), "Node 'try' is not detected as reachable.");
            Assert.That (dict.ContainsKey (lost), "Node 'lost' is not detected as reachable.");
            Assert.That (dict.ContainsKey (delivered), "Node 'delivered' is not detected as reachable.");
            
            Assert.AreEqual (1, dict [start]);
            Assert.AreEqual (1, dict [ntry]);
            Assert.AreEqual (1, dict [lost]);
            Assert.AreEqual (1, dict [delivered]);
        }
        
        [Test ()]
        public void TestConstrainedReachability ()
        {
            var mc = TestMarkovChain.GetExample ("craps");
            var start = mc.GetVertex ("start");
            var s4 = mc.GetVertex ("4");
            var s5 = mc.GetVertex ("5");
            var s6 = mc.GetVertex ("6");
            var won = mc.GetVertex ("won");

            var dict = mc.ConstrainedReachability (new [] { start, s4, s5, s6 }, new [] { won }, true, n: 2);
            Assert.NotNull (dict);
            
            Assert.That (dict.ContainsKey (won), "Node 'won' is not detected as reachable.");
            Assert.That (dict.ContainsKey (start), "Node 'start' is not detected as reachable.");
            Assert.That (dict.ContainsKey (s4), "Node '4' is not detected as reachable.");
            Assert.That (dict.ContainsKey (s5), "Node '5' is not detected as reachable.");
            Assert.That (dict.ContainsKey (s6), "Node '6' is not detected as reachable.");

            Assert.AreEqual (1, dict [won], .00001);
            Assert.AreEqual (338d / (36 * 36), dict [start], .00001);
            Assert.AreEqual (189d / (36 * 36), dict [s4], .00001);
            Assert.AreEqual (248d / (36 * 36), dict [s5], .00001);
            Assert.AreEqual (305d / (36 * 36), dict [s6], .00001);
        }
        
        [Test ()]
        public void TestConstrainedReachabilityTransient ()
        {
            var mc = TestMarkovChain.GetExample ("craps");
            var start = mc.GetVertex ("start");
            var s4 = mc.GetVertex ("4");
            var s5 = mc.GetVertex ("5");
            var s6 = mc.GetVertex ("6");
            var won = mc.GetVertex ("won");

            var dict = mc.TransientConstrainedReachability (new [] { start, s4, s5, s6 }, new [] { won }, 0);
            Assert.AreEqual (0, dict, .00001);

            dict = mc.TransientConstrainedReachability (new [] { start, s4, s5, s6 }, new [] { won }, 1);
            Assert.AreEqual (2d, dict * 9, .00001);
            
            dict = mc.TransientConstrainedReachability (new [] { start, s4, s5, s6 }, new [] { won }, 2);
            Assert.AreEqual (338d, dict * (36*36), .00001);
        }
        
        [Test ()]
        public void TestQualitativeRepeatedReachability ()
        {
            var mc = TestMarkovChain.GetExample ("pmc-lecture-15-13");
            var s0 = mc.GetVertex ("s0");
            var s1 = mc.GetVertex ("s1");
            var s2 = mc.GetVertex ("s2");
            var s3 = mc.GetVertex ("s3");
            var s4 = mc.GetVertex ("s4");
            var s5 = mc.GetVertex ("s5");
            
            var B = new HashSet<MarkovNode> (new [] { s3, s4 });
            var C = new HashSet<MarkovNode> (new [] { s5 });
            
            Assert.That (mc.QualitativeRepeatedReachability (B.Union (C)).Contains (s0));
            Assert.That (!mc.QualitativeRepeatedReachability (B).Contains (s0));
            Assert.That (mc.QualitativeRepeatedReachability (C).Contains (s2));
        }
        
        [Test ()]
        public void TestQuantitativeRepeatedReachability ()
        {
            var mc = TestMarkovChain.GetExample ("pmc-lecture-15-13");
            var s0 = mc.GetVertex ("s0");
            var s1 = mc.GetVertex ("s1");
            var s2 = mc.GetVertex ("s2");
            var s3 = mc.GetVertex ("s3");
            var s4 = mc.GetVertex ("s4");
            var s5 = mc.GetVertex ("s5");

            var B = new HashSet<MarkovNode> (new [] { s3, s4 });
            var C = new HashSet<MarkovNode> (new [] { s5 });

            var dict = mc.QuantitativeRepeatedReachability (B.Union (C));
            Assert.That (dict.ContainsKey (s0), "State 's0' cannot repeatly reach a state in B Union C.");
            Assert.That (dict.ContainsKey (s1), "State 's1' cannot repeatly reach a state in B Union C.");
            Assert.That (dict.ContainsKey (s2), "State 's2' cannot repeatly reach a state in B Union C.");
            Assert.That (dict.ContainsKey (s3), "State 's3' cannot repeatly reach a state in B Union C.");
            Assert.That (dict.ContainsKey (s4), "State 's4' cannot repeatly reach a state in B Union C.");
            Assert.That (dict.ContainsKey (s5), "State 's5' cannot repeatly reach a state in B Union C.");
            Assert.AreEqual (dict [s0], 1);
            Assert.AreEqual (dict [s1], 1);
            Assert.AreEqual (dict [s2], 1);
            Assert.AreEqual (dict [s3], 1);
            Assert.AreEqual (dict [s4], 1);
            Assert.AreEqual (dict [s5], 1);
            
            dict = mc.QuantitativeRepeatedReachability (B);
            Assert.That (dict.ContainsKey (s0), "State 's0' cannot repeatly reach a state in B.");
            Assert.That (dict.ContainsKey (s1), "State 's1' cannot repeatly reach a state in B.");
            Assert.That (dict.ContainsKey (s3), "State 's3' cannot repeatly reach a state in B.");
            Assert.That (dict.ContainsKey (s4), "State 's4' cannot repeatly reach a state in B.");
            Assert.AreEqual (dict [s0], 5d/6, 1e-5);
            Assert.AreEqual (dict [s1], 2d/3, 1e-5);
            Assert.AreEqual (dict [s3], 1);
            Assert.AreEqual (dict [s4], 1);
            
            dict = mc.QuantitativeRepeatedReachability (C);
            Assert.That (dict.ContainsKey (s0), "State 's0' cannot repeatly reach a state in C.");
            Assert.That (dict.ContainsKey (s1), "State 's1' cannot repeatly reach a state in C.");
            Assert.That (dict.ContainsKey (s5), "State 's5' cannot repeatly reach a state in C.");
            Assert.That (dict.ContainsKey (s2), "State 's2' cannot repeatly reach a state in C.");
            Assert.AreEqual (dict [s0], 1d/6, 1e-5);
            Assert.AreEqual (dict [s1], 1d/3, 1e-5);
            Assert.AreEqual (dict [s5], 1);
            Assert.AreEqual (dict [s2], 1);
        }
    }
}

