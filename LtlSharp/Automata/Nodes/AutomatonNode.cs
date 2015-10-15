using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Monitoring;
using LtlSharp.Utils;
using LtlSharp.Automata.Transitions;

namespace LtlSharp.Automata
{
    public class AutomatonNode : IAutomatonNode
    {
        static int currentId = 0;

        /// <summary>
        /// Gets the identifier of the node.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id {
            get;
            private set;
        }
        
        /// <summary>
        /// Gets or sets the name of the node.
        /// </summary>
        /// <value>The name.</value>
        public string Name {
            get;
            set;
        }

        /// <summary>
        /// Gets the labels attached to the node.
        /// </summary>
        /// <value>The labels.</value>
        public LiteralsSet Labels {
            get;
            set;
        }

        public AutomatonNode () : this ("") {}

        public AutomatonNode (AutomatonNode node) : this (node.Name, node.Labels) {}
        
        public AutomatonNode (string name) : this (name, Enumerable.Empty<ILiteral> ())
        {
            Name = name;
        }
        
        public AutomatonNode (string name, IEnumerable<ILiteral> labels)
        {
            Id = currentId++;
            Name = string.IsNullOrEmpty (name) ? "s" + Id : name;
            Labels = new LiteralsSet (labels);
        }
        
        public override string ToString ()
        {
            return string.Format ("[AutomatonNode: Name=\"{0}\" - {1}]", Name, GetHashCode ());
        }
        
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(AutomatonNode))
                return false;
            var other = (AutomatonNode)obj;
            return Name == other.Name && Labels.Equals (other.Labels);
        }

        public override int GetHashCode ()
        {
            return 17 + (Name != null ? Name.GetHashCode () : 0)
                          + 32 * (Labels != null ? Labels.GetHashCode () : 0);
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

