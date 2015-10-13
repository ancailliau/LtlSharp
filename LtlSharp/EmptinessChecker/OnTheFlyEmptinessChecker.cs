using System;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;
using LtlSharp;
using LtlSharp.Buchi.Automata;
using LtlSharp.Automata;

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
		
        Stack<Tuple<AutomatonNode, AutomatonNode>> dfsStack1;
        Stack<Tuple<AutomatonNode, AutomatonNode>> dfsStack2;
        
        public List<AutomatonNode> counterexample_prefix;
        public List<AutomatonNode> counterexample_loop;
        		
        public OnTheFlyEmptinessChecker (BuchiAutomata ltlAutomata, BuchiAutomata lts)
		{
            LTLAutomata = ltlAutomata;
            LTS = lts;
		}
		
		public bool Emptiness()
		{
            if (!LTLAutomata.AcceptanceCondition.IsSatisfiable)
                return false;

            var node = LTLAutomata.InitialNode;
            var node2 = LTS.InitialNode;
            dfsStack1 = new Stack<Tuple<AutomatonNode, AutomatonNode>> ();
        
            if (dfs1 (node, node2)) {
                return true;
            }
            
			return false;
		}
        
        bool dfs1(AutomatonNode n, AutomatonNode n2)
		{
            dfsStack1.Push (new Tuple<AutomatonNode, AutomatonNode>(n, n2));
            
            foreach (var succ in LTLAutomata.OutTransitions (n)) {
                foreach (var succ2 in LTS.OutTransitions (n2)) {
                    if (succ.Labels.IsSubsetOf (succ2.Labels)) {
                        if (!dfsStack1.Contains (new Tuple<AutomatonNode,AutomatonNode> (succ.Target, succ2.Target))) {
                            if (dfs1 (succ.Target, succ2.Target)) {
                                return true;
                            }
                        }
                    }
                }
            }
            
            dfsStack2 = new Stack<Tuple<AutomatonNode, AutomatonNode>>();
            if (LTLAutomata.AcceptanceCondition.Accept (n)) {
                if (dfs2 (n, n2)) {
                    return true;
                }
            }
            
            dfsStack1.Pop ();
            
            return false;
		}
        
        bool dfs2(AutomatonNode n, AutomatonNode n2) {
            dfsStack2.Push(new Tuple<AutomatonNode, AutomatonNode> (n, n2));
            foreach (var succ in LTLAutomata.OutTransitions (n)) {
                foreach (var succ2 in LTS.OutTransitions (n2)) {
                    if (succ2.Labels.IsSubsetOf (succ.Labels)) {
                        var tuple = new Tuple<AutomatonNode, AutomatonNode> (succ.Target, succ2.Target);
                        if (dfsStack1.Contains (tuple)) {
                            dfsStack2.Push (tuple);
                            BuildCounterExample ();
                            return true;
        					
                        } else if (!dfsStack2.Contains (tuple)) {
                            if (dfs2 (succ.Target, succ2.Target)) {
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
            counterexample_prefix = new List<AutomatonNode> ();
            counterexample_loop = new List<AutomatonNode> ();
                        
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

