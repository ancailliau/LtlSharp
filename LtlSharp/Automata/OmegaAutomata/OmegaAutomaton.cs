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
using LtlSharp.Automata.Transitions.Factories;

namespace LtlSharp.Automata.OmegaAutomata
{
    public abstract class OmegaAutomaton<T1,T2> 
        : Automata<T1,T2> 
        where T1 : IAutomatonNode
        where T2 : IAutomatonTransitionDecorator<T2>
    {
        public abstract IAcceptanceCondition<T1> AcceptanceCondition { get; }

        public OmegaAutomaton (IAutomatonNodeFactory<T1> factory,
                               IAutomatonTransitionFactory<T2> factory2) 
            : base (factory, factory2)
        {}
        
    }
}

