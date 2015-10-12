﻿using System;
using QuickGraph;
using System.Collections.Generic;
using System.Linq;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace LtlSharp.Models
{
    /// <summary>
    /// Represents a Markov Node
    /// </summary>
    public class MarkovNode
    {
        static int currentId = 0;
        
        /// <summary>
        /// Gets the identifier of the node.
        /// </summary>
        /// <value>The identifier.</value>
        public int Id {
            get;
            private set;
        }
        
        /// <summary>
        /// Gets or sets the name of the node.
        /// </summary>
        /// <value>The name.</value>
        public string Name {
            get;
            set;
        }
        
        /// <summary>
        /// Gets the labels attached to the node.
        /// </summary>
        /// <value>The labels.</value>
        public ISet<ILiteral> Labels {
            get;
            set;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LtlSharp.Models.MarkovNode"/> class.
        /// </summary>
        public MarkovNode ()
        {
            Id = currentId++;
            Labels = new HashSet<ILiteral> ();
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LtlSharp.Models.MarkovNode"/> class.
        /// </summary>
        /// <param name="name">The name of the node.</param>
        public MarkovNode (string name) : this ()
        {
            this.Name = name;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LtlSharp.Models.MarkovNode"/> class. Every property is copied
        /// except the identifier of the node.
        /// </summary>
        /// <param name="node">Node.</param>
        public MarkovNode (MarkovNode node) : this ()
        {
            Name = node.Name;
            Labels = new HashSet<ILiteral> (node.Labels);
        }
        
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(MarkovNode))
                return false;
            MarkovNode other = (MarkovNode)obj;
            // TODO Is it required to check for id?
            return Id == other.Id && Name == other.Name && Labels.SetEquals (other.Labels);
        }
        
        public override int GetHashCode ()
        {
            unchecked {
                // TODO Is it required to check for id?
                return Id.GetHashCode () 
                    ^ (Name != null ? Name.GetHashCode () : 0) 
                    ^ (Labels != null ? Labels.GetHashCode () : 0);
            }
        }
        
    }
    
    /// <summary>
    /// Represents a Markov Transition.
    /// </summary>
    /// <description>
    /// A Markov Transition has a source Markov node and a target Markov node. 
    /// The transition is decorated with its probability.
    /// </description>
    public class MarkovTransition : Edge<int>
    {
        /// <summary>
        /// Gets or sets the probability of the transition.
        /// </summary>
        /// <value>The probability.</value>
        public double Probability {
            get;
            set;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LtlSharp.Models.MarkovTransition"/> class.
        /// </summary>
        /// <param name="source">Source node.</param>
        /// <param name="probability">Transition probability.</param>
        /// <param name="target">Target node.</param>
        public MarkovTransition (int source, double probability, int target) 
            : base (source, target)
        {
            Probability = probability;
        }
        
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(MarkovTransition))
                return false;
            MarkovTransition other = (MarkovTransition)obj;
            return Probability == other.Probability 
                && Source.Equals (other.Source) 
                && Target.Equals (other.Target);
        }
        

        public override int GetHashCode ()
        {
            unchecked {
                return Probability.GetHashCode ()
                    ^ Source.GetHashCode ()
                    ^ Target.GetHashCode ();
            }
        }
        
        
    }
    
    /// <summary>
    /// Represents a Markov Chain.
    /// </summary>
    public class MarkovChain
    {
        AdjacencyGraph<int, MarkovTransition> graph;
        
        Dictionary<int, MarkovNode> nodes;
        
        public IEnumerable<MarkovNode> Nodes {
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
        public Dictionary<MarkovNode,double> Initial {
            get;
            private set;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LtlSharp.Models.MarkovChain"/> class.
        /// </summary>
        public MarkovChain ()
        {
            graph = new AdjacencyGraph<int, MarkovTransition> (false);
            nodes = new Dictionary<int, MarkovNode> ();
            Initial = new Dictionary<MarkovNode, double> ();
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LtlSharp.Models.MarkovChain"/> class. The nodes are shared
        /// but the underlying graph and initial distribution is copied.
        /// </summary>
        /// <param name="mc">Markov chain to copy.</param>
        public MarkovChain (MarkovChain mc)
        {
            nodes = mc.nodes;
            graph = mc.graph.Clone ();
            Initial = new Dictionary<MarkovNode, double> ();
            foreach (var i in mc.Initial) {
                Initial.Add (i.Key, i.Value);
            }
        }
        
        /// <summary>
        /// Sets the initial probability for the specified Markov node to <c>p</c>.
        /// </summary>
        /// <param name="v">The node.</param>
        /// <param name="p">The probability.</param>
        public void SetInitial (MarkovNode v, Double p)
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
        public MarkovNode GetVertex (string name)
        {
            return nodes.Values.Single (v => v.Name == name);
        }
        
        /// <summary>
        /// Adds a new vertex with the specified name to the Markov chain.
        /// </summary>
        /// <returns>The vertex.</returns>
        /// <param name="name">Name.</param>
        public MarkovNode AddVertex (string name)
        {
            var v = new MarkovNode (name);
            if (graph.AddVertex (v.Id)) {
                nodes.Add (v.Id, v);
                return v;
            }
            
            return null;
        }
        
        /// <summary>
        /// Adds a new edge with the specified source, target and a probability 1 to the Markov chain.
        /// </summary>
        /// <returns>The edge.</returns>
        /// <param name="source">Source node.</param>
        /// <param name="target">Target node.</param>
        public MarkovTransition AddEdge (MarkovNode source, MarkovNode target)
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
        public MarkovTransition AddEdge (MarkovNode source, double probability, MarkovNode target)
        {
            var e = new MarkovTransition (source.Id, probability, target.Id);
            return graph.AddEdge (e) ? e : null;
        }
        
        /// <summary>
        /// Returns the successors of the specified node <c>v</c>.
        /// </summary>
        /// <returns>The successors.</returns>
        /// <param name="v">The node.</param>
        public IEnumerable<MarkovNode> Post (MarkovNode v) 
        {
            IEnumerable<MarkovTransition> edges;
            if (graph.TryGetOutEdges (v.Id, out edges))
                return edges.Where (e => e.Probability > 0).Select (e => nodes[e.Target]).Distinct ();
            
            return Enumerable.Empty<MarkovNode> ();
        }
        
        public IEnumerable<MarkovNode> Post (IEnumerable<MarkovNode> vs) 
        {
            return vs.SelectMany (v => Post(v)).Distinct ();
        }
        
        /// <summary>
        /// Returns all the successors of the specified node <c>v</c>, i.e. all the nodes that can be reached from the
        /// specified node.
        /// </summary>
        /// <returns>The successors.</returns>
        /// <param name="v">The node.</param>
        public IEnumerable<MarkovNode> AllPost (MarkovNode v) 
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
            
            return sucessors.Select (x => nodes[x]);
        }
        
        
        /// <summary>
        /// Returns the predecessors of the specified node <c>v</c>.
        /// </summary>
        /// <returns>The predecessors.</returns>
        /// <param name="v">The node.</param>
        public IEnumerable<MarkovNode> Pre (MarkovNode v) 
        {
            return graph.Edges.Where (e => e.Probability > 0 & e.Target.Equals (v.Id)).Select (e => nodes[e.Source]).Distinct ();
        }
        
        /// <summary>
        /// Returns all the predecessors of the specified node <c>v</c>, i.e. all the nodes that can reach the
        /// specified node.
        /// </summary>
        /// <returns>The predecessors.</returns>
        /// <param name="v">The node.</param>
        public IEnumerable<MarkovNode> AllPre (MarkovNode v) 
        {
            return AllPre (new [] { v });
        }
        
        /// <summary>
        /// Returns all the predecessors of the specified node <c>v</c>, i.e. all the nodes that can reach the
        /// specified node.
        /// </summary>
        /// <returns>The predecessors.</returns>
        /// <param name="v">The node.</param>
        public IEnumerable<MarkovNode> AllPre (IEnumerable<MarkovNode> v) 
        {
            var pending = new Stack<int> (v.Select (v2 => v2.Id));
            var predecessors = new HashSet<int> ();

            while (pending.Count > 0) {
                var current = pending.Pop ();
                foreach (var v2 in GetInEdges(current).Where (e => e.Probability > 0).Select (e => e.Source)) {
                    if (!predecessors.Contains (v2)) {
                        predecessors.Add (v2);
                        pending.Push (v2);
                    }
                }
            }

            return predecessors.Select (x => nodes[x]);
        }
        
        public bool IsAbsorbing (MarkovNode v)
        {
            return Post (v).All (v2 => v2.Equals (v));
        }
        
        /// <summary>
        /// Returns the incoming edges to the specified node
        /// </summary>
        /// <returns>The in edges.</returns>
        /// <param name="v">The target node.</param>
        public IEnumerable<MarkovTransition> GetInEdges (MarkovNode v)
        {
            return GetInEdges (v.Id);
        }
        
        IEnumerable<MarkovTransition> GetInEdges (int v)
        {
            return graph.Edges.Where (e => e.Target == v);
        }
        
        public MarkovTransition GetEdge (MarkovNode source, MarkovNode target)
        {
            return graph.OutEdges (source.Id).SingleOrDefault (e => e.Target == target.Id);
        }
        
        public IEnumerable<MarkovNode> ExceptNodes (IEnumerable<MarkovNode> except)
        {
            return nodes.Values.Except (except);
        }
        
        public void ClearOutEdges (MarkovNode node)
        {
            graph.ClearOutEdges (node.Id);
        }
        
        public double GetProbability (MarkovNode source, MarkovNode target)
        {
            return graph.OutEdges (source.Id).Single (x => x.Target.Equals (target.Id)).Probability;
        }

        public string ToDot ()
        {
            var graphviz = new GraphvizAlgorithm<int, MarkovTransition> (this.graph);
            graphviz.FormatVertex += (object sender, FormatVertexEventArgs<int> e) => {
                e.VertexFormatter.Label = nodes[e.Vertex].Name + "{" + string.Join (",", nodes[e.Vertex].Labels) + "}";
                if (this.Initial.ContainsKey (nodes[e.Vertex]))
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

