using System;
using LtlSharp.Automata.AcceptanceConditions;
using LtlSharp.Automata.Nodes.Factories;
using LtlSharp.Automata.Transitions;

namespace LtlSharp.Automata.OmegaAutomata
{
    public class DegeneralizerAutomaton<T> 
        : OmegaAutomaton<T, DegeneralizerAutomataTransition>
        where T : IAutomatonNode
    {
        BuchiAcceptance<T> _acceptanceSet;

        public override IAcceptanceCondition<T> AcceptanceCondition {
            get {
                throw new NotImplementedException ();
            }
        }

        public DegeneralizerAutomaton (IAutomatonNodeFactory<T> factory) : base (factory)
        {
            _acceptanceSet = new BuchiAcceptance<T> ();
        }

        public override Automata<T, DegeneralizerAutomataTransition> Clone ()
        {
            throw new NotImplementedException ();
        }

        public BuchiAcceptance<T> GetAcceptanceCondition ()
        {
            return _acceptanceSet;
        }
    }
}

