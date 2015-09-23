using System;
using System.Collections.Generic;
using System.Linq;
using QuickGraph;

namespace LtlSharp.Buchi.Automata
{
    public class AutomataTransition : Edge<AutomataNode>
    {
        public HashSet<ILiteral> Labels;

        public AutomataTransition (AutomataTransition transition) 
            : base (transition.Source, transition.Target)
        {
            Labels = new HashSet<ILiteral> (transition.Labels);
        }
        
        public AutomataTransition (AutomataNode source, AutomataNode target, HashSet<ILiteral> labels)
            : base (source, target)
        {
            Labels = labels;
        }
        
        public override string ToString ()
        {
            return string.Format ("[AutomataTransition: Source={0}, Target={1}, Labels={2}]", 
                                  Source, Target, string.Join (",", Labels));
        }
        
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(AutomataTransition))
                return false;
            var transition = (AutomataTransition)obj;
            return base.Equals(obj) && Labels.All (l => transition.Labels.Contains (l))
                     && transition.Labels.All (l => Labels.Contains (l));
        }

        public override int GetHashCode ()
        {
            return base.GetHashCode () ^ Labels.GetHashCode ();
        }
    }
}

