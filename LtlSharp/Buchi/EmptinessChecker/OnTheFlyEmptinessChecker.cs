using System;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;
using LtlSharp;

namespace LittleSharp.Buchi
{
	
	/// <summary>
	/// The emptiness checker.
	/// </summary>
	public class OnTheFlyEmptinessChecker
    {
        public BuchiAutomata LTS {
            get;
            private set;
        }
        
        public BuchiAutomata LTLAutomata {
			get;
			private set;
		}
		
        public Stack<int> dfsStack1;
        public Stack<int> dfsStack2;
        
        public Stack<int> dfsStack11;
        public Stack<int> dfsStack12;
		
        public OnTheFlyEmptinessChecker (BuchiAutomata ltlAutomata, BuchiAutomata lts)
		{
            LTLAutomata = ltlAutomata;
            LTS = lts;
		}
		
		public bool Emptiness()
		{
            if (LTLAutomata.AcceptanceSet.Length == 0)
                return false;
            
            foreach (var node in LTLAutomata.Nodes.Where(n => n.Initial)) {
                foreach (var node2 in LTS.Nodes.Where (n => n.Initial)) {
                    Console.WriteLine ("*****");
                    dfsStack1 = new Stack<int> ();
                    dfsStack11 = new Stack<int> ();
                
                    if (dfs1 (node, node2)) {
                        return true;
                    }
                    Console.WriteLine ("*****");
                }
			}
            
			
			return false;
		}
        
        bool dfs1(BANode n, BANode n2)
		{
            Console.WriteLine ("dfs1 {0} {1}", n.Name, n2.Name);            
            
            Console.WriteLine (">> " + n2);
            foreach (var t in LTS.Transitions[n2.Id]) {
                Console.WriteLine (t);
            }
            Console.WriteLine ("<<");
            
            dfsStack1.Push (n.Id);
            dfsStack11.Push (n2.Id);
            foreach (var succ in LTLAutomata.Transitions[n.Id]) {
                foreach (var succ2 in LTS.Transitions[n2.Id]) {
                    if (succ.Labels.SetEquals (new [] { new True() }) |
                        succ.Labels.IsSubsetOf (succ2.Labels)) {
                        if (!dfsStack1.Contains (succ.To)) {
                            Console.WriteLine ("Transition from " + n2.Name + " to " + succ2);
                            if (dfs1 (LTLAutomata.Nodes [succ.To], LTS.Nodes [succ2.To])) {
                                return true;
                            }
                        }
                    } else {
                        Console.WriteLine ("no compatible transition found");
                        Console.WriteLine ("BA  " + string.Join (",", succ.Labels));
                        Console.WriteLine ("LTS " + string.Join (",", succ2.Labels));
                        Console.WriteLine (succ.Labels.SetEquals (new [] { new True() }));
                        Console.WriteLine (succ.Labels.IsSubsetOf (succ2.Labels));
                    }
                }
            }
            
            dfsStack2 = new Stack<int>();
            dfsStack12 = new Stack<int>();
            if (LTLAutomata.AcceptanceSet.Contains (n.Id)) {
                if (dfs2 (n, n2)) {
                    return true;
                }
            }
            
            dfsStack1.Pop ();
            
            return false;
		}
        
        bool dfs2(BANode n, BANode n2) {
            Console.WriteLine ("dfs2 {0} {1}", n.Name, n2.Name);
            
            dfsStack2.Push(n.Id);
            dfsStack12.Push(n.Id);
            foreach (var succ in LTLAutomata.Transitions[n.Id]) {
                foreach (var succ2 in LTS.Transitions[n2.Id]) {
                    if (succ.Labels.Contains (new True()) |
                        succ2.Labels.IsSubsetOf (succ.Labels)) {
                        if (dfsStack1.Contains (succ.To)) {
                            Console.WriteLine ("df2 : true (a)");
                            return true;
        					
                        } else if (!dfsStack2.Contains (succ.To)) {
                            if (dfs2 (LTLAutomata.Nodes [succ.To], LTS.Nodes[succ2.To])) {
                                Console.WriteLine ("df2 : true (b)");
                                return true;
                            }
                        }
                    }
                }
            }
            
            Console.WriteLine ("df2 : false");
			return false;
		}
		
	}
}

