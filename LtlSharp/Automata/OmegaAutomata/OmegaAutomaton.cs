using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Automata.AcceptanceConditions;
using QuickGraph;
using LtlSharp.Utils;
using LtlSharp.Language;
using QuickGraph.Graphviz;
using LtlSharp.Automata.Transitions;
using LtlSharp.Automata.Nodes.Factories;


namespace LtlSharp.Automata.OmegaAutomata
{
    public abstract class OmegaAutomaton<T1,T2> 
        : Automata<T1,T2> 
        where T1 : IAutomatonNode
        where T2 : IAutomatonTransitionDecorator<T2>
    {

        public T1 InitialNode { get; protected set; }

        public OmegaAutomaton (IAutomatonNodeFactory<T1> factory) 
            : base (factory)
        {}

        /// <summary>
        /// Sets the initial node.
        /// </summary>
        /// <param name="node">Node.</param>
        public void SetInitialNode (T1 node)
        {
            InitialNode = node;
        }
        
        public override string ToDot ()
        {
            var graphviz = new GraphvizAlgorithm<T1, ParametrizedEdge<T1, T2>> (graph);
            graphviz.FormatVertex += (object sender, FormatVertexEventArgs<T1> e) => {
                e.VertexFormatter.Label = e.Vertex.Name;
                if (this.InitialNode != null && this.InitialNode.Equals (e.Vertex))
                    e.VertexFormatter.Style = QuickGraph.Graphviz.Dot.GraphvizVertexStyle.Bold;
            };
            graphviz.FormatEdge += (object sender, FormatEdgeEventArgs<T1, ParametrizedEdge<T1, T2>> e) => {
                e.EdgeFormatter.Label.Value = string.Join (",", e.Edge.Value.ToString ());
            };
            return graphviz.Generate ();
        }
    }
}

