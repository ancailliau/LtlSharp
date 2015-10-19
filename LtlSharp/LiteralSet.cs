using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Utils;

namespace LtlSharp.Automata.Transitions
{
    public class LiteralSet : IEnumerable<ILiteral>
    {
        HashSet<ILiteral> _literals;
        
        public LiteralSet ()
        {
            _literals = new HashSet<ILiteral>();
        }
        
        public LiteralSet (IEnumerable<ILiteral> literals)
        {
            _literals = new HashSet<ILiteral>(literals);
        }
        
        public IEnumerable<Proposition> GetAlphabet ()
        {
            return _literals.Where (x => x is Negation | x is Proposition).Select (x => {
                if (x is Negation) {
                    var neg = (Negation)x;
                    return (Proposition)neg.Enclosed;
                    
                } else if (x is Proposition) {
                    return (Proposition)x;
                }
                
                return null; // Never happen.
            }).Distinct ();
        }
        
        private ISet<ILiteral> Project (IEnumerable<ILiteral> literals)
        {
            var alphabet = GetAlphabet ();
            return new HashSet<ILiteral> (literals.Where (x => {
                if (x is Negation) {
                    if (alphabet.Contains ((ILiteral)((Negation)x).Enclosed)) {
                        return true;
                    }

                } else {
                    if (alphabet.Contains (x)) {
                        return true;
                    }
                }

                return false;
            }));
        }
        
        public bool Entails (LiteralSet l)
        {
            return Project (l._literals).IsSubsetOf (_literals);
        }
        
        public IEnumerable<LiteralSet> Expand (IEnumerable<ILiteral> alphabet)
        {
            
            Console.WriteLine ("start");
            var s = new HashSet<LiteralSet> ();
            s.Add (new LiteralSet ());

            Console.WriteLine (string.Join (",", alphabet));
            
            var pending = new Stack<ILiteral> (alphabet);
            while (pending.Count > 0) {
                var current = pending.Pop ();
                Console.WriteLine ("current = " + current);
                
                if (_literals.Contains (current)) {
                    foreach (var e in s) {
                        e.Add (current);
                    }

                } else if (_literals.Contains (current.Negate ())) {
                    s = new HashSet<LiteralSet> (s.Where (l => !l.Contains (current)));

                } else {
                    foreach (var e in s.ToList ()) {
                        var ns = new LiteralSet (e);
                        ns.Add (current);
                        s.Add (ns);
                    }
                }
            }

            foreach (var a in alphabet) {
                foreach (var ss in s) {
                    if (!ss.Contains (a)) {
                        ss.Add ((ILiteral)a.Negate ());
                    }
                }
            }
            
            Console.WriteLine ("end");
            return s;
        }
        
        public override string ToString ()
        {
            return "{" + string.Join (", ", _literals) + "}";
        }
        
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(LiteralSet))
                return false;
            var other = (LiteralSet)obj;
            return _literals.SetEquals (other._literals);
        }

		public override int GetHashCode ()
        {
            return _literals.GetHashCodeForElements ();
        }

        public IEnumerator<ILiteral> GetEnumerator ()
        {
            return _literals.GetEnumerator ();
        }

        IEnumerator IEnumerable.GetEnumerator ()
        {
            return _literals.GetEnumerator ();
        }

        public void Add (ILiteral literal)
        {
            _literals.Add (literal);
        }
    }
}

