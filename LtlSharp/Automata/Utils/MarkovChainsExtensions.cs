using System;
using LtlSharp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using LtlSharp.Utils;
using LtlSharp.Automata;
using LtlSharp.Buchi.LTL2Buchi;
using System.Collections;
using LtlSharp.Automata.AcceptanceConditions;

namespace LtlSharp.Automata.Utils
{
    public static class MarkovChainsExtensions
    {
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
            // This method cannot be applied to all automaton with probability on their transitions as it requires
            // initial distribution. This is why the method is restricted to Markov Chains.
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
    }
}

