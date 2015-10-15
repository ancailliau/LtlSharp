using System;
using QuickGraph;
using System.Collections.Generic;
using System.Linq;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using LtlSharp.Automata;
using LtlSharp.Automata.Nodes.Factories;
using LtlSharp.Utils;
using LtlSharp.Automata.Transitions.Factories;

namespace LtlSharp.Models
{
    /// <summary>
    /// Represents a Markov Chain.
    /// </summary>
    public class MarkovChain<T>
        : Automata<T, ProbabilityTransitionDecorator>
        where T : IAutomatonNode
    {
        ProbabilityDecoratorFactory _factoryTrans;

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
        public MarkovChain (IAutomatonNodeFactory<T> factory,
                            ProbabilityDecoratorFactory factoryTrans)
            : base (factory, factoryTrans)
        {
            Initial = new Dictionary<T, double> ();
            _factoryTrans = factoryTrans;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LtlSharp.Models.MarkovChain"/> class. The nodes are shared
        /// but the underlying graph and initial distribution is copied.
        /// </summary>
        /// <param name="mc">Markov chain to copy.</param>
        public MarkovChain (MarkovChain<T> mc)
            : base (mc.factory, mc.factoryTransition)
        {
            _factoryTrans = mc._factoryTrans;
            
            graph = new AdjacencyGraph<T, ParametrizedEdge<T, ProbabilityTransitionDecorator>> ();
            foreach (var vertex in mc.graph.Vertices) {
                graph.AddVertex (vertex);
            }
            foreach (var edge in mc.graph.Edges) {
                graph.AddEdge (new ParametrizedEdge<T, ProbabilityTransitionDecorator> (
                    edge.Source, 
                    edge.Target, 
                    _factoryTrans.Clone (edge.Value)
                ));
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
        /// <param name="node">The node.</param>
        /// <param name="probability">The probability.</param>
        public void SetInitial (T node, double probability)
        {
            if (!Initial.ContainsKey (node)) {
                Initial.Add (node, probability);
            }
            Initial [node] = probability;
        }

        /// <summary>
        /// Checks if the sum of probability distributions for each node equals to one.
        /// </summary>
        /// <returns><c>true</c>, if probability distributions are valid, <c>false</c> otherwise.</returns>
        public bool CheckProbabilityDistributions ()
        {
            return Initial.Values.Sum () == 1d &&
                          graph.Vertices.All (v => graph.OutEdges (v).Sum (x => x.Value.Probability) == 1d);
        }

        /// <summary>
        /// Gets the vertex with the specified name. Assume that the name uniquely identify the node.
        /// </summary>
        /// <returns>The vertex.</returns>
        /// <param name="name">Name.</param>
        public T GetVertex (string name)
        {
            return Nodes.Single (v => v.Name == name);
        }

        /// <summary>
        /// Adds a new edge with the specified source, target and a probability 1 to the Markov chain.
        /// </summary>
        /// <param name="source">Source node.</param>
        /// <param name="target">Target node.</param>
        public void AddTransition (T source, T target)
        {
            AddTransition (source, 1, target);
        }

        /// <summary>
        /// Adds a new edge with the specified source, target and probability to the Markov chain.
        /// </summary>
        /// <param name="source">Source node.</param>
        /// <param name="probability">Probability.</param>
        /// <param name="target">Target node.</param>
        public void AddTransition (T source, double probability, T target)
        {
            graph.AddEdge (new ParametrizedEdge<T, ProbabilityTransitionDecorator> (source, target, _factoryTrans.Create (probability)));
        }

        public void SetProbability (T source, T target, double value)
        {
            var t = graph.OutEdges (source).SingleOrDefault (e => e.Target.Equals (target));
            if (t != null) {
                if (value == 0) {
                    graph.RemoveEdge (t);
                } else {
                    t.Value = _factoryTrans.Create (value);
                }
            } else if (value != 0) {
                AddTransition (source, value, target);
            }
        }

        public double GetProbability (T source, T target)
        {
            return graph.OutEdges (source).SingleOrDefault (x => x.Target.Equals (target))?.Value.Probability ?? 0;
        }

        public override string ToDot ()
        {
            var graphviz = new GraphvizAlgorithm<T, ParametrizedEdge<T, ProbabilityTransitionDecorator>> (this.graph);
            graphviz.FormatVertex += (object sender, FormatVertexEventArgs<T> e) => {
                e.VertexFormatter.Label = e.Vertex.Name + "\\n{" + string.Join (",", e.Vertex.Labels) + "}";
                if (this.Initial.ContainsKey (e.Vertex))
                    e.VertexFormatter.Style = GraphvizVertexStyle.Bold;
            };
            graphviz.FormatEdge += (object sender, FormatEdgeEventArgs<T, ParametrizedEdge<T, ProbabilityTransitionDecorator>> e) => {
                e.EdgeFormatter.Label.Value = Math.Round (e.Edge.Value.Probability, 2).ToString ();
            };
            return graphviz.Generate ();
        }
    }
}

