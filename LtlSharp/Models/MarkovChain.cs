using System;
using QuickGraph;
using System.Collections.Generic;
using System.Linq;

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
            private set;
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
    public class MarkovTransition : Edge<MarkovNode>
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
        public MarkovTransition (MarkovNode source, double probability, MarkovNode target) 
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
                    ^ (Source != null ? Source.GetHashCode () : 0)
                    ^ (Target != null ? Target.GetHashCode () : 0);
            }
        }
        
        
    }
    
    /// <summary>
    /// Represents a Markov Chain.
    /// </summary>
    public class MarkovChain : AdjacencyGraph<MarkovNode, MarkovTransition>
    {
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
        public MarkovChain () : base (false)
        {
            Initial = new Dictionary<MarkovNode, double> ();
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LtlSharp.Models.MarkovChain"/> class.
        /// </summary>
        /// <param name="mc">Markov chain to copy.</param>
        public MarkovChain (MarkovChain mc)
        {
            var mapping = new Dictionary<MarkovNode, MarkovNode> ();
            foreach (var node in mc.Vertices) {
                var newNode = new MarkovNode (node);
                mapping.Add (node, newNode);
                AddVertex (newNode);
            }
            foreach (var t in mc.Edges) {
                AddEdge (mapping [t.Source], t.Probability, mapping [t.Target]);
            }
            Initial = new Dictionary<MarkovNode, double> ();
            foreach (var i in mc.Initial) {
                Initial.Add (mapping[i.Key], i.Value);
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
                Vertices.All (v => OutEdges (v).Sum (x => x.Probability) == 1d);
        }
        
        /// <summary>
        /// Gets the vertex with the specified name. Assume that the name uniquely identify the node.
        /// </summary>
        /// <returns>The vertex.</returns>
        /// <param name="name">Name.</param>
        public MarkovNode GetVertex (string name)
        {
            return Vertices.Single (v => v.Name == name);
        }
        
        /// <summary>
        /// Adds a new vertex with the specified name to the Markov chain.
        /// </summary>
        /// <returns>The vertex.</returns>
        /// <param name="name">Name.</param>
        public MarkovNode AddVertex (string name)
        {
            var v = new MarkovNode (name);
            return base.AddVertex (v) ? v : null;
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
            var e = new MarkovTransition (source, probability, target);
            return base.AddEdge (e) ? e : null;
        }
        
        /// <summary>
        /// Returns the successors of the specified node <c>v</c>.
        /// </summary>
        /// <returns>The successors.</returns>
        /// <param name="v">The node.</param>
        public IEnumerable<MarkovNode> Post (MarkovNode v) 
        {
            IEnumerable<MarkovTransition> edges;
            if (TryGetOutEdges (v, out edges))
                return edges.Where (e => e.Probability > 0).Select (e => e.Target).Distinct ();
            
            return Enumerable.Empty<MarkovNode> ();
        }
        
        /// <summary>
        /// Returns all the successors of the specified node <c>v</c>, i.e. all the nodes that can be reached from the
        /// specified node.
        /// </summary>
        /// <returns>The successors.</returns>
        /// <param name="v">The node.</param>
        public IEnumerable<MarkovNode> AllPost (MarkovNode v) 
        {
            var pending = new Stack<MarkovNode> (new [] { v });
            var sucessors = new HashSet<MarkovNode> ();
            
            IEnumerable<MarkovTransition> edges;
            while (pending.Count > 0) {
                var current = pending.Pop ();
                if (TryGetOutEdges (current, out edges)) {
                    foreach (var v2 in edges.Where (e => e.Probability > 0).Select (e => e.Target)) {
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
        public IEnumerable<MarkovNode> Pre (MarkovNode v) 
        {
            return Edges.Where (e => e.Probability > 0 & e.Target.Equals (v)).Select (e => e.Source).Distinct ();
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
            var pending = new Stack<MarkovNode> (v);
            var predecessors = new HashSet<MarkovNode> ();

            while (pending.Count > 0) {
                var current = pending.Pop ();
                foreach (var v2 in GetInEdges(current).Where (e => e.Probability > 0).Select (e => e.Source)) {
                    if (!predecessors.Contains (v2)) {
                        predecessors.Add (v2);
                        pending.Push (v2);
                    }
                }
            }

            return predecessors;
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
            return Edges.Where (e => e.Target.Equals (v));
        }
        
        
        public MarkovTransition GetEdge (MarkovNode source, MarkovNode target)
        {
            return OutEdges (source).SingleOrDefault (e => e.Target.Equals (target));
        }
    }
}

