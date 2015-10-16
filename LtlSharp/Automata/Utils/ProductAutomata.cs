using System;
using LtlSharp.Buchi;
using LtlSharp.Models;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Automata;
using QuickGraph;
using LtlSharp.Automata.AcceptanceConditions;
using LtlSharp.Automata.OmegaAutomata;
using LtlSharp.Automata.Nodes.Factories;
using LtlSharp.Automata.Transitions;
using LtlSharp.Automata.Transitions.Factories;
using LtlSharp.Automata.Utils;

namespace LtlSharp.Translators
{
    public static class ProductAutomata
    {
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
        public static Automata<ProductAutomatonNode<T, AutomatonNode>, ProbabilityTransitionDecorator> Product<T> (
            this Automata<T, ProbabilityTransitionDecorator> mc,
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
        public static Automata<ProductAutomatonNode<T, AutomatonNode>, ProbabilityTransitionDecorator> Product<T> (
            this Automata<T, ProbabilityTransitionDecorator> mc,
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
        
        static Automata<ProductAutomatonNode<T1, T2>, ProbabilityTransitionDecorator> Product<T1, T2> (
            this Automata<T1, ProbabilityTransitionDecorator> automaton1,
            OmegaAutomaton<T2, LiteralsSet> automaton2,
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
                _productFactory,
                new ProbabilityDecoratorFactory ()
            );
            var pending = new Stack<ProductAutomatonNode<T1,T2>> ();
            var visited = new HashSet<ProductAutomatonNode<T1,T2>> ();

            var initWA = automaton2.InitialNode;
            IEnumerable<T2> successorsInWA;
            T2 successorInWA;
            ProductAutomatonNode<T1,T2> newNode;
            
            foreach (var initial in initials) {
                successorsInWA = automaton2.Post (initWA, initial.Labels);
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

