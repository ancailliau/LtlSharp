using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using LtlSharp.Automata;
using LtlSharp.Buchi.Automata;
using QuickGraph.Graphviz;

namespace LtlSharp.Buchi.LTL2Buchi
{
    public class InternalNode
    {
        public int NodeId {
            get;
            set;
        }

        public HashSet<int> Incoming {
            get;
            set;
        }

        public HashSet<ITLFormula> ToBeDone {
            get;
            set;
        }

        public BitArray Untils {
            get;
            set;
        }

        public BitArray RightOfUntils {
            get;
            set;
        }

        public HashSet<ILiteral> Old {
            get;
            set;
        }

        private HashSet<ITLFormula> _next ;
        public HashSet<ITLFormula> Next {
            get {
                return _next;
            }
            set {
                if (value == null) {
                    throw new NotImplementedException ();
                }
                _next = value;
            }
        }

        static int currentId = 1;
        
        static Dictionary<ITLFormula, int> indexUntils;
        static Dictionary<ITLFormula, int> rightOfWhichUntils;
        
        public InternalNode (int numberOfUntils) : this (currentId++, numberOfUntils) 
        {}

        public InternalNode (int id, int numberOfUntils)
        {
            NodeId = id;
            Incoming = new HashSet<int> ();
            ToBeDone = new HashSet<ITLFormula> ();
            Untils = new BitArray (numberOfUntils);
            RightOfUntils = new BitArray (numberOfUntils);
            Old = new HashSet<ILiteral> ();
            Next = new HashSet<ITLFormula> ();
        }
        
        public static InternalNode CreateInitial (ITLFormula phi)
        {
            int numberOfUntils = 0;
            rightOfWhichUntils = new Dictionary<ITLFormula, int> ();
            indexUntils = new Dictionary<ITLFormula, int> ();
            Stack<ITLFormula> toProcess = new Stack<ITLFormula> ();
            toProcess.Push (phi);
            while (toProcess.Count > 0) {
                var current = toProcess.Pop ();
                if (current is Until && !indexUntils.ContainsKey (current)) {
                    indexUntils.Add (current, numberOfUntils);
                    var currentUntil = (Until)current;
                    rightOfWhichUntils.Add (currentUntil.Right, numberOfUntils);
                    numberOfUntils++;
                }
                
                if (current is IBinaryOperator) {
                    toProcess.Push (((IBinaryOperator) current).Left);
                    toProcess.Push (((IBinaryOperator) current).Right);
                }
                if (current is IUnaryOperator) {
                    toProcess.Push (((IUnaryOperator)current).Enclosed);
                }
            }
            
            var node = new InternalNode (0, numberOfUntils);
            node.DecomposeAndForNext (phi);
            return node;
        }
        
        void DecomposeAndForNext (ITLFormula f)
        {
            if (f is Conjunction) {
                var conj = (Conjunction)f;
                DecomposeAndForNext (conj.Left);
                DecomposeAndForNext (conj.Right);
                
            } else if (!SI(f, null, Next)) {
                Next.Add (f);
            }
        }

        public HashSet<InternalState> Expand (HashSet<InternalState> states)
        {
            if (ToBeDone.Count == 0) {
                
                InternalState candidate;
                
                if ((candidate = states.SingleOrDefault (s => s.Next.SetEquals(Next))) != null) {
                    candidate.Merge (this);
                    return states;
                
                } else {
                    var newState = new InternalState (this);
                    states.Add (newState);
                    var newNode = new InternalNode (Untils.Length) {
                        Incoming = new HashSet<int> (new [] { NodeId }),
                        ToBeDone = new HashSet<ITLFormula> (Next)
                    };
                    return newNode.Expand (states);
                }
            } else {
                var nextFormula = ToBeDone.First ();
                ToBeDone.Remove (nextFormula);
                
                int rightOfWhichUntil = -1;
                if ((rightOfWhichUntil = RightOfWhichUntils(nextFormula)) >= 0) {
                    RightOfUntils.Set (rightOfWhichUntil, true);
                }
                
                if (testForContradictions(nextFormula)) {
                    return states;
                }
                
                if (isRedundant(nextFormula)) {
                    return Expand(states);
                }
                
                if (nextFormula is Until) {
					Untils.Set (indexUntils[nextFormula], true);
                }
                
                if (!(nextFormula is ILiteral)) {
                    if (nextFormula is Until | nextFormula is Release | nextFormula is Disjunction) {
                        var node2 = Split (nextFormula);
                        return node2.Expand (Expand (states));
                    } else if (nextFormula is Next) {
                        DecomposeAndForNext (((Next)nextFormula).Enclosed);
                        return Expand (states);
                        
                    } else if (nextFormula is Conjunction) {
                        var nextConjunction = (Conjunction) nextFormula;
                        
                        if (!Old.Contains(nextConjunction.Left))
                            ToBeDone.Add(nextConjunction.Left);
                        
                        if (!Old.Contains(nextConjunction.Right))
                            ToBeDone.Add(nextConjunction.Right);
                        
                        return Expand (states);
                    }
                    
                    throw new NotImplementedException ();
                } else {
                    Old.Add ((ILiteral)nextFormula);
                    return Expand(states);
                }
            }
        }
        
        int RightOfWhichUntils (ITLFormula f) {
            if (rightOfWhichUntils.ContainsKey (f)) {
                return rightOfWhichUntils[f];
            }
            return -1;
        }
        
        bool testForContradictions (ITLFormula f) {
            return SI(f.Negate(), Old, Next);
        }
        
        bool isRedundant (ITLFormula f) {
            return SI(f, Old, Next);
        }
        
        bool SI (ITLFormula f, HashSet<ILiteral> A, HashSet<ITLFormula> B) {
            if (f is True) 
                return true;

            if (f is ILiteral && A != null && A.Contains ((ILiteral)f)) {
                return true;
            }
            
            if (! (f is ILiteral)) {

                var form1 = GetNew1 (f);
                var form2 = GetNew2 (f);
                var form3 = GetNext (f);

                bool condition1, condition2, condition3;
                
                if (form2 != null) {
                    condition2 = SI (form2, A, B);
                } else {
                    condition2 = true;
                }
                
                if (form1 != null) {
                    condition1 = SI (form1, A, B);
                } else {
                    condition1 = true;
                }

                if (form3 != null) {
                    if (B != null) {
                        condition3 = B.Contains(form3);
                    } else {
                        condition3 = false;
                    }
                } else {
                    condition3 = true;
                }
                
                if (f is Until | f is Unless) {
                    return condition2 || (condition1 && condition3);
                }
                
                if (f is Next) {
                    return (form1 != null) ? (Next?.Contains (form1) ?? false) : false;
                }
                
                if (f is Release)
                    return ((condition1 && condition2) || (condition1 && condition3));
                
                if (f is Conjunction)
                    return (condition2 && condition1);
            }

            return false;
        }
        
        ITLFormula GetNew1 (ITLFormula f)
        {
            if (f is Release) {
                return ((Release)f).Right;
                
            } 
            
            if (f is IUnaryOperator) {
              return ((IUnaryOperator)f).Enclosed;
                
            }
            
            if (f is IBinaryOperator) {
                return ((IBinaryOperator)f).Left;
            }

            return null;
        }
        
        ITLFormula GetNew2 (ITLFormula f)
        {
            if (f is Release) {
                return ((Release)f).Left;
            } else if (f is IBinaryOperator) {
                return ((IBinaryOperator)f).Right;
            } else {
                return null;
            }
        }
        
        ITLFormula GetNext (ITLFormula f)
        {
            if (f is Release | f is Until) {
                return f;
            }
            return null;
        }
        
        InternalNode Split (ITLFormula form)
        {
            var toBeDone = new HashSet<ITLFormula> (ToBeDone);
            var tempFormula = GetNew2 (form);
            if (tempFormula != null && !Old.Contains (tempFormula)) {
                toBeDone.Add (tempFormula);
            }
            
            if (form is Release) {
                tempFormula = GetNew1 (form);
                if (!Old.Contains (tempFormula)) {
                    toBeDone.Add (tempFormula);
                }
            }
            
            var newNode = new InternalNode (Untils.Length) {
                Incoming = new HashSet<int> (Incoming),
                ToBeDone = new HashSet<ITLFormula> (toBeDone),
                Old = new HashSet<ILiteral> (Old),
                Next = new HashSet<ITLFormula> (Next),
                Untils = new BitArray (Untils),
                RightOfUntils = new BitArray (RightOfUntils)
            };
            
            toBeDone = new HashSet<ITLFormula> (ToBeDone);
            tempFormula = GetNew1 (form);
            if (tempFormula != null && !Old.Contains(tempFormula)) 
                toBeDone.Add (tempFormula);
            
            ToBeDone = toBeDone;
            if ((tempFormula = GetNext (form)) != null) {
                DecomposeAndForNext (tempFormula);
            }
            
            return newNode;
        }
    }

    public class InternalState
    {
        public int StateId {
            get;
            set;
        }

        public HashSet<InternalTransition> Transitions {
            get;
            set;
        }

        public HashSet<ITLFormula> Next {
            get;
            set;
        }
        
        public InternalState ()
        {
            Transitions = new HashSet<InternalTransition> ();
            Next = new HashSet<ITLFormula> ();
        }

        public InternalState (InternalNode node) : this ()
        {
            StateId = node.NodeId;
            var acc = new BitArray(node.RightOfUntils).Not ().And (node.Untils);
            Transitions = new HashSet<InternalTransition> (new [] {
                new InternalTransition (acc.Length) {
                    Source = new HashSet<int> (node.Incoming),
                    Label = new HashSet<ILiteral> (node.Old),
                    Accepting = acc
                }
            });
            Next = new HashSet<ITLFormula> (node.Next);
        }

        public void Merge (InternalNode node)
        {
            var acc = new BitArray(node.RightOfUntils).Not ().And (node.Untils);
            
            InternalTransition tr;
            if ((tr = Transitions.SingleOrDefault (t => t.Label.SetEquals(node.Old) &(t.Accepting.Equals (acc)))) != null) {
                foreach (var i in node.Incoming)
                    tr.Source.Add (i);
            
            } else {
                Transitions.Add (new InternalTransition (acc.Length) {
                    Source = new HashSet<int> (node.Incoming),
                    Label = new HashSet<ILiteral> (node.Old),
                    Accepting = acc
                });
            }
                
        }
        
        public override string ToString ()
        {
            return string.Format ("[InternalState: StateId={0}, Transitions={1}, Next={2}]", StateId, Transitions, Next);
        }
        
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(InternalState))
                return false;
            InternalState other = (InternalState)obj;
            return StateId == other.StateId & Transitions.SetEquals(other.Transitions) & Next.Equals(other.Next);
        }

        public override int GetHashCode ()
        {
            return 17 + 23 * (StateId.GetHashCode () + 23 * (Transitions.GetHashCode () + 23 * Next.GetHashCode ())) ;
        }
    }

    public class InternalTransition
    {
        public HashSet<int> Source {
            get;
            set;
        }

        public HashSet<ILiteral> Label {
            get;
            set;
        }

        public BitArray Accepting {
            get;
            set;
        }

        public InternalTransition (int numberOfUntils)
        {
            Source = new HashSet<int> ();
            Label = new HashSet<ILiteral> ();
            Accepting = new BitArray (numberOfUntils);
        }
        
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(InternalTransition))
                return false;
            InternalTransition other = (InternalTransition)obj;
            return Source.SetEquals(other.Source) & Label.SetEquals(other.Label) & Accepting.Equals(other.Accepting);
        }
        
        public override int GetHashCode ()
        {
            return 17 + 23 * (Source.GetHashCode () + 23 * (Label.GetHashCode () + 23 * Accepting.GetHashCode ())) ;
        }
    }
    
    public class Gia02 : ILTLTranslator
    {
        public Gia02 ()
        {
        }
        
        public HashSet<Node> CreateGraph (ITLFormula phi) { return null; }
        
        public BuchiAutomata GetAutomaton (ITLFormula phi) { 
            phi = phi.Normalize ();
            
            var init = InternalNode.CreateInitial (phi);
            var states = init.Expand (new HashSet<InternalState> ());
            
            var automaton = new TGBA ();
            automaton.AcceptanceSets = new TGBAAcceptanceSet[init.Untils.Length];
            for (int i = 0; i < automaton.AcceptanceSets.Length; i++) {
                automaton.AcceptanceSets[i] = new TGBAAcceptanceSet (i);   
            }
            
            var correspondingState = new Dictionary<int, AutomatonNode> ();
            int index = 0;
            foreach (var state in states) {
                var node = new AutomatonNode ("S" + (index++));
                automaton.AddVertex (node);
                correspondingState.Add (state.StateId, node);
            }

            automaton.InitialNodes.Add (correspondingState[0]);
            
            foreach (var state in states) {
                foreach (var transition in state.Transitions) {
                    foreach (var source in transition.Source) {
                        var ltrans = new LabeledAutomataTransition<AutomatonNode> (
                            correspondingState[source],
                            correspondingState[state.StateId],
                            transition.Label
                        );
                        automaton.AddEdge (ltrans);
                        
                        for (int i = 0; i < transition.Accepting.Length; i++) {
                            if (!transition.Accepting.Get (i)) {
                                automaton.AcceptanceSets[i].Add (ltrans);
                            }
                        }
                    }
                }
            }

            var degeneralizer = Generate (automaton.AcceptanceSets.Length);

            return SynchrounousProduct (automaton, degeneralizer); 
        }

        BuchiAutomata SynchrounousProduct (TGBA tgba, DegeneralizerBuchiAutomata degeneralizer)
        {
            var cache = new Dictionary<Tuple<AutomatonNode, AutomatonNode>, LabelledAutomataNode> ();
            var ba = new BuchiAutomata ();

            var n0 = tgba.InitialNodes.Single ();
            var n1 = degeneralizer.InitialNodes.Single ();
            DFSSynchrounousProduct (ba, tgba, degeneralizer,
                                    cache, n0, n1);

            ba.SetInitialNode (cache[new Tuple<AutomatonNode, AutomatonNode> (n0, n1)]);
            
            return ba;
        }

        void DFSSynchrounousProduct (BuchiAutomata ba, TGBA tgba, DegeneralizerBuchiAutomata degeneralizer,
                                     Dictionary<Tuple<AutomatonNode, AutomatonNode>, LabelledAutomataNode> cache, 
                                     AutomatonNode n0, AutomatonNode n1) {
            
            var n = Get (ba, tgba, degeneralizer, cache, n0, n1);

            var t0 = tgba.OutEdges (n0);
            var t1 = degeneralizer.OutEdges (n1).ToList ();

            // Make sure to go trough edges in the correct order. 
            // See technical report for more details.
            t1.Sort ((x, y) => {
                if (!x.Else & y.Else) return -1;
                if (!y.Else & x.Else) return 1;
                return x.Labels.Count.CompareTo (y.Labels.Count);
            });
            
            DegeneralizerAutomataTransition<AutomatonNode> theEdge = null;
            
            foreach (var e0 in t0) {
                var next0 = e0.Target;
                var found = false;
                foreach (var e1 in t1) {
                    if (found) {
                        break;
                    }
                    
                    if (e1.Else) {
                        if (theEdge == null) {
                            theEdge = e1;
                        }
                    } else {
                        found = true;
                        for (int i = 0; i < tgba.AcceptanceSets.Length; i++) {
                            var b0 = tgba.AcceptanceSets [i].Transitions.Contains (e0);
                            var b1 = e1.Labels.Contains (i);
                            if (b1 & !b0) {
                                found = false;
                                break;
                            }
                        }
                    }
                    
                    if (found) {
                        theEdge = e1;
                    }
                }
                
                if (theEdge != null) {
                    var next1 = theEdge.Target;
                    var newNext = !cache.ContainsKey (new Tuple<AutomatonNode, AutomatonNode> (next0, next1));
                    var next = Get (ba, tgba, degeneralizer, cache, next0, next1);

                    var t = new LabeledAutomataTransition<AutomatonNode> (n, next, e0.Labels);
                    ba.AddTransition (t);
                    
                    if (newNext) {
                        DFSSynchrounousProduct (ba, tgba, degeneralizer, cache, next0, next1);
                    }
                }
            }
        }
        
        LabelledAutomataNode Get (BuchiAutomata ba, TGBA automaton, DegeneralizerBuchiAutomata degeneralizer,
                                  Dictionary<Tuple<AutomatonNode, AutomatonNode>, LabelledAutomataNode> cache, 
                                  AutomatonNode n0, AutomatonNode n1)
        {
            LabelledAutomataNode cachedNode = null;
            var key = new Tuple<AutomatonNode, AutomatonNode> (n0, n1);
            if (!cache.TryGetValue (key, out cachedNode)) {
                cachedNode = new LabelledAutomataNode (n0.Name + " x " + n1.Name);
                if (degeneralizer.AcceptanceSet.Contains (n1)) {
                    ba.AddToAcceptance (cachedNode);
                }
                ba.AddNode (cachedNode);
                cache.Add (key, cachedNode);
            }

            return cachedNode;
        }
        
        DegeneralizerBuchiAutomata Generate (int nAcceptingSets)
        {
            var automaton = new DegeneralizerBuchiAutomata ();
            
            var nNodes = nAcceptingSets + 1;
            var nodes = new AutomatonNode [nNodes];
            var last = nAcceptingSets;
            for (int i = 0; i < nNodes; i++) {
                nodes[i] = new AutomatonNode ("S" + i);
                automaton.AddVertex (nodes [i]);
            }

            DegeneralizerAutomataTransition<AutomatonNode> transition;
            for (int i = 0; i < last; i++) {
                for (int j = last; j > i; j--) {
                    transition = new DegeneralizerAutomataTransition<AutomatonNode> (nodes[i], nodes[j]);
                    for (int k = i; k < j; k++) {
                        transition.Labels.Add (k);
                    }
                    automaton.AddEdge (transition);
                }
                transition = new DegeneralizerAutomataTransition<AutomatonNode> (nodes[i], nodes[i]);
                transition.Else = true;
                automaton.AddEdge (transition);
            }

            transition = new DegeneralizerAutomataTransition<AutomatonNode> (nodes[last], nodes[last]);
            for (int i = 0; i < last; i++) {
                transition.Labels.Add (i);
            }
            automaton.AddEdge (transition);
            
            for (int i = last - 1; i >= 0; i--) {
                if (i == 0) {
                    transition = new DegeneralizerAutomataTransition<AutomatonNode> (nodes[last], nodes[i]);
                    transition.Else = true;
                    automaton.AddEdge (transition);
                } else {
                    transition = new DegeneralizerAutomataTransition<AutomatonNode> (nodes[last], nodes[i]);
                    for (int k = 0; k < i; k++) {
                        transition.Labels.Add (k);
                    }
                    automaton.AddEdge (transition);
                }
            }

            automaton.AcceptanceSet.Add (nodes [last]);
            automaton.InitialNodes.Add (nodes[last]);
            
            return automaton;
        }
    }
}

