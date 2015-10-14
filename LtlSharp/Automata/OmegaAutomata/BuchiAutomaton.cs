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
    public class BuchiAutomaton<T> : OmegaAutomaton<T> where T : IAutomatonNode
    {
        BuchiAcceptance<T> _acceptanceCondition;
        
        public override IAcceptanceCondition<T> AcceptanceCondition {
            get { return _acceptanceCondition; }
        }

		public BuchiAutomaton () : base ()
        {
            _acceptanceCondition = new BuchiAcceptance<T> ();
        }
        
        public void SetAcceptanceCondition (BuchiAcceptance<T> condition) 
        {
            _acceptanceCondition = condition;
        }
        
        public void AddToAcceptance (T node)
        {
            // not a big fan of that design...
            _acceptanceCondition.Add (node);
        }
        
        public string ToDot ()
        {
            var graphviz = new GraphvizAlgorithm<T, AutomatonTransition<T>> (graph);
            graphviz.FormatVertex += (object sender, FormatVertexEventArgs<T> e) => {
                e.VertexFormatter.Label = e.Vertex.Name;
                if (this.InitialNode.Equals (e.Vertex))
                    e.VertexFormatter.Style = GraphvizVertexStyle.Bold;
                if (AcceptanceCondition.Accept (e.Vertex))
                    e.VertexFormatter.Shape = QuickGraph.Graphviz.Dot.GraphvizVertexShape.DoubleCircle;
            };
            graphviz.FormatEdge += (object sender, FormatEdgeEventArgs<T, AutomatonTransition<T>> e) => {
                e.EdgeFormatter.Label.Value = string.Join (",", e.Edge.Labels);
            };
            return graphviz.Generate ();
        }

    }
}

