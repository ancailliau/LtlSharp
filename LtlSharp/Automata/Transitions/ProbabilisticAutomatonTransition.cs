using System;
using System.Collections.Generic;
using LtlSharp.Automata.Transitions;
using QuickGraph;

namespace LtlSharp.Automata
{
    /// <summary>
    /// Represents a Markov Transition.
    /// </summary>
    /// <description>
    /// A Markov Transition has a source Markov node and a target Markov node. 
    /// The transition is decorated with its probability.
    /// </description>
    public class ProbabilityTransitionDecorator 
        : IAutomatonTransitionDecorator<ProbabilityTransitionDecorator>
    {   
        /// <summary>
        /// Gets or sets the probability of the transition.
        /// </summary>
        /// <value>The probability.</value>
        public double Probability {
            get;
            set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LtlSharp.Models.MarkovTransition"/> class.
        /// </summary>
        /// <param name="probability">Transition probability.</param>
        public ProbabilityTransitionDecorator (double probability)
        {
            Probability = probability;
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(ProbabilityTransitionDecorator))
                return false;
            ProbabilityTransitionDecorator other = (ProbabilityTransitionDecorator)obj;
            return Probability == other.Probability;
        }

        public override int GetHashCode ()
        {
            unchecked {
                return Probability.GetHashCode ();
            }
        }

        IEnumerable<ILiteral> IAutomatonTransitionDecorator<ProbabilityTransitionDecorator>.GetAlphabet ()
        {
            throw new NotImplementedException ();
        }

        LiteralsSet IAutomatonTransitionDecorator<ProbabilityTransitionDecorator>.ToLiteralSet ()
        {
            throw new NotImplementedException ();
        }

        bool IAutomatonTransitionDecorator<ProbabilityTransitionDecorator>.Entails (ProbabilityTransitionDecorator l)
        {
            throw new NotImplementedException ();
        }

        IEnumerable<ProbabilityTransitionDecorator> IAutomatonTransitionDecorator<ProbabilityTransitionDecorator>.UnfoldLabels (IEnumerable<ILiteral> enumerable)
        {
            throw new NotImplementedException ();
        }
    }
}

