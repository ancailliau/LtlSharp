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
    public class RabinAutomaton : OmegaAutomaton
    {
        RabinAcceptance<AutomatonNode> _acceptanceCondition;
        

        public override IAcceptanceCondition<AutomatonNode> AcceptanceCondition {
            get {
                return _acceptanceCondition;
            }
        }
        
        public RabinAutomaton () : base ()
        {
            _acceptanceCondition = new RabinAcceptance<AutomatonNode> ();
        }

        public void AddToAcceptance (IEnumerable<AutomatonNode> e, IEnumerable<AutomatonNode> f)
        {
            // not a big fan of that design...
            _acceptanceCondition.Add (e, f);
        }

        public string ToDot ()
        {
            var graphviz = new GraphvizAlgorithm<AutomatonNode, LabeledAutomataTransition<AutomatonNode>> (this.graph);
            graphviz.FormatVertex += (object sender, FormatVertexEventArgs<AutomatonNode> e) => {
                e.VertexFormatter.Label = e.Vertex.Name;
                if (this.InitialNode.Equals (e.Vertex))
                    e.VertexFormatter.Style = GraphvizVertexStyle.Bold;
                //                if (rabin.AcceptanceSet.Contains (e.Vertex))
                //                    e.VertexFormatter.Shape = QuickGraph.Graphviz.Dot.GraphvizVertexShape.DoubleCircle;
            };
            graphviz.FormatEdge += (object sender, FormatEdgeEventArgs<AutomatonNode, LabeledAutomataTransition<AutomatonNode>> e) => {
                e.EdgeFormatter.Label.Value = string.Join (",", e.Edge.Labels);
            };
            return graphviz.Generate ();
        }
    }
}

