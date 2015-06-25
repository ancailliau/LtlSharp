﻿using System;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;

namespace LtlSharp.Buchi.LTL2Buchi
{
    public class Seminal : ILTL2Buchi
    {
        public Seminal ()
        {
        }
        
        private List<ILTLFormula> New1 (ILTLFormula f)
        {
            if (f is Until) {
                return new List<ILTLFormula> ( new [] { ((Until)f).Left });
            } else if (f is Release) {
                return new List<ILTLFormula> ( new [] { ((Release)f).Right });
            } else if (f is Disjunction) {
                return new List<ILTLFormula> ( new [] { ((Disjunction) f).Left });
            }
            throw new NotImplementedException ();
        }
        
        private List<ILTLFormula> New2 (ILTLFormula f)
        {
            if (f is Until) {
                return new List<ILTLFormula> ( new [] { ((Until)f).Right });
            } else if (f is Release) {
                return new List<ILTLFormula> ( new [] { ((Release)f).Left, ((Release)f).Right });
            } else if (f is Disjunction) {
                return new List<ILTLFormula> ( new [] { ((Disjunction) f).Right });
            }
            throw new NotImplementedException ();
        }
        
        private List<ILTLFormula> Next1 (ILTLFormula f)
        {
            if (f is Until) {
                return new List<ILTLFormula> ( new [] { f });
            } else if (f is Release) {
                return new List<ILTLFormula> ( new [] { f });
            } else if (f is Disjunction) {
                return new List<ILTLFormula> ();
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
                    New = new List<ILTLFormula> (node.Next)
                };
                nodeSet.Add (node);
                
                return Expand (new_node, nodeSet);
                
            } else {
                var eta = node.New.First ();
                node.New.Remove (eta);
                
                if (eta is Proposition | eta is Negation | eta is True | eta is False) {
                    if (eta is False | node.Old.Contains(eta.Negate())) {
                        // Current node contains a contradiction
                        return nodeSet;
                    } else {
                        node.Old.Add (eta);
                        return Expand (node, nodeSet);
                    }
                } else if (eta is Until | eta is Release | eta is Disjunction) {
                    var nlist1 = new List<ILTLFormula> (node.New);
                    nlist1.AddRange (New1(eta));
                    foreach (var old in node.Old) {
                        nlist1.Remove (old);
                    }
                    
                    var nlist2 = new List<ILTLFormula> (node.New);
                    nlist2.AddRange (New2(eta));
                    foreach (var old in node.Old) {
                        nlist2.Remove (old);
                    }
                    
                    var olist1 = new List<ILTLFormula> (node.Old);
                    olist1.Add (eta);
                    
                    var xlist1 = new List<ILTLFormula> (node.Next);
                    xlist1.AddRange (Next1 (eta));
                                        
                    var node1 = new Node () {
                        Incoming = new List<string> (node.Incoming),
                        New = nlist1,
                        Old = olist1,
                        Next = xlist1
                    };
                    
                    var node2 = new Node () {
                        Incoming = new List<string> (node.Incoming),
                        New = nlist2,
                        Old = new List<ILTLFormula> (olist1),
                        Next = new List<ILTLFormula> (node.Next)
                    };
                    
                    return Expand (node2, Expand (node1, nodeSet));
                } else if (eta is Conjunction) {
                    var ceta = (Conjunction)eta;
                    
                    var list = new List<ILTLFormula> (node.New);
                    if (!node.Old.Contains (ceta.Left))
                        list.Add (ceta.Left);
                    if (!node.Old.Contains (ceta.Right))
                        list.Add (ceta.Right);
                    
                    var n = new Node (node.Name) {
                        Incoming = new List<string> (node.Incoming),
                        New = list,
                        Old = new List<ILTLFormula> (node.Old.Union (new [] {eta})),
                        Next = new List<ILTLFormula> (node.Next)
                    };
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
                New = new List<ILTLFormula> (new [] { phi }),
            };
            
            return Expand (n, new HashSet<Node> ());
        }
    }
}

