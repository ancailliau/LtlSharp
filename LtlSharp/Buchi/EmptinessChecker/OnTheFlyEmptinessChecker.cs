using System;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;
using LtlSharp;
using LtlSharp.Buchi.Automata;

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
		
        Stack<int> dfsStack1;
        Stack<int> dfsStack2;
        
        public List<AutomataNode> counterexample_prefix;
        public List<AutomataNode> counterexample_loop;
        
        int offset;
		
        public OnTheFlyEmptinessChecker (BuchiAutomata ltlAutomata, BuchiAutomata lts)
		{
            LTLAutomata = ltlAutomata;
            LTS = lts;
            offset = Math.Max (LTLAutomata.Nodes.Length, LTS.Nodes.Length);
		}
		
		public bool Emptiness()
		{
            if (LTLAutomata.AcceptanceSet.Length == 0)
                return false;
            
            foreach (var node in LTLAutomata.Nodes.Where(n => n.Initial)) {
                foreach (var node2 in LTS.Nodes.Where (n => n.Initial)) {
                    dfsStack1 = new Stack<int> ();
                
                    if (dfs1 (node, node2)) {
                        return true;
                    }
                }
			}
            
			
			return false;
		}
        
        bool dfs1(AutomataNode n, AutomataNode n2)
		{
            dfsStack1.Push (n.Id + n2.Id * offset);
            
            foreach (var succ in LTLAutomata.Transitions[n.Id]) {
                foreach (var succ2 in LTS.Transitions[n2.Id]) {
                    if (succ.Labels.IsSubsetOf (succ2.Labels)) {
                        if (!dfsStack1.Contains (succ.To + succ2.To * offset)) {
                            if (dfs1 (LTLAutomata.Nodes [succ.To], LTS.Nodes [succ2.To])) {
                                return true;
                            }
                        }
                    }
                }
            }

            
            dfsStack2 = new Stack<int>();
            if (LTLAutomata.AcceptanceSet.Contains (n.Id)) {
                if (dfs2 (n, n2)) {
                    return true;
                }
            }
            
            dfsStack1.Pop ();
            
            return false;
		}
        
        bool dfs2(AutomataNode n, AutomataNode n2) {
            dfsStack2.Push(n.Id + n2.Id * offset);
            foreach (var succ in LTLAutomata.Transitions[n.Id]) {
                foreach (var succ2 in LTS.Transitions[n2.Id]) {
                    if (succ2.Labels.IsSubsetOf (succ.Labels)) {
                        if (dfsStack1.Contains (succ.To + succ2.To * offset)) {
                            dfsStack2.Push (succ.To + succ2.To * offset);
                            BuildCounterExample ();
                            return true;
        					
                        } else if (!dfsStack2.Contains (succ.To + succ2.To * offset)) {
                            if (dfs2 (LTLAutomata.Nodes [succ.To], LTS.Nodes[succ2.To])) {
                                return true;
                            }
                        }
                    }
                }
            }
            
			return false;
		}
        
        void BuildCounterExample ()
        {
            counterexample_prefix = new List<AutomataNode> ();
            counterexample_loop = new List<AutomataNode> ();
                        
            var last_pair = dfsStack2.Pop ();
            bool toggle = true;
            foreach (var n in dfsStack1.Reverse ()) {
                if (n == last_pair) {
                    toggle = !toggle;
                }
                if (toggle) {
                    counterexample_prefix.Add (LTS.Nodes [n / offset]);
                } else {
                    counterexample_loop.Add (LTS.Nodes [n / offset]);
                }
            }
            dfsStack2.Pop ();
            foreach (var n in dfsStack2) {
                counterexample_loop.Add (LTS.Nodes [n / offset]);
            }
        }
		
	}
}

