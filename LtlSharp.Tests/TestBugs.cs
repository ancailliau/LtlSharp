using System;
using NUnit.Framework;

namespace LtlSharp.Tests
{
	[TestFixture()]
	public class TestBugs
	{
		[Test()]
        public void TestBug1 ()
        {
            var expression = Parser.Parse ("G ( proposalEncoded -> F ridePlanned)");
            Assert.IsNotNull (expression);
            
            expression = Parser.Parse ("G ( proposalEncoded -> F ridePlanned)");
            Assert.IsNotNull (expression);
            Assert.IsInstanceOf (typeof (Globally), expression);
        }
	}
}

