using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LtlSharp.Automata.AcceptanceConditions;
using LtlSharp.Buchi.Automata;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace LtlSharp.Automata.OmegaAutomata
{
    public class BuchiAutomaton : OmegaAutomaton
    {
        BuchiAcceptance<AutomatonNode> _acceptanceCondition;
        
        public override IAcceptanceCondition<AutomatonNode> AcceptanceCondition {
            get { return _acceptanceCondition; }
        }

		public BuchiAutomaton () : base ()
        {
            _acceptanceCondition = new BuchiAcceptance<AutomatonNode> ();
        }
        
        public void SetAcceptanceCondition (BuchiAcceptance<AutomatonNode> condition) 
        {
            _acceptanceCondition = condition;
        }
        
        public void AddToAcceptance (AutomatonNode node)
        {
            // not a big fan of that design...
            _acceptanceCondition.Add (node);
        }
        
        public string ToDot ()
        {
            var graphviz = new GraphvizAlgorithm<AutomatonNode, LabeledAutomataTransition<AutomatonNode>> (graph);
            graphviz.FormatVertex += (object sender, FormatVertexEventArgs<AutomatonNode> e) => {
                e.VertexFormatter.Label = e.Vertex.Name;
                if (this.InitialNode.Equals (e.Vertex))
                    e.VertexFormatter.Style = GraphvizVertexStyle.Bold;
                if (AcceptanceCondition.Accept (e.Vertex))
                    e.VertexFormatter.Shape = QuickGraph.Graphviz.Dot.GraphvizVertexShape.DoubleCircle;
            };
            graphviz.FormatEdge += (object sender, FormatEdgeEventArgs<AutomatonNode, LabeledAutomataTransition<AutomatonNode>> e) => {
                e.EdgeFormatter.Label.Value = string.Join (",", e.Edge.Labels);
            };
            return graphviz.Generate ();
        }

    }
}

