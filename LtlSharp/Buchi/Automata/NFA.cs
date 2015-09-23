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
        public HashSet<AutomataNode> InitialNodes;
        
        public NFA ()
        {
            AcceptanceSet = new HashSet<AutomataNode> ();
            InitialNodes = new HashSet<AutomataNode> ();
        }
        
        public void ToSingleInitialState ()
        {
            if (InitialNodes.Count == 1)
                return;
            
            var newInitialState = new AutomataNode ("init");
            this.AddVertex (newInitialState);
            
            foreach (var initialState in InitialNodes.ToList ()) {
                Console.WriteLine ("*>" + this.Edges.All (e => this.ContainsVertex(e.Target)));
                Console.WriteLine (initialState);
                Console.WriteLine (string.Join("\n", OutEdges(initialState)));
                foreach (var otransition in OutEdges (initialState)) {
                    var newTransition = new AutomataTransition (
                        newInitialState, 
                        otransition.Target, 
                        otransition.Labels
                    );
                    this.AddEdge (newTransition);
                }
                InitialNodes.Remove (initialState);
                Console.WriteLine ("*>" + this.Edges.All (e => this.ContainsVertex(e.Target)));
            }

            InitialNodes.Add (newInitialState);
            Console.WriteLine ("^^^^^^^^^^^^");
        }
        
        public bool IsDeterministic ()
        {
            var pending = new Stack<AutomataNode> (InitialNodes);
            var visited = new HashSet<AutomataNode> ();
            
            while (pending.Count > 0) {
                var s0 = pending.Pop ();
                visited.Add (s0);
                
                Console.WriteLine ("*" + s0);
                Console.WriteLine (string.Join("\n--", Edges));
                
                var transitions = OutEdges (s0);
                
                foreach (var c in transitions.SelectMany (x => x.Labels)) {
                    var succ = transitions.Where (t => t.Labels.Contains (c)).Select (t => t.Target);
                    if (succ.Count () > 1) {
                        return false;
                    } else {
                        foreach (var s in succ.Where (node => !visited.Contains (node))) {
                            Console.WriteLine (ContainsVertex (s));
                            pending.Push (s);
                        }
                    }
                }
            }

            return true;
        }
    }
}

