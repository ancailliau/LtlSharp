using System;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;
using LtlSharp;
using LtlSharp.Automata;
using LtlSharp.Automata.OmegaAutomata;

namespace LittleSharp.Buchi
{
	
	/// <summary>
	/// The emptiness checker.
	/// </summary>
    public class OnTheFlyEmptinessChecker<T1,T2> 
        where T1 : IAutomatonNode 
        where T2 : IAutomatonNode
    {
        public BuchiAutomaton<T1> LTS {
            get;
            private set;
        }
        
        public BuchiAutomaton<T2> LTLAutomata {
			get;
			private set;
		}
		
        Stack<Tuple<T2, T1>> dfsStack1;
        Stack<Tuple<T2, T1>> dfsStack2;
        
        public List<T2> counterexample_prefix;
        public List<T2> counterexample_loop;
        		
        public OnTheFlyEmptinessChecker (BuchiAutomaton<T2> ltlAutomata, BuchiAutomaton<T1> lts)
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
            dfsStack1 = new Stack<Tuple<T2, T1>> ();
        
            if (dfs1 (node, node2)) {
                return true;
            }
            
			return false;
		}
        
        bool dfs1(T2 n, T1 n2)
		{
            dfsStack1.Push (new Tuple<T2, T1>(n, n2));
            
            foreach (var succ in LTLAutomata.Post (n)) {
                foreach (var succ2 in LTS.Post (n2, succ.Labels)) {
                    //if (succ.Labels.IsSubsetOf (succ2.Labels)) {
                        if (!dfsStack1.Contains (new Tuple<T2,T1> (succ, succ2))) {
                            if (dfs1 (succ, succ2)) {
                                return true;
                            }
                        }
                    //}
                }
            }
            
            dfsStack2 = new Stack<Tuple<T2, T1>>();
            if (LTLAutomata.AcceptanceCondition.Accept (n)) {
                if (dfs2 (n, n2)) {
                    return true;
                }
            }
            
            dfsStack1.Pop ();
            
            return false;
		}
        
        bool dfs2(T2 n, T1 n2) {
            dfsStack2.Push(new Tuple<T2, T1> (n, n2));
            foreach (var succ in LTLAutomata.Post (n)) {
                foreach (var succ2 in LTS.Post (n2, succ.Labels)) {
                    //if (succ2.Labels.IsSubsetOf (succ.Labels)) {
                        var tuple = new Tuple<T2, T1> (succ, succ2);
                        if (dfsStack1.Contains (tuple)) {
                            dfsStack2.Push (tuple);
                            BuildCounterExample ();
                            return true;
        					
                        } else if (!dfsStack2.Contains (tuple)) {
                            if (dfs2 (succ, succ2)) {
                                return true;
                            }
                        }
                    //}
                }
            }
            
			return false;
		}
        
        void BuildCounterExample ()
        {
            counterexample_prefix = new List<T2> ();
            counterexample_loop = new List<T2> ();
                        
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

