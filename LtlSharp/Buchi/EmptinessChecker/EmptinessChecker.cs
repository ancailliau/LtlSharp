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
		
        public Stack<int> dfsStack1;
        public Stack<int> dfsStack2;
		
        public EmptinessChecker (BuchiAutomata automaton)
        {
            dfsStack1 = new Stack<int> ();
			Automaton = automaton;
		}
		
		public bool Emptiness()
		{
            if (Automaton.AcceptanceSet.Length == 0)
                return false;
            
            foreach (var node in Automaton.Nodes.Where(n => n.Initial)) {
                Console.WriteLine ("*****");
                dfsStack1 = new Stack<int> ();
                
				if (dfs1(node)) {
					return true;
                }
                Console.WriteLine ("*****");
			}
            
			
			return false;
		}
        
        public bool Emptiness (AutomataNode n)
        {
            return dfs1 (n);
        }
        
        bool dfs1(AutomataNode n)
		{
            dfsStack1.Push (n.Id);
            foreach (var succ in Automaton.Transitions[n.Id]) {
                if (!dfsStack1.Contains (succ.To)) {
                    if (dfs1 (Automaton.Nodes [succ.To])) {
                        return true;
                    }
                }
            }
            
            dfsStack2 = new Stack<int>();
            if (Automaton.AcceptanceSet.Contains (n.Id)) {
                if (dfs2 (n)) {
                    return true;
                }
            }
            
            dfsStack1.Pop ();
            
            return false;
		}
        
        bool dfs2(AutomataNode n) {
            dfsStack2.Push(n.Id);
            foreach (var succ in Automaton.Transitions[n.Id].Select (w => w.To)) {
                if (dfsStack1.Contains (succ)) {
                    return true;
					
                } else if (!dfsStack2.Contains (succ)) {
                    if (dfs2 (Automaton.Nodes [succ])) {
                        return true;
                    }
                }
            }
			return false;
		}
		
	}
}

