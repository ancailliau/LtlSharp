using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LtlSharp.Buchi.Automata;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace LtlSharp.Buchi
{
    public class BuchiAutomata : AdjacencyGraph<AutomataNode, LabeledAutomataTransition<AutomataNode>> 
    {
        public HashSet<AutomataNode> InitialNodes;
        public HashSet<AutomataNode> AcceptanceSet;
        
        public BuchiAutomata () : base ()
        {
            InitialNodes = new HashSet<AutomataNode> ();
            AcceptanceSet = new HashSet<AutomataNode> ();
        }
        
        /// <summary>
        /// Returns the set of literals that correspond to (at least) a transition from the specified node.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<HashSet<ILiteral>> OutAlphabet (AutomataNode node)
        {
            return OutEdges (node).Select (e => e.Labels).Distinct ();
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
            return OutEdges (node).Select (e => e.Target);
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
            return OutEdges (node).Where (e => e.Labels.SetEquals (labels)).Select (e => e.Target);
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
        
        
        public bool IsDeterministic ()
        {
            var pending = new Stack<AutomataNode> (InitialNodes);
            var visited = new HashSet<AutomataNode> ();

            while (pending.Count > 0) {
                var s0 = pending.Pop ();
                visited.Add (s0);

                var transitions = OutEdges (s0);

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
        

        public string ToDot ()
        {
            var graphviz = new GraphvizAlgorithm<AutomataNode, LabeledAutomataTransition<AutomataNode>> (this);
            graphviz.FormatVertex += (object sender, FormatVertexEventArgs<AutomataNode> e) => {
                e.VertexFormatter.Label = e.Vertex.Name;
                if (this.InitialNodes.Contains (e.Vertex))
                    e.VertexFormatter.Style = GraphvizVertexStyle.Bold;
                //                if (rabin.AcceptanceSet.Contains (e.Vertex))
                //                    e.VertexFormatter.Shape = QuickGraph.Graphviz.Dot.GraphvizVertexShape.DoubleCircle;
            };
            graphviz.FormatEdge += (object sender, FormatEdgeEventArgs<AutomataNode, LabeledAutomataTransition<AutomataNode>> e) => {
                e.EdgeFormatter.Label.Value = string.Join (",", e.Edge.Labels);
            };
            return graphviz.Generate ();
        }
        
    }
    
    public class DegeneralizerBuchiAutomata : AdjacencyGraph<AutomataNode, DegeneralizerAutomataTransition<AutomataNode>> 
    {
        public HashSet<AutomataNode> InitialNodes;
        public HashSet<AutomataNode> AcceptanceSet;

        public DegeneralizerBuchiAutomata () : base ()
        {
            InitialNodes = new HashSet<AutomataNode> ();
            AcceptanceSet = new HashSet<AutomataNode> ();
        }
    }
}

