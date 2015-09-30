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

namespace LtlSharp.Temp
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            var c = new Proposition ("C");
            var t = new Proposition ("T");
            ILTLFormula f = new Globally (new Implication (c, new Finally (t)));
            
            var nspawn = new Negation (new Proposition ("spawn"));
            var init = new Proposition ("init");
            f = new Until (nspawn, init);

            var ba = BA2NFA.Transform (new Gia02 ().GetAutomaton (f));

            //ba = ba.Unfold ();
            //ba = ba.Determinize ();
            
            //Console.WriteLine ("s****");
            
            //ba.Fold ();


            var monitor = new LTLMonitor (f);
            

            //return;
            
            monitor.PrintDot ();
            
            
            
//
//            var s0 = new MonitoredState ();
//            s0.Set (c, true);
//            s0.Set (t, false);
//            
//            var s1 = new MonitoredState ();
//            s1.Set (c, false);
//            s1.Set (t, true);
//            
//            Console.WriteLine ("({0}) x ({1}) : {2}", monitor.currentPositive, monitor.currentNegative, monitor.Status);
//            
//            monitor.Step (s0);
//            Console.WriteLine ("({0}) x ({1}) : {2}", monitor.currentPositive, monitor.currentNegative, monitor.Status);
//
//            monitor.Step (s0);
//            Console.WriteLine ("({0}) x ({1}) : {2}", monitor.currentPositive, monitor.currentNegative, monitor.Status);
//
//            monitor.Step (s1);
//            Console.WriteLine ("({0}) x ({1}) : {2}", monitor.currentPositive, monitor.currentNegative, monitor.Status);
//
//            monitor.Step (s0);
//            Console.WriteLine ("({0}) x ({1}) : {2}", monitor.currentPositive, monitor.currentNegative, monitor.Status);
//
//            monitor.Step (s1);
//            Console.WriteLine ("({0}) x ({1}) : {2}", monitor.currentPositive, monitor.currentNegative, monitor.Status);
//
//            monitor.Step (s0);
//            Console.WriteLine ("({0}) x ({1}) : {2}", monitor.currentPositive, monitor.currentNegative, monitor.Status);
//
//            monitor.Step (s0);
//            Console.WriteLine ("({0}) x ({1}) : {2}", monitor.currentPositive, monitor.currentNegative, monitor.Status);
//
//            monitor.Step (s0);
//            Console.WriteLine ("({0}) x ({1}) : {2}", monitor.currentPositive, monitor.currentNegative, monitor.Status);
        }
    }
}
