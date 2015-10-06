using System;
using LtlSharp.Models;
using System.Collections.Generic;
using System.Linq;

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
        public static IDictionary<MarkovNode, double> ReachabilityLinearSystem (this MarkovChain mc, IEnumerable<MarkovNode> B)
        {
            // nodes that can reach B
            var Stilde = B.SelectMany (v => mc.AllPre (v)).Except (B).Distinct ().ToArray ();
            
            // Build I - A
            double [,] A = new double[Stilde.Length, Stilde.Length];
            for (int i = 0; i < Stilde.Length; i++) {
                for (int j = 0; j < Stilde.Length; j++) {
                    var a = mc.GetEdge (Stilde[i], Stilde[j])?.Probability ?? 0;
                    A [i, j] =  ((i == j) ? 1 : 0) - a;
                }    
            }
            
            // Build b
            double [] b = new double[Stilde.Length];
            for (int i = 0; i < Stilde.Length; i++) {
                var s = Stilde [i];
                b [i] = B.Sum (u => mc.GetEdge (s, u)?.Probability ?? 0);
            }
            
            // Solve the system
            int info = 0;
            alglib.densesolver.densesolverreport report = new alglib.densesolver.densesolverreport ();
            double[] x = new double[Stilde.Length];
            alglib.densesolver.rmatrixsolve (A, Stilde.Length, b, ref info, report, ref x);
            
            if (info != 1) // no solution found. See ALGIB documentation for more details.
                return null;
            
            // Build a user-friendly dictionnary for storing the results.
            var dict = new Dictionary<MarkovNode, double> ();
            for (int i = 0; i < Stilde.Length; i++) {
                dict.Add (Stilde [i], x [i]);
            }
            
            return dict;
        }
        
        public static IDictionary<MarkovNode, double> UntilReachabilityIterative (this MarkovChain mc, 
            IEnumerable<MarkovNode> C, 
            IEnumerable<MarkovNode> B,
            double epsilon)
        {
            var S0 = ComputeS0 (mc, C, B);
            var S1 = ComputeS1 (mc, C, B);
            var Stilde = mc.ExceptNodes (S0.Union (S1)).ToArray ();
            
            // Build a
            var A = new double[Stilde.Length,Stilde.Length];
            for (int i = 0; i < Stilde.Length; i++) {
                for (int j = 0; j < Stilde.Length; j++) {
                    A [i, j] = mc.GetEdge (S [i], S [j])?.Probability ?? 0;
                }   
            }
            
            // Build b
            double [] b = new double[Stilde.Length];
            for (int i = 0; i < Stilde.Length; i++) {
                var s = Stilde [i];
                b [i] = B.Sum (u => mc.GetEdge (s, u)?.Probability ?? 0);
            }
            
            return new Dictionary<MarkovNode, double> ();
        }
        
        static IEnumerable<MarkovNode> ComputeS0 (MarkovChain mc, IEnumerable<MarkovNode> C, IEnumerable<MarkovNode> B)
        {
            var cSet = new HashSet<MarkovNode> (C);
            var nodes = new HashSet<MarkovNode> (B);
            var pending = new Stack<MarkovNode> (B);
            while (pending.Count > 0) {
                var current = pending.Pop ();
                foreach (var s in mc.Pre (current)) {
                    if (cSet.Contains (s) & !nodes.Contains (s)) {
                        pending.Push (s);
                        nodes.Add (s);
                    }
                }
            }
            
            return mc.ExceptNodes (nodes);
        }
        
        static IEnumerable<MarkovNode> ComputeS1 (MarkovChain mc, IEnumerable<MarkovNode> C, IEnumerable<MarkovNode> B)
        {
            // For detailled discussion about the following algorithm, check "Principles of Model Checking", p767ff.
            var mcprime = new MarkovChain (mc);
            var absorbing = B.Union (mcprime.ExceptNodes (C.Union (B))); // B U (S \ (B U C))
            foreach (var s in absorbing) {
                foreach (var t in mcprime.Nodes) {
                    var transition = mcprime.GetEdge (s, t);
                    if (transition == null) {
                        mcprime.AddEdge (s, 0, t);
                    }
                    transition.Probability = s == t ? 1 : 0;
                }
            }
            
            return GlobalAlmostSureReachability (mc, B);
        }
        
        /// <summary>
        /// Returns the nodes such that globals the almost sure reachability is guaranteed.
        /// </summary>
        /// <returns>The set of nodes with almost sure reachability.</returns>
        /// <param name="mc">Markov Chain.</param>
        /// <param name="B">B.</param>
        static IEnumerable<MarkovNode> GlobalAlmostSureReachability (MarkovChain mc, IEnumerable<MarkovNode> B)
        {
            // See "Principles of model checking", p 766ff.
            
            foreach (var b in B) {
                mc.ClearOutEdges (b);
                mc.AddEdge (b, b);
            }
            // S \ AllPre(S \ AllPre (B))
            return mc.ExceptNodes (mc.AllPre (mc.ExceptNodes (mc.AllPre (B))));
        }
    }
}

