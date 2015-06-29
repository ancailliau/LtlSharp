using System;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;

namespace LtlSharp.Buchi.LTL2Buchi
{
    public class GPVW : ILTL2Buchi
    {
        public GPVW ()
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
                    foreach (var i in node.Incoming) {
                        nd.Incoming.Add (i);
                    }
                    return nodeSet;
                }
                
                var new_node = new Node () {
                    Incoming = new HashSet<string> (new[] { node.Name }),
                    New = new HashSet<ILTLFormula> (node.Next)
                };
                nodeSet.Add (node);
                
                return Expand (new_node, nodeSet);
                
            } else {
                var eta = node.New.First ();
                node.New.Remove (eta);
                
                if (eta is Proposition | eta is Negation | eta is True | eta is False) {
                    if (eta is False | node.Old.Contains (eta.Negate ())) {
                        // Current node contains a contradiction
                        return nodeSet;
                    } else {
                        node.Old.Add (eta);
                        return Expand (node, nodeSet);
                    }
                } else if (eta is Until | eta is Release | eta is Disjunction) {
                    var nlist1 = new HashSet<ILTLFormula> (node.New.Union (New1(eta).Except (node.Old)));
                    var nlist2 = new HashSet<ILTLFormula> (node.New.Union(New2(eta).Except (node.Old)));
                    var olist1 = new HashSet<ILTLFormula> (node.Old.Union (new [] { eta }));
                    var xlist1 = new HashSet<ILTLFormula> (node.Next.Union (Next1 (eta)));
                                        
                    var node1 = new Node () {
                        Incoming = new HashSet<string> (node.Incoming),
                        New = nlist1,
                        Old = olist1,
                        Next = xlist1
                    };
                    
                    var node2 = new Node () {
                        Incoming = new HashSet<string> (node.Incoming),
                        New = nlist2,
                        Old = new HashSet<ILTLFormula> (olist1),
                        Next = new HashSet<ILTLFormula> (node.Next)
                    };
                    
                    return Expand (node2, Expand (node1, nodeSet));
                } else if (eta is Conjunction) {
                    var ceta = (Conjunction)eta;
                    
                    var list = new HashSet<ILTLFormula> (node.New);
                    if (!node.Old.Contains (ceta.Left))
                        list.Add (ceta.Left);
                    if (!node.Old.Contains (ceta.Right))
                        list.Add (ceta.Right);
                    
                    var n = new Node (node.Name) {
                        Incoming = new HashSet<string> (node.Incoming),
                        New = list,
                        Old = new HashSet<ILTLFormula> (node.Old.Union (new [] { eta })),
                        Next = new HashSet<ILTLFormula> (node.Next)
                    };
                    return Expand (n, nodeSet);  
                } else if (eta is Next) {
                    
                    var n = new Node (node.Name) {
                        Incoming = new HashSet<string> (node.Incoming),
                        New = new HashSet<ILTLFormula> (node.New),
                        Old = new HashSet<ILTLFormula> (node.Old.Union (new [] { eta })),
                        Next = new HashSet<ILTLFormula> (node.Next.Union (new [] { ((Next) eta).Enclosed }))
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
                Incoming = new HashSet<string> (new [] { "init" }),
                New = new HashSet<ILTLFormula> (new [] { phi }),
            };
            
            return Expand (n, new HashSet<Node> ());
        }
        
        public GBA3 GetAutomaton (ILTLFormula phi) {
            var formula = phi.Normalize ();
            
            var nodesSet = CreateGraph (formula);
            
            var automaton = new GBA3 (nodesSet.Count);
            
            int i = 0;
            var mapping = new Dictionary<string, int> ();
            foreach (var n in nodesSet) {
                automaton.Nodes[i] = new GBA3Node (i, n.Name, n.Incoming.Contains ("init"));
                automaton.Transitions [i] = new List<int> ();
                mapping.Add (n.Name, i);
                i++;
            }

            // Build the transitions
            var literals = new HashSet<ILiteral> ();
            foreach (Node node in nodesSet) {
                foreach (var incomingNodeName in node.Incoming.Except (new [] { "init" })) {
                    literals.Clear ();
                    automaton.Transitions[mapping[incomingNodeName]].Add (mapping[node.Name]);

                    foreach (var f in node.Old) {
                        if (f is Proposition | f is Negation) {
                            literals.Add ((ILiteral) f);
                        }
                    }
                    
                    if (literals.Count == 0)
                        literals.Add (new True ());
                    
                    automaton.Labels [mapping [incomingNodeName]] = literals.ToList ();
                }
            }

            // The acceptance set contains a separate set of states for
            // each subformula of the form x U y. The set contains the
            // states n such that y in Old(n) or x U y not in Old(n).
            var listAcceptanceSets = new LinkedList<AcceptanceSet>();

            // Subformulas are processed in a DFS-fashioned way
            Stack<ILTLFormula> formulasToProcess = new Stack<ILTLFormula>();
            formulasToProcess.Push(formula);

            int setIndex = 0;

            while(formulasToProcess.Count > 0) {
                ILTLFormula considered = formulasToProcess.Pop();

                if (considered is Until) {
                    Until consideredUntil = considered as Until;
                    var set = new HashSet<int>();

                    // Adds all nodes containing right member of until
                    // or not the until in their old set.
                    foreach (var q in nodesSet.Where(n => n.Old.Contains(consideredUntil.Right) | !n.Old.Contains(consideredUntil))) 
                    {
                        set.Add (mapping[q.Name]);        
                    }

                    listAcceptanceSets.AddLast(new AcceptanceSet (setIndex, set.ToArray ()));
                    setIndex++;

                } 

                if (considered is IBinaryOperator) {
                    formulasToProcess.Push(((IBinaryOperator) considered).Left);
                    formulasToProcess.Push(((IBinaryOperator) considered).Right);

                } else if (considered is IUnaryOperator) {
                    formulasToProcess.Push(((IUnaryOperator) considered).Enclosed);

                }
            }
            automaton.AcceptanceSets = listAcceptanceSets.ToArray ();
            
            return automaton;
        }
    }
}

