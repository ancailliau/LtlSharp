﻿using System;
using LtlSharp.Automata.Transitions;
using LtlSharp.Automata.AcceptanceConditions;
using LtlSharp.Automata.Nodes.Factories;
using System.Collections.Generic;
using LtlSharp.Automata.Transitions.Decorations;

namespace LtlSharp.Automata.OmegaAutomata
{
    public class GeneralizedBuchiAutomaton<T> : OmegaAutomaton<T, LiteralSetDecoration>
        where T : IAutomatonNode
    {
        GeneralizedBuchiAcceptance<T> _acceptanceCondition;
        
        public GeneralizedBuchiAcceptance<T> AcceptanceCondition {
            get {
                return _acceptanceCondition;
            }
        }
        
        public GeneralizedBuchiAutomaton (IAutomatonNodeFactory<T> factory) : base (factory)
        {
            _acceptanceCondition = new GeneralizedBuchiAcceptance<T> ();
        }
        
        public override Automaton<T, LiteralSetDecoration> Clone ()
        {
            throw new NotImplementedException ();
        }
    }
}

