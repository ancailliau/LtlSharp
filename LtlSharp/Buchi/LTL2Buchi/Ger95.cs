using System;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;

namespace LtlSharp.Buchi.LTL2Buchi
{
    public class Ger95 : ILTL2Buchi
    {
        public Ger95 ()
        {
        }
        
        private ConsistentSet New1 (ILTLFormula f)
        {
            if (f is Until) {
                return new ConsistentSet ( new [] { ((Until)f).Left });
            } else if (f is Release) {
                return new ConsistentSet ( new [] { ((Release)f).Right });
            } else if (f is Disjunction) {
                return new ConsistentSet ( new [] { ((Disjunction) f).Left });
            }
            throw new NotImplementedException ();
        }
        
        private ConsistentSet New2 (ILTLFormula f)
        {
            if (f is Until) {
                return new ConsistentSet ( new [] { ((Until)f).Right });
            } else if (f is Release) {
                return new ConsistentSet ( new [] { ((Release)f).Left, ((Release)f).Right });
            } else if (f is Disjunction) {
                return new ConsistentSet ( new [] { ((Disjunction) f).Right });
            }
            throw new NotImplementedException ();
        }
        
        private ConsistentSet Next1 (ILTLFormula f)
        {
            if (f is Until) {
                return new ConsistentSet ( new [] { f });
            } else if (f is Release) {
                return new ConsistentSet ( new [] { f });
            } else if (f is Disjunction) {
                return new ConsistentSet ();
            }
            throw new NotImplementedException ();
        }
        
        public HashSet<Node> Expand (Node node, HashSet<Node> nodeSet)
        {
            if (node.New.Count == 0) {
                var nd = nodeSet.FirstOrDefault (x => 
                    new HashSet<ILTLFormula> (x.Old).SetEquals (node.Old)
                         & new HashSet<ILTLFormula> (x.Next).SetEquals (node.Next));
                if (nd != null) {
                    nd.Incoming.AddRange (node.Incoming);
                    return nodeSet;
                }
                
                var new_node = new Node () {
                    Incoming = new List<string> (new[] { node.Name }),
                    New = new ConsistentSet (node.Next)
                };
                if (nodeSet.Add (node)) {
                    return Expand (new_node, nodeSet);   
                } else {
                    return nodeSet; // Semantically incoherent node
                }
                
            } else {
                var eta = node.New.First ();
                node.New.Remove (eta);
                
                if (eta is Proposition | eta is Negation | eta is True | eta is False) {
                    if (eta is False | node.Old.Contains (eta.Negate ())) {
                        // Current node contains a contradiction
                        return nodeSet;
                    } else {
                        if (node.Old.Add (eta)) {
                            return Expand (node, nodeSet);
                        } else {
                            return nodeSet; // Semantically incoherent node
                        }
                    }
                } else if (eta is Until | eta is Release | eta is Disjunction) {
                    
                    // Optimization
                    if (eta is Until) {
                        var ueta = (Until)eta;
                        if (node.Old.Contains(ueta.Right) | node.New.Contains (ueta.Right)) {
                            node.Old.Add (eta);
                            return Expand (node, nodeSet);
                        }
                    }
                    
                    // Optimization
                    if (eta is Release) {
                        var ueta = (Until)eta;
                        if ((node.Old.Contains(ueta.Right) | node.New.Contains (ueta.Right))
                            & (node.Old.Contains(ueta.Left) | node.New.Contains (ueta.Left))) {
                            node.Old.Add (eta);
                            return Expand (node, nodeSet);
                        }
                    }
                    
                    node.New.AddRange (New1 (eta));
                    foreach (var old in node.Old) {
                        node.New.Remove (old);
                    }
                    
                    var nlist2 = new ConsistentSet (node.New);
                    nlist2.AddRange (New2 (eta));
                    foreach (var old in node.Old) {
                        nlist2.Remove (old);
                    }
                    
                    if (!node.Old.Add (eta) | !node.Next.AddRange (Next1 (eta))) {
                        return nodeSet; // Semantically incoherent node
                    }
                                        
                    var node2 = new Node () {
                        Incoming = new List<string> (node.Incoming),
                        New = nlist2,
                        Old = new ConsistentSet (node.Old),
                        Next = new ConsistentSet (node.Next)
                    };
                    
                    return Expand (node2, Expand (node, nodeSet));
                } else if (eta is Conjunction) {
                    var ceta = (Conjunction)eta;
                    
                    if (!node.Old.Contains (ceta.Left)) {
                        if (!node.New.Add (ceta.Left)) { return nodeSet; }
                    }
                    if (!node.Old.Contains (ceta.Right)) {
                        if (!node.New.Add (ceta.Right)) { return nodeSet; }
                    }
                    if (!node.Old.Add (eta)) { return nodeSet; }
                    
                    return Expand (node, nodeSet);  
                } else if (eta is Next) {
                    
                    var n = new Node (node.Name) {
                        Incoming = new List<string> (node.Incoming),
                        New = new ConsistentSet (node.New),
                        Old = new ConsistentSet (node.Old),
                        Next = new ConsistentSet (node.Next)
                    };
                    if (!node.Old.Add (eta)) { return nodeSet; }
                    if (!node.Next.Add (((Next) eta).Enclosed)) { return nodeSet; }
                    
                    return Expand (n, nodeSet);
                    
                } else {
                    throw new NotImplementedException ();
                }
            }
        }
        
        public HashSet<Node> CreateGraph (ILTLFormula phi)
        {
            var n = new Node () {
                Incoming = new List<string> (new [] { "init" }),
                New = new ConsistentSet (new [] { phi }),
            };
            
            return Expand (n, new HashSet<Node> ());
        }
    }
}

