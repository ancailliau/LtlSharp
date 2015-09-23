using System;
using System.Collections.Generic;
using LittleSharp.Buchi;
using LtlSharp.Buchi.Automata;

namespace LtlSharp.Buchi.Translators
{
    public static class BA2NFA
    {
        public static NFA Translate (BuchiAutomata automata)
        {
            var emptinessChecker = new EmptinessChecker (automata);
            
            var newAcceptanceSet = new List<int> ();
            foreach (var n in automata.Nodes) {
                if (emptinessChecker.Emptiness (n))
                    newAcceptanceSet.Add (n.Id);
            }

            var nfa = new NFA (0);
            nfa.Nodes = automata.Nodes;
            nfa.Transitions = automata.Transitions;
            nfa.AcceptanceSet = newAcceptanceSet.ToArray ();
            return nfa;
        }
    }
}

