using System;
using System.Collections.Generic;

namespace LtlSharp.Automata
{
    public interface IAutomatonNode
    {
        int Id { get; }
        string Name { get; set; }
        ISet<ILiteral> Labels { get; set; }
    }
}

