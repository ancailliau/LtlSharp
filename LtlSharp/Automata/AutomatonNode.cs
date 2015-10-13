using System;
using System.Collections.Generic;
using LtlSharp.Monitoring;

namespace LtlSharp.Automata
{
    public class AutomatonNode
    {
        public string Name;
        
        public AutomatonNode (string name)
        {
            Name = name;
        }
        
        public override string ToString ()
        {
            return string.Format ("[AutomataNode: Name=\"{0}\"]", Name);
        }
        
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(AutomatonNode))
                return false;
            var node = (AutomatonNode)obj;
            return Name == node.Name;
        }

        public override int GetHashCode ()
        {
            return Name.GetHashCode ();
        }
    }
    
    public class LabelledAutomataNode : AutomatonNode 
    {   
        public HashSet<ILiteral> Labels;

        public LabelledAutomataNode (string name) : base (name)
        {
            Labels = new HashSet<ILiteral> ();
        }
    }

    public class MonitorNode : AutomatonNode 
    {   
        public MonitorStatus Status;

        public MonitorNode (string name, MonitorStatus status) : base (name)
        {
            Status = status;
        }
    }
}

