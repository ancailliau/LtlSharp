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
        public static NFA<AutomatonNode> Transform (BuchiAutomaton<AutomatonNode> automata)
        {
            var emptinessChecker = new EmptinessChecker<AutomatonNode> (automata);

            var newAcceptanceSet = new HashSet<AutomatonNode> ();
            foreach (var n in automata.Vertices) {
                if (emptinessChecker.Emptiness (n)) {
                    newAcceptanceSet.Add (n);
                }
            }

            var nfa = new NFA<AutomatonNode> (new AutomatonNodeFactory ());
            // TODO fixme
            //nfa.AddVertexRange (automata.Vertices);
            //nfa.AddEdgeRange (automata.Edges);
            //nfa.AcceptanceSet = newAcceptanceSet;
            //nfa.InitialNodes = new HashSet<AutomatonNode> (new [] { automata.InitialNode });
            return nfa;
        }
    }
}

