using System;
using System.Collections.Generic;

namespace LtlSharp.Buchi.Automata
{
    public class AutomataTransition
    {
        public int To;
        public HashSet<ILiteral> Labels;
        
        public AutomataTransition (int to, HashSet<ILiteral> labels)
        {
            To = to;
            Labels = labels;
        }
        
        public override string ToString ()
        {
            return string.Format ("[AutomataTransition: To={0}, Labels={1}]", To, string.Join (",", Labels));
        }
    }
}

