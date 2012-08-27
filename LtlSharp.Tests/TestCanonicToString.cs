using System;
using NUnit.Framework;
using LtlSharp.Utils;

namespace LtlSharp.Tests
{
	internal static class TestHelpers {
        public static string ToCanonicString (this string expression)
        {
            var formula = Parser.Parse (expression);
            var v = new CanonicToString (formula);
            return v.String;
        }
    }
    
    [TestFixture()]
	public class TestCanonicToString
	{   
		[Test()]
		public void TestProposition ()
		{
			Assert.AreEqual ("test", "test".ToCanonicString());
		}
        
        [Test()]
        public void TestConjunction ()
        {
            Assert.AreEqual ("a & b", "a & b".ToCanonicString());
        }
        
        [Test()]
        public void TestDisjunction ()
        {
            Assert.AreEqual ("a | b", "a | b".ToCanonicString());
        }
        
        [Test()]
        public void TestImplication ()
        {
            Assert.AreEqual ("a -> b", "a -> b".ToCanonicString());
        }
        
        [Test()]
        public void TestEquivalence ()
        {
            Assert.AreEqual ("a <-> b", "a <-> b".ToCanonicString());
        }
        
        [Test()]
        public void TestStrongEquivalence ()
        {
            Assert.AreEqual ("G (a <-> b)", "a <=> b".ToCanonicString());
        }
        
        [Test()]
        public void TestStrongImplication ()
        {
            Assert.AreEqual ("G (a -> b)", "a => b".ToCanonicString());
        }
        
        [Test()]
        public void TestRelease ()
        {
            Assert.AreEqual ("a R b", "a R b".ToCanonicString());
        }
        
        [Test()]
        public void TestUntil ()
        {
            Assert.AreEqual ("a U b", "a U b".ToCanonicString());
        }
        
        [Test()]
        public void TestUnless ()
        {
            Assert.AreEqual ("b R (b | a)", "a W b".ToCanonicString());
        }
	}
}

