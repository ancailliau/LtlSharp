using System;
using System.Collections.Generic;
using System.Linq;

namespace LtlSharp
{
	#region Interface

	public interface LTLFormula
	{
	}

	#endregion

	#region Abstract classes

	public abstract class UnaryOperator : LTLFormula
	{
		public LTLFormula Enclosed { get; private set; }

		public UnaryOperator (LTLFormula enclosed)
		{
			Enclosed = enclosed;
		}
	}

	public abstract class BinaryOperator : LTLFormula
	{
		public LTLFormula Left  { get; private set; }
		public LTLFormula Right { get; private set; }

		public BinaryOperator (LTLFormula left, LTLFormula right)
		{
			Left = left; Right = right;
		}
	}

	public abstract class NaryOperator : LTLFormula
	{
		public IList<LTLFormula> Expressions  { get; private set; }

		public NaryOperator (params LTLFormula[] expressions)
		{
			Expressions = new List<LTLFormula> ();
			foreach (var e in expressions)
				Expressions.Add (e);
		}

		public void Push (LTLFormula expression)
		{
			Expressions.Add (expression);
		}
	}

	#endregion

	#region Logic operators

	public class Proposition : LTLFormula
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
		public Implication (LTLFormula left, LTLFormula right)
			: base (left, right)
		{}
	}
	
	public class Equivalence : BinaryOperator
	{
		public Equivalence (LTLFormula left, LTLFormula right)
			: base (left, right)
		{}
	}

	public class Conjunction : NaryOperator
	{
		public Conjunction (params LTLFormula[] expressions)
			: base (expressions)
		{}
	}

	public class Disjunction : NaryOperator
	{
		public Disjunction (params LTLFormula[] expressions)
			: base (expressions)
		{}
	}

	public class Negation : UnaryOperator
	{
		public Negation (LTLFormula expression)
			: base (expression)
		{}
	}

	public class ParenthesedExpression : UnaryOperator
	{
		public ParenthesedExpression (LTLFormula expression)
			: base (expression)
		{}
	}

	#endregion

	#region Temporal operators

	public class Next : UnaryOperator
	{
		public Next (LTLFormula expression)
			: base (expression)
		{}
	}

	public class Finally : UnaryOperator
	{
		public Finally (LTLFormula expression)
			: base (expression)
		{}
	}

	public class Globally : UnaryOperator
	{
		public Globally (LTLFormula expression)
			: base (expression)
		{}
	}

	public class Until : BinaryOperator
	{
		public Until (LTLFormula left, LTLFormula right)
			: base (left, right)
		{}
	}
	
	public class Release : BinaryOperator
	{
		public Release (LTLFormula left, LTLFormula right)
			: base (left, right)
		{}
	}
	
	public class Unless : BinaryOperator
	{
		public Unless (LTLFormula left, LTLFormula right)
			: base (left, right)
		{}
	}
	
	public class StrongEquivalence : BinaryOperator
	{
		public StrongEquivalence (LTLFormula left, LTLFormula right)
			: base (left, right)
		{}
	}
	
	public class StrongImplication : BinaryOperator
	{
		public StrongImplication (LTLFormula left, LTLFormula right)
			: base (left, right)
		{}
	}

	#endregion
}

