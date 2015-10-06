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
            double [] b = new double[Stilde.Length];
            for (int i = 0; i < Stilde.Length; i++) {
                for (int j = 0; j < Stilde.Length; j++) {
                    var a = mc.Edges.SingleOrDefault (e => 
                        e.Source.Equals (Stilde [i])
                            && e.Target.Equals (Stilde [j])
                            )?.Probability ?? 0;
                    A [i, j] =  ((i == j) ? 1 : 0) - a;
                }    
            }
            
            // Build b
            for (int i = 0; i < Stilde.Length; i++) {
                var s = Stilde [i];
                b [i] = B.Sum (u => mc.OutEdges (s)
                    .SingleOrDefault (e => e.Target.Equals (u))
                    ?.Probability ?? 0
                );
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
    }
}

