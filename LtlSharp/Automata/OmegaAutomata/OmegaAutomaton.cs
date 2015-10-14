using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Automata.AcceptanceConditions;
using LtlSharp.Buchi.Automata;
using QuickGraph;

namespace LtlSharp.Automata.OmegaAutomata
{
    public abstract class OmegaAutomaton<T> where T : IAutomatonNode
    {
        protected AdjacencyGraph<T, AutomatonTransition<T>> graph;
        
        public T InitialNode { get; protected set; }

        public abstract IAcceptanceCondition<T> AcceptanceCondition { get; }
        
        public IEnumerable<T> Vertices { 
            get {
                return graph.Vertices;
            }
        }

        public IEnumerable<AutomatonTransition<T>> Edges { 
            get {
                return graph.Edges;
            }
        }

        public OmegaAutomaton ()
        {
            graph = new AdjacencyGraph<T, AutomatonTransition<T>> ();
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
        /// Returns the set of literals that correspond to (at least) a transition from the specified node.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<HashSet<ILiteral>> OutAlphabet (T node)
        {
            return graph.OutEdges (node).Select (e => e.Labels).Distinct ();
        }

        /// <summary>
        /// Returns the set of literals that correspond to (at least) a transition for (at least) a specified node.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<HashSet<ILiteral>> OutAlphabet (IEnumerable<T> nodes)
        {
            return nodes.SelectMany (OutAlphabet).Distinct ();
        }

        /// <summary>
        /// Returns all the successor nodes of the specified node
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<T> Post (T node)
        {
            return graph.OutEdges (node).Select (e => e.Target);
        }

        /// <summary>
        /// Returns all the successor nodes of the specified nodes
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<T> Post (IEnumerable<T> nodes)
        {
            return nodes.SelectMany (node => Post(node));
        }

        /// <summary>
        /// Returns all the successor nodes of the specified node for the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<T> Post (T node, ISet<ILiteral> labels)
        {
            return graph.OutEdges (node).Where (e => e.Labels.IsSubsetOf (labels)).Select (e => e.Target);
        }

        /// <summary>
        /// Returns all the successor nodes of the specified nodes for the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<T> Post (IEnumerable<T> nodes, HashSet<ILiteral> labels)
        {
            return nodes.SelectMany (node => Post(node, labels));
        }

        /// <summary>
        /// Returns all the successor nodes of the specified node for all the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<T> Post (T node, IEnumerable<HashSet<ILiteral>> labels)
        {
            return labels.SelectMany (l => Post(node, l));
        }

        /// <summary>
        /// Returns all the successor nodes of the specified nodes for all the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<T> Post (IEnumerable<T> nodes, IEnumerable<HashSet<ILiteral>> labels)
        {
            return nodes.SelectMany (node => Post(node, labels));
        }
        
        /// <summary>
        /// Returns all the outgoing transitions for the specified node.
        /// </summary>
        /// <returns>The transitions.</returns>
        /// <param name="node">Node.</param>
        public IEnumerable<AutomatonTransition<T>> OutTransitions (T node)
        {
            return graph.OutEdges (node);
        }

        /// <summary>
        /// Adds the specified transition to the automaton.
        /// </summary>
        /// <param name="transition">Transition.</param>
        public void AddTransition (AutomatonTransition<T> transition)
        {
            graph.AddEdge (transition);
        }

        /// <summary>
        /// Removes the transition.
        /// </summary>
        /// <param name="transition">Transition.</param>
        public void RemoveTransition (AutomatonTransition<T> transition)
        {
            graph.RemoveEdge (transition);
        }

        /// <summary>
        /// Adds the transitions.
        /// </summary>
        /// <param name="transitions">Transitions.</param>
        public void AddTransitions (IEnumerable<AutomatonTransition<T>> transitions)
        {
            graph.AddEdgeRange (transitions);
        }
        
        /// <summary>
        /// Adds the node.
        /// </summary>
        /// <param name="node">Node.</param>
        public void AddNode (T node)
        {
            graph.AddVertex (node);
        }

        /// <summary>
        /// Adds all the nodes.
        /// </summary>
        /// <param name="nodes">Nodes.</param>
        public void AddNodes (IEnumerable<T> nodes)
        {
            graph.AddVertexRange (nodes);
        }

        /// <summary>
        /// Returns whether the omega automaton is deterministic.
        /// </summary>
        /// <returns><c>True</c> if deterministic, <c>False</c> otherwise.</returns>
        public bool IsDeterministic ()
        {
            var pending = new Stack<T> (new [] { InitialNode });
            var visited = new HashSet<T> ();

            while (pending.Count > 0) {
                var s0 = pending.Pop ();
                visited.Add (s0);

                var transitions = graph.OutEdges (s0);
                foreach (var c in transitions.Select (x => x.Labels)) {
                    var succ = transitions.Where (t => c.IsSubsetOf(t.Labels)).Select (t => t.Target);
                    if (succ.Count () > 1)
                        return false;
                    
                    foreach (var s in succ.Except (visited)) {
                        pending.Push (s);
                    }
                }
            }

            return true;
        }
        
    }
}

