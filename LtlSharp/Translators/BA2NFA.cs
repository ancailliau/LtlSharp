using System;
using System.Collections.Generic;
using LittleSharp.Buchi;
using LtlSharp.Automata;
using LtlSharp.Automata.FiniteAutomata;
using LtlSharp.Automata.OmegaAutomata;
using LtlSharp.Automata.Nodes.Factories;


namespace LtlSharp.Buchi.Translators
{
    public static class BA2NFA
    {
        public static NFA<AutomatonNode> Transform (BuchiAutomaton<AutomatonNode> automaton)
        {
            var emptinessChecker = new EmptinessChecker<AutomatonNode> (automaton);

            var newAcceptanceSet = new HashSet<AutomatonNode> ();
            foreach (var n in automaton.Nodes) {
                if (emptinessChecker.Emptiness (n)) {
                    newAcceptanceSet.Add (n);
                }
            }

            var nfa = new NFA<AutomatonNode> (new AutomatonNodeFactory ());
            // TODO fixme
            //nfa.AddVertexRange (automaton.Vertices);
            //nfa.AddEdgeRange (automaton.Edges);
            //nfa.AcceptanceSet = newAcceptanceSet;
            //nfa.InitialNodes = new HashSet<AutomatonNode> (new [] { automaton.InitialNode });
            return nfa;
        }
    }
}

