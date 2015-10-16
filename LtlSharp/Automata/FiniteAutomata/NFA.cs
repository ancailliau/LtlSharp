using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LtlSharp.Language;
using QuickGraph;
using LtlSharp.Monitoring;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using LtlSharp.Automata;
using LtlSharp.Automata.Transitions;
using LtlSharp.Utils;
using LtlSharp.Automata.Nodes.Factories;
using LtlSharp.Automata.Transitions.Factories;
using LtlSharp.Automata.Utils;

namespace LtlSharp.Automata.FiniteAutomata
{
    /// <summary>
    /// Defines a generic non-deterministic finite automata with node of type <c>T</c>.
    /// </summary>
    /// <remarks>
    /// The implementation was modelling LTL monitors. See "Andreas Bauer et al, Runtime Verification for LTL and TLTL,
    /// TOSEM" for more details.
    /// </remarks>
    /// <typeparam name="T">Type of nodes</typeparam>
    public class NFA<T> 
        : Automata<T, LiteralSetDecoration>
        where T : IAutomatonNode
    {   
        

        public T InitialNode { get; protected set; }
        
        /// <summary>
        /// Gets the set of node accepting the finite word.
        /// </summary>
        /// <value>The set of accepting nodes.</value>
        public HashSet<T> AcceptingNodes {
            get;
            private set;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="T:LtlSharp.Automata.FiniteAutomata.NFA`1"/> class with the
        /// specified factory and an empty set of accepting nodes.
        /// </summary>
        /// <param name="factory">Factory.</param>
        public NFA (IAutomatonNodeFactory<T> factory, IAutomatonTransitionFactory<LiteralSetDecoration> factoryTransition) 
            : base (factory, factoryTransition)
        {
            AcceptingNodes = new HashSet<T> ();
        }

        /// <summary>
        /// Sets the initial node.
        /// </summary>
        /// <param name="node">Node.</param>
        public void SetInitialNode (T node)
        {
            InitialNode = node;
        }
        
        /// <summary>
        /// Determinize the automaton using the power set construction.
        /// </summary>
        public NFA<PowerSetAutomatonNode<T>> Determinize ()
        {
            this.UnfoldTransitions ();

            var factory = new PowerSetAutomatonNodeFactory<T> ();
            var factoryT = new LiteralSetDecorationFactory ();
            
            var deterministicAutomaton = new NFA<PowerSetAutomatonNode<T>> (factory, factoryT);

            var initialPowerSet = new [] { InitialNode };

            var mapping = new Dictionary<HashSet<T>, PowerSetAutomatonNode<T>> (HashSet<T>.CreateSetComparer ());
            var node = factory.Create (initialPowerSet);
            mapping.Add (new HashSet<T> (initialPowerSet), node);
            
            deterministicAutomaton.AddNode (node);
            deterministicAutomaton.SetInitialNode (node);
            
            if (AcceptingNodes.Contains (InitialNode)) {
                deterministicAutomaton.AcceptingNodes.Add (node);
            }

            var pending = new Stack<PowerSetAutomatonNode<T>> ();
            pending.Push (node);
            
            var visited = new HashSet<PowerSetAutomatonNode<T>> ();
            
            while (pending.Count > 0) {
                var current = pending.Pop ();
                visited.Add (current);
                
                var currentPowerSet = current.Nodes;
                var outLabels = GetOutDecorations (currentPowerSet);
                
                foreach (var label in outLabels) {
                    var successorPowerSet = new HashSet<T> ();
                    foreach (var state in currentPowerSet) {
                        foreach (var successor in Post (state, label)) {
                            successorPowerSet.Add (successor);
                        }
                    }
                    
                    node = factory.Create (successorPowerSet);
                    if (!visited.Contains (node)) {
                        deterministicAutomaton.AddNode (node);
                        
                        if (successorPowerSet.Any (succ => AcceptingNodes.Contains (succ))) {
                            deterministicAutomaton.AcceptingNodes.Add (node);
                        }
                    }
                    
                    if (!visited.Contains (node) & !pending.Contains (node)) {
                        pending.Push (node);
                    }

                    deterministicAutomaton.AddTransition (current, node, label);
                }   
            }
            
            deterministicAutomaton.SimplifyTransitions ();
            
            return deterministicAutomaton;
        }

        public override Automata<T, LiteralSetDecoration> Clone ()
        {
            throw new NotImplementedException ();
        }
    }
}

