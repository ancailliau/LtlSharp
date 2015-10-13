using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Buchi.Automata;
using QuickGraph;

namespace LtlSharp.Automata
{
    public interface IOmegaAutomaton 
    {
        IAcceptanceCondition<AutomataNode> AcceptanceCondition {
            get;
        }
    }
    
    public abstract class OmegaAutomaton : IOmegaAutomaton
    {
        protected AdjacencyGraph<AutomataNode, LabeledAutomataTransition<AutomataNode>> graph;
        
        public AutomataNode InitialNode { get; protected set; }

        public abstract IAcceptanceCondition<AutomataNode> AcceptanceCondition { get; }
        
        
        public IEnumerable<AutomataNode> Vertices { 
            get {
                return graph.Vertices;
            }
        }

        public IEnumerable<LabeledAutomataTransition<AutomataNode>> Edges { 
            get {
                return graph.Edges;
            }
        }

        internal void RemoveEdge (LabeledAutomataTransition<AutomataNode> e)
        {
            graph.RemoveEdge (e);
        }
        
        public OmegaAutomaton ()
        {
            graph = new AdjacencyGraph<AutomataNode, LabeledAutomataTransition<AutomataNode>> ();
        }

        /// <summary>
        /// Returns the set of literals that correspond to (at least) a transition from the specified node.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<HashSet<ILiteral>> OutAlphabet (AutomataNode node)
        {
            return graph.OutEdges (node).Select (e => e.Labels).Distinct ();
        }

        /// <summary>
        /// Returns the set of literals that correspond to (at least) a transition for (at least) a specified node.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<HashSet<ILiteral>> OutAlphabet (IEnumerable<AutomataNode> nodes)
        {
            return nodes.SelectMany (OutAlphabet).Distinct ();
        }

        /// <summary>
        /// Returns all the successor nodes of the specified node
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<AutomataNode> Post (AutomataNode node)
        {
            return graph.OutEdges (node).Select (e => e.Target);
        }

        /// <summary>
        /// Returns all the successor nodes of the specified nodes
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<AutomataNode> Post (IEnumerable<AutomataNode> nodes)
        {
            return nodes.SelectMany (node => Post(node));
        }

        /// <summary>
        /// Returns all the successor nodes of the specified node for the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<AutomataNode> Post (AutomataNode node, ISet<ILiteral> labels)
        {
            return graph.OutEdges (node).Where (e => e.Labels.IsSubsetOf (labels)).Select (e => e.Target);
        }

        /// <summary>
        /// Returns all the successor nodes of the specified nodes for the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<AutomataNode> Post (IEnumerable<AutomataNode> nodes, HashSet<ILiteral> labels)
        {
            return nodes.SelectMany (node => Post(node, labels));
        }

        /// <summary>
        /// Returns all the successor nodes of the specified node for all the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<AutomataNode> Post (AutomataNode node, IEnumerable<HashSet<ILiteral>> labels)
        {
            return labels.SelectMany (l => Post(node, l));
        }

        /// <summary>
        /// Returns all the successor nodes of the specified nodes for all the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<AutomataNode> Post (IEnumerable<AutomataNode> nodes, IEnumerable<HashSet<ILiteral>> labels)
        {
            return nodes.SelectMany (node => Post(node, labels));
        }
        
        
        public IEnumerable<LabeledAutomataTransition<AutomataNode>> OutTransitions (AutomataNode n)
        {
            return graph.OutEdges (n);
        }

        public void AddEdgeRange (IEnumerable<LabeledAutomataTransition<AutomataNode>> edges)
        {
            graph.AddEdgeRange (edges);
        }

        public void AddVertexRange (IEnumerable<AutomataNode> vertices)
        {
            graph.AddVertexRange (vertices);
        }

        public void AddVertex (AutomataNode bANode)
        {
            graph.AddVertex (bANode);
        }

        public void SetInitialNode (AutomataNode bANode)
        {
            InitialNode = bANode;
        }

        public void AddEdge (LabeledAutomataTransition<AutomataNode> t2)
        {
            graph.AddEdge (t2);
        }
        
        public bool IsDeterministic ()
        {
            var pending = new Stack<AutomataNode> (new [] { InitialNode });
            var visited = new HashSet<AutomataNode> ();

            while (pending.Count > 0) {
                var s0 = pending.Pop ();
                visited.Add (s0);

                var transitions = graph.OutEdges (s0);

                foreach (var c in transitions.Select (x => x.Labels)) {
                    // TODO Remove assumption that transitions were unfolded
                    var succ = transitions.Where (t => t.Labels.SetEquals (c)).Select (t => t.Target);
                    if (succ.Count () > 1) {
                        return false;
                    } else {
                        foreach (var s in succ.Where (node => !visited.Contains (node))) {
                            pending.Push (s);
                        }
                    }
                }
            }

            return true;
        }
        
    }
}

