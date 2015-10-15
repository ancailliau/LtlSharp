using System;
using QuickGraph;
using System.Collections.Generic;
using System.Linq;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using LtlSharp.Automata;
using LtlSharp.Automata.Nodes.Factories;
using LtlSharp.Utils;

namespace LtlSharp.Models
{
    
    /// <summary>
    /// Represents a Markov Chain.
    /// </summary>
    public class MarkovChain<T> where T : IAutomatonNode
    {
        AdjacencyGraph<int, ParametrizedEdge<int, double>> graph;

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
            graph = new AdjacencyGraph<int, ParametrizedEdge<int, double>> (false);
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
            graph = new AdjacencyGraph<int, ParametrizedEdge<int, double>> ();
            foreach (var vertex in mc.graph.Vertices) {
                graph.AddVertex (vertex);
            }
            foreach (var edge in mc.graph.Edges) {
                graph.AddEdge (new ParametrizedEdge<int, double> (edge.Source, edge.Target, edge.Value));
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
                          graph.Vertices.All (v => graph.OutEdges (v).Sum (x => x.Value) == 1d);
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
        /// Adds a new vertex with the specified name and specified labels to the Markov chain.
        /// </summary>
        /// <returns>The vertex.</returns>
        /// <param name="name">Name.</param>
        /// <param name="labels">Labels.</param>
        public T AddVertex (string name, IEnumerable<ILiteral> labels)
        {
            var v = factory.Create (name, labels);
            if (graph.AddVertex (v.Id)) {
                nodes.Add (v.Id, v);
                return v;
            }

            return default (T);
        }

        /// <summary>
        /// Adds a new edge with the specified source, target and a probability 1 to the Markov chain.
        /// </summary>
        /// <param name="source">Source node.</param>
        /// <param name="target">Target node.</param>
        public void AddEdge (T source, T target)
        {
            AddEdge (source, 1, target);
        }

        /// <summary>
        /// Adds a new edge with the specified source, target and probability to the Markov chain.
        /// </summary>
        /// <param name="source">Source node.</param>
        /// <param name="probability">Probability.</param>
        /// <param name="target">Target node.</param>
        public void AddEdge (T source, double probability, T target)
        {
            graph.AddEdge (new ParametrizedEdge<int, double> (source.Id, target.Id, probability));
        }

        /// <summary>
        /// Returns the successors of the specified node <c>v</c>.
        /// </summary>
        /// <returns>The successors.</returns>
        /// <param name="v">The node.</param>
        public IEnumerable<T> Post (T v)
        {
            IEnumerable<ParametrizedEdge<int, double>> edges;
            if (graph.TryGetOutEdges (v.Id, out edges))
                return edges.Where (e => e.Value > 0).Select (e => nodes [e.Target]).Distinct ();

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

            IEnumerable<ParametrizedEdge<int, double>> edges;
            while (pending.Count > 0) {
                var current = pending.Pop ();
                if (graph.TryGetOutEdges (current, out edges)) {
                    foreach (var v2 in edges.Where (e => e.Value > 0).Select (e => e.Target)) {
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
            return graph.Edges.Where (e => e.Value > 0 & e.Target.Equals (v.Id)).Select (e => nodes [e.Source]).Distinct ();
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
                foreach (var v2 in graph.Edges.Where (e => e.Target.Equals (current) && e.Value > 0).Select (e => e.Source)) {
                    if (!predecessors.Contains (v2)) {
                        predecessors.Add (v2);
                        pending.Push (v2);
                    }
                }
            }

            return predecessors.Select (x => nodes [x]);
        }
        
        

        public void SetProbability (T source, T target, double value)
        {
            var t = graph.OutEdges (source.Id).SingleOrDefault (e => e.Target.Equals (target.Id));
            if (t != null) {
                if (value == 0) {
                    graph.RemoveEdge (t);
                } else {
                    t.Value = value;
                }
            } else if (value != 0) {
                AddEdge (source, value, target);
            }
        }

        public bool IsAbsorbing (T v)
        {
            return Post (v).All (v2 => v2.Equals (v));
        }
        
        //public MarkovTransition GetEdge (T source, T target)
        //{
        //    return graph.OutEdges (source.Id).SingleOrDefault (e => e.Target == target.Id)?.Value;
        //}

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
            return graph.OutEdges (source.Id).SingleOrDefault (x => x.Target.Equals (target.Id))?.Value ?? 0;
        }

        public string ToDot ()
        {
            var graphviz = new GraphvizAlgorithm<int, ParametrizedEdge<int, double>> (this.graph);
            graphviz.FormatVertex += (object sender, FormatVertexEventArgs<int> e) => {
                e.VertexFormatter.Label = nodes [e.Vertex].Name + "\\n{" + string.Join (",", nodes [e.Vertex].Labels) + "}";
                if (this.Initial.ContainsKey (nodes [e.Vertex]))
                    e.VertexFormatter.Style = GraphvizVertexStyle.Bold;
            };
            graphviz.FormatEdge += (object sender, FormatEdgeEventArgs<int, ParametrizedEdge<int, double>> e) => {
                e.EdgeFormatter.Label.Value = Math.Round (e.Edge.Value, 2).ToString ();
            };
            return graphviz.Generate ();
        }
    }
}

