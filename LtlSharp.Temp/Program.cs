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
using QuickGraph.Graphviz.Dot;
using CheckMyModels.Tests.Models;
using LtlSharp.ProbabilisticSystems;

namespace LtlSharp.Temp
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            var mc = TestMarkovChain.GetExample ("101");
            var delivered = mc.GetVertex ("delivered");
            var start = mc.GetVertex ("start");
            var ntry = mc.GetVertex ("try");
            var lost = mc.GetVertex ("lost");

            var dict = mc.ReachabilityLinearSystem (new [] { delivered });
            
            if (dict == null) {
                Console.WriteLine ("Failed");
            }
            
            foreach (var kv in dict) {
                Console.WriteLine (kv.Key.Name + " = " + kv.Value);
            }
        }
    }
}
