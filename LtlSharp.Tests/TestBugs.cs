using System;
using NUnit.Framework;
using LtlSharp.Utils;

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
        
        [Test()]
        public void TestBug2 ()
        {
            var formula = Parser.Parse ("G (a -> a U b) & G (b -> b U a)");
            Assert.IsNotNull (formula);
            
            var v = new ExtractAlphabet (formula);
            var alphabet = v.Alphabet;
            Assert.AreEqual (2, alphabet.Count);
        }
	}
}

