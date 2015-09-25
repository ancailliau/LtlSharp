using System;
using LittleSharp.Buchi;
using LtlSharp.Buchi.LTL2Buchi;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;
using LtlSharp.Buchi.Translators;
using LtlSharp.Buchi.Automata;
using LtlSharp.Monitoring;
using QuickGraph.Graphviz;

namespace LtlSharp.Temp
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            var spawn = new Proposition ("spawn");
            var init = new Proposition ("init");
            ILTLFormula f = new Until (spawn.Negate (), init);

            //var ap = new Proposition ("a");
            //var b = new Proposition ("b");
            //var c = new Proposition ("c");
            //f = new Until (ap, new Until (b, c)); // .Negate ();
            
            var translator = new Gia02 ();
            var a = translator.GetAutomaton (f);
            
            
            //Console.WriteLine ("-----");
            //var graphviz = new GraphvizAlgorithm<AutomataNode,LabeledAutomataTransition<AutomataNode>>(a);
            //graphviz.FormatVertex += (object sender, FormatVertexEventArgs<AutomataNode> e) => {
            //    e.VertexFormatter.Label = e.Vertex.Name;
            //    if (a.InitialNodes.Contains (e.Vertex)) {
            //        e.VertexFormatter.FillColor = QuickGraph.Graphviz.Dot.GraphvizColor.LightYellow;
            //    }
            //};
            //graphviz.FormatEdge += (object sender, FormatEdgeEventArgs<AutomataNode, LabeledAutomataTransition<AutomataNode>> e) => {
            //    e.EdgeFormatter.Label.Value = string.Join (",", e.Edge.Labels);
            //    if (a.AcceptanceSets.Any (acc => acc.Transitions.Contains (e.Edge))) {
            //        e.EdgeFormatter.Style = QuickGraph.Graphviz.Dot.GraphvizEdgeStyle.Dashed;
            //    }
            //};
            //string output = graphviz.Generate();
            //Console.WriteLine (output);
            
            
            /*
            var spawn = new Proposition ("spawn");
            var init = new Proposition ("init");
            
            var f = new Until (spawn.Negate (), init);
            var m = new LTLMonitor (f);

            var state = new MonitoredState ();
            state.Set (spawn, false);
            state.Set (init, false);
            
            Console.WriteLine (m.Status);
            m.Consume (state);
            Console.WriteLine (m.Status);
            
            state.Set (spawn, true);
            m.Consume (state);
            Console.WriteLine (m.Status);
            
            state.Set (init, true);
            m.Consume (state);
            Console.WriteLine (m.Status);
            */
            
        }
    }
}
