using System;
using System.Collections.Generic;
using System.Linq;

namespace LtlSharp.Automata.Nodes.Factories
{
    public class PowerSetAutomatonNodeFactory<T> 
        : IAutomatonNodeFactory<PowerSetAutomatonNode<T>> 
        where T : IAutomatonNode
    {
        public PowerSetAutomatonNode<T> Create ()
        {
            return new PowerSetAutomatonNode<T> ();
        }
        public PowerSetAutomatonNode<T> Create (string name)
        {
            return new PowerSetAutomatonNode<T> (name);
        }
        public PowerSetAutomatonNode<T> Create (string name, IEnumerable<ILiteral> labels)
        {
            return new PowerSetAutomatonNode<T> (name, labels);
        }
        public PowerSetAutomatonNode<T> Create (IEnumerable<T> nodes)
        {
            return new PowerSetAutomatonNode<T> (
                string.Join (",", nodes.Select (x => x.ToString ())),
                nodes.SelectMany (n => n.Labels).Distinct (),
                nodes
            );
        }
    }
}

