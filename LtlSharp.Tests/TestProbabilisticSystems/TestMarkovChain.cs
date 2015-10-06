using System;
using NUnit.Framework;
using LtlSharp.Models;
using System.Linq;
using LtlSharp.ProbabilisticSystems;

namespace CheckMyModels.Tests.Models
{
    [TestFixture()]
    public class TestReachabilityLinearSystem
    {
        public void TestAbsorbing ()
        {
            var mc = TestMarkovChain.GetExample ("101");
            var delivered = mc.GetVertex ("delivered");
            var start = mc.GetVertex ("start");
            var ntry = mc.GetVertex ("try");
            var lost = mc.GetVertex ("lost");
            
            var dict = mc.ReachabilityLinearSystem (new [] { delivered });

            Assert.AreEqual (dict[start], 1);
            Assert.AreEqual (dict[ntry], 1);
            Assert.AreEqual (dict[lost], 1);
            Assert.AreEqual (dict[delivered], 1);
        }
    }
}

