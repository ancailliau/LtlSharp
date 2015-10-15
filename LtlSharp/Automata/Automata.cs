using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Automata.Transitions;
using LtlSharp.Utils;
using QuickGraph;
using LtlSharp.Automata.Nodes.Factories;
using LtlSharp.Language;
using QuickGraph.Graphviz;
using LtlSharp.Automata.Transitions.Factories;

namespace LtlSharp.Automata
{
    public class Automata<T,T2>
        where T : IAutomatonNode
        where T2 : IAutomatonTransitionDecorator<T2>
    {
        protected AdjacencyGraph<T, ParametrizedEdge<T, T2>> graph;
        
        protected IAutomatonNodeFactory<T> factory;
        protected IAutomatonTransitionFactory<T2> factoryTransition;
        
        public T InitialNode { get; protected set; }

        public IEnumerable<T> Vertices { 
            get {
                return graph.Vertices;
            }
        }

        public IEnumerable<Tuple<T, T2, T>> Edges { 
            get {
                return graph.Edges.Select (x => new Tuple<T, T2, T> (x.Source, x.Value, x.Target));
            }
        }
        
        public Automata (IAutomatonNodeFactory<T> factory,
                         IAutomatonTransitionFactory<T2> factoryTransition)
        {
            graph = new AdjacencyGraph<T, ParametrizedEdge<T, T2>> ();
            this.factory = factory;
            this.factoryTransition = factoryTransition;
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
        /// Maps the label of each transition using the specified function.
        /// </summary>
        /// <description>
        /// The mapping function receive a set of label and returns the new set of label for the mapped transition.
        /// Note that the mapping function might return more than one set of literals; in that case, the transition
        /// is duplicated for each new set of literals.
        /// </description>
        /// <param name="map">Mapping function.</param>
        public void MapLabel (Func<T2, IEnumerable<T2>> map)
        {
            foreach (var trans in graph.Edges.ToList ()) {
                graph.RemoveEdge (trans);
                foreach (var m in map (trans.Value)) {
                    graph.AddEdge (new ParametrizedEdge<T, T2> (trans.Source, trans.Target, m));
                }
            }
        }

        /// <summary>
        /// Returns the set of literals that correspond to (at least) a transition.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<ILiteral> Alphabet ()
        {
            return graph.Edges.SelectMany (e => e.Value.GetAlphabet ()).Distinct ();
        }

        /// <summary>
        /// Returns the set of literals that correspond to (at least) a transition from the specified node.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<T2> OutAlphabet (T node)
        {
            return graph.OutEdges (node).Select (e => e.Value).Distinct ();
        }

        /// <summary>
        /// Returns the set of literals that correspond to (at least) a transition for (at least) a specified node.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<T2> OutAlphabet (IEnumerable<T> nodes)
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
        public IEnumerable<T> Post (T node, T2 labels)
        {
            return Post (node, (l, target) => labels.Entails(l));
        }

        /// <summary>
        /// Returns all the successor nodes of the specified nodes for the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<T> Post (IEnumerable<T> nodes, T2 labels)
        {
            return nodes.SelectMany (node => Post(node, labels));
        }

        /// <summary>
        /// Returns all the successor nodes of the specified node for all the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<T> Post (T node, IEnumerable<T2> labels)
        {
            return labels.SelectMany (l => Post(node, l));
        }

        /// <summary>
        /// Returns all the successor nodes of the specified nodes for all the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<T> Post (IEnumerable<T> nodes, IEnumerable<T2> labels)
        {
            return nodes.SelectMany (node => Post(node, labels));
        }

        /// <summary>
        /// Returns all the successor nodes of the specified node whose transition satisfy the predicate.
        /// </summary>
        /// <param name="node">Node.</param>
        /// <param name="predicate">The predicate to satify. First argument is the set of literals on the
        /// transition. Second argument is the target node.</param>
        public IEnumerable<T> Post (T node, Func<T2, T, bool> predicate)
        {
            return graph.OutEdges (node).Where (e => predicate(e.Value, e.Target)).Select (e => e.Target);
        }

        /// <summary>
        /// Returns all the successor nodes of the specified nodes whose transition satisfy the predicate.
        /// </summary>
        /// <param name="node">Node.</param>
        /// <param name="predicate">The predicate to satify. First argument is the set of literals on the
        /// transition. Second argument is the target node.</param>
        public IEnumerable<T> Post (IEnumerable<T> nodes, Func<T2, T, bool> predicate)
        {
            return nodes.SelectMany (node => Post(node, predicate));
        }
        
        /// <summary>
        /// Adds the specified transition to the automaton.
        /// </summary>
        /// <param name="transition">Transition.</param>
        public void AddTransition (T source, T target, T2 value)
        {
            var edge = new ParametrizedEdge<T, T2> (source, target, value);
            if (!graph.ContainsEdge(edge))
                graph.AddEdge (edge);
        }

        /// <summary>
        /// Adds the specified transition to the automaton.
        /// </summary>
        /// <param name="transition">Transition.</param>
        public void AddTransition (T source, T target, IEnumerable<ILiteral> value)
        {
            var edge = new ParametrizedEdge<T, T2> (source, target, factoryTransition.Create (value));
            if (!graph.ContainsEdge(edge))
                graph.AddEdge (edge);
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
                // TODO Simpler expression MUST exist !
                foreach (var c in transitions.Select (x => x.Value)) {
                    var succ = transitions.Where (t => c.Entails(t.Value)).Select (t => t.Target);
                    if (succ.Count () > 1)
                        return false;

                    foreach (var s in succ.Except (visited)) {
                        pending.Push (s);
                    }
                }
            }

            return true;
        }



        public void SimplifyTransitions ()
        {
            foreach (var node in graph.Vertices) {
                var transitions = graph.OutEdges (node);
                foreach (var trans in transitions.ToList ()) {
                    var sameTarget = transitions.Where (t => t.Target.Equals (trans.Target)).ToList ();
                    var labels = sameTarget.Select (x => x.Value.ToLiteralSet ());
                    var lf = new LiteralFormula (labels);
                    var newLabels = lf.Simplify ();
                    foreach (var e in sameTarget) {
                        graph.RemoveEdge (e);
                    }
                    foreach (var nl in newLabels) {
                        graph.AddEdge (new ParametrizedEdge<T,T2> (trans.Source, trans.Target, factoryTransition.Create (nl)));
                    }
                }
            }
        }

        public void UnfoldTransitions ()
        {
            MapLabel ((arg) => arg.UnfoldLabels (Alphabet ()));
        }

        public string ToDot ()
        {
            var graphviz = new GraphvizAlgorithm<T, ParametrizedEdge<T, T2>> (graph);
            graphviz.FormatVertex += (object sender, FormatVertexEventArgs<T> e) => {
                e.VertexFormatter.Label = e.Vertex.Name;
                if (this.InitialNode != null && this.InitialNode.Equals (e.Vertex))
                    e.VertexFormatter.Style = QuickGraph.Graphviz.Dot.GraphvizVertexStyle.Bold;
            };
            graphviz.FormatEdge += (object sender, FormatEdgeEventArgs<T, ParametrizedEdge<T, T2>> e) => {
                e.EdgeFormatter.Label.Value = string.Join (",", e.Edge.Value.ToString ());
            };
            return graphviz.Generate ();
        }
    }
}

