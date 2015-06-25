using System;
using System.Collections.Generic;
using System.Linq;

namespace LtlSharp.Buchi.LTL2Buchi
{
    public class Gia01 : ILTL2Buchi
    {
        public Gia01 ()
        {
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
                nodeSet.Add (node);

                return Expand (new_node, nodeSet);

            } else {
                var eta = node.New.First ();
                node.New.Remove (eta);

                if (eta is ILiteral) {
                    var notN = eta.Negate();
                    if (node.Old.Contains(notN) | notN is True) {
                        return nodeSet;

                    } else {
                        node.Old.Add (eta);
                        return Expand(node, nodeSet);
                    }

                } else if (eta is Until | eta is Release | eta is Disjunction) {
                    var n1 = new Node () {
                        Incoming = new List<string> (node.Incoming),
                        New = new ConsistentSet (node.New),
                        Old = new ConsistentSet (node.Old),
                        Next = new ConsistentSet (node.Next)
                    };
                    var n2 = new Node () {
                        Incoming = new List<string> (node.Incoming),
                        New = new ConsistentSet (node.New),
                        Old = new ConsistentSet (node.Old),
                        Next = new ConsistentSet (node.Next)
                    };

                    ILTLFormula new1 = null;
                    ILTLFormula new2 = null;
                    ILTLFormula next = null;

                    if (eta is Until) {
                        new1 = ((Until) eta).Left;
                        next = eta;
                        new2 = ((Until) eta).Right;

                    } else if (eta is Release) {
                        new1 = ((Release) eta).Right;
                        next = eta;
                        new2 = ((Release) eta).Left;

                    } else if (eta is Disjunction) {
                        new1 = ((Disjunction) eta).Left;
                        new2 = ((Disjunction) eta).Right;

                    }

                    if (!node.Old.Contains(new1)) {
                        n1.New.Add(new1);
                    }

                    if (!node.Old.Contains(new2)) {
                        n2.New.Add (new2);
                    }

                    n1.Old.Add (eta);
                    n2.Old.Add (eta);

                    if (next != null) {
                        n1.Next.Add (next);
                    }

                    return Expand(n2, Expand(n1, nodeSet));

                } else if (eta is Conjunction) {
                    var andN = (Conjunction) eta;
                    var newNode = new Node () {
                        Incoming = new List<string> (node.Incoming),
                        New = new ConsistentSet (node.New),
                        Old = new ConsistentSet (node.Old),
                        Next = new ConsistentSet (node.Next)
                    };
                    if (!node.Old.Contains(andN.Left)) {
                        newNode.New.Add (andN.Left);
                    }
                    if (!node.Old.Contains(andN.Right)) {
                        newNode.New.Add (andN.Right);
                    }
                    newNode.Old.Add (eta);

                    return Expand(newNode, nodeSet);

                } else if (eta is Next) {
                    var newNode = new Node () {
                        Incoming = new List<string> (node.Incoming),
                        New = new ConsistentSet (node.New.Union (new [] {eta})),
                        Old = new ConsistentSet (node.Old.Union (new [] {eta})),
                        Next = new ConsistentSet (node.Next)
                    };
                    return Expand(newNode, nodeSet);

                }

                throw new NotImplementedException (eta.GetType () + " is not supported.");
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

