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
    }
}

