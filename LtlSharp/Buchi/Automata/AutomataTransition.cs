using System;
using System.Collections.Generic;
using System.Linq;

namespace LtlSharp.Buchi.Automata
{
    public class AutomataTransition
    {
        public AutomataNode To;
        public HashSet<ILiteral> Labels;
        
        public AutomataTransition (AutomataNode to, HashSet<ILiteral> labels)
        {
            To = to;
            Labels = labels;
        }
        
        public override string ToString ()
        {
            return string.Format ("[AutomataTransition: To={0}, Labels={1}]", To, string.Join (",", Labels));
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
            return To.Equals (transition.To) && Labels.All (l => transition.Labels.Contains (l))
                     && transition.Labels.All (l => Labels.Contains (l));
        }

        public override int GetHashCode ()
        {
            return To.GetHashCode () ^ Labels.GetHashCode ();
        }
    }
}

