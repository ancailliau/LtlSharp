using System;
using System.Collections.Generic;
namespace LtlSharp.Automata.Nodes.Factories
{
    public class AutomatonNodeProductFactory<T> : IAutomatonNodeFactory<ProductAutomatonNode<T>> where T : IAutomatonNode
    {
        public ProductAutomatonNode<T> Create ()
        {
            return new ProductAutomatonNode<T> ();
        }
        public ProductAutomatonNode<T> Create (string name)
        {
            return new ProductAutomatonNode<T> (name);
        }
        public ProductAutomatonNode<T> Create (string name, IEnumerable<ILiteral> labels)
        {
            return new ProductAutomatonNode<T> (name, labels);
        }
    }
}

