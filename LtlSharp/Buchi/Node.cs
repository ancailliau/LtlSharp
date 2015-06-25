using System;
using System.Collections.Generic;

namespace LtlSharp.Buchi
{
    public class ConsistentSet : HashSet<ILTLFormula> {
        public ConsistentSet () : base()
        {
        }
        public ConsistentSet (IEnumerable<ILTLFormula> set) : base(set)
        {
        }
        public new bool Add (ILTLFormula formula) {
            var neg = formula.Negate ();
            if (base.Contains (neg)) {
                return false;
            }
            base.Add (formula);
            return true;
        }
        public bool AddRange (IEnumerable<ILTLFormula> formulas) {
            foreach (var v in formulas) {
                if (!Add (v)) {
                    return false;
                }
            }
            return true;
        }
    }
    
    public class Node
    {
        public string Name {
            get;
            set;
        }
        
        public List<string> Incoming {
            get;
            set;
        }
        
        public ConsistentSet New {
            get;
            set;
        }
        
        public ConsistentSet Old {
            get;
            set;
        }
        
        public ConsistentSet Next {
            get;
            set;
        }
        
        public Node () : this (Guid.NewGuid ().ToString ()) 
        {}
        
        public Node (string name)
        {
            Name = name;
            Incoming = new List<string> ();
            New = new ConsistentSet ();
            Old = new ConsistentSet ();
            Next = new ConsistentSet ();
        }
        
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(Node))
                return false;
            Node other = (Node)obj;
            return Name == other.Name;
        }
        

        public override int GetHashCode ()
        {
            unchecked {
                return (Name != null ? Name.GetHashCode () : 0);
            }
        }
        
    }
}

