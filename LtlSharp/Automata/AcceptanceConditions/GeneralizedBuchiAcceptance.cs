using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LtlSharp.Automata.AcceptanceConditions
{
    public class GeneralizedBuchiAcceptance<T> : IAcceptanceCondition<T>, IEnumerable<BuchiAcceptance<T>>
    {
        Dictionary<int, BuchiAcceptance<T>> _buchiAcceptances;
        
        public BuchiAcceptance<T> this[int i]
        {
            get {
                return _buchiAcceptances [i];
            }
        }
        
        public GeneralizedBuchiAcceptance ()
        {
            _buchiAcceptances = new Dictionary<int, BuchiAcceptance<T>> ();
        }
        
        public void Add (int setIndex, IEnumerable<T> set)
        {
            _buchiAcceptances.Add (setIndex, new BuchiAcceptance<T> (set));
        }

        public IEnumerable<int> GetAcceptingConditions (T node)
        {
            return _buchiAcceptances.Where (c => ((IAcceptanceCondition<T>) c.Value).Accept (node))
                                    .Select (c => c.Key);
        }

        public IEnumerable<int> AllKeys ()
        {
            return _buchiAcceptances.Keys;
        }

        public bool IsEmpty { 
            get {
                return _buchiAcceptances.Count == 0;
            }
        }

        public bool IsBuchi { 
            get {
                return _buchiAcceptances.Count == 1;
            }
        }

        public BuchiAcceptance<T> Get (int key)
        {
            BuchiAcceptance<T> value;
            if (_buchiAcceptances.TryGetValue(key, out value)) {
                return value;
            }
            return null;
        }

        public bool HasKey (int i)
        {
            return _buchiAcceptances.ContainsKey (i);
        }
        
        #region IAcceptanceCondition<T> Members

        bool IAcceptanceCondition<T>.IsSatisfiable {
            get {
                return _buchiAcceptances.All (kv => ((IAcceptanceCondition<T>)kv.Value).IsSatisfiable);
            }
        }

        public int Count {
            get {
                return _buchiAcceptances.Count;
            }
        }

		bool IAcceptanceCondition<T>.Accept (System.Collections.Generic.IEnumerable<T> nodes)
        {
            throw new NotImplementedException ();
        }

        bool IAcceptanceCondition<T>.Accept (T node)
        {
            return _buchiAcceptances.Any ((arg) => ((IAcceptanceCondition<T>) arg.Value).Accept (node));
        }

        IAcceptanceCondition<T1> IAcceptanceCondition<T>.Map<T1>(Func<T, System.Collections.Generic.IEnumerable<T1>> map)
        {
            throw new NotImplementedException ();
        }

        public IEnumerator<BuchiAcceptance<T>> GetEnumerator ()
        {
            return _buchiAcceptances.Values.GetEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return _buchiAcceptances.Values.GetEnumerator ();
        }

        #endregion
    }
}

