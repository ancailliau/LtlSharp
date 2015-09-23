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
            foreach (var n in automata.Nodes) {
                if (emptinessChecker.Emptiness (n))
                    newAcceptanceSet.Add (n);
            }

            var nfa = new NFA (0);
            nfa.Transitions = automata.Transitions;
            nfa.AcceptanceSet = newAcceptanceSet;
            return nfa;
        }
    }
}

