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
		public LTLFormula Enclosed { get; protected set; }

		public UnaryOperator (LTLFormula enclosed)
		{
			Enclosed = enclosed;
		}
        
        public override string ToString ()
        {
            return Enclosed is BinaryOperator | Enclosed is NaryOperator ? "( " + Enclosed.ToString () + ")" : Enclosed.ToString ();
        }
	}

	public abstract class BinaryOperator : LTLFormula
	{
		public LTLFormula Left  { get; protected set; }
		public LTLFormula Right { get; protected set; }

		public BinaryOperator (LTLFormula left, LTLFormula right)
		{
			Left = left; Right = right;
		}
	}

	public abstract class NaryOperator : LTLFormula
	{
		public IList<LTLFormula> Expressions  { get; protected set; }

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
        
        public override string ToString ()
        {
            return string.Format ("{0} -> {1}", Left, Right);
        }
	}
	
	public class Equivalence : BinaryOperator
	{
		public Equivalence (LTLFormula left, LTLFormula right)
			: base (left, right)
		{}
        
        public override string ToString ()
        {
            return string.Format ("{0} <-> {1}", Left, Right);
        }
	}

	public class Conjunction : NaryOperator
	{
		public Conjunction (params LTLFormula[] expressions)
			: base (expressions)
		{}
        
        public override string ToString ()
        {
            return string.Join (" & ", (from e in Expressions select e.ToString ()).ToArray ());
        }
	}

	public class Disjunction : NaryOperator
	{
		public Disjunction (params LTLFormula[] expressions)
			: base (expressions)
		{}
        
        public override string ToString ()
        {
            return string.Join (" | ", (from e in Expressions select e.ToString ()).ToArray ());
        }
	}

	public class Negation : UnaryOperator
	{
		public Negation (LTLFormula expression)
			: base (expression)
		{}
        
        public override string ToString ()
        {
            return string.Format ("! {0}", base.ToString ());
        }
	}

    public class ParenthesedExpression : UnaryOperator
    {
        public ParenthesedExpression (LTLFormula expression)
            : base (expression)
        {
            if (expression is ParenthesedExpression)
                Enclosed = (expression as ParenthesedExpression).Enclosed;
        }
        
        public override string ToString ()
        {
            return string.Format ("{0}", base.ToString ());
        }
    }
	#endregion

	#region Temporal operators

	public class Next : UnaryOperator
	{
		public Next (LTLFormula expression)
			: base (expression)
		{}
        
        public override string ToString ()
        {
            return string.Format ("X {0}", base.ToString ());
        }
	}

	public class Finally : UnaryOperator
	{
		public Finally (LTLFormula expression)
			: base (expression)
		{}
        
        public override string ToString ()
        {
            return string.Format ("F {0}", base.ToString ());
        }
	}

	public class Globally : UnaryOperator
	{
		public Globally (LTLFormula expression)
			: base (expression)
		{}
        
        public override string ToString ()
        {
            return string.Format ("G {0}", base.ToString ());
        }
	}

	public class Until : BinaryOperator
	{
		public Until (LTLFormula left, LTLFormula right)
			: base (left, right)
		{}
        
        public override string ToString ()
        {
            return string.Format ("{0} U {1}", Left, Right);
        }
	}
	
	public class Release : BinaryOperator
	{
		public Release (LTLFormula left, LTLFormula right)
			: base (left, right)
		{}
        
        public override string ToString ()
        {
            return string.Format ("{0} R {1}", Left, Right);
        }
	}
	
	public class Unless : BinaryOperator
	{
		public Unless (LTLFormula left, LTLFormula right)
			: base (left, right)
		{}
        
        public override string ToString ()
        {
            return string.Format ("{0} W {1}", Left, Right);
        }
	}
	
	public class StrongEquivalence : BinaryOperator
	{
		public StrongEquivalence (LTLFormula left, LTLFormula right)
			: base (left, right)
		{}
        
        public override string ToString ()
        {
            return string.Format ("{0} <=> {1}", Left, Right);
        }
	}
	
	public class StrongImplication : BinaryOperator
	{
		public StrongImplication (LTLFormula left, LTLFormula right)
			: base (left, right)
		{}
        
        public override string ToString ()
        {
            return string.Format ("{0} => {1}", Left, Right);
        }
	}

	#endregion
}

