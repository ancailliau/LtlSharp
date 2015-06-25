using System;
using System.Linq.Expressions;
using System.Collections.Generic;

namespace LtlSharp
{
    public class RabinState
    {
        public string Label { get; set; }
    }
    
    public class RabinTransition
    {
        // Contains the propositions that are true
        // For example, if the set of proposition contains {allocated}
        // it is considered that mobilized is false and allocated is true
        public ISet<ISet<Proposition>> Propositions { get; set; }
        
        public RabinTransition ()
        {
            Propositions = new HashSet<ISet<Proposition>> ();
        }
    }
    
    public class RabinAutomata
    {
        public ISet<RabinState> States { get; set; }
        public IDictionary<RabinState, RabinTransition> Transitions { get; set; }
        
        public RabinAutomata ()
        {
            States = new HashSet<RabinState> ();
            Transitions = new Dictionary<RabinState, RabinTransition> ();
        }
    }
}

