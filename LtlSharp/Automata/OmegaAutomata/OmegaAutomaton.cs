using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Automata.AcceptanceConditions;
using LtlSharp.Buchi.Automata;
using QuickGraph;

namespace LtlSharp.Automata.OmegaAutomata
{
    public abstract class OmegaAutomaton
    {
        protected AdjacencyGraph<AutomatonNode, LabeledAutomataTransition<AutomatonNode>> graph;
        
        public AutomatonNode InitialNode { get; protected set; }

        public abstract IAcceptanceCondition<AutomatonNode> AcceptanceCondition { get; }
        
        public IEnumerable<AutomatonNode> Vertices { 
            get {
                return graph.Vertices;
            }
        }

        public IEnumerable<LabeledAutomataTransition<AutomatonNode>> Edges { 
            get {
                return graph.Edges;
            }
        }

        public OmegaAutomaton ()
        {
            graph = new AdjacencyGraph<AutomatonNode, LabeledAutomataTransition<AutomatonNode>> ();
        }

        /// <summary>
        /// Sets the initial node.
        /// </summary>
        /// <param name="node">Node.</param>
        public void SetInitialNode (AutomatonNode node)
        {
            InitialNode = node;
        }

        /// <summary>
        /// Returns the set of literals that correspond to (at least) a transition from the specified node.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<HashSet<ILiteral>> OutAlphabet (AutomatonNode node)
        {
            return graph.OutEdges (node).Select (e => e.Labels).Distinct ();
        }

        /// <summary>
        /// Returns the set of literals that correspond to (at least) a transition for (at least) a specified node.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<HashSet<ILiteral>> OutAlphabet (IEnumerable<AutomatonNode> nodes)
        {
            return nodes.SelectMany (OutAlphabet).Distinct ();
        }

        /// <summary>
        /// Returns all the successor nodes of the specified node
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<AutomatonNode> Post (AutomatonNode node)
        {
            return graph.OutEdges (node).Select (e => e.Target);
        }

        /// <summary>
        /// Returns all the successor nodes of the specified nodes
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<AutomatonNode> Post (IEnumerable<AutomatonNode> nodes)
        {
            return nodes.SelectMany (node => Post(node));
        }

        /// <summary>
        /// Returns all the successor nodes of the specified node for the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<AutomatonNode> Post (AutomatonNode node, ISet<ILiteral> labels)
        {
            return graph.OutEdges (node).Where (e => e.Labels.IsSubsetOf (labels)).Select (e => e.Target);
        }

        /// <summary>
        /// Returns all the successor nodes of the specified nodes for the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<AutomatonNode> Post (IEnumerable<AutomatonNode> nodes, HashSet<ILiteral> labels)
        {
            return nodes.SelectMany (node => Post(node, labels));
        }

        /// <summary>
        /// Returns all the successor nodes of the specified node for all the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<AutomatonNode> Post (AutomatonNode node, IEnumerable<HashSet<ILiteral>> labels)
        {
            return labels.SelectMany (l => Post(node, l));
        }

        /// <summary>
        /// Returns all the successor nodes of the specified nodes for all the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<AutomatonNode> Post (IEnumerable<AutomatonNode> nodes, IEnumerable<HashSet<ILiteral>> labels)
        {
            return nodes.SelectMany (node => Post(node, labels));
        }
        
        /// <summary>
        /// Returns all the outgoing transitions for the specified node.
        /// </summary>
        /// <returns>The transitions.</returns>
        /// <param name="node">Node.</param>
        public IEnumerable<LabeledAutomataTransition<AutomatonNode>> OutTransitions (AutomatonNode node)
        {
            return graph.OutEdges (node);
        }

        /// <summary>
        /// Adds the specified transition to the automaton.
        /// </summary>
        /// <param name="transition">Transition.</param>
        public void AddTransition (LabeledAutomataTransition<AutomatonNode> transition)
        {
            graph.AddEdge (transition);
        }

        /// <summary>
        /// Removes the transition.
        /// </summary>
        /// <param name="transition">Transition.</param>
        public void RemoveTransition (LabeledAutomataTransition<AutomatonNode> transition)
        {
            graph.RemoveEdge (transition);
        }

        /// <summary>
        /// Adds the transitions.
        /// </summary>
        /// <param name="transitions">Transitions.</param>
        public void AddTransitions (IEnumerable<LabeledAutomataTransition<AutomatonNode>> transitions)
        {
            graph.AddEdgeRange (transitions);
        }
        
        /// <summary>
        /// Adds the node.
        /// </summary>
        /// <param name="node">Node.</param>
        public void AddNode (AutomatonNode node)
        {
            graph.AddVertex (node);
        }

        /// <summary>
        /// Adds all the nodes.
        /// </summary>
        /// <param name="nodes">Nodes.</param>
        public void AddNodes (IEnumerable<AutomatonNode> nodes)
        {
            graph.AddVertexRange (nodes);
        }

        /// <summary>
        /// Returns whether the omega automaton is deterministic.
        /// </summary>
        /// <returns><c>True</c> if deterministic, <c>False</c> otherwise.</returns>
        public bool IsDeterministic ()
        {
            var pending = new Stack<AutomatonNode> (new [] { InitialNode });
            var visited = new HashSet<AutomatonNode> ();

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

