using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Buchi.Automata;
using LtlSharp.Models;
using LtlSharp.Utils;

namespace LtlSharp.Automata
{
    public interface IAcceptanceCondition<T>
    {
        /// <summary>
        /// Returns whether the specified nodes is accepted by the w-automaton when repeatedly reached.
        /// </summary>
        /// <param name="nodes">Node repeatedly reached.</param>
        bool Accept (T nodes);
        
        /// <summary>
        /// Returns whether the specified set of nodes is accepted by the w-automaton when repeatedly reached.
        /// </summary>
        /// <param name="nodes">Nodes repeatedly reached.</param>
        bool Accept (IEnumerable<T> nodes);

        /// <summary>
        /// Returns whether the condition might be satisfied.
        /// </summary>
        /// <returns><c>True</c> if the condition can be satisfied, <c>False</c> otherwise.</returns>
        bool IsSatisfiable { get; }

        /// <summary>
        /// Returns the subset of nodes that are accepted by the acceptance condition when repeatedly reached.
        /// </summary>
        /// <returns>The accepting nodes.</returns>
        /// <param name="nodes">Nodes.</param>
        IEnumerable<T> GetAcceptingNodes (HashSet<T> nodes);

        IAcceptanceCondition<T1> Map<T1>(Func<T, IEnumerable<T1>> p);
    }

    public class BuchiAcceptance<T> : IAcceptanceCondition<T>
    {
        HashSet<T> acceptingSet;

        public bool IsSatisfiable
        {
            get { 
                return acceptingSet.Count > 0;
            }
        }
        
        public BuchiAcceptance ()
        {
            acceptingSet = new HashSet<T> ();
        }
        
        public BuchiAcceptance (BuchiAcceptance<T> t)
        {
            acceptingSet = new HashSet<T> (t.acceptingSet);
        }
        
        public BuchiAcceptance (IEnumerable<T> nodes)
        {
            acceptingSet = new HashSet<T> (nodes);
        }

        public bool Accept (T node)
        {
            return acceptingSet.Contains (node);
        }
        
        public bool Accept (IEnumerable<T> nodes)
        {
            return acceptingSet.Intersect (nodes).Any ();
        }
        
        public void Add (T node)
        {
            acceptingSet.Add (node);
        }

        public IEnumerable<T> GetAcceptingNodes (HashSet<T> nodes)
        {
            return acceptingSet.Intersect (nodes);
        }
        
        public override string ToString ()
        {
            return string.Format ("[BuchiAcceptance: acceptingSet={{{0}}}]", 
                                  string.Join (",", acceptingSet.Select (x => x.ToString ())));
        }

        public IAcceptanceCondition<T1> Map<T1>(Func<T, IEnumerable<T1>> p)
        {
            return null;
        }
    }
    
    public class RabinAcceptance<T> : IAcceptanceCondition<T> {

        private HashSet<RabinCondition<T>> conditions;
        
        public bool IsSatisfiable {
            get {
                throw new NotImplementedException ();
            }
        }
        
        public RabinAcceptance ()
        {
            conditions = new HashSet<RabinCondition<T>> ();
        }

        public bool Accept (IEnumerable<T> nodes)
        {
            return conditions.Any (c => c.E.Intersect (nodes).Count () == 0 && (c.F.Intersect (nodes).Count () > 0));
        }

        public bool Accept (T nodes)
        {
            throw new NotImplementedException ();
        }

        public IEnumerable<T> GetAcceptingNodes (HashSet<T> nodes)
        {
            throw new NotImplementedException ();
        }

        public void Add (IEnumerable<T> e, IEnumerable<T> f)
        {
            conditions.Add (new RabinCondition<T> (e, f));
        }
        
        public override string ToString ()
        {
            return string.Format ("[RabinAcceptance: {{{0}}}]", 
                                  string.Join (",", conditions.Select (x => x.ToString ())));
        }
        
        
        public IAcceptanceCondition<T1> Map<T1>(Func<T, IEnumerable<T1>> map)
        {
            var nacc = new RabinAcceptance<T1> ();

            foreach (var condition in this.conditions) {
                var hashE = new HashSet<T1> ();
                var hashF = new HashSet<T1> ();
                foreach (var e in condition.E) {
                    foreach (var mappedE in map (e)) {
                        hashE.Add (mappedE);
                    }
                }
                foreach (var f in condition.F) {
                    foreach (var mappedF in map (f)) {
                        hashF.Add (mappedF);
                    }
                }
                nacc.Add (hashE, hashF);
            }
            
            return nacc;
        }

        private class RabinCondition<T>
        {
            public HashSet<T> E {
                get;
                set;
            }

            public HashSet<T> F {
                get;
                set;
            }

            public RabinCondition ()
            {
                E = new HashSet<T> ();
                F = new HashSet<T> ();
            }

            public RabinCondition (IEnumerable<T> e, IEnumerable<T> f)
            {
                E = new HashSet<T> (e);
                F = new HashSet<T> (f);
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
                return E.SetEquals (other.E) && F.SetEquals (other.F);
            }


            public override int GetHashCode ()
            {
                unchecked {
                    return (E != null ? E.GetHashCodeForElements () : 0) 
                        ^ (F != null ? F.GetHashCodeForElements () : 0);
                }
            }

            public override string ToString ()
            {
                return string.Format ("E={{{0}}}, F={{{1}}}]", 
                                      string.Join (",", E.Select (x => x.ToString ())),
                                      string.Join (",", F.Select (x => x.ToString ()))
                                     );
            }
        }
        
    }
    
}

