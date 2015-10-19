using System;
using System.Collections.Generic;
using LittleSharp.Buchi;
using LtlSharp.Automata;
using LtlSharp.Automata.FiniteAutomata;
using LtlSharp.Automata.OmegaAutomata;
using LtlSharp.Automata.Nodes.Factories;


namespace LtlSharp.Automata.Utils
{
    public static class BA2NFA
    {
        public static NFA<T> ToNFA<T> (this BuchiAutomaton<T> automaton)
            where T : IAutomatonNode
        {
            var emptinessChecker = new EmptinessChecker<T> (automaton);

            var newAcceptanceSet = new HashSet<T> ();
            foreach (var n in automaton.Nodes) {
                if (emptinessChecker.Emptiness (n)) {
                    newAcceptanceSet.Add (n);
                }
            } 

            var nfa = new NFA<T> (automaton.GetNodeFactory ());

            nfa.AddNodes (automaton.Nodes);
            foreach (var t in automaton.Edges) {
                nfa.AddTransition (t.Source, t.Target, t.Decoration);
            }

            nfa.SetAcceptingNodes (newAcceptanceSet);
            nfa.SetInitialNode (automaton.InitialNode);
            
            return nfa;
        }
    }
}

