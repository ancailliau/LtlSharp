using System;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;

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
		
        public Stack<int> dfsStack1;
        public Stack<int> dfsStack2;
		
        public EmptinessChecker (BuchiAutomata automaton)
		{
			Automaton = automaton;
		}
		
		public bool Emptiness()
		{
            if (Automaton.AcceptanceSet.Length == 0)
                return false;
            
            
            foreach (var node in Automaton.Nodes.Where(n => n.Initial)) {
                dfsStack1 = new Stack<int> ();
                dfsStack2 = new Stack<int>();
                
				if (dfs1(node)) {
					return true;
                }
			}
            
			
			return false;
		}
        
        bool dfs1(BANode n)
		{
            dfsStack1.Push (n.Id);
            foreach (var succ in Automaton.Transitions[n.Id]) {
                if (!dfsStack1.Contains (succ.To)) {
                    if (dfs1 (Automaton.Nodes [succ.To])) {
                        return true;
                    }
                }
            }
            if (Automaton.AcceptanceSet.Contains (n.Id)) {
                if (dfs2 (n)) {
                    return true;
                }
            }
            return false;
		}
        
        bool dfs2(BANode n) {
            dfsStack2.Push(n.Id);
            foreach (var succ in Automaton.Transitions[n.Id]) {
                if (dfsStack1.Contains (succ.To)) {
                    return true;
					
                }
                if (!dfsStack2.Contains (succ.To)) {
                    if (dfs2 (Automaton.Nodes [succ.To])) {
                        return true;
                    }
                }
            }
			return false;
		}
		
	}
}

