using System;
using System.Collections.Generic;
using System.Linq;

namespace LtlSharp
{
	#region Interface

    public interface ITLFormula
    {
        ITLFormula Negate ();
        ITLFormula Normalize ();
    }

	#endregion

	#region Abstract classes

	public abstract class IUnaryOperator : ITLFormula
	{
		public ITLFormula Enclosed { get; protected set; }

		public IUnaryOperator (ITLFormula enclosed)
		{
			Enclosed = enclosed;
		}
        
        public abstract ITLFormula Negate ();
        public abstract ITLFormula Normalize ();
        
        public override string ToString ()
        {
            return Enclosed is IBinaryOperator ? "( " + Enclosed.ToString () + ")" : Enclosed.ToString ();
        }
	}

	public abstract class IBinaryOperator : ITLFormula
	{
		public ITLFormula Left  { get; protected set; }
        public ITLFormula Right { get; protected set; }

        public abstract ITLFormula Negate ();
        public abstract ITLFormula Normalize ();

		public IBinaryOperator (ITLFormula left, ITLFormula right)
		{
			Left = left; Right = right;
		}
	}
    
    
    public interface ILiteral : ITLFormula
    {}

    #endregion

	#region Logic operators

    public class True : ILiteral {
        public ITLFormula Negate ()
        {
            return new False ();
        }
        public ITLFormula Normalize ()
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
        public ITLFormula Negate ()
        {
            return new True ();
        }
        public ITLFormula Normalize ()
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
        
        public ITLFormula Negate ()
        {
            return new Negation (this);
        }
        
        public ITLFormula Normalize ()
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
		public Implication (ITLFormula left, ITLFormula right)
			: base (left, right)
        {}

        public override ITLFormula Negate ()
        {
            return Normalize ().Negate ();
        }
        
        public override ITLFormula Normalize ()
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
		public Equivalence (ITLFormula left, ITLFormula right)
			: base (left, right)
        {}

        public override ITLFormula Negate ()
        {
            return Normalize ().Negate ();
        }

        public override ITLFormula Normalize ()
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
        public Conjunction (ITLFormula left, ITLFormula right)
            : base (left, right)
        {}

        public override ITLFormula Negate ()
        {
            return new Disjunction (Left.Negate (), Right.Negate ());
        }

        public override ITLFormula Normalize ()
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
        public Disjunction (ITLFormula left, ITLFormula right)
            : base (left, right)
        {}

        public override ITLFormula Negate ()
        {
            return new Conjunction (Left.Negate (), Right.Negate ());
        }

        public override ITLFormula Normalize ()
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
		public Negation (ITLFormula expression)
			: base (expression)
        {}

        public override ITLFormula Negate ()
        {
            return Enclosed;
        }

        public override ITLFormula Normalize ()
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
        public ParenthesedExpression (ITLFormula expression)
            : base (expression)
        {
            if (expression is ParenthesedExpression)
                Enclosed = (expression as ParenthesedExpression).Enclosed;
        }

        public override ITLFormula Negate ()
        {
            return Enclosed.Negate ();
        }

        public override ITLFormula Normalize ()
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
		public Next (ITLFormula expression)
			: base (expression)
        {}

        public override ITLFormula Negate ()
        {
            return new Next (Enclosed.Negate ());
        }

        public override ITLFormula Normalize ()
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
		public Finally (ITLFormula expression)
			: base (expression)
        {}

        public override ITLFormula Negate ()
        {
            return new Globally (Enclosed.Negate ());
        }

        public override ITLFormula Normalize ()
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
		public Globally (ITLFormula expression)
			: base (expression)
        {}

        public override ITLFormula Negate ()
        {
            return new Finally (Enclosed.Negate ());
        }

        public override ITLFormula Normalize ()
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
        public int Bound {
            get;
            set;
        }
        
        public Until (ITLFormula left, ITLFormula right, int bound)
            : base (left, right)
        {
            Bound = bound;
        }
        
		public Until (ITLFormula left, ITLFormula right)
			: this (left, right, -1)
        {}

        public override ITLFormula Negate ()
        {
            return new Release (Left.Negate (), Right.Negate ());
        }    

        public override ITLFormula Normalize ()
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
		public Release (ITLFormula left, ITLFormula right)
			: base (left, right)
        {}

        public override ITLFormula Negate ()
        {
            return new Until (Left.Negate (), Right.Negate ());
        }

        public override ITLFormula Normalize ()
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
		public Unless (ITLFormula left, ITLFormula right)
			: base (left, right)
        {}

        public override ITLFormula Negate ()
        {
            return Normalize ().Negate ();
        }

        public override ITLFormula Normalize ()
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
		public StrongEquivalence (ITLFormula left, ITLFormula right)
			: base (left, right)
        {}

        public override ITLFormula Negate ()
        {
            return Normalize ().Negate ();
        }

        public override ITLFormula Normalize ()
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
		public StrongImplication (ITLFormula left, ITLFormula right)
			: base (left, right)
        {}

        public override ITLFormula Negate ()
        {
            return Normalize ().Negate ();
        }

        public override ITLFormula Normalize ()
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
    
    #region 
    
    public class ProbabilisticOperator : IUnaryOperator
    {
        public double LowerBound {
            get;
            set;
        }
        
        public double UpperBound {
            get;
            set;
        }
        
        public bool InclusiveLowerBound {
            get;
            set;
        }
        
        public bool InclusiveUpperBound {
            get;
            set;
        }
        
        public ProbabilisticOperator (ITLFormula expression, double p)
            : this (expression, p, p)
        {}
        
        public ProbabilisticOperator (ITLFormula expression, double lower, double upper)
            : base (expression)
        {
            LowerBound = lower;
            UpperBound = upper;
            InclusiveLowerBound = true;
            InclusiveUpperBound = true;
        }
        
        public bool IsSatisfied (double p, double epsilon)
        {
            bool v = true;
            v &= InclusiveLowerBound ? GreaterThanOrEquals (p, LowerBound, epsilon) : GreaterThan (p, LowerBound, epsilon);
            v &= InclusiveUpperBound ? LessThanOrEquals (p, UpperBound, epsilon) : LessThan (p, UpperBound, epsilon);
            return v;
        }
        
        bool GreaterThanOrEquals(double a, double b, double epsilon)
        {
            return (a - b) >= -epsilon;
        }

        bool LessThanOrEquals(double a, double b, double epsilon)
        {
            return (b - a) >= -epsilon;
        }
        
        bool GreaterThan(double a, double b, double epsilon)
        {
            return (a - b) > -epsilon;
        }

        bool LessThan(double a, double b, double epsilon)
        {
            return (b - a) > -epsilon;
        }

        public override ITLFormula Negate ()
        {
            return new ProbabilisticOperator (Enclosed.Negate (), 1 - UpperBound, 1 - LowerBound) {
                InclusiveLowerBound = !this.InclusiveUpperBound,
                InclusiveUpperBound = !this.InclusiveLowerBound
            };
        }

        public override ITLFormula Normalize ()
        {
            return new ProbabilisticOperator (Enclosed.Normalize (), LowerBound, UpperBound) {
                InclusiveLowerBound = this.InclusiveLowerBound,
                InclusiveUpperBound = this.InclusiveUpperBound
            };
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(ProbabilisticOperator))
                return false;
            ProbabilisticOperator other = (ProbabilisticOperator)obj;
            return Enclosed.Equals (other.Enclosed)
                & LowerBound == other.LowerBound & InclusiveLowerBound == other.InclusiveLowerBound
                    & UpperBound == other.UpperBound & InclusiveUpperBound == other.InclusiveUpperBound;
        }

        public override int GetHashCode ()
        {
            return 17 + 23 * (Enclosed.GetHashCode () + 23 * (LowerBound.GetHashCode () + 23 * (InclusiveLowerBound.GetHashCode ()
                + 23 * (UpperBound.GetHashCode () + 23 * InclusiveUpperBound.GetHashCode ()))));
        }

        public override string ToString ()
        {
            return string.Format ("P{1}{2},{4}{3}{0}", 
                string.Format (Enclosed is IBinaryOperator ? "({0})" : "{0}", Enclosed),
                InclusiveLowerBound ? "[" : "]", LowerBound, InclusiveUpperBound ? "]" : "[", UpperBound
            );
        }
    }
    
    #endregion
}

