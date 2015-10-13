using System;
using System.Collections.Generic;
using System.Linq;
using QuickGraph;
using LtlSharp.Utils;

namespace LtlSharp.Automata
{
    public class AutomatonTransition<T> : Edge<T>
        where T : AutomatonNode
    {
        public AutomatonTransition (AutomatonTransition<T> transition) 
            : base (transition.Source, transition.Target)
        {
        }
        
        public AutomatonTransition (T source, T target)
            : base (source, target)
        {
        }
        
        public override string ToString ()
        {
            return string.Format ("[AutomataTransition: Source={0}, Target={1}]", Source.Name, Target.Name);
        }
        
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(LabeledAutomataTransition<T>))
                return false;
            return base.Equals(obj);
        }

        public override int GetHashCode ()
        {
            return base.GetHashCode ();
        }
    }

    public class LabeledAutomataTransition<T> : AutomatonTransition<T>
        where T : AutomatonNode
    {   
        public HashSet<ILiteral> Labels;
        
        public LabeledAutomataTransition (T source, T target)
            : this (source, target, new HashSet<ILiteral> ())
        {
        }

        public LabeledAutomataTransition (T source, T target, HashSet<ILiteral> labels)
            : base (source, target)
        {
            Labels = labels;
        }
        
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(LabeledAutomataTransition<T>))
                return false;
            var other = (LabeledAutomataTransition<T>)obj;
            return Source.Equals (other.Source) && Target.Equals (other.Target) && Labels.SetEquals (other.Labels);
        }

        public override int GetHashCode ()
        {
            return 17 + Source.GetHashCode () + 32 * ( Target.GetHashCode () + 32 * Labels.GetHashCodeForElements ());
        }
    }
    
    public class DegeneralizerAutomataTransition<T> : AutomatonTransition<T>
        where T : AutomatonNode
    {   
        public HashSet<int> Labels;
        public bool Else;

        public DegeneralizerAutomataTransition (T source, T target)
            : base (source, target)
        {
            Labels = new HashSet<int> ();
            Else = false;
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(DegeneralizerAutomataTransition<T>))
                return false;
            var other = (DegeneralizerAutomataTransition<T>)obj;
            return base.Equals(obj) && Labels.All (l => other.Labels.Contains (l))
                       && other.Labels.All (l => Labels.Contains (l)) & Else == other.Else;
        }

        public override int GetHashCode ()
        {
            return 17 + 23 * (base.GetHashCode () + 23 * (Labels.GetHashCode () + 23 * Else.GetHashCode ()));
        }
    }
}

