using System;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;
using LtlSharp.Buchi.Automata;

namespace LittleSharp.Buchi
{
	
	/// <summary>
	/// The emptiness checker.
	/// </summary>
	public class EmptinessChecker
	{
        public BuchiAutomata Automaton {
			get;
			private set;
		}
		
        public Stack<AutomataNode> dfsStack1;
        public Stack<AutomataNode> dfsStack2;
		
        public EmptinessChecker (BuchiAutomata automaton)
        {
            dfsStack1 = new Stack<AutomataNode> ();
			Automaton = automaton;
		}
		
		public bool Emptiness()
		{
            if (Automaton.AcceptanceSet.Count == 0)
                return false;
            
            foreach (var node in Automaton.Nodes.Where(n => n.Initial)) {
                dfsStack1 = new Stack<AutomataNode> ();
				if (dfs1(node)) {
					return true;
                }
			}
            
			
			return false;
		}
        
        public bool Emptiness (AutomataNode n)
        {
            return dfs1 (n);
        }
        
        bool dfs1(AutomataNode n)
		{
            Console.WriteLine ("Push DFS1 " + n);
            dfsStack1.Push (n);
            foreach (var succ in Automaton.Transitions[n]) {
                Console.WriteLine ("Contains " + succ.To + " in DFS1 ? " + dfsStack1.Contains (succ.To));
                if (!dfsStack1.Contains (succ.To)) {
                    if (dfs1 (succ.To)) {
                        return true;
                    }
                }
            }
            
            Console.WriteLine (n + " in " + string.Join (",", Automaton.AcceptanceSet.Select (x => x) ) + " - " + Automaton.AcceptanceSet.Contains (n));
            
            dfsStack2 = new Stack<AutomataNode>();
            if (Automaton.AcceptanceSet.Contains (n)) {
                if (dfs2 (n)) {
                    return true;
                }
            }
            
            dfsStack1.Pop ();
            
            return false;
		}
        
        bool dfs2(AutomataNode n) {
            Console.WriteLine ("Push DFS2 " + n);
            dfsStack2.Push(n);
            foreach (var succ in Automaton.Transitions[n].Select (w => w.To)) {
                Console.WriteLine ("Contains " + succ + " in DFS1 ? " + dfsStack1.Contains (succ));
                Console.WriteLine ("Contains " + succ + " in DFS2 ? " + dfsStack2.Contains (succ));
                if (dfsStack1.Contains (succ)) {
                    return true;
					
                } else if (!dfsStack2.Contains (succ)) {
                    if (dfs2 (succ)) {
                        return true;
                    }
                }
            }
			return false;
		}
		
	}
}

