using System;
using System.Collections.Generic;
using LtlSharp.Monitoring;

namespace LtlSharp.Automata
{
    public class AutomatonNode
    {
        public string Name;
        public HashSet<ILiteral> Labels;
        
        public AutomatonNode (string name)
        {
            Name = name;
            Labels = new HashSet<ILiteral> ();
        }
        
        public override string ToString ()
        {
            return string.Format ("[AutomatonNode: Name=\"{0}\"]", Name);
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

    public class MonitorNode : AutomatonNode 
    {   
        public MonitorStatus Status;

        public MonitorNode (string name, MonitorStatus status) : base (name)
        {
            Status = status;
        }
    }
}

