using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LtlSharp.Buchi.Automata;
using QuickGraph;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;

namespace LtlSharp.Automata
{
    public class BuchiAutomata : OmegaAutomaton
    {
        BuchiAcceptance<AutomataNode> _acceptanceCondition;
        
        public override IAcceptanceCondition<AutomataNode> AcceptanceCondition {
            get { return _acceptanceCondition; }
        }

		public BuchiAutomata () : base ()
        {
            _acceptanceCondition = new BuchiAcceptance<AutomataNode> ();
        }
        
        public void SetAcceptanceCondition (BuchiAcceptance<AutomataNode> condition) 
        {
            _acceptanceCondition = condition;
        }
        
        public void AddToAcceptance (AutomataNode node)
        {
            // not a big fan of that design...
            _acceptanceCondition.Add (node);
        }
        
        public string ToDot ()
        {
            var graphviz = new GraphvizAlgorithm<AutomataNode, LabeledAutomataTransition<AutomataNode>> (graph);
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
    
    public class DegeneralizerBuchiAutomata : AdjacencyGraph<AutomataNode, DegeneralizerAutomataTransition<AutomataNode>> 
    {
        public HashSet<AutomataNode> InitialNodes;
        public HashSet<AutomataNode> AcceptanceSet;

        public DegeneralizerBuchiAutomata () : base ()
        {
            InitialNodes = new HashSet<AutomataNode> ();
            AcceptanceSet = new HashSet<AutomataNode> ();
        }
    }
}

