using System;
using System.Collections.Generic;
using LtlSharp.Automata.Transitions;

namespace LtlSharp.Automata
{
    public interface IAutomatonNode
    {
        int Id { get; }
        string Name { get; set; }
        LiteralsSet Labels { get; set; }
    }
}

