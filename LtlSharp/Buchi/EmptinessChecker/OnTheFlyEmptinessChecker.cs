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
		
        Stack<Tuple<AutomataNode, AutomataNode>> dfsStack1;
        Stack<Tuple<AutomataNode, AutomataNode>> dfsStack2;
        
        public List<AutomataNode> counterexample_prefix;
        public List<AutomataNode> counterexample_loop;
        
        int offset;
		
        public OnTheFlyEmptinessChecker (BuchiAutomata ltlAutomata, BuchiAutomata lts)
		{
            LTLAutomata = ltlAutomata;
            LTS = lts;
            offset = Math.Max (LTLAutomata.Nodes.Count(), LTS.Nodes.Count());
		}
		
		public bool Emptiness()
		{
            if (LTLAutomata.AcceptanceSet.Count == 0)
                return false;
            
            foreach (var node in LTLAutomata.Nodes.Where(n => n.Initial)) {
                foreach (var node2 in LTS.Nodes.Where (n => n.Initial)) {
                    dfsStack1 = new Stack<Tuple<AutomataNode, AutomataNode>> ();
                
                    if (dfs1 (node, node2)) {
                        return true;
                    }
                }
			}
            
			
			return false;
		}
        
        bool dfs1(AutomataNode n, AutomataNode n2)
		{
            dfsStack1.Push (new Tuple<AutomataNode, AutomataNode>(n, n2));
            
            foreach (var succ in LTLAutomata.Transitions[n]) {
                foreach (var succ2 in LTS.Transitions[n2]) {
                    if (succ.Labels.IsSubsetOf (succ2.Labels)) {
                        if (!dfsStack1.Contains (new Tuple<AutomataNode,AutomataNode> (succ.To, succ2.To))) {
                            if (dfs1 (succ.To, succ2.To)) {
                                return true;
                            }
                        }
                    }
                }
            }

            
            dfsStack2 = new Stack<Tuple<AutomataNode, AutomataNode>>();
            if (LTLAutomata.AcceptanceSet.Contains (n)) {
                if (dfs2 (n, n2)) {
                    return true;
                }
            }
            
            dfsStack1.Pop ();
            
            return false;
		}
        
        bool dfs2(AutomataNode n, AutomataNode n2) {
            dfsStack2.Push(new Tuple<AutomataNode, AutomataNode> (n, n2));
            foreach (var succ in LTLAutomata.Transitions[n]) {
                foreach (var succ2 in LTS.Transitions[n2]) {
                    if (succ2.Labels.IsSubsetOf (succ.Labels)) {
                        var tuple = new Tuple<AutomataNode, AutomataNode> (succ.To, succ2.To);
                        if (dfsStack1.Contains (tuple)) {
                            dfsStack2.Push (tuple);
                            BuildCounterExample ();
                            return true;
        					
                        } else if (!dfsStack2.Contains (tuple)) {
                            if (dfs2 (succ.To, succ2.To)) {
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
                    counterexample_prefix.Add (n.Item1);
                } else {
                    counterexample_loop.Add (n.Item1);
                }
            }
            dfsStack2.Pop ();
            foreach (var n in dfsStack2) {
                counterexample_loop.Add (n.Item1);
            }
        }
		
	}
}

