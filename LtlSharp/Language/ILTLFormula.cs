using System;
using System.Collections.Generic;
using System.Linq;

namespace LtlSharp
{
	#region Interface

	public interface ILTLFormula
    {
        ILTLFormula Negate ();
        ILTLFormula Normalize ();
	}

	#endregion

	#region Abstract classes

	public abstract class IUnaryOperator : ILTLFormula
	{
		public ILTLFormula Enclosed { get; protected set; }

		public IUnaryOperator (ILTLFormula enclosed)
		{
			Enclosed = enclosed;
		}
        
        public abstract ILTLFormula Negate ();
        public abstract ILTLFormula Normalize ();
        
        public override string ToString ()
        {
            return Enclosed is IBinaryOperator ? "( " + Enclosed.ToString () + ")" : Enclosed.ToString ();
        }
	}

	public abstract class IBinaryOperator : ILTLFormula
	{
		public ILTLFormula Left  { get; protected set; }
        public ILTLFormula Right { get; protected set; }

        public abstract ILTLFormula Negate ();
        public abstract ILTLFormula Normalize ();

		public IBinaryOperator (ILTLFormula left, ILTLFormula right)
		{
			Left = left; Right = right;
		}
	}
    
    
    public interface ILiteral : ILTLFormula
    {}

    #endregion

	#region Logic operators

    public class True : ILiteral {
        public ILTLFormula Negate ()
        {
            return new False ();
        }
        public ILTLFormula Normalize ()
        {
            return this;
        }
        public override string ToString ()
        {
            return string.Format ("TRUE");
        }
        public override int GetHashCode ()
        {
            return "TRUE".GetHashCode ();
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(True))
                return false;
            return true;
        }
    }
    
    public class False : ILiteral {
        public ILTLFormula Negate ()
        {
            return new True ();
        }
        public ILTLFormula Normalize ()
        {
            return this;
        }
        public override string ToString ()
        {
            return string.Format ("FALSE");
        }
        public override int GetHashCode ()
        {
            return "FALSE".GetHashCode ();
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(False))
                return false;
            return true;
        }
    }
        
    public class Proposition : ILiteral
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
        
        public ILTLFormula Negate ()
        {
            return new Negation (this);
        }
        
        public ILTLFormula Normalize ()
        {
            return this;
        }
        
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(Proposition))
                return false;
            Proposition other = (Proposition)obj;
            return Name == other.Name;
        }
        
        public override int GetHashCode ()
        {
            unchecked {
                return (Name != null ? Name.GetHashCode () : 0);
            }
        }
        
	}

	public class Implication : IBinaryOperator
	{
		public Implication (ILTLFormula left, ILTLFormula right)
			: base (left, right)
        {}

        public override ILTLFormula Negate ()
        {
            return Normalize ().Negate ();
        }
        
        public override ILTLFormula Normalize ()
        {
            return new Disjunction (Left.Normalize ().Negate (), Right.Normalize ());
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(Implication))
                return false;
            Implication other = (Implication)obj;
            return Left.Equals (other.Left) & Right.Equals (other.Right);
        }

        public override int GetHashCode ()
        {
            return 17 + 23 * ("->".GetHashCode () + 23 * (Left.GetHashCode () + 23 * Right.GetHashCode ()));
        }
        
        public override string ToString ()
        {
            return string.Format ("{0} -> {1}", 
                string.Format (Left is IBinaryOperator ? "({0})" : "{0}", Left), 
                string.Format (Right is IBinaryOperator ? "({0})" : "{0}", Right));
        }
	}
	
	public class Equivalence : IBinaryOperator
	{
		public Equivalence (ILTLFormula left, ILTLFormula right)
			: base (left, right)
        {}

        public override ILTLFormula Negate ()
        {
            return Normalize ().Negate ();
        }

        public override ILTLFormula Normalize ()
        {
            var left = Left.Normalize ();
            var right = Right.Normalize ();
            return new Conjunction (
                new Disjunction (left.Negate (), right),
                new Disjunction (right.Negate (), left)
            );
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(Equivalence))
                return false;
            Equivalence other = (Equivalence)obj;
            return (Left.Equals (other.Left) & Right.Equals (other.Right))
                | (Left.Equals (other.Right) & Right.Equals (other.Left));
        }

        public override int GetHashCode ()
        {
            return 17 + 23 * ("<->".GetHashCode () + 23 * (Left.GetHashCode () + Right.GetHashCode ()));
        }
        
        public override string ToString ()
        {
            return string.Format ("{0} <-> {1}", 
                string.Format (Left is IBinaryOperator ? "({0})" : "{0}", Left), 
                string.Format (Right is IBinaryOperator ? "({0})" : "{0}", Right));
        }
	}

    public class Conjunction : IBinaryOperator
    {
        public Conjunction (ILTLFormula left, ILTLFormula right)
            : base (left, right)
        {}

        public override ILTLFormula Negate ()
        {
            return new Disjunction (Left.Negate (), Right.Negate ());
        }

        public override ILTLFormula Normalize ()
        {
            return new Conjunction (Left.Normalize (), Right.Normalize ());
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(Conjunction))
                return false;
            Conjunction other = (Conjunction)obj;
            return (Left.Equals (other.Left) & Right.Equals (other.Right))
                | (Left.Equals (other.Right) & Right.Equals (other.Left));
        }

        public override int GetHashCode ()
        {
            return 17 + 23 * ("&".GetHashCode () + 23 * (Left.GetHashCode () + Right.GetHashCode ()));
        }
        
        public override string ToString ()
        {
            return string.Format ("{0} & {1}", 
                string.Format (Left is IBinaryOperator ? "({0})" : "{0}", Left), 
                string.Format (Right is IBinaryOperator ? "({0})" : "{0}", Right));
        }
	}

    public class Disjunction : IBinaryOperator
    {
        public Disjunction (ILTLFormula left, ILTLFormula right)
            : base (left, right)
        {}

        public override ILTLFormula Negate ()
        {
            return new Conjunction (Left.Negate (), Right.Negate ());
        }

        public override ILTLFormula Normalize ()
        {
            return new Disjunction (Left.Normalize (), Right.Normalize ());
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(Disjunction))
                return false;
            Disjunction other = (Disjunction)obj;
            return (Left.Equals (other.Left) & Right.Equals (other.Right))
                | (Left.Equals (other.Right) & Right.Equals (other.Left));
        }

        public override int GetHashCode ()
        {
            return 17 + 23 * ("|".GetHashCode () + 23 * (Left.GetHashCode () + Right.GetHashCode ()));
        }
        
        public override string ToString ()
        {
            return string.Format ("{0} | {1}", 
                string.Format (Left is IBinaryOperator ? "({0})" : "{0}", Left), 
                string.Format (Right is IBinaryOperator ? "({0})" : "{0}", Right));
        }
	}

    public class Negation : IUnaryOperator, ILiteral
	{
		public Negation (ILTLFormula expression)
			: base (expression)
        {}

        public override ILTLFormula Negate ()
        {
            return Enclosed;
        }

        public override ILTLFormula Normalize ()
        {
            return Enclosed.Negate ();
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(Negation))
                return false;
            Negation other = (Negation)obj;
            return Enclosed.Equals (other.Enclosed);
        }

        public override int GetHashCode ()
        {
            return 17 + 23 * ("!".GetHashCode () + 23 * Enclosed.GetHashCode ());
        }
        
        public override string ToString ()
        {
            return string.Format ("!{0}", 
                string.Format (Enclosed is IBinaryOperator ? "({0})" : "{0}", Enclosed));
        }
	}

    public class ParenthesedExpression : IUnaryOperator
    {
        public ParenthesedExpression (ILTLFormula expression)
            : base (expression)
        {
            if (expression is ParenthesedExpression)
                Enclosed = (expression as ParenthesedExpression).Enclosed;
        }

        public override ILTLFormula Negate ()
        {
            return Enclosed.Negate ();
        }

        public override ILTLFormula Normalize ()
        {
            return Enclosed.Normalize ();
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(ParenthesedExpression))
                return false;
            ParenthesedExpression other = (ParenthesedExpression)obj;
            return Enclosed.Equals (other.Enclosed);
        }

        public override int GetHashCode ()
        {
            return 17 + 23 * ("()".GetHashCode () + 23 * Enclosed.GetHashCode ());
        }
        
        public override string ToString ()
        {
            return string.Format ("{0}", 
                string.Format (Enclosed is IBinaryOperator ? "({0})" : "{0}", Enclosed));
        }
    }
	#endregion

	#region Temporal operators

	public class Next : IUnaryOperator
	{
		public Next (ILTLFormula expression)
			: base (expression)
        {}

        public override ILTLFormula Negate ()
        {
            return new Next (Enclosed.Negate ());
        }

        public override ILTLFormula Normalize ()
        {
            return new Next (Enclosed.Normalize ());
        }
        
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(Next))
                return false;
            Next other = (Next)obj;
            return Enclosed.Equals (other.Enclosed);
        }

        public override int GetHashCode ()
        {
            return 17 + 23 * ("X".GetHashCode () + 23 * Enclosed.GetHashCode ());
        }
        
        public override string ToString ()
        {
            return string.Format ("X {0}", 
                string.Format (Enclosed is IBinaryOperator ? "({0})" : "{0}", Enclosed));
        }
	}

	public class Finally : IUnaryOperator
	{
		public Finally (ILTLFormula expression)
			: base (expression)
        {}

        public override ILTLFormula Negate ()
        {
            return new Globally (Enclosed.Negate ());
        }

        public override ILTLFormula Normalize ()
        {
            return new Until (new True (), Enclosed.Normalize ());
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(Finally))
                return false;
            Finally other = (Finally)obj;
            return Enclosed.Equals (other.Enclosed);
        }

        public override int GetHashCode ()
        {
            return 17 + 23 * ("F".GetHashCode () + 23 * Enclosed.GetHashCode ());
        }
        
        public override string ToString ()
        {
            return string.Format ("F {0}", 
                string.Format (Enclosed is IBinaryOperator ? "({0})" : "{0}", Enclosed));
        }
	}

	public class Globally : IUnaryOperator
	{
		public Globally (ILTLFormula expression)
			: base (expression)
        {}

        public override ILTLFormula Negate ()
        {
            return new Finally (Enclosed.Negate ());
        }

        public override ILTLFormula Normalize ()
        {
            return new Release (new False (), Enclosed.Normalize ());
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(Globally))
                return false;
            Globally other = (Globally)obj;
            return Enclosed.Equals (other.Enclosed);
        }

        public override int GetHashCode ()
        {
            return 17 + 23 * ("G".GetHashCode () + 23 * Enclosed.GetHashCode ());
        }
        
        public override string ToString ()
        {
            return string.Format ("G {0}", 
                string.Format (Enclosed is IBinaryOperator ? "({0})" : "{0}", Enclosed));
        }
	}

	public class Until : IBinaryOperator
	{
		public Until (ILTLFormula left, ILTLFormula right)
			: base (left, right)
        {}

        public override ILTLFormula Negate ()
        {
            return new Release (Left.Negate (), Right.Negate ());
        }    

        public override ILTLFormula Normalize ()
        {
            return new Until (Left.Normalize (), Right.Normalize ());
        }    

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(Until))
                return false;
            Until other = (Until)obj;
            return Left.Equals (other.Left) & Right.Equals (other.Right);
        }

        public override int GetHashCode ()
        {
            return 17 + 23 * ("U".GetHashCode () + 23 * (Left.GetHashCode () + 23 * Right.GetHashCode ()));
        }
        
        public override string ToString ()
        {
            return string.Format ("{0} U {1}", 
                string.Format (Left is IBinaryOperator ? "({0})" : "{0}", Left), 
                string.Format (Right is IBinaryOperator ? "({0})" : "{0}", Right));
        }
	}
	
	public class Release : IBinaryOperator
	{
		public Release (ILTLFormula left, ILTLFormula right)
			: base (left, right)
        {}

        public override ILTLFormula Negate ()
        {
            return new Until (Left.Negate (), Right.Negate ());
        }

        public override ILTLFormula Normalize ()
        {
            return new Release (Left.Normalize (), Right.Normalize ());
        } 

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(Release))
                return false;
            Release other = (Release)obj;
            return Left.Equals (other.Left) & Right.Equals (other.Right);
        }

        public override int GetHashCode ()
        {
            return 17 + 23 * ("R".GetHashCode () + 23 * (Left.GetHashCode () + 23 * Right.GetHashCode ()));
        }
        
        public override string ToString ()
        {
            return string.Format ("{0} R {1}", 
                string.Format (Left is IBinaryOperator ? "({0})" : "{0}", Left), 
                string.Format (Right is IBinaryOperator ? "({0})" : "{0}", Right));
        }
	}
	
	public class Unless : IBinaryOperator
	{
		public Unless (ILTLFormula left, ILTLFormula right)
			: base (left, right)
        {}

        public override ILTLFormula Negate ()
        {
            return Normalize ().Negate ();
        }

        public override ILTLFormula Normalize ()
        {
            var right = Right.Normalize ();
            var left = Left.Normalize ();
            return new Release (right, new Disjunction (left, right));
        } 
        
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(Unless))
                return false;
            Unless other = (Unless)obj;
            return Left.Equals (other.Left) & Right.Equals (other.Right);
        }

        public override int GetHashCode ()
        {
            return 17 + 23 * ("W".GetHashCode () + 23 * (Left.GetHashCode () + 23 * Right.GetHashCode ()));
        }
        
        public override string ToString ()
        {
            return string.Format ("{0} W {1}",
                string.Format (Left is IBinaryOperator ? "({0})" : "{0}", Left), 
                string.Format (Right is IBinaryOperator ? "({0})" : "{0}", Right));
        }
	}
	
	public class StrongEquivalence : IBinaryOperator
	{
		public StrongEquivalence (ILTLFormula left, ILTLFormula right)
			: base (left, right)
        {}

        public override ILTLFormula Negate ()
        {
            return Normalize ().Negate ();
        }

        public override ILTLFormula Normalize ()
        {
            return new Globally (new Equivalence (Left, Right)).Normalize ();
        } 
        
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(StrongEquivalence))
                return false;
            StrongEquivalence other = (StrongEquivalence)obj;
            return (Left.Equals (other.Left) & Right.Equals (other.Right))
                | (Right.Equals (other.Left) & Left.Equals (other.Right));
        }

        public override int GetHashCode ()
        {
            return 17 + 23 * ("<=>".GetHashCode () + 23 * (Left.GetHashCode () + Right.GetHashCode ()));
        }
        
        public override string ToString ()
        {
            return string.Format ("{0} <=> {1}", 
                string.Format (Left is IBinaryOperator ? "({0})" : "{0}", Left), 
                string.Format (Right is IBinaryOperator ? "({0})" : "{0}", Right));
        }
	}
	
	public class StrongImplication : IBinaryOperator
	{
		public StrongImplication (ILTLFormula left, ILTLFormula right)
			: base (left, right)
        {}

        public override ILTLFormula Negate ()
        {
            return Normalize ().Negate ();
        }

        public override ILTLFormula Normalize ()
        {
            return new Globally (new Implication (Left, Right)).Normalize ();
        } 

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(StrongImplication))
                return false;
            StrongImplication other = (StrongImplication)obj;
            return (Left.Equals (other.Left) & Right.Equals (other.Right));
        }

        public override int GetHashCode ()
        {
            return 17 + 23 * ("=>".GetHashCode () + 23 * (Left.GetHashCode () + 23 * Right.GetHashCode ()));
        }
        
        public override string ToString ()
        {
            return string.Format ("{0} => {1}", 
                string.Format (Left is IBinaryOperator ? "({0})" : "{0}", Left), 
                string.Format (Right is IBinaryOperator ? "({0})" : "{0}", Right));
        }
	}

	#endregion
}

