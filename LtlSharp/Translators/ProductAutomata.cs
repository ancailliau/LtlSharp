﻿using System;
using LtlSharp.Buchi;
using LtlSharp.Models;
using System.Collections.Generic;
using LtlSharp.Buchi.Automata;
using System.Linq;
using LtlSharp.Automata;
using QuickGraph;

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
        public static MarkovChain<ProductMarkovNode<T>> Product<T> (this MarkovChain<T> mc, 
                                                                    BuchiAutomata ba, 
                                                                    IEnumerable<T> initials,
                                                                    out IAcceptanceCondition<ProductMarkovNode<T>> condition,
                                                                    out Dictionary<T, ProductMarkovNode<T>> correspondingNodes)
            where T : IMarkovNode
        {
            var product = mc.Product<T> (ba, initials, out correspondingNodes);
            
            condition = ba.AcceptanceCondition.Map<ProductMarkovNode<T>> (x => product.Nodes.Where (t => t.AutomataNode.Equals (x)));

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
        public static MarkovChain<ProductMarkovNode<T>> Product<T> (this MarkovChain<T> mc, 
                                                                    RabinAutomata rabin, 
                                                                    IEnumerable<T> initials, 
                                                                    out IAcceptanceCondition<ProductMarkovNode<T>> condition,
                                                                    out Dictionary<T, ProductMarkovNode<T>> correspondingNodes)
            where T : IMarkovNode
        {
            var product = mc.Product<T> (rabin, initials, out correspondingNodes);

            condition = rabin.AcceptanceCondition.Map<ProductMarkovNode<T>> (x => {
                return product.Nodes.Where (t => t.AutomataNode.Equals (x));
            });

            return product;
        }
        
        static MarkovChain<ProductMarkovNode<T>> Product<T> (this MarkovChain<T> mc,
                                                             OmegaAutomaton automaton,
                                                             IEnumerable<T> initials,
                                                             out Dictionary<T, ProductMarkovNode<T>> correspondingNodes)
            where T : IMarkovNode
        {
            // For more details about the product algorithm, see "Principles of Model Checking", p787ff

            var unique = new Dictionary<Tuple<T,AutomataNode>, ProductMarkovNode<T>> ();
            correspondingNodes = new Dictionary<T, ProductMarkovNode<T>> ();
            
            var product = new MarkovChain<ProductMarkovNode<T>> (new MarkovNodeProductFactory<T> ());
            var pending = new Stack<ProductMarkovNode<T>> ();
            var visited = new HashSet<ProductMarkovNode<T>> ();

            var initWA = automaton.InitialNode;
            IEnumerable<AutomataNode> successorsInWA;
            AutomataNode successorInWA;
            ProductMarkovNode<T> newNode;

            foreach (var initial in initials) {
                successorsInWA = automaton.Post (initWA, initial.Labels);
                
                if (successorsInWA.Count () > 1)
                    throw new NotSupportedException ("Product between a Markov Chain and a non-deterministic " +
                                                     "w-automaton is not supported.");
                
                if (successorsInWA.Any ()) {
                    successorInWA = successorsInWA.Single ();
                    newNode = product.AddVertex ();
                    newNode.SetNodes (initial, successorInWA);
                    
                    var tuple = new Tuple<T, AutomataNode> (initial, successorInWA);
                    
                    unique.Add (tuple, newNode);
                    correspondingNodes.Add (initial, newNode);
                    
                    pending.Push (newNode);
                }
            }
            
            while (pending.Count > 0) {
                var current = pending.Pop ();
                var currentNodeInMC = current.MarkovNode;
                var currentNodeInWA = current.AutomataNode;
                var currentNodeInPA = current;
                visited.Add (current);

                foreach (var successorInMC in mc.Post (currentNodeInMC)) {
                    successorsInWA = automaton.Post (currentNodeInWA, successorInMC.Labels);
                    if (successorsInWA.Count () > 1)
                        throw new NotSupportedException ("Product between a Markov Chain and a non-deterministic " +
                                                         "w-automaton is not supported.");
                        
                    if (successorsInWA.Any ()) {
                        successorInWA = successorsInWA.Single ();
                        var tuple = new Tuple<T, AutomataNode> (successorInMC, successorInWA);

                        if (!unique.ContainsKey (tuple)) {
                            newNode = product.AddVertex ();
                            newNode.SetNodes (successorInMC, successorInWA);
                            unique.Add (tuple, newNode);

                        } else {
                            newNode = unique [tuple];
                        }

                        if (!pending.Contains (newNode) & !visited.Contains (newNode)) {
                            pending.Push (newNode);
                        }
                        
                        product.AddEdge (currentNodeInPA, mc.GetProbability (currentNodeInMC, successorInMC), newNode);
                    }
                }
            }

            return product;
        }
    }
}

