using System;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;
using LtlSharp.Automata;
using LtlSharp.Automata.OmegaAutomata;

namespace LittleSharp.Buchi
{
	
	/// <summary>
	/// The emptiness checker.
	/// </summary>
    public class EmptinessChecker<T> where T : IAutomatonNode
	{
        public BuchiAutomaton<T> Automaton {
			get;
			private set;
		}
		
        public Stack<T> dfsStack1;
        public Stack<T> dfsStack2;
		
        public EmptinessChecker (BuchiAutomaton<T> automaton)
        {
            dfsStack1 = new Stack<T> ();
			Automaton = automaton;
		}
		
		public bool Emptiness()
		{
            if (!Automaton.AcceptanceCondition.IsSatisfiable) {
                return false;
            }
            
            dfsStack1 = new Stack<T> ();
            if (dfs1(Automaton.InitialNode)) {
				return true;
            }
            
			return false;
		}
        
        public bool Emptiness (T n)
        {
            dfsStack1 = new Stack<T> ();
            return dfs1 (n);
        }
        
        bool dfs1(T n)
		{
            dfsStack1.Push (n);
            foreach (var succ in Automaton.Post(n)) {
                if (!dfsStack1.Contains (succ)) {
                    if (dfs1 (succ)) {
                        return true;
                    }
                }
            }
            
            dfsStack2 = new Stack<T>();
            if (Automaton.AcceptanceCondition.Accept (n)) {
                if (dfs2 (n)) {
                    return true;
                }
            }
            
            dfsStack1.Pop ();
            
            return false;
		}
        
        bool dfs2(T n) {
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

