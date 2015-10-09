using System;
using QuickGraph;
using System.Collections.Generic;
using LtlSharp.Buchi.Automata;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using LtlSharp.Utils;

namespace LtlSharp.Automata
{
    public class RabinCondition
    {
        public HashSet<AutomataNode> E {
            get;
            set;
        }
        
        public HashSet<AutomataNode> F {
            get;
            set;
        }
        
        public RabinCondition (IEnumerable<AutomataNode> e, IEnumerable<AutomataNode> f)
        {
            E = new HashSet<AutomataNode> (e);
            F = new HashSet<AutomataNode> (f);
        }
        
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(RabinCondition))
                return false;
            RabinCondition other = (RabinCondition)obj;
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
        public HashSet<RabinCondition> AcceptanceSet;

        public RabinAutomata () : base ()
        {
            InitialNodes = new HashSet<AutomataNode> ();
            AcceptanceSet = new HashSet<RabinCondition> ();
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

