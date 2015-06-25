using System;
using System.Collections.Generic;

namespace LtlSharp.Buchi
{
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
        
        public List<ILTLFormula> New {
            get;
            set;
        }
        
        public List<ILTLFormula> Old {
            get;
            set;
        }
        
        public List<ILTLFormula> Next {
            get;
            set;
        }
        
        public Node () : this (Guid.NewGuid ().ToString ()) 
        {}
        
        public Node (string name)
        {
            Name = name;
            Incoming = new List<string> ();
            New = new List<ILTLFormula> ();
            Old = new List<ILTLFormula> ();
            Next = new List<ILTLFormula> ();
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

