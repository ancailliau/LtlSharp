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
        
        public HashSet<string> Incoming {
            get;
            set;
        }
        
        public HashSet<ILTLFormula> New {
            get;
            set;
        }
        
        public HashSet<ILTLFormula> Old {
            get;
            set;
        }
        
        public HashSet<ILTLFormula> Next {
            get;
            set;
        }
        
        public Node () : this (Guid.NewGuid ().ToString ()) 
        {}
        
        public Node (string name)
        {
            Name = name;
            Incoming = new HashSet<string> ();
            New = new HashSet<ILTLFormula> ();
            Old = new HashSet<ILTLFormula> ();
            Next = new HashSet<ILTLFormula> ();
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
        
        public override string ToString ()
        {
            return string.Format ("[Node: Name={0}, New={{{1}}}, Old={{{2}}}, Next={{{3}}}]", 
                Name.Substring(0, 5) + "...", string.Join(",", New), string.Join(",", Old), string.Join(",",Next));
        }
        
        
    }
}

