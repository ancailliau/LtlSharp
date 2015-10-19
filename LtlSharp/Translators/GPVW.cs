using System;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;
using LtlSharp.Buchi.Translators;
using LtlSharp.Automata;
using LtlSharp.Automata.OmegaAutomata;
using LtlSharp.Automata.Transitions;
using LtlSharp.Automata.Nodes.Factories;
using LtlSharp.Automata.AcceptanceConditions;
using LtlSharp.Automata.Transitions.Decorations;

namespace LtlSharp.Buchi.LTL2Buchi
{
    public class GPVW : ILTLTranslator
    {
        public GPVW ()
        {
        }
        
        private List<ITLFormula> New1 (ITLFormula f)
        {
            if (f is Until) {
                return new List<ITLFormula> ( new [] { ((Until)f).Left });
            } else if (f is Release) {
                return new List<ITLFormula> ( new [] { ((Release)f).Right });
            } else if (f is Disjunction) {
                return new List<ITLFormula> ( new [] { ((Disjunction) f).Left });
            }
            throw new NotImplementedException ();
        }
        
        private List<ITLFormula> New2 (ITLFormula f)
        {
            if (f is Until) {
                return new List<ITLFormula> ( new [] { ((Until)f).Right });
            } else if (f is Release) {
                return new List<ITLFormula> ( new [] { ((Release)f).Left, ((Release)f).Right });
            } else if (f is Disjunction) {
                return new List<ITLFormula> ( new [] { ((Disjunction) f).Right });
            }
            throw new NotImplementedException ();
        }
        
        private List<ITLFormula> Next1 (ITLFormula f)
        {
            if (f is Until) {
                return new List<ITLFormula> ( new [] { f });
            } else if (f is Release) {
                return new List<ITLFormula> ( new [] { f });
            } else if (f is Disjunction) {
                return new List<ITLFormula> ();
            }
            throw new NotImplementedException ();
        }
        
        public HashSet<Node> Expand (Node node, HashSet<Node> nodeSet)
        {
            if (node.New.Count == 0) {
                var nd = nodeSet.FirstOrDefault (x => 
                    new HashSet<ITLFormula> (x.Old).SetEquals (node.Old)
                         & new HashSet<ITLFormula> (x.Next).SetEquals (node.Next));
                if (nd != null) {
                    foreach (var i in node.Incoming) {
                        nd.Incoming.Add (i);
                    }
                    return nodeSet;
                }
                
                var new_node = new Node () {
                    Incoming = new HashSet<string> (new[] { node.Name }),
                    New = new HashSet<ITLFormula> (node.Next)
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
                    var nlist1 = new HashSet<ITLFormula> (node.New.Union (New1(eta).Except (node.Old)));
                    var nlist2 = new HashSet<ITLFormula> (node.New.Union(New2(eta).Except (node.Old)));
                    var olist1 = new HashSet<ITLFormula> (node.Old.Union (new [] { eta }));
                    var xlist1 = new HashSet<ITLFormula> (node.Next.Union (Next1 (eta)));
                                        
                    var node1 = new Node () {
                        Incoming = new HashSet<string> (node.Incoming),
                        New = nlist1,
                        Old = olist1,
                        Next = xlist1
                    };
                    
                    var node2 = new Node () {
                        Incoming = new HashSet<string> (node.Incoming),
                        New = nlist2,
                        Old = new HashSet<ITLFormula> (olist1),
                        Next = new HashSet<ITLFormula> (node.Next)
                    };
                    
                    return Expand (node2, Expand (node1, nodeSet));
                } else if (eta is Conjunction) {
                    var ceta = (Conjunction)eta;
                    
                    var list = new HashSet<ITLFormula> (node.New);
                    if (!node.Old.Contains (ceta.Left))
                        list.Add (ceta.Left);
                    if (!node.Old.Contains (ceta.Right))
                        list.Add (ceta.Right);
                    
                    var n = new Node (node.Name) {
                        Incoming = new HashSet<string> (node.Incoming),
                        New = list,
                        Old = new HashSet<ITLFormula> (node.Old.Union (new [] { eta })),
                        Next = new HashSet<ITLFormula> (node.Next)
                    };
                    return Expand (n, nodeSet);  
                } else if (eta is Next) {
                    
                    var n = new Node (node.Name) {
                        Incoming = new HashSet<string> (node.Incoming),
                        New = new HashSet<ITLFormula> (node.New),
                        Old = new HashSet<ITLFormula> (node.Old.Union (new [] { eta })),
                        Next = new HashSet<ITLFormula> (node.Next.Union (new [] { ((Next) eta).Enclosed }))
                    };
                    
                    return Expand (n, nodeSet);
                    
                } else {
                    throw new NotImplementedException ();
                }
            }
        }
        
        public BuchiAutomaton<AutomatonNode> GetAutomaton (ITLFormula phi) {
            return GBA2BA.Transform (GetGBA (phi));
        }
        
        HashSet<Node> CreateGraph (ITLFormula phi)
        {
            var n = new Node () {
                Incoming = new HashSet<string> (new [] { "init" }),
                New = new HashSet<ITLFormula> (new [] { phi }),
            };
            
            var set = Expand (n, new HashSet<Node> ());
            
            var init = new Node () {
                Name = "init"
            };
            set.Add (init);
            
            return set;
        }
        
        public GeneralizedBuchiAutomaton<AutomatonNode> GetGBA (ITLFormula phi) {
            var formula = phi.Normalize ();

            var nodesSet = CreateGraph (formula);

            var automaton = new GeneralizedBuchiAutomaton<AutomatonNode> (
                new AutomatonNodeFactory ()
            );
            

            int i = 0;
            var mapping = new Dictionary<string, AutomatonNode> ();
            foreach (var n in nodesSet) {
                var newNode = new AutomatonNode ("s" + i);
                automaton.AddNode (newNode);
                if (n.Name == "init") {
                    automaton.SetInitialNode (newNode);
                }

                mapping.Add (n.Name, newNode);
                i++;
            }

            // Build the transitions
            foreach (Node node in nodesSet) {
                foreach (var incomingNodeName in node.Incoming) {
                    //var transition = new AutomatonTransition<AutomatonNode> (
                    //    mapping [incomingNodeName],
                    //    mapping [node.Name]
                    //);

                    var literals = new LiteralsSet ();
                    bool contradiction = false;
                    foreach (var f in node.Old) {
                        if (f is Proposition | f is Negation) {
                            if (f is Negation) {
                                if (literals.Contains (((Negation) f).Enclosed)) {
                                    contradiction = true;
                                    break;
                                }
                            }
                            literals.Add ((ILiteral) f);
                        }
                    }
                    
                    if (!contradiction) {
                        // TODO fixme
                        //transition.Labels = new LiteralsSet (literals);
                        automaton.AddTransition (mapping [incomingNodeName], mapping [node.Name], new LiteralSetDecoration (literals));
                    } else {
                        automaton.AddTransition (mapping [incomingNodeName], mapping [node.Name], new LiteralSetDecoration ());   
                    }
                    
                }
            }

            // The acceptance set contains a separate set of states for
            // each subformula of the form x U y. The set contains the
            // states n such that y in Old(n) or x U y not in Old(n).
            
            // Subformulas are processed in a DFS-fashioned way
            Stack<ITLFormula> formulasToProcess = new Stack<ITLFormula>();
            formulasToProcess.Push(formula);

            int setIndex = 0;

            while(formulasToProcess.Count > 0) {
                ITLFormula considered = formulasToProcess.Pop();

                if (considered is Until) {
                    Until consideredUntil = considered as Until;
                    var set = new HashSet<AutomatonNode>();

                    // Adds all nodes containing right member of until
                    // or not the until in their old set.
                    foreach (var q in nodesSet.Where(n => n.Old.Contains(consideredUntil.Right) 
                                                     | !n.Old.Contains(consideredUntil))) 
                    {
                        set.Add (mapping[q.Name]);
                    }

                    automaton.AcceptanceCondition.Add (setIndex, set);
                    setIndex++;

                } 

                if (considered is IBinaryOperator) {
                    formulasToProcess.Push(((IBinaryOperator) considered).Left);
                    formulasToProcess.Push(((IBinaryOperator) considered).Right);

                } else if (considered is IUnaryOperator) {
                    formulasToProcess.Push(((IUnaryOperator) considered).Enclosed);

                }
            }

            return automaton;
        }
            
        /*
        public GeneralizedBuchiAutomata GetAutomaton (ILTLFormula phi) {
            var formula = phi.Normalize ();
            
            var nodesSet = CreateGraph (formula);
            
            var automaton = new GeneralizedBuchiAutomata (nodesSet.Count);
            
            int i = 0;
            var mapping = new Dictionary<string, AutomatonNode> ();
            var literals = new HashSet<ILiteral> ();
            foreach (var n in nodesSet) {
                var newNode = new AutomatonNode ("s" + i);
                automaton.AddVertex (newNode);
                if (n.Incoming.Contains ("init"))
                    automaton.InitialNodes.Add (newNode);
                
                
                literals.Clear ();
                bool contradiction = false;
                foreach (var f in n.Old) {
                    if (f is Proposition | f is Negation) {
                        if (f is Negation) {
                            if (literals.Contains (((Negation) f).Enclosed)) {
                                contradiction = true;
                                break;
                            }
                        }
                        literals.Add ((ILiteral) f);
                    }
                }
                if (!contradiction) {
                    newNode.Labels = literals;
                }
                
                mapping.Add (n.Name, newNode);
                i++;
            }

            // Build the transitions
            foreach (Node node in nodesSet) {
                foreach (var incomingNodeName in node.Incoming.Except (new [] { "init" })) {
                    automaton.AddEdge (
                        new AutomataTransition<AutomatonNode> (
                            mapping [incomingNodeName],
                            mapping [node.Name]
                        )
                    );
                }
            }
            
            // automaton.Edges = transitions;

            // The acceptance set contains a separate set of states for
            // each subformula of the form x U y. The set contains the
            // states n such that y in Old(n) or x U y not in Old(n).
            var listAcceptanceSets = new LinkedList<GBAAcceptanceSet>();

            // Subformulas are processed in a DFS-fashioned way
            Stack<ILTLFormula> formulasToProcess = new Stack<ILTLFormula>();
            formulasToProcess.Push(formula);

            int setIndex = 0;

            while(formulasToProcess.Count > 0) {
                ILTLFormula considered = formulasToProcess.Pop();

                if (considered is Until) {
                    Until consideredUntil = considered as Until;
                    var set = new HashSet<AutomatonNode>();

                    // Adds all nodes containing right member of until
                    // or not the until in their old set.
                    foreach (var q in nodesSet.Where(n => n.Old.Contains(consideredUntil.Right) | !n.Old.Contains(consideredUntil))) 
                    {
                        set.Add (mapping[q.Name]);
                    }

                    listAcceptanceSets.AddLast(new GBAAcceptanceSet (setIndex, set.ToArray ()));
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
        */
    
    }
}

