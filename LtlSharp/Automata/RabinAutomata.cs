using System;
using QuickGraph;
using System.Collections.Generic;
using LtlSharp.Buchi.Automata;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using LtlSharp.Utils;

namespace LtlSharp.Automata
{
    public class RabinAutomata : OmegaAutomaton
    {
        RabinAcceptance<AutomataNode> _acceptanceCondition;
        

        public override IAcceptanceCondition<AutomataNode> AcceptanceCondition {
            get {
                return _acceptanceCondition;
            }
        }
        
        public RabinAutomata () : base ()
        {
            _acceptanceCondition = new RabinAcceptance<AutomataNode> ();
        }

        public void AddToAcceptance (IEnumerable<AutomataNode> e, IEnumerable<AutomataNode> f)
        {
            // not a big fan of that design...
            _acceptanceCondition.Add (e, f);
        }

        public string ToDot ()
        {
            var graphviz = new GraphvizAlgorithm<AutomataNode, LabeledAutomataTransition<AutomataNode>> (this.graph);
            graphviz.FormatVertex += (object sender, FormatVertexEventArgs<AutomataNode> e) => {
                e.VertexFormatter.Label = e.Vertex.Name;
                if (this.InitialNode.Equals (e.Vertex))
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

