using System;
using NUnit.Framework;

namespace LtlSharp.Tests
{
	[TestFixture()]
	public class Test
	{
		[Test()]
		public void TestProposition ()
		{
			var expression = Parser.Parse ("test");
			Assert.That (expression.GetType () == typeof (Proposition));
			Assert.That (((Proposition) expression).Name == "test");
		}

		[Test()]
		public void TestEquivalence ()
		{
			var expression = Parser.Parse ("test1 <-> test2");
			Assert.IsInstanceOf (typeof (Equivalence), expression);
			Assert.IsInstanceOf (typeof (Proposition), ((Equivalence) expression).Left);
			Assert.IsInstanceOf (typeof (Proposition), ((Equivalence) expression).Right);

			expression = Parser.Parse ("test1 <-> test2 <-> test3");
			Assert.IsInstanceOf (typeof (Equivalence), expression);
			Assert.IsInstanceOf (typeof (Proposition), ((Equivalence) expression).Left);
			Assert.IsInstanceOf (typeof (Equivalence), ((Equivalence) expression).Right);

			var right = (Equivalence) ((Equivalence) expression).Right;
			Assert.IsInstanceOf (typeof (Proposition), right.Left);
			Assert.IsInstanceOf (typeof (Proposition), right.Right);
			
			expression = Parser.Parse ("test1 <-> test2 -> test3");
			Assert.IsInstanceOf (typeof (Equivalence), expression);
			Assert.IsInstanceOf (typeof (Proposition), ((Equivalence) expression).Left);
			Assert.IsInstanceOf (typeof (Implication), ((Equivalence) expression).Right);
		}

		[Test()]
		public void TestImplication ()
		{
			var expression = Parser.Parse ("test1");
			Assert.IsInstanceOf (typeof (Proposition), expression);

			expression = Parser.Parse ("test1 -> test2");
			Assert.IsInstanceOf (typeof (Implication), expression);
			Assert.IsInstanceOf (typeof (Proposition), ((Implication) expression).Left);
			Assert.IsInstanceOf (typeof (Proposition), ((Implication) expression).Right);

			expression = Parser.Parse ("test1 -> test2 -> test3");
			Assert.IsInstanceOf (typeof (Implication), expression);
			Assert.IsInstanceOf (typeof (Proposition), ((Implication) expression).Left);
			Assert.IsInstanceOf (typeof (Implication), ((Implication) expression).Right);

			var right = (Implication) ((Implication) expression).Right;
			Assert.IsInstanceOf (typeof (Proposition), right.Left);
			Assert.IsInstanceOf (typeof (Proposition), right.Right);

			expression = Parser.Parse ("test1 -> test2 <-> test3");
			Assert.IsInstanceOf (typeof (Implication), expression);
			Assert.IsInstanceOf (typeof (Proposition), ((Implication) expression).Left);
			Assert.IsInstanceOf (typeof (Equivalence), ((Implication) expression).Right);
		}

		[Test()]
		public void TestConjunction ()
		{
			var expression = Parser.Parse ("test1 & test2");
			Assert.IsInstanceOf (typeof (Conjunction), expression);
			Assert.AreEqual (((Conjunction) expression).Expressions.Count, 2);

			expression = Parser.Parse ("test1 & test2 & test3");
			Assert.IsInstanceOf (typeof (Conjunction), expression);
			Assert.AreEqual (((Conjunction) expression).Expressions.Count, 2);
		}

		[Test()]
		public void TestDisjunction ()
		{
			var expression = Parser.Parse ("test1 | test2");
			Assert.IsInstanceOf (typeof (Disjunction), expression);
			Assert.AreEqual (((Disjunction) expression).Expressions.Count, 2);

			expression = Parser.Parse ("test1 | test2 | test3");
			Assert.IsInstanceOf (typeof (Disjunction), expression);
			Assert.AreEqual (((Disjunction) expression).Expressions.Count, 2);
		}
		
		[Test()]
		public void TestDisjunctionConjunction ()
		{
			var expression = Parser.Parse ("test1 & test2 | test3");
			Assert.IsInstanceOf (typeof (Conjunction), expression);
			Assert.AreEqual (((Conjunction) expression).Expressions.Count, 2);

			var left = ((Conjunction) expression).Expressions[0];
			var right = ((Conjunction) expression).Expressions[1];
			Assert.IsInstanceOf (typeof (Proposition), left);
			Assert.IsInstanceOf (typeof (Disjunction), right);

			Assert.AreEqual (((Disjunction) right).Expressions.Count, 2);
		}

		[Test()]
		public void TestNegation ()
		{
			var expression = Parser.Parse ("!test1");
			Assert.IsInstanceOf (typeof (Negation), expression);
			Assert.IsInstanceOf (typeof (Proposition), ((Negation) expression).Enclosed);

			expression = Parser.Parse ("! ! test2");
			Assert.IsInstanceOf (typeof (Negation), expression);
			Assert.IsInstanceOf (typeof (Negation), ((Negation) expression).Enclosed);
			Assert.IsInstanceOf (typeof (Proposition), ((Negation) ((Negation) expression).Enclosed).Enclosed);
		}
		
		[Test()]
		public void TestParenthesedExpression ()
		{
			var expression = Parser.Parse ("(test1)");
			Assert.IsInstanceOf (typeof (ParenthesedExpression), expression);
			Assert.IsInstanceOf (typeof (Proposition), ((ParenthesedExpression) expression).Enclosed);
			
			expression = Parser.Parse ("((test1))");
			Assert.IsInstanceOf (typeof (ParenthesedExpression), expression);
			Assert.IsInstanceOf (typeof (ParenthesedExpression), ((ParenthesedExpression) expression).Enclosed);
			Assert.IsInstanceOf (typeof (Proposition), ((ParenthesedExpression) ((ParenthesedExpression) expression).Enclosed).Enclosed);
		}
		
		[Test()]
		public void TestNextExpression ()
		{
			var expression = Parser.Parse ("X test1");
			Assert.IsInstanceOf (typeof (Next), expression);
			Assert.IsInstanceOf (typeof (Proposition), ((Next) expression).Enclosed);
			
			expression = Parser.Parse ("X X test1");
			Assert.IsInstanceOf (typeof (Next), expression);
			Assert.IsInstanceOf (typeof (Next), ((Next) expression).Enclosed);
			Assert.IsInstanceOf (typeof (Proposition), ((Next) ((Next) expression).Enclosed).Enclosed);
		}
		
		[Test()]
		public void TestFinallyExpression ()
		{
			var expression = Parser.Parse ("F test1");
			Assert.IsInstanceOf (typeof (Finally), expression);
			Assert.IsInstanceOf (typeof (Proposition), ((Finally) expression).Enclosed);
			
			expression = Parser.Parse ("F F test1");
			Assert.IsInstanceOf (typeof (Finally), expression);
			Assert.IsInstanceOf (typeof (Finally), ((Finally) expression).Enclosed);
			Assert.IsInstanceOf (typeof (Proposition), ((Finally) ((Finally) expression).Enclosed).Enclosed);
		}
		
		[Test()]
		public void TestGloballyExpression ()
		{
			var expression = Parser.Parse ("G test1");
			Assert.IsInstanceOf (typeof (Globally), expression);
			Assert.IsInstanceOf (typeof (Proposition), ((Globally) expression).Enclosed);
			
			expression = Parser.Parse ("G G test1");
			Assert.IsInstanceOf (typeof (Globally), expression);
			Assert.IsInstanceOf (typeof (Globally), ((Globally) expression).Enclosed);
			Assert.IsInstanceOf (typeof (Proposition), ((Globally) ((Globally) expression).Enclosed).Enclosed);
		}

		[Test()]
		public void TestUntilExpression ()
		{
			var expression = Parser.Parse ("test1 U test2");
			Assert.IsInstanceOf (typeof (Until), expression);

			var until = (Until) expression;
			Assert.IsInstanceOf (typeof (Proposition), until.Left);
			Assert.IsInstanceOf (typeof (Proposition), until.Right);
			
			expression = Parser.Parse ("test1 U test2 U test3");
			until = (Until) expression;
			Assert.IsInstanceOf (typeof (Proposition), until.Left);
			Assert.IsInstanceOf (typeof (Until), until.Right);

			until = (Until) until.Right;
			Assert.IsInstanceOf (typeof (Proposition), until.Left);
			Assert.IsInstanceOf (typeof (Proposition), until.Right);
		}

		[Test()]
		public void TestUnlessExpression ()
		{
			var expression = Parser.Parse ("test1 W test2");
			Assert.IsInstanceOf (typeof (Unless), expression);

			var unless = (Unless) expression;
			Assert.IsInstanceOf (typeof (Proposition), unless.Left);
			Assert.IsInstanceOf (typeof (Proposition), unless.Right);
			
			expression = Parser.Parse ("test1 W test2 W test3");
			unless = (Unless) expression;
			Assert.IsInstanceOf (typeof (Proposition), unless.Left);
			Assert.IsInstanceOf (typeof (Unless), unless.Right);

			unless = (Unless) unless.Right;
			Assert.IsInstanceOf (typeof (Proposition), unless.Left);
			Assert.IsInstanceOf (typeof (Proposition), unless.Right);
		}

		[Test()]
		public void TestReleaseExpression ()
		{
			var expression = Parser.Parse ("test1 R test2");
			Assert.IsInstanceOf (typeof (Release), expression);

			var release = (Release) expression;
			Assert.IsInstanceOf (typeof (Proposition), release.Left);
			Assert.IsInstanceOf (typeof (Proposition), release.Right);
			
			expression = Parser.Parse ("test1 R test2 R test3");
			release = (Release) expression;
			Assert.IsInstanceOf (typeof (Proposition), release.Left);
			Assert.IsInstanceOf (typeof (Release), release.Right);

			release = (Release) release.Right;
			Assert.IsInstanceOf (typeof (Proposition), release.Left);
			Assert.IsInstanceOf (typeof (Proposition), release.Right);
		}

		
		[Test()]
		public void TestUntilUnlessReleaseExpression ()
		{
			var expression = Parser.Parse ("test1 R test2 U test3 W test4");
			Assert.IsInstanceOf (typeof (Release), expression);

			var release = (Release) expression;
			Assert.IsInstanceOf (typeof (Proposition), release.Left);
			Assert.IsInstanceOf (typeof (Until), release.Right);

			var until = (Until) release.Right;
			Assert.IsInstanceOf (typeof (Proposition), until.Left);
			Assert.IsInstanceOf (typeof (Unless), until.Right);
			
			var unless = (Unless) until.Right;
			Assert.IsInstanceOf (typeof (Proposition), unless.Left);
			Assert.IsInstanceOf (typeof (Proposition), unless.Right);
		}

		[Test()]
		public void TestTemporalEquivalence ()
		{
			var expression = Parser.Parse ("test1 <=> test2");
			Assert.IsInstanceOf (typeof (StrongEquivalence), expression);
			Assert.IsInstanceOf (typeof (Proposition), ((StrongEquivalence) expression).Left);
			Assert.IsInstanceOf (typeof (Proposition), ((StrongEquivalence) expression).Right);

			expression = Parser.Parse ("test1 <=> test2 <=> test3");
			Assert.IsInstanceOf (typeof (StrongEquivalence), expression);
			Assert.IsInstanceOf (typeof (Proposition), ((StrongEquivalence) expression).Left);
			Assert.IsInstanceOf (typeof (StrongEquivalence), ((StrongEquivalence) expression).Right);

			var right = (StrongEquivalence) ((StrongEquivalence) expression).Right;
			Assert.IsInstanceOf (typeof (Proposition), right.Left);
			Assert.IsInstanceOf (typeof (Proposition), right.Right);
			
			expression = Parser.Parse ("test1 <=> test2 => test3");
			Assert.IsInstanceOf (typeof (StrongEquivalence), expression);
			Assert.IsInstanceOf (typeof (Proposition), ((StrongEquivalence) expression).Left);
			Assert.IsInstanceOf (typeof (StrongImplication), ((StrongEquivalence) expression).Right);
		}

		[Test()]
		public void TestTemporalImplication ()
		{
			var expression = Parser.Parse ("test1 => test2");
			Assert.IsInstanceOf (typeof (StrongImplication), expression);
			Assert.IsInstanceOf (typeof (Proposition), ((StrongImplication) expression).Left);
			Assert.IsInstanceOf (typeof (Proposition), ((StrongImplication) expression).Right);

			expression = Parser.Parse ("test1 => test2 => test3");
			Assert.IsInstanceOf (typeof (StrongImplication), expression);
			Assert.IsInstanceOf (typeof (Proposition), ((StrongImplication) expression).Left);
			Assert.IsInstanceOf (typeof (StrongImplication), ((StrongImplication) expression).Right);

			var right = (StrongImplication) ((StrongImplication) expression).Right;
			Assert.IsInstanceOf (typeof (Proposition), right.Left);
			Assert.IsInstanceOf (typeof (Proposition), right.Right);

			expression = Parser.Parse ("test1 => test2 <=> test3");
			Assert.IsInstanceOf (typeof (StrongImplication), expression);
			Assert.IsInstanceOf (typeof (Proposition), ((StrongImplication) expression).Left);
			Assert.IsInstanceOf (typeof (StrongEquivalence), ((StrongImplication) expression).Right);
		}
	}
}

