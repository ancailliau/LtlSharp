using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Automata.Transitions;

namespace LtlSharp.Automata
{
    public class ProductAutomatonNode<T> : AutomatonNode where T : IAutomatonNode
    {
        public T MarkovNode {
            get;
            set;
        }
        public AutomatonNode AutomatonNode {
            get;
            set;
        }
        
        public ProductAutomatonNode () 
            : base ()
        {}
        
        public ProductAutomatonNode (string name) 
            : base(name)
        {}
        
        public ProductAutomatonNode (string name, IEnumerable<ILiteral> labels)
            : base (name, labels)
        {}
        
        public override string ToString ()
        {
            return string.Format ("[ProductAutomatonNode: AutomatonNode={0}, AutomatonNode={1}]", AutomatonNode.Name, AutomatonNode.Name);
        }

        public void SetNodes (T markovNode, AutomatonNode automataNode)
        {
            this.MarkovNode = markovNode;
            this.AutomatonNode = automataNode;
        }
    }
}

