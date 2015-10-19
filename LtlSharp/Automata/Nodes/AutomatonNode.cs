using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Monitoring;
using LtlSharp.Utils;
using LtlSharp.Automata.Transitions;

namespace LtlSharp.Automata
{
    /// <summary>
    /// Defines an automaton node
    /// </summary>
    public class AutomatonNode : IAutomatonNode
    {
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
        /// Gets the labels of the nodes.
        /// </summary>
        /// <value>The labels.</value>
        public LiteralSet Label {
            get;
            private set;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LtlSharp.Automata.AutomatonNode"/> class with an empty name
        /// and an empty label.
        /// </summary>
        public AutomatonNode (int id) 
            : this (id, "")
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LtlSharp.Automata.AutomatonNode"/> class with the specified
        /// name and an empty label.
        /// </summary>
        /// <param name="name">Name.</param>
        public AutomatonNode (int id, string name) : this (id, name, Enumerable.Empty<ILiteral> ())
        { }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LtlSharp.Automata.AutomatonNode"/> class with the specified
        /// name (if not empty or null) and the provided literals.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="literals">Literals.</param>
        public AutomatonNode (int id, string name, IEnumerable<ILiteral> literals)
        {
            Id = id;
            Name = string.IsNullOrEmpty (name) ? "s" + Id : name;
            Label = new LiteralSet (literals);
        }
        
        #region IAutomatonNode Members
        
        int IAutomatonNode.Id {
            get {
                return Id;
            }
        }
        
        string IAutomatonNode.Name {
            get {
                return Name;
            }
            set {
                Name = value;
            }
        }

        LiteralSet IAutomatonNode.Labels {
            get {
                return Label;
            }
        }
        
        #endregion
        
        public override string ToString ()
        {
            return Name;
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
            return Name == other.Name && Label.Equals (other.Label);
        }

        public override int GetHashCode ()
        {
            return 17 + (Name != null ? Name.GetHashCode () : 0)
                          + 32 * (Label != null ? Label.GetHashCode () : 0);
        }
    }
}

