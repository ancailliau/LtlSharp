using System;
using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace LtlSharp.Buchi.Automata
{
    public class AutomataTransition<T> : Edge<T>
        where T : AutomataNode
    {
        public AutomataTransition (AutomataTransition<T> transition) 
            : base (transition.Source, transition.Target)
        {
        }
        
        public AutomataTransition (T source, T target)
            : base (source, target)
        {
        }
        
        public override string ToString ()
        {
            return string.Format ("[AutomataTransition: Source={0}, Target={1}]", Source, Target);
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

    public class LabeledAutomataTransition<T> : AutomataTransition<T>
        where T : AutomataNode
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
            var transition = (LabeledAutomataTransition<T>)obj;
            return base.Equals(obj) && Labels.All (l => transition.Labels.Contains (l))
                       && transition.Labels.All (l => Labels.Contains (l));
        }

        public override int GetHashCode ()
        {
            return base.GetHashCode () ^ Labels.GetHashCode ();
        }
    }
    
    public class DegeneralizerAutomataTransition<T> : AutomataTransition<T>
        where T : AutomataNode
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

