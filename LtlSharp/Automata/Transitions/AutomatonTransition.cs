using System;
namespace LtlSharp.Automata.Transitions
{
    /// <summary>
    /// Defines a transition in an automaton.
    /// </summary>
    public struct AutomatonTransition<T1, T2>
    {
        /// <summary>
        /// The source node.
        /// </summary>
        public readonly T1 Source;
        
        /// <summary>
        /// The target node.
        /// </summary>
        public readonly T1 Target;
        
        /// <summary>
        /// The decoration on the transition.
        /// </summary>
        public readonly T2 Decoration;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LtlSharp.Automata.Transitions.AutomatonTransition`2"/> 
        /// struct with the specified source node, target node and decoration.
        /// </summary>
        /// <param name="source">Source node.</param>
        /// <param name="target">Target node.</param>
        /// <param name="decoration">Decoration.</param>
        public AutomatonTransition (T1 source, T1 target, T2 decoration)
        {
            Source = source;
            Target = target;
            Decoration = decoration;
        }

        public override string ToString ()
        {
            return string.Format ("{0} -> {1} [{2}]", Source, Target, Decoration);
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(AutomatonTransition<T1, T2>))
                return false;
            var other = (AutomatonTransition<T1, T2>)obj;
            return Source.Equals(other.Source) & Target.Equals (other.Target) & Decoration.Equals (other.Decoration);
        }

        public override int GetHashCode ()
        {
            return 17 + Source.GetHashCode () + 32 * (Target.GetHashCode () + 32 * Decoration.GetHashCode ());
        }
    }
}

