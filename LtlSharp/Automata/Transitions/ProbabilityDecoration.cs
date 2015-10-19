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
    public class ProbabilityDecoration 
        : ITransitionDecoration<ProbabilityDecoration>
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
        public ProbabilityDecoration (double probability)
        {
            Probability = probability;
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(ProbabilityDecoration))
                return false;
            ProbabilityDecoration other = (ProbabilityDecoration)obj;
            return Probability == other.Probability;
        }

        public override int GetHashCode ()
        {
            unchecked {
                return Probability.GetHashCode ();
            }
        }

    }
}

