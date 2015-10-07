using System;
using System.Collections.Generic;

namespace LtlSharp.Utils
{
    public class ExtractAlphabet : Traversal
    {
        public IList<string> Alphabet { get {
                alphabet = new List<string> ();
                Visit ();
                return alphabet;
            }
        }
        
        private IList<string> alphabet;
        
        public ExtractAlphabet (ITLFormula formula)
            : base (formula)
        {
        }
        
        protected override void VisitProposition (Proposition proposition)
        {
            if (!alphabet.Contains (proposition.Name))
                alphabet.Add (proposition.Name);
        }
    }
}

