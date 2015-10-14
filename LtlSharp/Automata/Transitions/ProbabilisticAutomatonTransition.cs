using System;
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
    public class MarkovTransition : IAutomatonTransition<int>
    {
        public int Source {
            get;
            set;
        }
        
        public int Target {
            get;
            set;
        }
        
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
        /// <param name="source">Source node.</param>
        /// <param name="probability">Transition probability.</param>
        /// <param name="target">Target node.</param>
        public MarkovTransition (int source, double probability, int target)
        {
            Probability = probability;
            Source = source;
            Target = target;
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(MarkovTransition))
                return false;
            MarkovTransition other = (MarkovTransition)obj;
            return Probability == other.Probability 
                                       && Source.Equals (other.Source) 
                                       && Target.Equals (other.Target);
        }


        public override int GetHashCode ()
        {
            unchecked {
                return Probability.GetHashCode ()
                                  ^ Source.GetHashCode ()
                                  ^ Target.GetHashCode ();
            }
        }

        public IAutomatonTransition<int> Clone ()
        {
            return new MarkovTransition (Source, Probability, Target);
        }
    }
}

