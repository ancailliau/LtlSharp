using System;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;
using LtlSharp.Buchi.Automata;
using LtlSharp.Automata;

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
            if (!Automaton.AcceptanceCondition.IsSatisfiable) {
                return false;
            }
            
            dfsStack1 = new Stack<AutomataNode> ();
            if (dfs1(Automaton.InitialNode)) {
				return true;
            }
            
			return false;
		}
        
        public bool Emptiness (AutomataNode n)
        {
            dfsStack1 = new Stack<AutomataNode> ();
            return dfs1 (n);
        }
        
        bool dfs1(AutomataNode n)
		{
            dfsStack1.Push (n);
            foreach (var succ in Automaton.Post(n)) {
                if (!dfsStack1.Contains (succ)) {
                    if (dfs1 (succ)) {
                        return true;
                    }
                }
            }
            
            dfsStack2 = new Stack<AutomataNode>();
            if (Automaton.AcceptanceCondition.Accept (n)) {
                if (dfs2 (n)) {
                    return true;
                }
            }
            
            dfsStack1.Pop ();
            
            return false;
		}
        
        bool dfs2(AutomataNode n) {
            dfsStack2.Push(n);
            foreach (var succ in Automaton.Post(n)) {
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

