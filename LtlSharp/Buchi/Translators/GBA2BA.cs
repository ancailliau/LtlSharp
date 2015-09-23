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
                    new GBAAcceptanceSet (0, gba.Nodes.Select (x => x.Id).ToArray ())
                };
            }
            
            // If the GBA contains only one acceptance set, then it is a BA.
            if (gba.AcceptanceSets.Length == 1) {
                var automata = new BuchiAutomata (gba.Nodes.Length);
                automata.Nodes = gba.Nodes.Select (x => new AutomataNode (x.Id, x.Name, x.Initial)).ToArray ();
                automata.Transitions = gba.Transitions.Select (x => x.Select (y => new AutomataTransition (y.To, y.Labels)).ToList ()).ToArray ();
                automata.AcceptanceSet = gba.AcceptanceSets [0].Nodes;
                return automata;
            }
            
            mapping = new AutomataNode[gba.AcceptanceSets.Length, gba.Nodes.Length];
            var nodes = new List<AutomataNode> ();
            var transitions = new Dictionary<int, List<AutomataTransition>> ();
            foreach (var qi in gba.Nodes.Where (x => x.Initial)) {
                Recur (qi, nodes, transitions, 0, gba);
            }
            
            var ba = new BuchiAutomata (nodes.Count);
            foreach (var node in nodes) {
                ba.Nodes [node.Id] = node;
            }
            foreach (var transition in transitions) {
                ba.Transitions [transition.Key] = transition.Value;
            }
            
            var nodes2 = gba.AcceptanceSets [0].Nodes;
            ba.AcceptanceSet = new int[nodes2.Length];
            for (int i = 0; i < nodes2.Length; i++) {
                var bANode = mapping [0, nodes2[i]];
                if (bANode != null) {
                    ba.AcceptanceSet [i] = bANode.Id;
                }
            }
            
            return ba;
        }
        
        static AutomataNode Recur (GBANode root, List<AutomataNode> nodes, Dictionary<int, List<AutomataTransition>> transitions, int acceptanceIndex, GeneralizedBuchiAutomata gba)
        {
            if (mapping[acceptanceIndex,root.Id] != null) {
                return mapping [acceptanceIndex, root.Id];
            }
            
            var bANode = new AutomataNode (nodes.Count (), root.Name + " x " + acceptanceIndex, root.Initial & acceptanceIndex == 0);
            mapping [acceptanceIndex, root.Id] = bANode;
            nodes.Add (bANode);
            
            int newAI = acceptanceIndex;
            if (gba.AcceptanceSets[acceptanceIndex].Nodes.Contains (root.Id)) {
                newAI = (acceptanceIndex + 1) % gba.AcceptanceSets.Length;
            }
            
            foreach (var t in gba.Transitions[root.Id]) {
                var n2 = Recur (gba.Nodes [t.To], nodes, transitions, newAI, gba);
                var t2 = new AutomataTransition (n2.Id, t.Labels);

                if (!transitions.ContainsKey (bANode.Id)) {
                    transitions.Add (bANode.Id, new List<AutomataTransition> ());
                }
                transitions[bANode.Id].Add (t2);
            }
            
            return bANode;
        }
    }
}

