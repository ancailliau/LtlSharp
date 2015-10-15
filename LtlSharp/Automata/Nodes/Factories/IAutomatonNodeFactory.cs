using System;
using System.Collections.Generic;
namespace LtlSharp.Automata.Nodes.Factories
{
    public interface IAutomatonNodeFactory<T> {
        T Create ();
        T Create (string name);
        T Create (string name, IEnumerable<ILiteral> labels);
    }
}

