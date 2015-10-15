using System;
using System.Collections.Generic;

namespace LtlSharp.Automata.Transitions.Factories
{
    public class ProbabilityDecoratorFactory
        : IAutomatonTransitionFactory<ProbabilityTransitionDecorator>
    {
        public ProbabilityDecoratorFactory ()
        {
        }

        ProbabilityTransitionDecorator IAutomatonTransitionFactory<ProbabilityTransitionDecorator>.Create (IEnumerable<ILiteral> literals)
        {
            // fixme
            return new ProbabilityTransitionDecorator (0);
        }
        
        public ProbabilityTransitionDecorator Create (double probability)
        {
            return new ProbabilityTransitionDecorator (probability);
        }

        internal ProbabilityTransitionDecorator Clone (ProbabilityTransitionDecorator value)
        {
            return new ProbabilityTransitionDecorator (value.Probability);
        }
    }
}

