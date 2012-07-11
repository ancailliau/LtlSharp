using System;
using System.Collections.Generic;
using System.Linq;

namespace LtlSharp
{
	#region Interface

	public interface Expression
	{
	}

	#endregion

	#region Abstract classes

	public abstract class UnaryOperator : Expression
	{
		public Expression Enclosed { get; private set; }

		public UnaryOperator (Expression enclosed)
		{
			Enclosed = enclosed;
		}
	}

	public abstract class BinaryOperator : Expression
	{
		public Expression Left  { get; private set; }
		public Expression Right { get; private set; }

		public BinaryOperator (Expression left, Expression right)
		{
			Left = left; Right = right;
		}
	}

	public abstract class NaryOperator : Expression
	{
		public IList<Expression> Expressions  { get; private set; }

		public NaryOperator (params Expression[] expressions)
		{
			Expressions = new List<Expression> ();
			foreach (var e in expressions)
				Expressions.Add (e);
		}

		public void Push (Expression expression)
		{
			Expressions.Add (expression);
		}
	}

	#endregion

	#region Logic operators

	public class Proposition : Expression
	{
		public string Name  { get; private set; }

		public Proposition (string name)
		{
			Name = name;
		}

		public override string ToString ()
		{
			return Name;
		}
	}

	public class Implication : BinaryOperator
	{
		public Implication (Expression left, Expression right)
			: base (left, right)
		{}
	}

	public class Conjunction : NaryOperator
	{
		public Conjunction (params Expression[] expressions)
			: base (expressions)
		{}
	}

	public class Disjunction : NaryOperator
	{
		public Disjunction (params Expression[] expressions)
			: base (expressions)
		{}
	}

	public class Negation : UnaryOperator
	{
		public Negation (Expression expression)
			: base (expression)
		{}
	}

	public class ParenthesedExpression : UnaryOperator
	{
		public ParenthesedExpression (Expression expression)
			: base (expression)
		{}
	}

	#endregion

	#region Temporal operators

	public class Next : UnaryOperator
	{
		public Next (Expression expression)
			: base (expression)
		{}
	}

	public class Finally : UnaryOperator
	{
		public Finally (Expression expression)
			: base (expression)
		{}
	}

	public class Globally : UnaryOperator
	{
		public Globally (Expression expression)
			: base (expression)
		{}
	}

	public class Until : BinaryOperator
	{
		public Until (Expression left, Expression right)
			: base (left, right)
		{}
	}
	
	public class Release : BinaryOperator
	{
		public Release (Expression left, Expression right)
			: base (left, right)
		{}
	}
	
	public class Unless : BinaryOperator
	{
		public Unless (Expression left, Expression right)
			: base (left, right)
		{}
	}

	#endregion
}

