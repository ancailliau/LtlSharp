using System;
using NUnit.Framework;
using LtlSharp.Models;
using System.Linq;
using LtlSharp.ProbabilisticSystems;

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
    }
}

