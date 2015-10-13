using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Utils;

namespace LtlSharp.Automata.AcceptanceConditions
{
    /// <summary>
    /// Represents a Rabin Acceptance condition.
    /// </summary>
    /// <description>
    /// An omega automata with a Rabin condition accepts the words where there is at least a rabin condition
    /// that is met.
    /// </description>
    /// <typeparam name="T">Types of the nodes in omega automaton.</typeparam>
    public class RabinAcceptance<T> : IAcceptanceCondition<T> {

        /// <summary>
        /// Gets all the Rabin conditions.
        /// </summary>
        /// <value>The conditions.</value>
        public HashSet<RabinCondition<T>> Conditions {
            get;
            private set;
        }

        public bool IsSatisfiable {
            get {
                throw new NotImplementedException ();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LtlSharp.Automata.AcceptanceConditions.RabinAcceptance`1"/> 
        /// class with no rabin condition.
        /// </summary>
        public RabinAcceptance ()
        {
            Conditions = new HashSet<RabinCondition<T>> ();
        }

        public bool Accept (IEnumerable<T> nodes)
        {
            return Conditions.Any (c => c.Rejecting.Intersect (nodes).Count () == 0 && (c.Accepting.Intersect (nodes).Count () > 0));
        }

        public bool Accept (T nodes)
        {
            throw new NotImplementedException ();
        }

        public IEnumerable<T> GetAcceptingNodes (HashSet<T> nodes)
        {
            throw new NotImplementedException ();
        }
        
        public IAcceptanceCondition<T1> Map<T1>(Func<T, IEnumerable<T1>> map)
        {
            var nacc = new RabinAcceptance<T1> ();

            foreach (var condition in this.Conditions) {
                nacc.Add (condition.Rejecting.SelectMany (map),
                          condition.Accepting.SelectMany (map));
            }

            return nacc;
        }

        /// <summary>
        /// Add a new rabin condition with the specified set of nodes e and f as rejecting and accepting set.
        /// </summary>
        /// <param name="e">Rejecting sets.</param>
        /// <param name="f">Accepting sets.</param>
        public void Add (IEnumerable<T> e, IEnumerable<T> f)
        {
            Conditions.Add (new RabinCondition<T> (e, f));
        }

        public override string ToString ()
        {
            return string.Format ("[RabinAcceptance: {{{0}}}]", 
                                  string.Join (",", Conditions.Select (x => x.ToString ())));
        }
    }
    
    /// <summary>
    /// Represents a Rabin condition. 
    /// </summary>
    /// <description>
    /// An omega automata with a Rabin condition accepts the words where no nodes in the rejecting set is met infinitely
    /// often and where at least a node in the accepting set is met infinitely often.
    /// </description>
    /// <typeparam name="T">Types of the nodes in omega automaton.</typeparam>
    public class RabinCondition<T>
    {
        /// <summary>
        /// Gets the rejecting set.
        /// </summary>
        /// <value>The rejecting.</value>
        public HashSet<T> Rejecting {
            get;
            private set;
        }

        /// <summary>
        /// Gets the accepting set.
        /// </summary>
        /// <value>The accepting.</value>
        public HashSet<T> Accepting {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LtlSharp.Automata.AcceptanceConditions.RabinCondition`1"/> 
        /// class with an empty rejecting and accepting set.
        /// </summary>
        public RabinCondition ()
        {
            Rejecting = new HashSet<T> ();
            Accepting = new HashSet<T> ();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:LtlSharp.Automata.AcceptanceConditions.RabinCondition`1"/> 
        /// class with the specified rejecting and accepting sets.
        /// </summary>
        /// <param name="rejecting">Accepting set.</param>
        /// <param name="accepting">Rejecting set.</param>
        public RabinCondition (IEnumerable<T> rejecting, IEnumerable<T> accepting)
        {
            Rejecting = new HashSet<T> (rejecting);
            Accepting = new HashSet<T> (accepting);
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(RabinCondition<T>))
                return false;
            var other = (RabinCondition<T>)obj;
            return Rejecting.SetEquals (other.Rejecting) && Accepting.SetEquals (other.Accepting);
        }

        public override int GetHashCode ()
        {
            unchecked {
                return (Rejecting != null ? Rejecting.GetHashCodeForElements () : 0) 
                    ^ (Accepting != null ? Accepting.GetHashCodeForElements () : 0);
            }
        }

        public override string ToString ()
        {
            return string.Format ("E={{{0}}}, F={{{1}}}]", 
                                  string.Join (",", Rejecting.Select (x => x.ToString ())),
                                  string.Join (",", Accepting.Select (x => x.ToString ()))
                                 );
        }
    }
}

