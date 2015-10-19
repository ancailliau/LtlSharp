using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Automata.Transitions;
using LtlSharp.Utils;
using QuickGraph;
using LtlSharp.Automata.Nodes.Factories;
using LtlSharp.Language;
using QuickGraph.Graphviz;


namespace LtlSharp.Automata
{
    public abstract class Automata<T,T2>
        where T : IAutomatonNode
        where T2 : ITransitionDecoration<T2>
    {
        protected AdjacencyGraph<T, ParametrizedEdge<T, T2>> graph;
        
        protected IAutomatonNodeFactory<T> factory;

        public IEnumerable<T> Nodes { 
            get {
                return graph.Vertices;
            }
        }

        public IEnumerable<AutomataTransition<T, T2>> Edges { 
            get {
                return graph.Edges.Select (x => new AutomataTransition<T, T2> (x.Source, x.Target, x.Value));
            }
        }

        public Automata (IAutomatonNodeFactory<T> factory)
        {
            graph = new AdjacencyGraph<T, ParametrizedEdge<T, T2>> ();
            this.factory = factory;
        }
        
        public IAutomatonNodeFactory<T> GetNodeFactory ()
        {
            return factory;
        }

        public void RemoveTransition (T source, T target, T2 value)
        {
            graph.RemoveOutEdgeIf (source, (e) => e.Target.Equals (target) & e.Value.Equals (value));
        }
        
        public void ReplaceTransitionValue (T source, T item1, T2 oldValue, T2 newValue)
        {
            foreach (var e in graph.OutEdges (source).Where (e => e.Target.Equals (item1) & e.Value.Equals (oldValue))) {
                e.Value = newValue;
            }
        }
        
        public void SetTransitionsValue (T source, T item1, T2 value)
        {
            foreach (var e in graph.OutEdges (source).Where (e => e.Target.Equals (item1))) {
                e.Value = value;
            }
        }

        public IEnumerable<AutomataTransition<T, T2>> GetTransitions (T source)
        {
            return graph.OutEdges (source).Select (x => new AutomataTransition<T, T2> (source, x.Target, x.Value));
        }

        public T2 GetTransition (T source, T target)
        {
            return graph.OutEdges (source).SingleOrDefault (e => e.Target.Equals (target))?.Value;
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
                    AddTransition (trans.Source, trans.Target, m);
                }
            }
        }

        /// <summary>
        /// Returns the set of decorations that correspond to (at least) a transition from the specified node.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<T2> GetOutDecorations (T node)
        {
            return graph.OutEdges (node).Select (e => e.Value).Distinct ();
        }

        /// <summary>
        /// Returns the set of decorations that correspond to (at least) a transition for (at least) a specified node.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<T2> GetOutDecorations (IEnumerable<T> nodes)
        {
            return nodes.SelectMany (GetOutDecorations).Distinct ();
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
        
        public IEnumerable<T> Post (T node, T2 value)
        {
            return Post(node, (arg1, arg2) => arg1.Equals(value));
        }
        
        public IEnumerable<T> Post (IEnumerable<T> nodes, T2 value)
        {
            return nodes.SelectMany (node => Post(node, value));
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
        /// Returns all the successors of the specified node <c>v</c>, i.e. all the nodes that can be reached from the
        /// specified node.
        /// </summary>
        /// <returns>The successors.</returns>
        /// <param name="v">The node.</param>
        public IEnumerable<T> AllPost (T v)
        {
            var pending = new Stack<T> (new [] { v });
            var sucessors = new HashSet<T> ();

            IEnumerable<ParametrizedEdge<T, T2>> edges;
            while (pending.Count > 0) {
                var current = pending.Pop ();
                if (graph.TryGetOutEdges (current, out edges)) {
                    foreach (var v2 in edges.Select (e => e.Target)) {
                        if (!sucessors.Contains (v2)) {
                            sucessors.Add (v2);
                            pending.Push (v2);
                        }
                    }
                }
            }

            return sucessors;
        }

        /// <summary>
        /// Returns the predecessors of the specified node <c>v</c>.
        /// </summary>
        /// <returns>The predecessors.</returns>
        /// <param name="v">The node.</param>
        public IEnumerable<T> Pre (T v)
        {
            return graph.Edges.Where (e => e.Target.Equals (v)).Select (e => e.Source).Distinct ();
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
            var pending = new Stack<T> (v);
            var predecessors = new HashSet<T> ();

            while (pending.Count > 0) {
                var current = pending.Pop ();
                foreach (var v2 in graph.Edges.Where (e => e.Target.Equals (current)).Select (e => e.Source)) {
                    if (!predecessors.Contains (v2)) {
                        predecessors.Add (v2);
                        pending.Push (v2);
                    }
                }
            }

            return predecessors.Select (x => x);
        }
        
        public void AddTransition (AutomataTransition<T, T2> trans)
        {
            AddTransition (trans.Source, trans.Target, trans.Decoration);
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

        public void RemoveAllTransitions (T node)
        {
            graph.ClearOutEdges (node);
        }

        /// <summary>
        /// Adds the node.
        /// </summary>
        /// <param name="node">Node.</param>
        public void AddNode (T node)
        {
            graph.AddVertex (node);
        }
        
        public T AddNode (string name)
        {
            var t = factory.Create (name);
            graph.AddVertex (t);
            return t;
        }
        
        public T AddNode (string name, ILiteral literal)
        {
            var t = factory.Create (name, new [] { literal });
            graph.AddVertex (t);
            return t;
        }
        
        public T AddNode (string name, IEnumerable<ILiteral> literals)
        {
            var t = factory.Create (name, literals);
            graph.AddVertex (t);
            return t;
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
        /// Indicates whether the specified node is absorbing.
        /// </summary>
        /// <param name="node">Node.</param>
        /// <returns><c>True</c> if nodes are absorbing, <c>False</c> otherwise.</returns>
        public bool IsAbsorbing (T node)
        {
            return Post (node).All (v2 => v2.Equals (node));
        }

        public virtual string ToDot ()
        {
            var graphviz = new GraphvizAlgorithm<T, ParametrizedEdge<T, T2>> (graph);
            graphviz.FormatVertex += (object sender, FormatVertexEventArgs<T> e) => {
                e.VertexFormatter.Label = e.Vertex.Name;
            };
            graphviz.FormatEdge += (object sender, FormatEdgeEventArgs<T, ParametrizedEdge<T, T2>> e) => {
                e.EdgeFormatter.Label.Value = string.Join (",", e.Edge.Value.ToString ());
            };
            return graphviz.Generate ();
        }

        public abstract Automata<T, T2> Clone ();
    }
    
    public struct AutomataTransition<T1, T2>
    {
        public readonly T1 Source;
        public readonly T1 Target;
        public readonly T2 Decoration;

        public AutomataTransition (T1 source, T1 target, T2 decoration)
        {
            Source = source;
            Target = target;
            Decoration = decoration;
        }
        
        public override string ToString ()
        {
            return string.Format ("{0} -> {1} [{2}]", Source, Target, Decoration);
        }
        
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(AutomataTransition<T1, T2>))
                return false;
            var other = (AutomataTransition<T1, T2>)obj;
            return Source.Equals(other.Source) & Target.Equals (other.Target) & Decoration.Equals (other.Decoration);
        }

        public override int GetHashCode ()
        {
            return 17 + Source.GetHashCode () + 32 * (Target.GetHashCode () + 32 * Decoration.GetHashCode ());
        }
    }
}

