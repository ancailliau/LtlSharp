using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using QuickGraph;

namespace LtlSharp.Buchi.Automata
{
    /// <summary>
    /// This class represents a non-deterministic finite automata.
    /// </summary>
    /// <description>
    /// See Andreas Bauer et al, Runtime Verification for LTL and TLTL, TOSEM.
    /// </description>
    public class NFA : AdjacencyGraph<AutomataNode, AutomataTransition>
    {
        public HashSet<AutomataNode> AcceptanceSet;

        public NFA ()
        {
            AcceptanceSet = new HashSet<AutomataNode> ();
        }
        
        public void ToSingleInitialState ()
        {
            if (Vertices.Count (a => a.Initial) == 1)
                return;
            
            var newInitialState = new AutomataNode (-1, "init", true);
            this.AddVertex (newInitialState);
            
            foreach (var initialState in Vertices.Where (a => a.Initial & a != newInitialState).ToList ()) {
                foreach (var otransition in OutEdges (initialState)) {
                    this.AddEdge (new AutomataTransition (newInitialState, otransition.Target, otransition.Labels));
                }
                initialState.Initial = false;
            }
        }
        
        public bool IsDeterministic ()
        {
            var pending = new Stack<AutomataNode> (Vertices.Where (x => x.Initial));
            var visited = new HashSet<AutomataNode> ();
            
            while (pending.Count > 0) {
                var s0 = pending.Pop ();
                visited.Add (s0);
                
                Console.WriteLine ("*" + s0);
                
                var transitions = OutEdges (s0);
                
                foreach (var c in transitions.SelectMany (x => x.Labels)) {
                    var succ = transitions.Where (t => t.Labels.Contains (c)).Select (t => t.Target);
                    if (succ.Count () > 1) {
                        return false;
                    } else {
                        foreach (var s in succ.Where (node => !visited.Contains (node))) {
                            pending.Push (s);
                        }
                    }
                }
            }

            return true;
        }
    }
}

