using System;
using LtlSharp.Automata.AcceptanceConditions;
using LtlSharp.Automata.Nodes.Factories;
using LtlSharp.Automata.Transitions;

namespace LtlSharp.Automata.OmegaAutomata
{
    public class DegeneralizerAutomaton<T> 
        : OmegaAutomaton<T, DegeneralizerDecoration>
        where T : IAutomatonNode
    {
        BuchiAcceptance<T> _acceptanceSet;

        public BuchiAcceptance<T> AcceptanceCondition {
            get {
                return _acceptanceSet;
            }
        }

        public DegeneralizerAutomaton (IAutomatonNodeFactory<T> factory) : base (factory)
        {
            _acceptanceSet = new BuchiAcceptance<T> ();
        }

        public override Automata<T, DegeneralizerDecoration> Clone ()
        {
            throw new NotImplementedException ();
        }
    }
}

