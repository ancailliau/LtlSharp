using System;
using QuickGraph;
using System.Collections.Generic;
using LtlSharp.Buchi.Automata;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using LtlSharp.Utils;
using LtlSharp.Automata.AcceptanceConditions;

namespace LtlSharp.Automata.OmegaAutomata
{
    public class RabinAutomaton<T> : OmegaAutomaton<T> where T : IAutomatonNode
    {
        RabinAcceptance<T> _acceptanceCondition;
        

        public override IAcceptanceCondition<T> AcceptanceCondition {
            get {
                return _acceptanceCondition;
            }
        }
        
        public RabinAutomaton () : base ()
        {
            _acceptanceCondition = new RabinAcceptance<T> ();
        }

        public void AddToAcceptance (IEnumerable<T> e, IEnumerable<T> f)
        {
            // not a big fan of that design...
            _acceptanceCondition.Add (e, f);
        }

        public string ToDot ()
        {
            var graphviz = new GraphvizAlgorithm<T, AutomatonTransition<T>> (this.graph);
            graphviz.FormatVertex += (object sender, FormatVertexEventArgs<T> e) => {
                e.VertexFormatter.Label = e.Vertex.Name;
                if (this.InitialNode.Equals (e.Vertex))
                    e.VertexFormatter.Style = GraphvizVertexStyle.Bold;
                //                if (rabin.AcceptanceSet.Contains (e.Vertex))
                //                    e.VertexFormatter.Shape = QuickGraph.Graphviz.Dot.GraphvizVertexShape.DoubleCircle;
            };
            graphviz.FormatEdge += (object sender, FormatEdgeEventArgs<T, AutomatonTransition<T>> e) => {
                e.EdgeFormatter.Label.Value = string.Join (",", e.Edge.Labels);
            };
            return graphviz.Generate ();
        }
    }
}

