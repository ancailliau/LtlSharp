using System;
using LtlSharp.Buchi;
using LtlSharp.Models;
using System.Collections.Generic;
using LtlSharp.Buchi.Automata;
using System.Linq;
using LtlSharp.Automata;

namespace LtlSharp.Translators
{
    public static class ProductAutomata
    {
        public static MarkovChain Product (this MarkovChain mc, BuchiAutomata ba, IEnumerable<MarkovNode> initials, 
            out HashSet<MarkovNode> condition,
            out Dictionary<MarkovNode, MarkovNode> mapping2)
        {
            var mapping = new Dictionary<Tuple<MarkovNode,AutomataNode>, MarkovNode> ();
            mapping2 = new Dictionary<MarkovNode, MarkovNode> ();
            condition = new HashSet<MarkovNode> ();
            
            var accept = new Proposition ("Accept");
            var naccept = new Negation (accept);

            int i = 0;

            var product = new MarkovChain ();
            var pending = new Stack<Tuple<MarkovNode,AutomataNode>> ();

            var initBA = ba.InitialNodes.Single ();
            foreach (var initial in initials) {
                //Console.WriteLine ("initial " + initial.Name);

                //Console.WriteLine ("{"+string.Join (",", initial.Labels.Select (x => x.ToString ()))+"}");
                //            foreach (var sss in ba.OutEdges (initBA)) {
                //Console.WriteLine ("{"+string.Join (",", sss.Labels.Select (x => x.ToString ()))+"}");
                //            }

                AutomataNode succBA = ba.OutEdges (initBA).SingleOrDefault (e => e.Labels.IsSubsetOf (initial.Labels))?.Target;

                if (succBA != null) {
                    var n = product.AddVertex (/*"s" + (i++)*/ initial.Name + " x (" + succBA.Name + ")" );
                    if (ba.AcceptanceSet.Contains (succBA)) {
                        condition.Add (n);
                    }

                    var tuple = new Tuple<MarkovNode, AutomataNode> (initial, succBA);
                    mapping.Add (tuple, n);
                    pending.Push (tuple);
                    mapping2.Add (initial, n);
                    
//                    product.SetInitial (n, 1); // initial.Value);
                }
            }

            ////Console.WriteLine (product.Nodes.Count ());

            while (pending.Count > 0) {
                var current = pending.Pop ();
                var currentMC = current.Item1;
                var currentBA = current.Item2;
                var currentProduct = mapping [current];
                //Console.WriteLine ("currentMC " + currentMC.Name);
                //Console.WriteLine ("currentBA " + currentBA.Name);

                foreach (var succMC in mc.Post (currentMC)) {
                    // var succBA = ba.Post (initBA, succMC.Labels);
                    //Console.WriteLine ("{"+string.Join (",", succMC.Labels)+"}");

                    var succBA = ba.OutEdges (currentBA).SingleOrDefault (e => e.Labels.IsSubsetOf (succMC.Labels))?.Target;
                    if (succBA != null) {

                        MarkovNode n;
                        var tuple = new Tuple<MarkovNode, AutomataNode> (succMC, succBA);

                        if (!pending.Contains (tuple) & !mapping.ContainsKey (tuple)) {
                            pending.Push (tuple);
                        }

                        if (!mapping.ContainsKey (tuple)) {
                            n = product.AddVertex ( succMC.Name + " x (" + succBA.Name + ")"  /* "s" + (i++) */ );
                            if (ba.AcceptanceSet.Contains (succBA)) {
                                condition.Add (n);
                            }

                            mapping.Add (tuple, n);
                        } else {
                            n = mapping [tuple];
                        }

                        product.AddEdge (currentProduct, mc.GetProbability (currentMC, succMC), n);
                    } else {
                        //Console.WriteLine ("no succ in ba");
                    }
                }
            }

            return product;
        }
        
        public static MarkovChain Product (this MarkovChain mc, RabinAutomata rabin, IEnumerable<MarkovNode> initials, 
            out IEnumerable<RabinCondition<MarkovNode>> conditions,
            out Dictionary<MarkovNode, MarkovNode> mapping2)
        {
            var mapping = new Dictionary<Tuple<MarkovNode,AutomataNode>, MarkovNode> ();
            mapping2 = new Dictionary<MarkovNode, MarkovNode> ();
            
//            var accept = new Proposition ("Accept");
//            var naccept = new Negation (accept);
            
            
            int i = 0;
            
            var product = new MarkovChain ();
            var pending = new Stack<Tuple<MarkovNode,AutomataNode>> ();
            
            var initBA = rabin.InitialNodes.Single ();
            foreach (var initial in initials) {
                //Console.WriteLine ("initial " + initial.Name);
//            
//            Console.WriteLine ("* {"+string.Join (",", initial.Labels.Select (x => x.ToString ()))+"}");
//                foreach (var sss in rabin.OutEdges (initBA)) {
//                    Console.WriteLine ("{"+string.Join (",", sss.Labels.Select (x => x.ToString ()))+"} to " + sss.Target.Name);
//            }
//            
                AutomataNode succBA = rabin.OutEdges (initBA).SingleOrDefault (e => e.Labels.IsSubsetOf (initial.Labels))?.Target;
                
                if (succBA != null) {
                    var n = product.AddVertex (/*"s" + (i++)*/ initial.Name + " x (" + succBA.Name + ")" );
//                    n.Labels = succBA;
//                    if (rabin.AcceptanceSet.Contains (succBA)) {
//                        n.Labels.Add (accept);
//                    } else {
//                        n.Labels.Add (naccept);
//                    }
                    
                    var tuple = new Tuple<MarkovNode, AutomataNode> (initial, succBA);
                    mapping.Add (tuple, n);
                    pending.Push (tuple);
                    
                    mapping2.Add (initial, n);
                    
                    // We are cheating here, as we have no inital node in fact. 
                    // product.SetInitial (n, 1); // initial.Value);
                }
            }
            
            ////Console.WriteLine (product.Nodes.Count ());
            
            while (pending.Count > 0) {
                var current = pending.Pop ();
                var currentMC = current.Item1;
                var currentBA = current.Item2;
                var currentProduct = mapping [current];
                //Console.WriteLine ("currentMC " + currentMC.Name);
                //Console.WriteLine ("currentBA " + currentBA.Name);
                
                foreach (var succMC in mc.Post (currentMC)) {
                    // var succBA = ba.Post (initBA, succMC.Labels);
                    //Console.WriteLine ("{"+string.Join (",", succMC.Labels)+"}");
                    
                    var succBA = rabin.OutEdges (currentBA).SingleOrDefault (e => e.Labels.IsSubsetOf (succMC.Labels))?.Target;
                    if (succBA != null) {
                        
                        MarkovNode n;
                        var tuple = new Tuple<MarkovNode, AutomataNode> (succMC, succBA);

                        if (!pending.Contains (tuple) & !mapping.ContainsKey (tuple)) {
                            pending.Push (tuple);
                        }
                        
                        if (!mapping.ContainsKey (tuple)) {
                            n = product.AddVertex ( succMC.Name + " x (" + succBA.Name + ")"  /* "s" + (i++) */ );
//                            if (rabin.AcceptanceSet.Contains (succBA)) {
//                                n.Labels.Add (accept);
//                            } else {
//                                n.Labels.Add (naccept);
//                            }
                            
                            mapping.Add (tuple, n);
                        } else {
                            n = mapping [tuple];
                        }
                        
                        product.AddEdge (currentProduct, mc.GetProbability (currentMC, succMC), n);
                    } else {
                        //Console.WriteLine ("no succ in ba");
                    }
                }
            }
            
            var cond = new HashSet<RabinCondition<MarkovNode>> ();
            foreach (var ac in rabin.AcceptanceSet) {
                var nc = new RabinCondition<MarkovNode> ();
                foreach (var n in mapping) {
                    var mcNode = n.Key.Item1;
                    var raNode = n.Key.Item2;
                    var prNode = n.Value;
                    if (ac.E.Contains (raNode)) {
                        nc.E.Add (prNode);
                    }
                    if (ac.F.Contains (raNode)) {
                        nc.F.Add (prNode);
                    }
                }
                cond.Add (nc);
            }
            conditions = cond;
            
            return product;
        }
    }
}

