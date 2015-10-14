using System;
using System.Collections.Generic;
using System.Linq;
using QuickGraph;
using LtlSharp.Utils;

namespace LtlSharp.Automata
{
    public class AutomatonTransition<T> : Edge<T>
        where T : IAutomatonNode
    {
        public HashSet<ILiteral> Labels;
        
        public AutomatonTransition (T source, T target)
            : base (source, target)
        {
            Labels = new HashSet<ILiteral> ();
        }

        public AutomatonTransition (T source, T target, IEnumerable<ILiteral> labels)
            : base (source, target)
        {
            Labels = new HashSet<ILiteral> (labels);
        }
        
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(AutomatonTransition<T>))
                return false;
            var other = (AutomatonTransition<T>)obj;
            return Source.Equals (other.Source) && Target.Equals (other.Target) && Labels.SetEquals (other.Labels);
        }

        public override int GetHashCode ()
        {
            return 17 + Source.GetHashCode () + 32 * ( Target.GetHashCode () + 32 * Labels.GetHashCodeForElements ());
        }
    }
    
    
}

