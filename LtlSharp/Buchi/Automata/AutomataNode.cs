using System;
using System.Collections.Generic;

namespace LtlSharp.Buchi.Automata
{
    public class AutomataNode
    {
        public string Name;
        
        public AutomataNode (string name)
        {
            Name = name;
        }
        
        public override string ToString ()
        {
            return string.Format ("[BANode: Name=\"{0}\"]", Name);
        }
        
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(AutomataNode))
                return false;
            var node = (AutomataNode)obj;
            return Name == node.Name;
        }

        public override int GetHashCode ()
        {
            return Name.GetHashCode ();
        }
    }
    
    public class LabelledAutomataNode : AutomataNode 
    {   
        public HashSet<ILiteral> Labels;

        public LabelledAutomataNode (string name) : base (name)
        {
            Labels = new HashSet<ILiteral> ();
        }
    }
}

