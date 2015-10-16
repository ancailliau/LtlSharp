using System;
using LtlSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using LtlSharp.Utils;
using LtlSharp.Automata;
using LtlSharp.Buchi.LTL2Buchi;
using LtlSharp.Translators;
using System.Collections;
using LtlSharp.Automata.AcceptanceConditions;

namespace LtlSharp.ProbabilisticSystems
{
    public static class MarkovChainsAlgorithms
    {
        /// <summary>
        /// Returns the probability to reach a node of <c>B</c> for each node in <c>AllPre(B)</c>
        /// </summary>
        /// <description>
        /// The probability to reach a node in <c>B</c> is computed by resolving the corresponding linear system.
        /// Linear system is solved using ALGLIB <c>rmatrixsolve</c> function. For more details about the linear
        /// system, refers to "Baier, Christel, and Joost-Pieter Katoen. Principles of model checking. 
        /// Cambridge: MIT press, 2008".
        /// 
        /// Nodes not in the returned datastructure cannot reach a node B (because not in <c>AllPre(B)</c>), 
        /// and therefore have a reaching probability of 0.
        /// </description>
        /// <returns>The linear system.</returns>
        /// <param name="mc">Mc.</param>
        /// <param name="B">B.</param>
        /// <param name="iterative">If set to <c>true</c>, use iterative resolution. Exact resolution otherwise.</param>
        /// <param name="epsilon">Epsilon.</param>
        /// <param name="n">Number of steps, if iterative resolution. Set to <c>-1</c> to not stop after <c>n</c> 
        /// steps.</param>
        public static IDictionary<T, double> Reachability<T> (this MarkovChain<T> mc, 
                                                                       IEnumerable<T> B,
                                                                    bool iterative = false, 
                                                                    double epsilon = 1e-5,
                                                                       int n = -1) where T : IAutomatonNode
        {
            return mc.ConstrainedReachability (mc.Nodes, B, iterative, epsilon, n);
        }
        
        /// <summary>
        /// Returns the probability to reach a node in B, while only visiting nodes in C. This is the constrained
        /// reachability probability and corresponds to the probability to satisfy C U B. 
        /// </summary>
        /// <description>
        /// When using iterative resolution, the iteration is stop when the maximum difference between two solutions
        /// is bounded by <c>Epsilon</c> or if the <c>n</c> steps were performed.
        /// </description>
        /// <returns>The reachability probability.</returns>
        /// <param name="mc">Markov Chain.</param>
        /// <param name="C">C.</param>
        /// <param name="B">B.</param>
        /// <param name="iterative">If set to <c>true</c>, use iterative resolution. Exact resolution otherwise.</param>
        /// <param name="epsilon">Epsilon.</param>
        /// <param name="n">Number of steps, if iterative resolution. Set to <c>-1</c> to not stop after <c>n</c> 
        /// steps.</param>
        public static IDictionary<T, double> ConstrainedReachability<T> (this MarkovChain<T> mc, 
                                                                                  IEnumerable<T> C, 
                                                                                  IEnumerable<T> B,
                                                                               bool iterative = false, 
                                                                               double epsilon = 1e-5, 
                                                                                  int n = -1) where T : IAutomatonNode
        {
            var S0 = ComputeS0 (mc, C, B);
            var S1 = ComputeS1 (mc, C, B);
            var Stilde = mc.Nodes.Except (S0.Union (S1)).ToArray ();
            
            // Build I - A if not iterative, and A if iterative
            var A = new double[Stilde.Length,Stilde.Length];
            for (int i = 0; i < Stilde.Length; i++) {
                for (int j = 0; j < Stilde.Length; j++) {
                    var a = mc.GetProbability (Stilde[i], Stilde[j]);
                    if (iterative) {
                        A [i, j] = a;
                    } else {
                        A [i, j] = ((i == j) ? 1 : 0) - a;
                    }
                }
            }
            
            // Build b
            double [] b = new double[Stilde.Length];
            for (int i = 0; i < Stilde.Length; i++) {
                var s = Stilde [i];
                b [i] = B.Sum (u => mc.GetProbability (s, u));
            }
            
            var x = new double[Stilde.Length];
            
            if (iterative & n > 0) {
                // iteratively solve the system.
                int step = 0;
                var y = new double[Stilde.Length];
                var err = -1d;
                do {
                    err = -1;
                    // Compute Ax
                    alglib.rmatrixmv (Stilde.Length, Stilde.Length, A, 0, 0, 0, x, 0, ref y, 0);
                    
                    for (int i = 0; i < Stilde.Length; i++) {
                        // Compute Ax + b
                        y[i] = y[i] + b[i];
                        
                        // Compute the error as Max_S |x[s] - y[s]|
                        var dif = x [i] - y [i];
                        dif = (dif < 0) ? -dif : dif;
                        err = (err > dif) ? err : dif;
                    }
                    
                    x = y;
                    step ++;
                } while (err > epsilon | (n < 0 || step < n));
            } else {
                // solve the system by resolving the linear equations
                int info = 0;
                alglib.densesolver.densesolverreport report = new alglib.densesolver.densesolverreport ();
                alglib.densesolver.rmatrixsolve (A, Stilde.Length, b, ref info, report, ref x);
            }
            
            var dict = new Dictionary<T, double> ();
            foreach (var s in S1) {
                dict.Add (s, 1);
            }
            for (int i = 0; i < Stilde.Length; i++) {
                dict.Add (Stilde[i], x[i]);
            }
            return dict;
        }
        
        static IEnumerable<T> ComputeS0<T> (MarkovChain<T> mc, 
                                                  IEnumerable<T> C, 
                                                     IEnumerable<T> B) where T : IAutomatonNode
        {
            // For detailled discussion about the following algorithm, check "Principles of Model Checking", p767ff.
            
            var cSet = new HashSet<T> (C);
            var nodes = new HashSet<T> (B);
            var pending = new Stack<T> (B);
            
            while (pending.Count > 0) {
                var current = pending.Pop ();
                foreach (var s in mc.Pre (current)) {
                    if (cSet.Contains (s) & !nodes.Contains (s)) {
                        pending.Push (s);
                        nodes.Add (s);
                    }
                }
            }
            
            return mc.Nodes.Except (nodes);
        }
        
        static IEnumerable<T> ComputeS1<T> (MarkovChain<T> mc,
                                                  IEnumerable<T> C, 
                                                     IEnumerable<T> B) where T : IAutomatonNode
        {
            // For detailled discussion about the following algorithm, check "Principles of Model Checking", p767ff.
            var mcprime = new MarkovChain<T> (mc);
            var absorbing = B.Union (mcprime.Nodes.Except (C.Union (B))); // B U (S \ (B U C))
            foreach (var s in absorbing) {
                foreach (var t in mcprime.Nodes) {
                    mcprime.SetProbability (s, t, s.Equals (t) ? 1 : 0);
                }
            }
            
            // S \ AllPre(S \ AllPre (B))
            return mcprime.Nodes.Except (mcprime.AllPre (mcprime.Nodes.Except (mcprime.AllPre (B))));
        }
        
        /// <summary>
        /// Returns the nodes such that globals the almost sure reachability is guaranteed.
        /// </summary>
        /// <returns>The set of nodes with almost sure reachability.</returns>
        /// <param name="mc">Markov Chain.</param>
        /// <param name="B">B.</param>
        public static IEnumerable<T> GlobalAlmostSureReachability<T> (this MarkovChain<T> mc, 
                                                                      IEnumerable<T> B) where T : IAutomatonNode
        {
            // See "Principles of model checking", p 766ff.
            var mcprime = new MarkovChain<T> (mc);
            foreach (var b in B) {
                mcprime.RemoveAllTransitions (b);
                mcprime.AddTransition (b, b);
            }
            // S \ AllPre(S \ AllPre (B))
            return mcprime.Nodes.Except (mcprime.AllPre (mcprime.Nodes.Except (mcprime.AllPre (B))));
        }
        
        /// <summary>
        /// Returns the nodes satisfying G(F(B)), i.e. nodes that always eventually reach a node in B.
        /// </summary>
        /// <returns>The nodes repeatly reaching a node in B.</returns>
        /// <param name="mc">Markov Chain.</param>
        /// <param name="B">B.</param>
        public static IEnumerable<T> QualitativeRepeatedReachability<T> (this MarkovChain<T> mc, 
                                                                               IEnumerable<T> B) where T : IAutomatonNode
        {
            var bsccs = mc.GetBSCC ();
            return mc.Nodes.Where (n => bsccs.Where (bscc => mc.AllPre (bscc).Contains (n))
                                             .All   (bscc => bscc.Intersect (B).Any ()));
        }

        /// <summary>
        /// Returns the probability for repeated reachability of all nodes in B.
        /// </summary>
        /// <returns>The repeated reachability.</returns>
        /// <param name="mc">Mc.</param>
        /// <param name="B">B.</param>
        public static IDictionary<T, double> QuantitativeRepeatedReachability<T>(this MarkovChain<T> mc,
                                                                                 IAcceptanceCondition<T> B) where T : IAutomatonNode
        {
            var U = mc.GetBSCC ().Where (bscc => B.Accept(bscc)).SelectMany (x => x);
            return mc.Reachability (U);
        }

        /// <summary>
        /// Compute the transient probability from the initial states to reach a state in B.
        /// </summary>
        /// <returns>The reachability probability.</returns>
        /// <param name="mc">Markov Chain.</param>
        /// <param name="B">B.</param>
        /// <param name="n">Number of steps.</param>
        public static double TransientReachability<T> (this MarkovChain<T> mc,
                                                    IEnumerable<T> B,
                                                    int n) where T : IAutomatonNode
        {
            // F(B) is equivalent to true U B
            return TransientConstrainedReachability (mc, mc.Nodes, B, n);
        }
        
        /// <summary>
        /// Compute the transient constrained probability from the initial states to reach a state in B while only
        /// visiting nodes in C.
        /// </summary>
        /// <returns>The reachability probability.</returns>
        /// <param name="mc">Markov Chain.</param>
        /// <param name="C">C.</param>
        /// <param name="B">B.</param>
        /// <param name="n">Number of steps.</param>
        public static double TransientConstrainedReachability<T> (this MarkovChain<T> mc, 
                                                               IEnumerable<T> C,
                                                               IEnumerable<T> B,
                                                                  int n) where T : IAutomatonNode
        {
            if (mc.Initial.Count == 0) {
                Debug.Print ("No initial node. Transient probability will be zero.");
                return 0;
            }
            
            // To ensure that checking a node is absorbing is O(1)
            // and to avoid multiple enumeration of B and C.
            var absorbing = new HashSet<T> (B.Union (mc.Nodes.Except(C.Union (B))));
            
            // See "Principles of Model-Checking", p 758ff.
            var nodes = mc.Nodes.ToArray ();
            var len = nodes.Length;
            
            // Build A for nodes which have an initial value
            var A = new double[len,len];
            for (int i = 0; i < len; i++) {
                for (int j = 0; j < len; j++) {
                    if (!absorbing.Contains (nodes [i])) {
                        var a = mc.GetProbability (nodes [i], nodes [j]);
                        A [i, j] = a;
                        
                    } else if (i == j) {
                        A [i, j] = 1;
                    } else {
                        A [i, j] = 0;
                    }
                }
            }
            
            // Compute theta^M
            var theta = new double [len];
            for (int i = 0; i < len; i++) {
                theta [i] = mc.Initial.ContainsKey(nodes[i]) ? mc.Initial[nodes [i]].Probability : 0;
            }
            
            var y = new double [len];
            for (int i = 0; i < n; i++) {
                // Note that we need to multiply A^T and not A, hence the 1 in opA.
                alglib.rmatrixmv (len, len, A, 0, 0, 1, theta, 0, ref y, 0);
                Array.Copy (y, theta, len);
            }
            
            // Returns the computed probability
            return B.Sum (b => theta[Array.IndexOf (nodes, b)]);
        }
        
        public static Dictionary<T, double> QuantitativeLinearProperty<T> (this MarkovChain<T> mc, 
                                                                                 ITLFormula formula) where T : IAutomatonNode
        {
            // For more details, see "Principles of Model Checking", p785ff
            
            MarkovChain<ProductAutomatonNode<T, AutomatonNode>> productMC;
            Dictionary<T, ProductAutomatonNode<T, AutomatonNode>> correspondingNodes;
            IDictionary<ProductAutomatonNode<T, AutomatonNode>, double> probabilities;
            Dictionary<T, double> dict;
            
            var buchi = (new Gia02 ()).GetAutomaton (formula);
            buchi.UnfoldTransitions ();

            // If buchi automaton is deterministic, no need for transforming to rabin automaton. 
            // This save a little computation.
            if (buchi.IsDeterministic(buchi.InitialNode)) {
                IAcceptanceCondition<ProductAutomatonNode<T, AutomatonNode>> condition = null;
                productMC = mc.Product (buchi, mc.Nodes, out condition, out correspondingNodes);
                probabilities = productMC.QuantitativeRepeatedReachability (condition);
                
                dict = new Dictionary<T, double> ();
                foreach (var k in correspondingNodes.Values) {
                    dict.Add (k.Node1, probabilities [k]);
                }
                return dict;
            }

            var safra = new SafraDeterminization ();
            var rabin = safra.Transform (buchi);
            
            IAcceptanceCondition<ProductAutomatonNode<T, AutomatonNode>> conditions;
            productMC = mc.Product (rabin, mc.Nodes, out conditions, out correspondingNodes);
            probabilities = productMC.QuantitativeRepeatedReachability (conditions);
            
            dict = new Dictionary<T, double> ();
            foreach (var k in correspondingNodes.Values) {
                dict.Add (k.Node1, probabilities [k]);
            }
            
            return dict;
        }
    }
}

