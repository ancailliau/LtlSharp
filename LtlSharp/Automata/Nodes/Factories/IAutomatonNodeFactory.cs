using System;
namespace LtlSharp.Automata.Nodes.Factories
{
    public interface IAutomatonNodeFactory<T> {
        T Create ();
        T Create (string name);
    }
}

