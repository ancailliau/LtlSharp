using System;
using System.Collections.Generic;
using LtlSharp.Automata.Transitions;

namespace LtlSharp.Automata
{
    public class ProductAutomatonNode<T> : IAutomatonNode where T : IAutomatonNode
    {
        public T MarkovNode {
            get;
            set;
        }
        public AutomatonNode AutomatonNode {
            get;
            set;
        }
        public int Id {
            get; private set; 
        }

        public LiteralsSet Labels {
            get {
                return MarkovNode.Labels;
            }

            set {
                MarkovNode.Labels = value;
            }
        }
        static int currentId;
        public string Name {
            get; set;
        }
        public ProductAutomatonNode ()
        {
            this.Id = currentId++;
        }
        public ProductAutomatonNode (string name) : this()
        {
            this.Name = name;
        }
        public ProductAutomatonNode (string name, IEnumerable<ILiteral> labels) : this()
        {
            // Todo fix and use this
            throw new NotImplementedException ();
            this.Name = name;
        }
        public override string ToString ()
        {
            return string.Format ("[ProductAutomatonNode: AutomatonNode={0}, AutomatonNode={1}]", AutomatonNode.Name, AutomatonNode.Name);
        }

        public void SetNodes (T markovNode, AutomatonNode automataNode)
        {
            this.MarkovNode = markovNode;
            this.AutomatonNode = automataNode;
            Name = string.Format ("{0} x {1}", markovNode.Name, automataNode.Name);
        }
    }
}

