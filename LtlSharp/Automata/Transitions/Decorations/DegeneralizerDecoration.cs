using System;
using System.Collections.Generic;
using LtlSharp.Utils;
using System.Linq;

namespace LtlSharp.Automata.Transitions.Decorations
{
    /// <summary>
    /// Defines a decoration for a degeneralizer, as defined in [Gia02].
    /// </summary>
    public class DegeneralizerDecoration
        : ITransitionDecoration<DegeneralizerDecoration>
    {   
        /// <summary>
        /// The labels.
        /// </summary>
        public HashSet<int> Labels {
            get;
            protected set;
        }
        
        /// <summary>
        /// Gets or sets the else flag.
        /// </summary>
        /// <value>The else flag.</value>
        public bool Else {
            get;
            set;
        }
        
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:LtlSharp.Automata.Transitions.Decorations.DegeneralizerDecoration"/> class with else flag
        /// set to false and an empty set of labels.
        /// </summary>
        public DegeneralizerDecoration () : this (false, Enumerable.Empty<int> ())
        {}
        
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:LtlSharp.Automata.Transitions.Decorations.DegeneralizerDecoration"/> class with the specified
        /// else flag and an empty set of labels.
        /// </summary>
        /// <param name="else">Else flag.</param>
        public DegeneralizerDecoration (bool @else) : this (@else, Enumerable.Empty<int> ())
        {}

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:LtlSharp.Automata.Transitions.Decorations.DegeneralizerDecoration"/> class with else flag
        /// set to false and the specified set of labels.
        /// </summary>
        /// <param name="labels">Labels.</param>
        public DegeneralizerDecoration (IEnumerable<int> labels): this (false, labels)
        {}
        
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:LtlSharp.Automata.Transitions.Decorations.DegeneralizerDecoration"/> class with the specified
        /// else flag and the specified labels.
        /// </summary>
        /// <param name="else">Else flag.</param>
        /// <param name="labels">Labels.</param>
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

