using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LtlSharp.Buchi.Automata
{
    /// <summary>
    /// This class represents a non-deterministic finite automata.
    /// </summary>
    /// <description>
    /// See Andreas Bauer et al, Runtime Verification for LTL and TLTL, TOSEM.
    /// </description>
    public class NFA
    {
        public IEnumerable<AutomataNode> Nodes {
            get {
                return Transitions.Keys;
            }
        }

        public Dictionary<AutomataNode, HashSet<AutomataTransition>> Transitions;

        public HashSet<AutomataNode> AcceptanceSet;

        public NFA (int n_nodes)
        {
            Transitions = new Dictionary<AutomataNode, HashSet<AutomataTransition>> ();
            AcceptanceSet = new HashSet<AutomataNode> ();
        }
    }
}

