using System;
namespace LtlSharp.Automata.Transitions
{
    public struct AutomataTransition<T1, T2>
    {
        public readonly T1 Source;
        public readonly T1 Target;
        public readonly T2 Decoration;

        public AutomataTransition (T1 source, T1 target, T2 decoration)
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
            if (obj.GetType () != typeof(AutomataTransition<T1, T2>))
                return false;
            var other = (AutomataTransition<T1, T2>)obj;
            return Source.Equals(other.Source) & Target.Equals (other.Target) & Decoration.Equals (other.Decoration);
        }

        public override int GetHashCode ()
        {
            return 17 + Source.GetHashCode () + 32 * (Target.GetHashCode () + 32 * Decoration.GetHashCode ());
        }
    }
}

