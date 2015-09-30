using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;

namespace LtlSharp.Language
{
    public class Term
    {
        public const byte False = 0;
        public const byte True = 1;
        public const byte DontCare = 2;

        public byte[] Values;

        public Term (byte[] values)
        {
            this.Values = values;
        }

        public int NumVars
        {
            get {
                return Values.Length;
            }
        }

        public Term Combine (Term term)
        {
            int positionWhereDifferent = -1;

            for (int i=0; i<Values.Length; i++) {
                if (Values [i] != term.Values [i]) {
                    if (positionWhereDifferent == -1) {
                        positionWhereDifferent = i;

                    } else {
                        // They're different in at least two places
                        return null;
                    }
                }
            }

            // They're identical
            if (positionWhereDifferent == -1) {
                return null;
            }

            byte[] resultVars = new byte[Values.Length];
            Array.Copy (Values, resultVars, Values.Length);
            resultVars [positionWhereDifferent] = DontCare;

            return new Term (resultVars);
        }

        public int CountValues (byte value)
        {
            int result = 0;
            for (int i=0; i<Values.Length; i++) {
                if (Values [i] == value) {
                    result++;
                }
            }
            return result;
        }

        public bool Implies (Term term)
        {
            for (int i=0; i<Values.Length; i++) {
                if (Values [i] != DontCare &&
                    Values [i] != term.Values [i]) {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(Term))
                return false;
            Term other = (Term)obj;
            return Enumerable.SequenceEqual (Values, other.Values);
        }

        public override int GetHashCode ()
        {
            unchecked {
                return (Values != null ? Values.GetHashCode () : 0);
            }
        }

        public override string ToString ()
        {
            var result = "{";
            for (int i=0; i<Values.Length; i++) {
                if (Values [i] == DontCare)
                    result += "X";
                else
                    result += Values [i];
                result += " ";
            }
            result += "}";
            return result;
        }
    }

    public class LiteralFormula
    {
        public List<Term> Terms;

        List<Term> originalTermList;
        Dictionary<ILiteral, int> encoding;

        public void ReduceToPrimeImplicants ()
        {
            originalTermList = new List<Term> (Terms);

            // Create term table
            int numVars = Terms [0].NumVars;

            // At position [i,j], there is a list of terms having i "DontCare" and j "True"
            var table = new List<Term>[numVars + 1, numVars + 1];

            for (int dontKnows=0; dontKnows <= numVars; dontKnows++) {
                for (int ones=0; ones <= numVars; ones++) {
                    table [dontKnows, ones] = new List<Term> ();
                }
            }

            for (int i=0; i<Terms.Count; i++) {
                int dontCares = Terms [i].CountValues (Term.DontCare);
                int ones = Terms [i].CountValues ((byte)1);
                table [dontCares, ones].Add (Terms [i]);
            }


            // Generate new terms with combine() while updating prime implicant list
            for (int dontKnows=0; dontKnows <= numVars - 1; dontKnows++) {
                for (int ones=0; ones <= numVars - 1; ones++) {
                    List<Term> left = table [dontKnows, ones];
                    List<Term> right = table [dontKnows, ones + 1];
                    List<Term> @out = table [dontKnows + 1, ones];
                    for (int leftIdx = 0; leftIdx < left.Count; leftIdx++) {
                        for (int rightIdx = 0; rightIdx < right.Count; rightIdx++) {
                            Term combined = left [leftIdx].Combine (right [rightIdx]);
                            if (combined != null) {
                                if (!@out.Contains (combined)) {
                                    @out.Add (combined); 
                                }

                                // Update prime implicant list
                                Terms.Remove (left [leftIdx]);
                                Terms.Remove (right [rightIdx]);
                                if (!Terms.Contains (combined)) {
                                    Terms.Add (combined);
                                }


                            }
                        }
                    }
                }
            }
        }

        void EnumerateAllSolutions (bool[,] table, List<Term> terms, List<List<Term>> AllSolutions)
        {
            bool changed = false;
            for (int i = 0; i < table.GetLength (0); i++) {
                bool isNonZero = false;
                for (int j = 0; j < table.GetLength (1); j++) {
                    if (table [i, j]) {
                        isNonZero = true;
                        break;
                    }
                }

                if (isNonZero) {
                    changed = true;

                    // copy the table
                    var newTable = new bool[table.GetLength (0), table.GetLength (1)];
                    Array.Copy (table, newTable, table.Length);

                    // remove the implicant
                    extractImplicant (newTable, i);

                    // add the new implicant to the list
                    var newTerms = new List<Term> (terms);
                    newTerms.Add (Terms[i]);

                    // go recursive
                    EnumerateAllSolutions (newTable, newTerms, AllSolutions);
                }
            }

            if (!changed) {
                AllSolutions.Add (terms);
            }
        }

        public void ReducePrimeImplicantsToSubset (bool heuristically = false)
        {
            // create implies table
            int numPrimeImplicants = Terms.Count ();
            int numOriginalTerms = originalTermList.Count ();

            // table[i,j] is true only if implicant number i implies original term number j
            var table = new bool[numPrimeImplicants, numOriginalTerms];
            for (int impl=0; impl < numPrimeImplicants; impl++) {
                for (int term=0; term < numOriginalTerms; term++) {
                    table [impl, term] = Terms [impl].Implies (originalTermList [term]);
                }
            }

            // extract implicants heuristically until done
            List<Term> newTermList = new List<Term> ();
            var done = false;
            int impl2;
            while (!done) {
                impl2 = extractEssentialImplicant (table);
                if (impl2 != -1) {
                    newTermList.Add (Terms [impl2]);
                } else {
                    if (!heuristically) {

                        var allSolutions = new List<List<Term>> ();
                        EnumerateAllSolutions (table, newTermList, allSolutions);

                        // Best solution is the smallest one
                        newTermList = allSolutions.OrderBy (x => x.Count).First ();
                        done = true;
                    }

                    if (heuristically) {
                        impl2 = extractLargestImplicant (table);
                        if (impl2 != -1) {
                            newTermList.Add (Terms [impl2]);
                        } else {
                            done = true;
                        }
                    }
                }
            }
            Terms = newTermList;

            originalTermList = null;
        }

        private int extractEssentialImplicant (bool[,] table)
        {
            for (int term=0; term < table.GetLength (1); term++) {
                int lastImplFound = -1;
                for (int impl=0; impl < table.GetLength (0); impl++) {
                    if (table [impl, term]) {
                        if (lastImplFound == -1) {
                            lastImplFound = impl;
                        } else {
                            // This term has multiple implications
                            lastImplFound = -1;
                            break;
                        }
                    }
                }
                if (lastImplFound != -1) {
                    extractImplicant (table, lastImplFound);
                    return lastImplFound;
                }
            }
            return -1;
        }

        private void extractImplicant (bool[,] table, int impl)
        {
            for (int term=0; term < table.GetLength (1); term++) {
                if (table [impl, term]) {
                    for (int impl2=0; impl2 < table.GetLength (0); impl2++) {
                        table [impl2, term] = false;
                    }
                }
            }
        }

        private int extractLargestImplicant (bool[,] table)
        {
            int maxNumTerms = 0;
            int maxNumTermsImpl = -1;
            for (int impl=0; impl < table.GetLength (0); impl++) {
                int numTerms = 0;
                for (int term=0; term < table.GetLength (1); term++) {
                    if (table [impl, term]) {
                        numTerms++;
                    }
                }
                if (numTerms > maxNumTerms) {
                    maxNumTerms = numTerms;
                    maxNumTermsImpl = impl;
                }
            }
            if (maxNumTermsImpl != -1) {
                extractImplicant (table, maxNumTermsImpl);
                return maxNumTermsImpl;
            }
            return -1;
        }

        public LiteralFormula (IEnumerable<HashSet<ILiteral>> terms)
        {
            Terms = new List<Term> ();
            
            var alphabet = terms.SelectMany (x => x).Select (x => x is Negation ? ((ILiteral)((Negation)x).Enclosed) : x).Distinct ().ToList ();
            encoding = new Dictionary<ILiteral, int> ();
            int i = 0;
            foreach (var a in alphabet) {
                encoding.Add (a, i++);
            }
            
            foreach (var term in terms) {
                var array = new byte [alphabet.Count];
                foreach (var val in term) {
                    if (val is Proposition) {
                        array [encoding [val]] = Term.True;
                    } else if (val is Negation) {
                        var key = ((ILiteral)((Negation)val).Enclosed);
                        array [encoding [key]] = Term.False;
                    } else {
                        array [encoding [val]] = Term.DontCare;
                    }
                }
                Terms.Add (new Term (array));
            }
        }
        
        public HashSet<HashSet<ILiteral>> Simplify ()
        {
            ReduceToPrimeImplicants ();
            ReducePrimeImplicantsToSubset ();

            var ret = new HashSet<HashSet<ILiteral>> ();
            foreach (var term in Terms) {
                var s = new HashSet<ILiteral> ();
                for (int i = 0; i < term.Values.Length; i++) {
                    var key = encoding.Single (kv => kv.Value.Equals (i)).Key;
                    if (term.Values[i] == Term.True) {
                        s.Add (key);
                    } else if (term.Values[i] == Term.False) {
                        s.Add (new Negation (key));
                    }
                }
                ret.Add (s);
            }
            return ret;
        }

        public override string ToString ()
        {
            if (Terms.Count == 0)
                return "No terms, no variables\n";

            var result = "";
            result += Terms.Count + " terms, " + Terms [0].NumVars + " variables\n";
            for (int i=0; i<Terms.Count; i++) {
                result += Terms [i] + "\n";
            }
            return result;
        }
        
    }
}

