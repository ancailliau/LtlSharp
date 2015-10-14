using System;
using QuickGraph;

namespace LtlSharp.Utils
{
    public class ParametrizedEdge<T1, T2> : Edge<T1>
    {
        public T2 Value {
            get;
            set;
        }
        
        public ParametrizedEdge (T1 source, T1 target)
            : base (source, target)
        {}
        
        public ParametrizedEdge (T1 source, T1 target, T2 value)
            : base (source, target)
        {
            Value = value;
        }
        
        
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(ParametrizedEdge<T1,T2>))
                return false;
            var other = (ParametrizedEdge<T1,T2>)obj;
            return base.Equals (other) & Value.Equals (other.Value);
        }

        public override int GetHashCode ()
        {
            return 17 + base.GetHashCode () + 32 * Value.GetHashCode ();
        }
    }
}

