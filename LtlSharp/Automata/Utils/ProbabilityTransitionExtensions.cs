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
using LtlSharp.Automata.OmegaAutomata;
using LtlSharp.Automata.Transitions;
using LtlSharp.Automata.Nodes.Factories;


namespace LtlSharp.Automata.Utils
{
    public static class ProbabilityTransitionExtensions
    {

        /// <summary>
        /// Sets the probability for the transition between the source and the target.
        /// </summary>
        /// <remarks>The transition will be updated if it exists, created if not.</remarks>
        /// <param name="source">Source.</param>
        /// <param name="target">Target.</param>
        /// <param name="value">Value.</param>
        public static void SetProbability<T>(this Automata<T, ProbabilityDecoration> mc,
                                      T source,
                                      T target,
                                      double value)
            where T : IAutomatonNode
        {
            var t = mc.GetTransitions (target);
            foreach (var tr in t) {
                if (value == 0) {
                    mc.RemoveTransition (source, tr.Target, tr.Decoration);
                } else {
                    var v = new ProbabilityDecoration (value);
                    mc.ReplaceTransitionValue (source, tr.Target, tr.Decoration, v);
                }
            }

            if (!t.Any () & value > 0) {
                var v = new ProbabilityDecoration (value);
                mc.AddTransition (source, target, v);
            }
        }

        /// <summary>
        /// Gets the probability of taking the transition from source to target.
        /// </summary>
        /// <returns>The probability.</returns>
        /// <param name="source">Source.</param>
        /// <param name="target">Target.</param>
        public static double GetProbability<T>(this Automata<T, ProbabilityDecoration> mc,
                                        T source,
                                        T target)
            where T : IAutomatonNode
        {
            return mc.GetTransition (source, target)?.Probability ?? 0;
            //return graph.OutEdges (source).SingleOrDefault (x => x.Target.Equals (target))?.Value.Probability ?? 0;
        }


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
        public static IDictionary<T, double> Reachability<T>(this Automata<T, ProbabilityDecoration> mc,
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
        public static IDictionary<T, double> ConstrainedReachability<T>(this Automata<T, ProbabilityDecoration> mc,
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
            var A = new double [Stilde.Length, Stilde.Length];
            for (int i = 0; i < Stilde.Length; i++) {
                for (int j = 0; j < Stilde.Length; j++) {
                    var a = mc.GetProbability (Stilde [i], Stilde [j]);
                    if (iterative) {
                        A [i, j] = a;
                    } else {
                        A [i, j] = ((i == j) ? 1 : 0) - a;
                    }
                }
            }

            // Build b
            double [] b = new double [Stilde.Length];
            for (int i = 0; i < Stilde.Length; i++) {
                var s = Stilde [i];
                b [i] = B.Sum (u => mc.GetProbability (s, u));
            }

            var x = new double [Stilde.Length];

            if (iterative & n > 0) {
                // iteratively solve the system.
                int step = 0;
                var y = new double [Stilde.Length];
                var err = -1d;
                do {
                    err = -1;
                    // Compute Ax
                    alglib.rmatrixmv (Stilde.Length, Stilde.Length, A, 0, 0, 0, x, 0, ref y, 0);

                    for (int i = 0; i < Stilde.Length; i++) {
                        // Compute Ax + b
                        y [i] = y [i] + b [i];

                        // Compute the error as Max_S |x[s] - y[s]|
                        var dif = x [i] - y [i];
                        dif = (dif < 0) ? -dif : dif;
                        err = (err > dif) ? err : dif;
                    }

                    x = y;
                    step++;
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
                dict.Add (Stilde [i], x [i]);
            }
            return dict;
        }

        static IEnumerable<T> ComputeS0<T>(Automata<T, ProbabilityDecoration> mc,
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

        static IEnumerable<T> ComputeS1<T>(Automata<T, ProbabilityDecoration> mc,
                                    IEnumerable<T> C,
                                    IEnumerable<T> B) where T : IAutomatonNode
        {
            // For detailled discussion about the following algorithm, check "Principles of Model Checking", p767ff.
            var mcprime = (Automata<T, ProbabilityDecoration>)mc.Clone ();
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
        public static IEnumerable<T> GlobalAlmostSureReachability<T>(this Automata<T, ProbabilityDecoration> mc,
                                                              IEnumerable<T> B) where T : IAutomatonNode
        {
            // See "Principles of model checking", p 766ff.
            var mcprime = (Automata<T, ProbabilityDecoration>)mc.Clone ();
            foreach (var b in B) {
                mcprime.RemoveAllTransitions (b);
                mcprime.AddTransition (b, b, new ProbabilityDecoration (1));
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
        public static IEnumerable<T> QualitativeRepeatedReachability<T>(this Automata<T, ProbabilityDecoration> mc,
                                                                 IEnumerable<T> B) where T : IAutomatonNode
        {
            var bsccs = mc.GetBSCC ();
            return mc.Nodes.Where (n => bsccs.Where (bscc => mc.AllPre (bscc).Contains (n))
                           .All (bscc => bscc.Intersect (B).Any ()));
        }

        /// <summary>
        /// Returns the probability for repeated reachability of all nodes in B.
        /// </summary>
        /// <returns>The repeated reachability.</returns>
        /// <param name="mc">Mc.</param>
        /// <param name="B">B.</param>
        public static IDictionary<T, double> QuantitativeRepeatedReachability<T>(this Automata<T, ProbabilityDecoration> mc,
                                                                         IAcceptanceCondition<T> B) where T : IAutomatonNode
        {
            var U = mc.GetBSCC ().Where (bscc => B.Accept (bscc)).SelectMany (x => x);
            return mc.Reachability (U);
        }

        public static Dictionary<T, double> QuantitativeLinearProperty<T>(this Automata<T, ProbabilityDecoration> mc,
                                                                   ITLFormula formula) where T : IAutomatonNode
        {
            // For more details, see "Principles of Model Checking", p785ff

            Automata<ProductAutomatonNode<T, AutomatonNode>, ProbabilityDecoration> productMC;
            Dictionary<T, ProductAutomatonNode<T, AutomatonNode>> correspondingNodes;
            IDictionary<ProductAutomatonNode<T, AutomatonNode>, double> probabilities;
            Dictionary<T, double> dict;

            var buchi = (new Gia02 ()).GetAutomaton (formula);
            
            Console.WriteLine ("/*");
            Console.WriteLine (buchi.ToDot ());
            
            buchi.UnfoldTransitions ();
            
            Console.WriteLine (buchi.ToDot ());
            Console.WriteLine ("*/");
            
            // If buchi automaton is deterministic, no need for transforming to rabin automaton. 
            // This save a little computation.
            if (buchi.IsDeterministic (buchi.InitialNode)) {
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
        
        /// <summary>
        /// Returns the product automata of the specified Markov Chain and the specified Buchï Automata.
        /// </summary>
        /// <description>
        /// This methods is used to compute the product automata between a Markov Chain and a Buchï Automata.
        /// 
        /// A trace in the resulting automata is accepting if it contains at least a state in <c>condition</c>.
        /// 
        /// The mapping table <c>mappingProductToInitial</c> is used to identify the initial node when computing
        /// the probability to reach a state in <c>condition</c>. See 
        /// <see cref="LtlSharp.ProbabilisticSystems.MarkovChainsAlgorithms.QuantitativeLinearProperty"/> for an
        /// example of usage.
        /// </description>
        /// <param name="mc">Markov Chain.</param>
        /// <param name="ba">Buchï Automata.</param>
        /// <param name="initials">Node of the Markov Chain to start the product with.</param>
        /// <param name="condition">Condition to be satisfied by the product Markov Chain to accept a trace.</param>
        /// <param name="correspondingNodes">Mapping table <c>(x,y)</c> where <c>x</c> is the node in product 
        /// automata and <c>y</c> is the corresponding node in the specified Markov Chain for the initial state of 
        /// the Buchï Automata.</param>
        public static Automata<ProductAutomatonNode<T, AutomatonNode>, ProbabilityDecoration> Product<T> (
            this Automata<T, ProbabilityDecoration> mc,
            BuchiAutomaton<AutomatonNode> ba, 
            IEnumerable<T> initials,
            out IAcceptanceCondition<ProductAutomatonNode<T, AutomatonNode>> condition,
            out Dictionary<T, ProductAutomatonNode<T, AutomatonNode>> correspondingNodes)
            where T : IAutomatonNode
        {
            var product = mc.Product<T, AutomatonNode> (ba, initials, out correspondingNodes);

            condition = ba.AcceptanceCondition.Map<ProductAutomatonNode<T, AutomatonNode>> (
                x => product.Nodes.Where (t => t.Node2.Equals (x))
            );

            return product;
        }

        /// <summary>
        /// Returns the product automata of the specified Markov Chain and the specified Rabin Automata.
        /// </summary>
        /// <description>
        /// This methods is used to compute the product automata between a Markov Chain and a Rabin Automata.
        /// 
        /// A trace in the resulting automata is accepting if it there is a pair in <c>conditions</c> such that
        /// eventually first item is never met and second item is always enventually met. See "Principles of Model
        /// Checking", p790ff for a detailled discussion.
        /// 
        /// The mapping table <c>mappingProductToInitial</c> is used to identify the initial node when computing
        /// the probability to reach a state in <c>condition</c>. See 
        /// <see cref="LtlSharp.ProbabilisticSystems.MarkovChainsAlgorithms.QuantitativeLinearProperty"/> for an
        /// example of usage.
        /// </description>
        /// <param name="mc">Markov Chain.</param>
        /// <param name="rabin">Rabin Automata.</param>
        /// <param name="initials">Initials.</param>
        /// <param name="condition">Condition to be satisfied by the product Markov Chain to accept a trace.</param>
        /// <param name="correspondingNodes">Mapping table <c>(x,y)</c> where <c>x</c> is the node in product 
        /// automata and <c>y</c> is the corresponding node in the specified Markov Chain for the initial state of 
        /// the Rabin Automata.</param>
        public static Automata<ProductAutomatonNode<T, AutomatonNode>, ProbabilityDecoration> Product<T> (
            this Automata<T, ProbabilityDecoration> mc,
            RabinAutomaton<AutomatonNode> rabin, 
            IEnumerable<T> initials, 
            out IAcceptanceCondition<ProductAutomatonNode<T, AutomatonNode>> condition,
            out Dictionary<T, ProductAutomatonNode<T, AutomatonNode>> correspondingNodes)
            where T : IAutomatonNode
        {
            var product = mc.Product<T, AutomatonNode> (rabin, initials, out correspondingNodes);

            condition = rabin.AcceptanceCondition.Map<ProductAutomatonNode<T, AutomatonNode>> (x => {
                return product.Nodes.Where (t => t.Node2.Equals (x));
            });

            return product;
        }

        static Automata<ProductAutomatonNode<T1, T2>, ProbabilityDecoration> Product<T1, T2> (
            this Automata<T1, ProbabilityDecoration> automaton1,
            OmegaAutomaton<T2, LiteralSetDecoration> automaton2,
            IEnumerable<T1> initials,
            out Dictionary<T1, ProductAutomatonNode<T1,T2>> correspondingNodes)
            where T1 : IAutomatonNode
            where T2 : IAutomatonNode
        {
            // For more details about the product algorithm, see "Principles of Model Checking", p787ff

            var unique = new Dictionary<Tuple<T1,T2>, ProductAutomatonNode<T1,T2>> ();
            correspondingNodes = new Dictionary<T1, ProductAutomatonNode<T1,T2>> ();

            var _productFactory = new AutomatonNodeProductFactory<T1,T2> ();
            var product = new MarkovChain<ProductAutomatonNode<T1,T2>> (
                _productFactory
            );
            var pending = new Stack<ProductAutomatonNode<T1,T2>> ();
            var visited = new HashSet<ProductAutomatonNode<T1,T2>> ();

            var initWA = automaton2.InitialNode;
            IEnumerable<T2> successorsInWA;
            T2 successorInWA;
            ProductAutomatonNode<T1,T2> newNode;

            Console.WriteLine (automaton2.ToDot ());
            
            foreach (var initial in initials) {
                successorsInWA = automaton2.Post (initWA, initial.Labels);
                
                Console.WriteLine ("----");
                foreach (var s in automaton2.GetTransitions (initWA)) {
                    Console.WriteLine (s);
                }
                Console.WriteLine ("----");
                Console.WriteLine ("---- " + initWA + " -- " + initial.Labels);
                foreach (var s in successorsInWA) {
                    Console.WriteLine (s);
                }
                Console.WriteLine ("----");
                
                if (successorsInWA.Count () > 1)
                    throw new NotSupportedException ("Product between non-deterministic automaton is not supported.");

                if (successorsInWA.Any ()) {
                    successorInWA = successorsInWA.Single ();
                    newNode = _productFactory.Create (initial, successorInWA, initial.Labels);
                    product.AddNode (newNode);

                    var tuple = new Tuple<T1,T2> (initial, successorInWA);

                    unique.Add (tuple, newNode);
                    correspondingNodes.Add (initial, newNode);

                    pending.Push (newNode);
                }
            }

            while (pending.Count > 0) {
                var current = pending.Pop ();
                var currentNodeInMC = current.Node1;
                var currentNodeInWA = current.Node2;
                var currentNodeInPA = current;
                visited.Add (current);

                foreach (var successorInMC in automaton1.Post (currentNodeInMC)) {
                    successorsInWA = automaton2.Post (currentNodeInWA, successorInMC.Labels);
                    if (successorsInWA.Count () > 1)
                        throw new NotSupportedException ("Product between non-deterministic automaton is not " +
                                                         "supported.");

                    if (successorsInWA.Any ()) {
                        successorInWA = successorsInWA.Single ();
                        var tuple = new Tuple<T1,T2> (successorInMC, successorInWA);

                        if (!unique.ContainsKey (tuple)) {
                            newNode = _productFactory.Create (successorInMC, successorInWA, successorInMC.Labels);
                            product.AddNode (newNode);
                            unique.Add (tuple, newNode);

                        } else {
                            newNode = unique [tuple];
                        }

                        if (!pending.Contains (newNode) & !visited.Contains (newNode)) {
                            pending.Push (newNode);
                        }

                        product.AddTransition (currentNodeInPA, automaton1.GetProbability (currentNodeInMC, successorInMC), newNode);
                    }
                }
            }

            return product;
        }
    }
}

