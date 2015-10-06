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
        abstract class State {
            public abstract string Name { get; }
            public abstract State GetSuccessor ();
            public abstract MonitoredState GetMonitoredState ();
            protected static Proposition C = new Proposition ("C");
            protected static Proposition T = new Proposition ("T");
        }
        
        class State0 : State {
            public override string Name { get { return "S0"; } }
            public static State0 Singleton = new State0 ();
            Random _r = new Random ();
            public State0 ()
            {
                _state = new MonitoredState ();
                _state.Set (C, true);
                _state.Set (T, false);
            }
            public override State GetSuccessor () {
                if (_r.NextDouble () >= .321) {
                    return State0.Singleton;
                } else {
                    return State1.Singleton;
                }
            }
            MonitoredState _state;
            public override MonitoredState GetMonitoredState () {
                return _state;
            }
        }
        
        class State1 : State {
            public override string Name { get { return "S1"; } }
            public static State1 Singleton = new State1 ();
            public State1 ()
            {
                _state = new MonitoredState ();
                _state.Set (C, false);
                _state.Set (T, true);
            }
            public override State GetSuccessor () {
                return State2.Singleton;
            }
            MonitoredState _state;
            public override MonitoredState GetMonitoredState () {
                return _state;
            }
        }

        class State2 : State {
            public override string Name { get { return "S2"; } }
            public static State2 Singleton = new State2 ();
            public State2 ()
            {
                _state = new MonitoredState ();
                _state.Set (C, false);
                _state.Set (T, false);
            }
            public override State GetSuccessor () {
                return State0.Singleton;
            }
            MonitoredState _state;
            public override MonitoredState GetMonitoredState () {
                return _state;
            }
        }
        
        public static void Main (string[] args)
        {
            var c = new Proposition ("C");
            var t = new Proposition ("T");
            ILTLFormula f = new Implication (c, new Next (t));
            
//            var monitor = new LTLMonitor (f);
//            monitor.PrintDot ();
            
            State current = State0.Singleton;
            
            
            int ntry = 1000;
            int tsum = 0;
            int csum = 0;
            
            for (int k = 0; k < 10; k++) {
                int ctrue = 0;
                int cfalse = 0;
                
                var monitors = new List<LTLMonitor> (ntry);
                for (int i = 0; i < ntry; i++) {
//                    Console.Write(current.Name + " ");
                    
                    var monitor = new LTLMonitor (f);
                    monitors.Add (monitor);
                    var state = current.GetMonitoredState ();
                    current = current.GetSuccessor ();
                
                    for (int j = 0; j <= i; j++) {
                        var stateBefore = monitors [j].Status;
                        monitors [j].Step (state);
//                        Console.Write (StateToStr(monitors[j].Status) + " ");
                        
                        if (stateBefore == MonitorStatus.Inconclusive & j != i) {
                            if (monitors[j].Status == MonitorStatus.True) {
//                                Console.WriteLine ("T");
                                ctrue++;
                            } else if (monitors[j].Status == MonitorStatus.False) {
//                                Console.WriteLine ("F");
                                cfalse++;
                            }
                        }
                    }
//                    Console.WriteLine ();
                }
                
                csum += cfalse;
                tsum += ctrue;
//            for (int i = 0; i < monitors.Count; i++) {
//                Console.WriteLine ("s["+i+"] = " + monitors[i].Status);
//            }
//                Console.WriteLine (ctrue);
//                Console.WriteLine (cfalse);
                Console.WriteLine (Math.Round ((double) ctrue / (ctrue + cfalse), 3));
            
            }
            Console.WriteLine ("-");
            Console.WriteLine (Math.Round ((double) tsum / (tsum + csum), 3));
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
        
        static string StateToStr (MonitorStatus status) {
            if (status == MonitorStatus.False) return "F";
            if (status == MonitorStatus.True) return "T";
            if (status == MonitorStatus.Inconclusive) return "I";
            throw new NotImplementedException ();
        }
    }
}
