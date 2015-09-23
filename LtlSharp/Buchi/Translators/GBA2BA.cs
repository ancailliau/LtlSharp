using System;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;
using LtlSharp.Buchi.Automata;

namespace LtlSharp.Buchi.Translators
{
    /// <summary>
    /// Provides static methods for transforming a Generalized Büchi Automata to a Büchi Automata
    /// </summary>
    public static class GBA2BA
    {
        private static AutomataNode[,] mapping;
        
        /// <summary>
        /// Returns the Büchi Automata corresponding to the specified GBA.
        /// </summary>
        /// <param name="gba">A Generalized Büchi Automata.</param>
        public static BuchiAutomata Transform (GeneralizedBuchiAutomata gba)
        {
            // A GBA without acceptance set the same than a GBA with one acceptance set containing all nodes.
            if (gba.AcceptanceSets.Length == 0) {
                gba.AcceptanceSets = new GBAAcceptanceSet[] { 
                    new GBAAcceptanceSet (0, gba.Nodes.ToArray ())
                };
            }
            
            // If the GBA contains only one acceptance set, then it is a BA.
            if (gba.AcceptanceSets.Length == 1) {
                var automata = new BuchiAutomata (gba.Transitions.Count);
                foreach (var n in gba.Nodes.Select (x => new AutomataNode (x.Id, x.Name, x.Initial))) {
                    automata.Transitions.Add (n, new HashSet<AutomataTransition> (gba.Transitions[n].Select (
                        (arg) => new AutomataTransition (arg.To, arg.Labels)))
                                             );
                    
                }
                //automata.Transitions = gba.Transitions.Select (x => x.Select (y => new AutomataTransition (y.To, y.Labels)).ToList ()).ToArray ();
                automata.AcceptanceSet = new HashSet<AutomataNode> (gba.AcceptanceSets [0].Nodes);
                return automata;
            }
            
            mapping = new AutomataNode[gba.AcceptanceSets.Length, gba.Transitions.Count];
            var nodes = new HashSet<AutomataNode> ();
            var transitions = new Dictionary<AutomataNode, HashSet<AutomataTransition>> ();
            foreach (var qi in gba.Nodes.Where (x => x.Initial)) {
                Recur (qi, nodes, transitions, 0, gba);
            }
            
            var ba = new BuchiAutomata (nodes.Count);
            ba.Transitions = transitions;
            //foreach (var node in nodes) {
            //    ba.Nodes [node.Id] = node;
            //}
            //foreach (var transition in transitions) {
            //    ba.Transitions [transition.Key] = transition.Value;
            //}

            var nodes2 = gba.AcceptanceSets [0].Nodes;
            ba.AcceptanceSet = new HashSet<AutomataNode> ();
            for (int i = 0; i < nodes2.Length; i++) {
                var bANode = mapping [0, nodes2[i].Id];
                if (bANode != null) {
                    ba.AcceptanceSet.Add (bANode);
                }
            }
            
            return ba;
        }
        
        static AutomataNode Recur (AutomataNode root, HashSet<AutomataNode> nodes, Dictionary<AutomataNode, HashSet<AutomataTransition>> transitions, int acceptanceIndex, GeneralizedBuchiAutomata gba)
        {
            if (mapping[acceptanceIndex,root.Id] != null) {
                return mapping [acceptanceIndex, root.Id];
            }
            
            var bANode = new AutomataNode (nodes.Count (), root.Name + " x " + acceptanceIndex, root.Initial & acceptanceIndex == 0);
            mapping [acceptanceIndex, root.Id] = bANode;
            nodes.Add (bANode);
            
            int newAI = acceptanceIndex;
            if (gba.AcceptanceSets[acceptanceIndex].Nodes.Contains (root)) {
                newAI = (acceptanceIndex + 1) % gba.AcceptanceSets.Length;
            }
            
            foreach (var t in gba.Transitions[root]) {
                var n2 = Recur (t.To, nodes, transitions, newAI, gba);
                var t2 = new AutomataTransition (n2, t.Labels);

                if (!transitions.ContainsKey (bANode)) {
                    transitions.Add (bANode, new HashSet<AutomataTransition> ());
                }
                transitions[bANode].Add (t2);
            }
            
            return bANode;
        }
    }
}

