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
                Console.WriteLine ("*****");
                dfsStack1 = new Stack<int> ();
                
				if (dfs1(node)) {
					return true;
                }
                Console.WriteLine ("*****");
			}
            
			
			return false;
		}
        
        bool dfs1(BANode n)
		{
            Console.WriteLine ("dfs1 n={0}", n);
            
            
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
        
        bool dfs2(BANode n) {
            Console.WriteLine ("dfs2 n={0}", n);
            
            dfsStack2.Push(n.Id);
            foreach (var succ in Automaton.Transitions[n.Id].Select (w => w.To)) {
                if (dfsStack1.Contains (succ)) {
                    Console.WriteLine ("df2 : true (a)");
                    return true;
					
                } else if (!dfsStack2.Contains (succ)) {
                    if (dfs2 (Automaton.Nodes [succ])) {
                        Console.WriteLine ("df2 : true (b)");
                        return true;
                    }
                }
            }
            
            Console.WriteLine ("df2 : false");
			return false;
		}
		
	}
}

