using System;
using QuickGraph;
using System.Collections.Generic;
using System.Linq;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using LtlSharp.Buchi.Automata;
using LtlSharp.Automata;
using LtlSharp.Automata.Nodes.Factories;

namespace LtlSharp.Models
{
    
    /// <summary>
    /// Represents a Markov Chain.
    /// </summary>
    public class MarkovChain<T> where T : IAutomatonNode
    {
        AdjacencyGraph<int, MarkovTransition> graph;

        Dictionary<int, T> nodes;

        IAutomatonNodeFactory<T> factory;

        public IEnumerable<T> Nodes {
            get { return nodes.Values; }
        }

        public int NodesCount {
            get { return nodes.Count; }
        }

        /// <summary>
        /// Gets the initial distribution of the nodes. If a node is not contained, it is assumed that its
        /// initial probability is 0.
        /// </summary>
        /// <value>The initial distribution .</value>
        public Dictionary<T, double> Initial {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LtlSharp.Models.MarkovChain"/> class.
        /// </summary>
        public MarkovChain (IAutomatonNodeFactory<T> factory)
        {
            graph = new AdjacencyGraph<int, MarkovTransition> (false);
            nodes = new Dictionary<int, T> ();
            Initial = new Dictionary<T, double> ();
            this.factory = factory;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LtlSharp.Models.MarkovChain"/> class. The nodes are shared
        /// but the underlying graph and initial distribution is copied.
        /// </summary>
        /// <param name="mc">Markov chain to copy.</param>
        public MarkovChain (MarkovChain<T> mc)
        {
            nodes = mc.nodes;
            graph = new AdjacencyGraph<int, MarkovTransition> ();
            foreach (var vertex in mc.graph.Vertices) {
                graph.AddVertex (vertex);
            }
            foreach (var edge in mc.graph.Edges) {
                graph.AddEdge (new MarkovTransition (edge.Source, edge.Probability, edge.Target));
            }
            Initial = new Dictionary<T, double> ();
            foreach (var i in mc.Initial) {
                Initial.Add (i.Key, i.Value);
            }
            factory = mc.factory;
        }

        /// <summary>
        /// Sets the initial probability for the specified Markov node to <c>p</c>.
        /// </summary>
        /// <param name="v">The node.</param>
        /// <param name="p">The probability.</param>
        public void SetInitial (T v, Double p)
        {
            if (!Initial.ContainsKey (v)) {
                Initial.Add (v, p);
            }
            Initial [v] = p;
        }

        /// <summary>
        /// Checks if the sum of probability distributions for each node equals to one.
        /// </summary>
        /// <returns><c>true</c>, if probability distributions are valid, <c>false</c> otherwise.</returns>
        public bool CheckProbabilityDistributions ()
        {
            return Initial.Values.Sum () == 1d &&
                graph.Vertices.All (v => graph.OutEdges (v).Sum (x => x.Probability) == 1d);
        }

        /// <summary>
        /// Gets the vertex with the specified name. Assume that the name uniquely identify the node.
        /// </summary>
        /// <returns>The vertex.</returns>
        /// <param name="name">Name.</param>
        public T GetVertex (string name)
        {
            return nodes.Values.Single (v => v.Name == name);
        }
        
        /// <summary>
        /// Adds a new vertex to the Markov chain.
        /// </summary>
        /// <returns>The vertex.</returns>
        public T AddVertex ()
        {
            var v = factory.Create ();
            if (graph.AddVertex (v.Id)) {
                nodes.Add (v.Id, v);
                return v;
            }

            return default (T);
        }
        
        /// <summary>
        /// Adds a new vertex with the specified name to the Markov chain.
        /// </summary>
        /// <returns>The vertex.</returns>
        /// <param name="name">Name.</param>
        public T AddVertex (string name)
        {
            var v = factory.Create (name);
            if (graph.AddVertex (v.Id)) {
                nodes.Add (v.Id, v);
                return v;
            }

            return default (T);
        }

        /// <summary>
        /// Adds a new edge with the specified source, target and a probability 1 to the Markov chain.
        /// </summary>
        /// <returns>The edge.</returns>
        /// <param name="source">Source node.</param>
        /// <param name="target">Target node.</param>
        public MarkovTransition AddEdge (T source, T target)
        {
            return AddEdge (source, 1, target);
        }

        /// <summary>
        /// Adds a new edge with the specified source, target and probability to the Markov chain.
        /// </summary>
        /// <returns>The edge.</returns>
        /// <param name="source">Source node.</param>
        /// <param name="probability">Probability.</param>
        /// <param name="target">Target node.</param>
        public MarkovTransition AddEdge (T source, double probability, T target)
        {
            var e = new MarkovTransition (source.Id, probability, target.Id);
            return graph.AddEdge (e) ? e : null;
        }

        /// <summary>
        /// Returns the successors of the specified node <c>v</c>.
        /// </summary>
        /// <returns>The successors.</returns>
        /// <param name="v">The node.</param>
        public IEnumerable<T> Post (T v)
        {
            IEnumerable<MarkovTransition> edges;
            if (graph.TryGetOutEdges (v.Id, out edges))
                return edges.Where (e => e.Probability > 0).Select (e => nodes [e.Target]).Distinct ();

            return Enumerable.Empty<T> ();
        }

        public IEnumerable<T> Post (IEnumerable<T> vs)
        {
            return vs.SelectMany (v => Post (v)).Distinct ();
        }

        /// <summary>
        /// Returns all the successors of the specified node <c>v</c>, i.e. all the nodes that can be reached from the
        /// specified node.
        /// </summary>
        /// <returns>The successors.</returns>
        /// <param name="v">The node.</param>
        public IEnumerable<T> AllPost (T v)
        {
            var pending = new Stack<int> (new [] { v.Id });
            var sucessors = new HashSet<int> ();

            IEnumerable<MarkovTransition> edges;
            while (pending.Count > 0) {
                var current = pending.Pop ();
                if (graph.TryGetOutEdges (current, out edges)) {
                    foreach (var v2 in edges.Where (e => e.Probability > 0).Select (e => e.Target)) {
                        if (!sucessors.Contains (v2)) {
                            sucessors.Add (v2);
                            pending.Push (v2);
                        }
                    }
                }
            }

            return sucessors.Select (x => nodes [x]);
        }


        /// <summary>
        /// Returns the predecessors of the specified node <c>v</c>.
        /// </summary>
        /// <returns>The predecessors.</returns>
        /// <param name="v">The node.</param>
        public IEnumerable<T> Pre (T v)
        {
            return graph.Edges.Where (e => e.Probability > 0 & e.Target.Equals (v.Id)).Select (e => nodes [e.Source]).Distinct ();
        }

        /// <summary>
        /// Returns all the predecessors of the specified node <c>v</c>, i.e. all the nodes that can reach the
        /// specified node.
        /// </summary>
        /// <returns>The predecessors.</returns>
        /// <param name="v">The node.</param>
        public IEnumerable<T> AllPre (T v)
        {
            return AllPre (new [] { v });
        }

        /// <summary>
        /// Returns all the predecessors of the specified node <c>v</c>, i.e. all the nodes that can reach the
        /// specified node.
        /// </summary>
        /// <returns>The predecessors.</returns>
        /// <param name="v">The node.</param>
        public IEnumerable<T> AllPre (IEnumerable<T> v)
        {
            var pending = new Stack<int> (v.Select (v2 => v2.Id));
            var predecessors = new HashSet<int> ();

            while (pending.Count > 0) {
                var current = pending.Pop ();
                foreach (var v2 in GetInEdges (current).Where (e => e.Probability > 0).Select (e => e.Source)) {
                    if (!predecessors.Contains (v2)) {
                        predecessors.Add (v2);
                        pending.Push (v2);
                    }
                }
            }

            return predecessors.Select (x => nodes [x]);
        }

        public bool IsAbsorbing (T v)
        {
            return Post (v).All (v2 => v2.Equals (v));
        }

        /// <summary>
        /// Returns the incoming edges to the specified node
        /// </summary>
        /// <returns>The in edges.</returns>
        /// <param name="v">The target node.</param>
        public IEnumerable<MarkovTransition> GetInEdges (T v)
        {
            return GetInEdges (v.Id);
        }

        IEnumerable<MarkovTransition> GetInEdges (int v)
        {
            return graph.Edges.Where (e => e.Target == v);
        }

        public MarkovTransition GetEdge (T source, T target)
        {
            return graph.OutEdges (source.Id).SingleOrDefault (e => e.Target == target.Id);
        }

        public IEnumerable<T> ExceptNodes (IEnumerable<T> except)
        {
            return nodes.Values.Except (except);
        }

        public void ClearOutEdges (T node)
        {
            graph.ClearOutEdges (node.Id);
        }

        public double GetProbability (T source, T target)
        {
            return graph.OutEdges (source.Id).Single (x => x.Target.Equals (target.Id)).Probability;
        }

        public string ToDot ()
        {
            var graphviz = new GraphvizAlgorithm<int, MarkovTransition> (this.graph);
            graphviz.FormatVertex += (object sender, FormatVertexEventArgs<int> e) => {
                e.VertexFormatter.Label = nodes [e.Vertex].Name + "\\n{" + string.Join (",", nodes [e.Vertex].Labels) + "}";
                if (this.Initial.ContainsKey (nodes [e.Vertex]))
                    e.VertexFormatter.Style = GraphvizVertexStyle.Bold;
                //                if (rabin.AcceptanceSet.Contains (e.Vertex))
                //                    e.VertexFormatter.Shape = QuickGraph.Graphviz.Dot.GraphvizVertexShape.DoubleCircle;
            };
            graphviz.FormatEdge += (object sender, FormatEdgeEventArgs<int, MarkovTransition> e) => {
                e.EdgeFormatter.Label.Value = Math.Round (e.Edge.Probability, 2).ToString ();
            };
            return graphviz.Generate ();
        }
    }
}

