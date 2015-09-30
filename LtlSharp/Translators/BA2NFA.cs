using System;
using System.Collections.Generic;
using LittleSharp.Buchi;
using LtlSharp.Buchi.Automata;

namespace LtlSharp.Buchi.Translators
{
    public static class BA2NFA
    {
        public static NFA Transform (BuchiAutomata automata)
        {
            var emptinessChecker = new EmptinessChecker (automata);
            
            var newAcceptanceSet = new HashSet<AutomataNode> ();
            foreach (var n in automata.Vertices) {
                if (emptinessChecker.Emptiness (n)) {
                    newAcceptanceSet.Add (n);
                }
            }

            var nfa = new NFA ();
            nfa.AddVertexRange (automata.Vertices);
            nfa.AddEdgeRange (automata.Edges);
            nfa.AcceptanceSet = newAcceptanceSet;
            nfa.InitialNodes = new HashSet<AutomataNode> (automata.InitialNodes);
            return nfa;
        }
    }
}

