using System;
using NUnit.Framework;

namespace LtlSharp.Tests
{
	[TestFixture()]
	public class TestLAS
	{
		[TestCase("G (incidentReported -> F ambulanceOnScene)")]
		[TestCase("G (incidentReported -> F ambulanceAllocated)")]
		[TestCase("G (ambulanceAllocated -> F ambulanceOnScene)")]
		[TestCase("G (ambulanceAllocated -> F ambulanceMobilized)")]
		[TestCase("G (ambulanceAllocated & onRoad -> F ambulanceMobilized)")]
		[TestCase("G (ambulanceAllocated & !onRoad -> F ambulanceMobilized)")]
		[TestCase("G (ambulanceMobilized -> F ambulanceOnScene)")]
		[TestCase("G ((ambulanceMobilizedByFax | ambulanceMobilizedByPhone) -> ambulanceMobilized)")]
		public void TestProposition (string input)
		{
			Assert.IsNotNull (Parser.Parse (input));
		}
	}
}

