using System;
using System.Collections.Generic;
using LtlSharp.Utils;
using System.Linq;

namespace LtlSharp.Automata.Transitions.Decorations
{
    public class DegeneralizerDecoration
        : ITransitionDecoration<DegeneralizerDecoration>
    {   
        public HashSet<int> Labels;
        public bool Else;
        
        public DegeneralizerDecoration () : this (false, Enumerable.Empty<int> ())
        {}
        
        public DegeneralizerDecoration (bool @else) : this (@else, Enumerable.Empty<int> ())
        {}

        public DegeneralizerDecoration (IEnumerable<int> labels): this (false, labels)
        {}
        
        public DegeneralizerDecoration (bool @else, IEnumerable<int> labels)
        {
            Labels = new HashSet<int> (labels);
            Else = @else;
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(DegeneralizerDecoration))
                return false;
            var other = (DegeneralizerDecoration)obj;
            return Labels.SetEquals (other.Labels) & Else == other.Else;
        }

        public override int GetHashCode ()
        {
            return 17 + 23 * (base.GetHashCode () + 23 * (Labels.GetHashCodeForElements () + 23 * Else.GetHashCode ()));
        }

    }
}

