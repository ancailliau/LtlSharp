using System;
using QuickGraph;
using System.Collections.Generic;
using LtlSharp.Buchi.Automata;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using LtlSharp.Utils;

namespace LtlSharp.Automata
{
    public class RabinCondition<T>
    {
        public HashSet<T> E {
            get;
            set;
        }
        
        public HashSet<T> F {
            get;
            set;
        }
        
        public RabinCondition ()
        {
            E = new HashSet<T> ();
            F = new HashSet<T> ();
        }
        
        public RabinCondition (IEnumerable<T> e, IEnumerable<T> f)
        {
            E = new HashSet<T> (e);
            F = new HashSet<T> (f);
        }
        
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(RabinCondition<T>))
                return false;
            var other = (RabinCondition<T>)obj;
            return E.SetEquals (other.E) && F.SetEquals (other.F);
        }
        

        public override int GetHashCode ()
        {
            unchecked {
                return (E != null ? E.GetHashCodeForElements () : 0) 
                    ^ (F != null ? F.GetHashCodeForElements () : 0);
            }
        }
        
    }
    
    public class RabinAutomata : AdjacencyGraph<AutomataNode, LabeledAutomataTransition<AutomataNode>> 
    {
        public HashSet<AutomataNode> InitialNodes;
        public HashSet<RabinCondition<AutomataNode>> AcceptanceSet;

        public RabinAutomata () : base ()
        {
            InitialNodes = new HashSet<AutomataNode> ();
            AcceptanceSet = new HashSet<RabinCondition<AutomataNode>> ();
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
}

