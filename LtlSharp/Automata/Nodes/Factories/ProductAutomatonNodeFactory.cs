using System;
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
    }
}

