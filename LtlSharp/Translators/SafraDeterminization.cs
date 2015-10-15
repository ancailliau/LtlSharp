using System;
using System.Collections.Generic;
using LtlSharp.Buchi;
using LtlSharp.Automata;
using LtlSharp.Buchi.Automata;
using System.Linq;
using LtlSharp.Utils;
using LtlSharp.Automata.OmegaAutomata;
using LtlSharp.Automata.Transitions;

namespace LtlSharp.Translators
{
    /// <summary>
    /// Determinize Non-Deterministic Buchï Automata into Rabin Automata using Safra construction.
    /// </summary>
    public class SafraDeterminization : Transformer<BuchiAutomaton<AutomatonNode>, RabinAutomaton<AutomatonNode>>
    {
        /// <summary>
        /// Represents a node (and a tree if the node is the root) of a Safra Tree.
        /// </summary>
        protected class SafraTree
        {
            /// <summary>
            /// Gets the set of corresponding states in the Buchi Automata
            /// </summary>
            /// <value>The set of states.</value>
            public HashSet<AutomatonNode> MacroState {
                get ;
                private set ;
            }
            
            /// <summary>
            /// Gets the children of the node.
            /// </summary>
            /// <value>The children.</value>
            public List<SafraTree> Children { 
                get; 
                private set;
            }
            
            /// <summary>
            /// Whether the node is marked or not.
            /// </summary>
            /// <value><c>True</c> if marked, <c>False</c> otherwise.</value>
            public bool Mark {
                get;
                private set;
            }
            
            /// <summary>
            /// Gets the identifier of the Safra tree node.
            /// </summary>
            /// <value>The identifier.</value>
            public int Id {
                get;
                private set;
            }
            
            // The Buchi Automata to determinize
            BuchiAutomaton<AutomatonNode> ba;
            
            // The last identifier used for creating Safra Tree nodes.
            public static int CurrentId = 1;
            
            /// <summary>
            /// Initializes a new instance of the <see cref="LtlSharp.Translators.SafraDeterminization+SafraTree"/> class
            /// that has the specified set of nodes as macrostate. The Safra Tree Node refers to the specified Buchi Auomata.
            /// </summary>
            /// <param name="nodes">Nodes of the Macro State.</param>
            /// <param name="ba">Buchi Automata.</param>
            public SafraTree (IEnumerable<AutomatonNode> nodes, BuchiAutomaton<AutomatonNode> ba) : this (0, nodes, ba)
            {
                Id = CurrentId++;
            }
            
            /// <summary>
            /// Initializes a new instance of the <see cref="LtlSharp.Translators.SafraDeterminization+SafraTree"/> class
            /// that has the specified set of nodes as macrostate and the specified identifier. The Safra Tree Node refers 
            /// to the specified Buchi Auomata.
            /// </summary>
            /// <param name="Id">Identifier.</param>
            /// <param name="nodes">Nodes of the Macro State.</param>
            /// <param name="ba">Buchi Automata.</param>
            public SafraTree (int Id, IEnumerable<AutomatonNode> nodes, BuchiAutomaton<AutomatonNode> ba)
            {
                MacroState = new HashSet<AutomatonNode> (nodes);
                Children = new List<SafraTree> ();
                Mark = false;
                this.ba = ba;
                this.Id = Id;
            }
            
            /// <summary>
            /// Clone this instance. (Copies the identifier too)
            /// </summary>
            public SafraTree Clone ()
            {
                return new SafraTree (Id, MacroState, ba) {
                    Children = Children.Select (x => x.Clone ()).ToList (),
                    Mark = Mark
                };
            }
            
            /// <summary>
            /// Determines whether the specified <see cref="System.Object"/> is equal to the current 
            /// <see cref="LtlSharp.Translators.SafraDeterminization+SafraTree"/>. List of children
            /// and set of nodes in the macrostate are used to compare two elements. Lists of children
            /// shall be sequence equal for two nodes to be equal.
            /// </summary>
            /// <param name="obj">The <see cref="System.Object"/> to compare with the current 
            /// <see cref="LtlSharp.Translators.SafraDeterminization+SafraTree"/>.</param>
            /// <returns><c>true</c> if the specified <see cref="System.Object"/> is equal to the current
            /// <see cref="LtlSharp.Translators.SafraDeterminization+SafraTree"/>; otherwise, <c>false</c>.</returns>
            public override bool Equals (object obj)
            {
                if (obj == null)
                    return false;
                if (ReferenceEquals (this, obj))
                    return true;
                if (obj.GetType () != typeof(SafraTree))
                    return false;
                SafraTree other = (SafraTree)obj;
                return Children.SequenceEqual (other.Children) && MacroState.SetEquals (other.MacroState);
            }
            
            /// <summary>
            /// Serves as a hash function for a <see cref="LtlSharp.Translators.SafraDeterminization+SafraTree"/> object.
            /// </summary>
            /// <description>
            /// Computing the hash might be computer intensive as the hash is computed recursively for each children and for each
            /// node of the macro state.
            /// </description>
            /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as
            /// a hash table.</returns>
            public override int GetHashCode ()
            {
                unchecked {
                    int curHash;
                    int hashChildren = 0;
                    foreach (var element in Children) {
                        curHash = element.GetHashCode ();
                        hashChildren = 32 * hashChildren + curHash;
                    }
                    
                    var hashMacrostate = MacroState.GetHashCodeForElements ();

                    return 17 + hashMacrostate + 32 * hashChildren;
                }
            }
            
            /// <summary>
            /// Removes the mark on this node and all children nodes.
            /// </summary>
            public void RemoveAllMarks () 
            {
                Mark = false;
                foreach (var child in Children)
                    child.RemoveAllMarks ();
            }
            
            /// <summary>
            /// Creates a new child if there is a set in the macro state that is accepting in the Buchi Automata.
            /// The procedure is recursively applied on children.
            /// </summary>
            public void Create ()
            {   
                var children = Children.ToList ();
                
                var inter = ba.AcceptanceCondition.GetAcceptingNodes (MacroState);
                if (inter.Any ()) {
                    var newNode = new SafraTree (inter, ba);
                    Children.Add (newNode);
                }
                
                foreach (var c in children) {
                    c.Create ();
                }
            }
            
            /// <summary>
            /// Updates the node with the set of successor nodes (for the specified literals) in the corresponding Buchi Automata.
            /// The procedure is recursively applied on all children.
            /// </summary>
            /// <param name="a">The literals.</param>
            public void Update (LiteralsSet a)
            {
                MacroState = new HashSet<AutomatonNode> (ba.Post (MacroState, a));
                
                foreach (var c in Children) {
                    c.Update (a);
                }
            }
            
            /// <summary>
            /// Removes states from the macrostate of children that already appear in an older brother node.
            /// The procedure is recursively applied on all children.
            /// </summary>
            public void HorizontalMerge ()
            {
                foreach (var c in Children) {
                    c.HorizontalMerge ();
                }
                
                var visitedState = new HashSet<AutomatonNode> ();
                foreach (var c in Children) {
                    foreach (var s in c.MacroState.ToList ()) {
                        if (visitedState.Contains (s)) {
                            c.MacroState.Remove (s);
                        } else {
                            visitedState.Add (s);
                        }
                    }
                }
            }
            
            /// <summary>
            /// Recursively removes the children that are empty.
            /// </summary>
            public void RemoveEmpty ()
            {
                foreach (var c in Children) {
                    c.RemoveEmpty ();
                }
                
                foreach (var c in Children.ToList ()) {
                    if (c.MacroState.Count == 0) {
                        Children.Remove (c);
                    }
                }
            }
            
            /// <summary>
            /// Recursively removes children if the union of their macrostate is equal to the parent macrostate.
            /// </summary>
            public void VerticalMerge ()
            {
                foreach (var c in Children) {
                    c.VerticalMerge ();
                }
                
                if (MacroState.SetEquals (Children.SelectMany (c => c.MacroState))) {
                    Children.Clear ();
                    Mark = true;
                }
            }
            
            /// <summary>
            /// Returns whether the tree rooted in the node contains a node whose Id correspond to the specified identifier.
            /// </summary>
            /// <param name="id">Identifier to test.</param>
            /// <returns><c>True</c> if there is a node with the corresponding identifier, <c>False</c> otherwise</returns>
            public bool ContainsIdentifier (int id)
            {
                return id == this.Id | Children.Any (x => x.ContainsIdentifier (id));
            }

            /// <summary>
            /// Returns whether the tree rooted in the node contains a marked node with the specified identifier.
            /// </summary>
            /// <param name="id">Identifier to test.</param>
            /// <returns><c>True</c> if there is a marked node with the corresponding identifier, <c>False</c> otherwise</returns>
            public bool ContainsMarkedNode (int id)
            {
                return (id == this.Id & Mark) | Children.Any (x => x.ContainsMarkedNode (id));
            }
        }
        
        /// <summary>
        /// Represents a transition betwee two Safra Trees.
        /// </summary>
        protected class SafraTransition
        {
            
            /// <summary>
            /// Gets or sets the labels for the transition.
            /// </summary>
            /// <value>The labels.</value>
            public LiteralsSet Labels {
                get;
                set;
            }
            
            /// <summary>
            /// Gets or sets the target Safra tree.
            /// </summary>
            /// <value>The target.</value>
            public SafraTree Target {
                get;
                set;
            }
            
            /// <summary>
            /// Initializes a new instance of the
            /// <see cref="LtlSharp.Translators.SafraDeterminization+SafraTransition"/> class
            /// with the specified label and target safra tree.
            /// </summary>
            /// <param name="labels">Labels.</param>
            /// <param name="target">Target.</param>
            public SafraTransition (LiteralsSet labels, SafraTree target)
            {
                this.Labels = labels;
                this.Target = target;
            }

        }
        
        /// <summary>
        /// The transition relation between Safra trees.
        /// </summary>
        private Dictionary<SafraTree, List<SafraTransition>> Transitions;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="LtlSharp.Translators.SafraDeterminization"/> class.
        /// </summary>
        public SafraDeterminization ()
        {
        }
        
        string PrintTree (SafraTree t, int indent = 0)
        {
            return new String ('-', indent) + t.Id + " - " + string.Join (",", t.MacroState.Select (x => x.Name)) + " - " + (t.Mark ? "!" : "")
                + "\n" + string.Join ("\n", t.Children.Select (x => PrintTree (x, indent + 1)));
        }
        
        /// <summary>
        /// Transform the specified Non-Deterministic Buchi Automata into a Deterministic Rabin Automata.
        /// </summary>
        /// <param name="ba">Buchi Automata.</param>
        public override RabinAutomaton<AutomatonNode> Transform (BuchiAutomaton<AutomatonNode> ba)
        {
            Transitions = new Dictionary<SafraTree, List<SafraTransition>> ();
            // For more details about the algorithm, check "Determinization of Büchi-Automata" by Markus Roggenbach
            
            // Assume single initial node
            var initialBA = ba.InitialNode;
            
            var initial = new SafraTree (new [] { initialBA }, ba);
            
            var pending = new Stack<SafraTree> (new [] { initial });
            var visited = new HashSet<SafraTree> ();
            
            while (pending.Count > 0) {
                var source = pending.Pop ();                
                visited.Add (source);
                
                foreach (var a in ba.OutAlphabet (source.MacroState)) {
                    var target = source.Clone ();
                    target.RemoveAllMarks (); // Step 1
                    target.Create (); // Step 2
                    target.Update (a); // Step 3
                    target.HorizontalMerge (); // Step 4
                    target.RemoveEmpty (); // Step 5
                    if (target.MacroState.Count > 0) {
                        target.VerticalMerge (); // Step 6
                        
                        if (!Transitions.ContainsKey (source)) {
                            Transitions.Add (source, new List<SafraTransition> ());
                        }
                        Transitions[source].Add (new SafraTransition (a, target));
                        
                        if (!Transitions.ContainsKey (target) & !pending.Contains (target)) {
                            pending.Push (target);
                        }   
                    }
                }
            }
            
            var rabin = new RabinAutomaton<AutomatonNode> ();
            var mapping = new Dictionary<SafraTree, AutomatonNode> ();
            int i = 0;
            foreach (var t in Transitions.Keys) {
                var n = new AutomatonNode ("s" + (i++));
                rabin.AddNode (n);
                mapping.Add (t, n);
            }
            
            rabin.SetInitialNode (mapping [initial]);
            
            foreach (var t in Transitions) {
                foreach (var e in t.Value) {
                    rabin.AddTransition (mapping [t.Key], mapping [e.Target], e.Labels);
                }
            }
            
//            foreach (var kk in Transitions.Keys) {
//                Console.WriteLine (mapping[kk].Name + " = ");
//                Console.WriteLine (PrintTree (kk));
//            }
            
            foreach (int k in Transitions.Keys.Select(t => t.Id).Distinct ()) {
//                Console.WriteLine (k + " identifier on the test");
                var r1 = Transitions.Keys.Where (x => !x.ContainsIdentifier (k)).Select (x => mapping[x]);
                var r2 = Transitions.Keys.Where (x => x.ContainsMarkedNode (k)).Select (x => mapping[x]);
                rabin.AddToAcceptance (r1, r2);
            }
            
            return rabin;
        }
    }
}

