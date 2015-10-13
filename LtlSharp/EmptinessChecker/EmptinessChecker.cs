using System;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;
using LtlSharp.Buchi.Automata;
using LtlSharp.Automata;
using LtlSharp.Automata.OmegaAutomata;

namespace LittleSharp.Buchi
{
	
	/// <summary>
	/// The emptiness checker.
	/// </summary>
	public class EmptinessChecker
	{
        public BuchiAutomaton Automaton {
			get;
			private set;
		}
		
        public Stack<AutomatonNode> dfsStack1;
        public Stack<AutomatonNode> dfsStack2;
		
        public EmptinessChecker (BuchiAutomaton automaton)
        {
            dfsStack1 = new Stack<AutomatonNode> ();
			Automaton = automaton;
		}
		
		public bool Emptiness()
		{
            if (!Automaton.AcceptanceCondition.IsSatisfiable) {
                return false;
            }
            
            dfsStack1 = new Stack<AutomatonNode> ();
            if (dfs1(Automaton.InitialNode)) {
				return true;
            }
            
			return false;
		}
        
        public bool Emptiness (AutomatonNode n)
        {
            dfsStack1 = new Stack<AutomatonNode> ();
            return dfs1 (n);
        }
        
        bool dfs1(AutomatonNode n)
		{
            dfsStack1.Push (n);
            foreach (var succ in Automaton.Post(n)) {
                if (!dfsStack1.Contains (succ)) {
                    if (dfs1 (succ)) {
                        return true;
                    }
                }
            }
            
            dfsStack2 = new Stack<AutomatonNode>();
            if (Automaton.AcceptanceCondition.Accept (n)) {
                if (dfs2 (n)) {
                    return true;
                }
            }
            
            dfsStack1.Pop ();
            
            return false;
		}
        
        bool dfs2(AutomatonNode n) {
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

