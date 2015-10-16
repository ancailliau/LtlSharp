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
    public class MarkovChain<T>
        : Automata<T, ProbabilityTransitionDecorator>
        where T : IAutomatonNode
    {

        /// <summary>
        /// Gets the initial distribution of the nodes. If a node is not contained, it is assumed that its
        /// initial probability is 0.
        /// </summary>
        /// <value>The initial distribution .</value>
        public Dictionary<T, ProbabilityTransitionDecorator> Initial {
            get;
            private set;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LtlSharp.Models.MarkovChain"/> class.
        /// </summary>
        public MarkovChain (IAutomatonNodeFactory<T> factory)
            : base (factory)
        {
            Initial = new Dictionary<T, ProbabilityTransitionDecorator> ();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LtlSharp.Models.MarkovChain"/> class. The nodes are shared
        /// but the underlying graph and initial distribution is copied.
        /// </summary>
        /// <param name="mc">Markov chain to copy.</param>
        public MarkovChain (MarkovChain<T> mc)
            : base (mc.factory)
        {
        }

        /// <summary>
        /// Sets the initial probability for the specified Markov node to <c>p</c>.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="probability">The probability.</param>
        public void SetInitial (T node, double probability)
        {
            if (!Initial.ContainsKey (node)) {
                Initial.Add (node, new ProbabilityTransitionDecorator (probability));
            }
            Initial [node] = new ProbabilityTransitionDecorator (probability);
        }

        /// <summary>
        /// Checks if the sum of probability distributions for each node equals to one.
        /// </summary>
        /// <returns><c>true</c>, if probability distributions are valid, <c>false</c> otherwise.</returns>
        public bool CheckProbabilityDistributions ()
        {
            return Initial.Values.Sum (x => x.Probability) == 1d &&
                          graph.Vertices.All (v => graph.OutEdges (v).Sum (x => x.Value.Probability) == 1d);
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
            graph.AddEdge (new ParametrizedEdge<T, ProbabilityTransitionDecorator> (source, target, new ProbabilityTransitionDecorator (probability)));
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

        public override Automata<T, ProbabilityTransitionDecorator> Clone ()
        {
            var mc = new MarkovChain<T> (factory);
            foreach (var vertex in graph.Vertices) {
                mc.graph.AddVertex (vertex);
            }
            foreach (var edge in graph.Edges) {
                mc.graph.AddEdge (new ParametrizedEdge<T, ProbabilityTransitionDecorator> (
                    edge.Source, 
                    edge.Target, 
                    new ProbabilityTransitionDecorator (edge.Value.Probability)
                ));
            }
            foreach (var i in Initial) {
                mc.Initial.Add (i.Key, i.Value);
            }
            return mc;
        }
    }
}

