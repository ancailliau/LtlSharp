using System;
using System.Collections.Generic;
namespace LtlSharp.Automata.Nodes.Factories
{
    public class AutomatonNodeProductFactory<T1, T2> 
        : IAutomatonNodeFactory<ProductAutomatonNode<T1, T2>> 
        where T1 : IAutomatonNode
        where T2 : IAutomatonNode
    {
        public ProductAutomatonNode<T1, T2> Create ()
        {
            return new ProductAutomatonNode<T1, T2> ();
        }
        public ProductAutomatonNode<T1, T2> Create (string name)
        {
            return new ProductAutomatonNode<T1, T2> (name);
        }
        public ProductAutomatonNode<T1, T2> Create (string name, IEnumerable<ILiteral> labels)
        {
            return new ProductAutomatonNode<T1, T2> (name, labels);
        }
    }
}

