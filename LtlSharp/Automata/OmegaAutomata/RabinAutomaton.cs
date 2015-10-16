using System;
using QuickGraph;
using System.Collections.Generic;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using LtlSharp.Utils;
using LtlSharp.Automata.AcceptanceConditions;
using LtlSharp.Automata.Transitions;
using LtlSharp.Automata.Nodes.Factories;


namespace LtlSharp.Automata.OmegaAutomata
{
    public class RabinAutomaton<T> : OmegaAutomaton<T, LiteralSetDecoration> where T : IAutomatonNode
    {
        RabinAcceptance<T> _acceptanceCondition;
        

        public override IAcceptanceCondition<T> AcceptanceCondition {
            get {
                return _acceptanceCondition;
            }
        }
        
        public RabinAutomaton (IAutomatonNodeFactory<T> factory)
            : base (factory)
        {
            _acceptanceCondition = new RabinAcceptance<T> ();
        }

        public void AddToAcceptance (IEnumerable<T> e, IEnumerable<T> f)
        {
            // not a big fan of that design...
            _acceptanceCondition.Add (e, f);
        }

        public override string ToDot ()
        {
            var graphviz = new GraphvizAlgorithm<T, ParametrizedEdge<T, LiteralSetDecoration>> (this.graph);
            graphviz.FormatVertex += (object sender, FormatVertexEventArgs<T> e) => {
                e.VertexFormatter.Label = e.Vertex.Name;
                if (this.InitialNode.Equals (e.Vertex))
                    e.VertexFormatter.Style = GraphvizVertexStyle.Bold;
                //                if (rabin.AcceptanceSet.Contains (e.Vertex))
                //                    e.VertexFormatter.Shape = QuickGraph.Graphviz.Dot.GraphvizVertexShape.DoubleCircle;
            };
            graphviz.FormatEdge += (object sender, FormatEdgeEventArgs<T, ParametrizedEdge<T, LiteralSetDecoration>> e) => {
                e.EdgeFormatter.Label.Value = string.Join (",", e.Edge.Value);
            };
            return graphviz.Generate ();
        }

        public override Automata<T, LiteralSetDecoration> Clone ()
        {
            throw new NotImplementedException ();
        }
    }
}

