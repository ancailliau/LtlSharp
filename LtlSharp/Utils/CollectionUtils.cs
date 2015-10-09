using System;
using System.Collections.Generic;

namespace LtlSharp.Utils
{
    public static class CollectionUtils
    {
        public static int GetHashCodeForElements<T> (this HashSet<T> hashset)
        {
            // See http://stackoverflow.com/questions/670063/getting-hash-of-a-list-of-strings-regardless-of-order
            // for more details
            var hash = 0;
            int curHash = 0;
            int bitOffset = 0;
            // Stores number of occurences so far of each value.
            var valueCounts2 = new Dictionary<T, int> ();

            foreach (var element in hashset) {
                curHash = element.GetHashCode ();
                if (valueCounts2.TryGetValue (element, out bitOffset))
                    valueCounts2 [element] = bitOffset + 1;
                else
                    valueCounts2.Add (element, bitOffset);
                hash = hash + ((curHash << bitOffset) |
                    (curHash >> (32 - bitOffset))) * 37;
            }
            
            return hash;
        }
    }
}

