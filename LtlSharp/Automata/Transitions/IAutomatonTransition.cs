using System;
namespace LtlSharp.Automata
{
    public interface IAutomatonTransition<T>
    {
        T Source { get; }
        T Target { get; }
        IAutomatonTransition<T> Clone ();
    }
}

