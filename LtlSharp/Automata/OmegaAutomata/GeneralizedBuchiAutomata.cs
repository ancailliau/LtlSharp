using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LtlSharp.Automata;
using QuickGraph;
using LtlSharp.Automata.Nodes.Factories;
using LtlSharp.Automata.Transitions;
using LtlSharp.Utils;
using LtlSharp.Automata.AcceptanceConditions;
using LtlSharp.Automata.Transitions.Decorations;

namespace LtlSharp.Automata.OmegaAutomata
{
    public class TransitionGeneralizedBuchiAutomata<T>
        : OmegaAutomaton<T, LiteralSetDecoration>
        where T : IAutomatonNode
    {
        GeneralizedBuchiAcceptance<AutomataTransition<T, LiteralSetDecoration>> _acceptanceCondition;

        public GeneralizedBuchiAcceptance<AutomataTransition<T, LiteralSetDecoration>> AcceptanceCondition {
            get {
                return _acceptanceCondition;
            }
        }

        public TransitionGeneralizedBuchiAutomata (IAutomatonNodeFactory<T> factory)
            : base (factory)
        {
            _acceptanceCondition = new GeneralizedBuchiAcceptance<AutomataTransition<T, LiteralSetDecoration>> ();
        }
        
        public GeneralizedBuchiAcceptance<AutomataTransition<T, LiteralSetDecoration>> GetAcceptanceCondition ()
        {
            return _acceptanceCondition;
        }

        public override Automata<T, LiteralSetDecoration> Clone ()
        {
            throw new NotImplementedException ();
        }
    }
}

