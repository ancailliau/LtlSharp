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
        // TODO Move this to factories
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
        /// Gets the labels of the nodes.
        /// </summary>
        /// <value>The labels.</value>
        public LiteralsSet Label {
            get;
            private set;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LtlSharp.Automata.AutomatonNode"/> class with an empty name
        /// and an empty label.
        /// </summary>
        public AutomatonNode () 
            : this ("")
        {}

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LtlSharp.Automata.AutomatonNode"/> class with the name of the
        /// specified node and the same label.
        /// </summary>
        /// <param name="node">Node.</param>
        public AutomatonNode (IAutomatonNode node) 
            : this (node.Name, node.Labels)
        {}
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LtlSharp.Automata.AutomatonNode"/> class with the specified
        /// name and an empty label.
        /// </summary>
        /// <param name="name">Name.</param>
        public AutomatonNode (string name) : this (name, Enumerable.Empty<ILiteral> ())
        {
            Name = name;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LtlSharp.Automata.AutomatonNode"/> class with the specified
        /// name (if not empty or null) and the provided literals.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="literals">Literals.</param>
        public AutomatonNode (string name, IEnumerable<ILiteral> literals)
        {
            Id = currentId++;
            Name = string.IsNullOrEmpty (name) ? "s" + Id : name;
            Label = new LiteralsSet (literals);
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

        LiteralsSet IAutomatonNode.Labels {
            get {
                return Label;
            }
        }
        
        #endregion
        
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
            return Name == other.Name && Label.Equals (other.Label);
        }

        public override int GetHashCode ()
        {
            return 17 + (Name != null ? Name.GetHashCode () : 0)
                          + 32 * (Label != null ? Label.GetHashCode () : 0);
        }
    }
}

