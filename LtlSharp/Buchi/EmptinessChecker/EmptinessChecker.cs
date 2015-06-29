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
		
        HashSet<int> dfsStack1;
        HashSet<int> dfsStack2;
		
        public EmptinessChecker (BuchiAutomata automaton)
		{
			Automaton = automaton;
		}
		
		public bool Emptiness()
		{
            if (Automaton.AcceptanceSet.Length == 0)
                return false;
            
            dfsStack1 = new HashSet<int> ();
            dfsStack2 = new HashSet<int>();
				
			foreach (var node in Automaton.Nodes.Where(n => n.Initial)) {
				if (dfs1(node)) {
					return true;
				}
			}
			
			return false;
		}
        
        bool dfs1(BANode n)
		{
            dfsStack1.Add (n.Id);
            foreach (var succ in Automaton.Transitions[n.Id]) {
                if (!dfsStack1.Contains (succ)) {
                    dfs1 (Automaton.Nodes[succ]);
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
            dfsStack2.Add(n.Id);
            foreach (var succ in Automaton.Transitions[n.Id]) {
                if (dfsStack1.Contains (succ)) {
                    return true;
					
                }
                if (!dfsStack2.Contains (succ)) {
                    if (dfs2 (Automaton.Nodes [succ])) {
                        return true;
                    }
                }
            }
			return false;
		}
		
	}
}

