using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Automata.AcceptanceConditions;
using QuickGraph;
using LtlSharp.Utils;
using LtlSharp.Language;
using QuickGraph.Graphviz;
using LtlSharp.Automata.Transitions;
using LtlSharp.Automata.Nodes.Factories;

namespace LtlSharp.Automata.OmegaAutomata
{
    public abstract class OmegaAutomaton<T> : Automata<T> where T : IAutomatonNode
    {
        public abstract IAcceptanceCondition<T> AcceptanceCondition { get; }

        public OmegaAutomaton (IAutomatonNodeFactory<T> factory) 
            : base (factory)
        {}
        
    }
}

