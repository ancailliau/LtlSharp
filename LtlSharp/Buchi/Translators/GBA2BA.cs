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
        // Cache for acceptance condition set x node
        private static Dictionary<int, Dictionary<AutomataNode,AutomataNode>> mapping;
        
        /// <summary>
        /// Returns the Büchi Automata corresponding to the specified GBA.
        /// </summary>
        /// <param name="gba">A Generalized Büchi Automata.</param>
        public static BuchiAutomata Transform (GeneralizedBuchiAutomata gba)
        {
            // A GBA without acceptance set the same than a GBA with one acceptance set containing all nodes.
            if (gba.AcceptanceSets.Length == 0) {
                gba.AcceptanceSets = new GBAAcceptanceSet[] { 
                    new GBAAcceptanceSet (0, gba.Vertices.ToArray ())
                };
            }
            
            // If the GBA contains only one acceptance set, then it is a BA.
            if (gba.AcceptanceSets.Length == 1) {
                var automata = new BuchiAutomata ();
                automata.AddVertexRange (gba.Vertices);
                automata.AddEdgeRange (gba.Edges);
                automata.AcceptanceSet = new HashSet<AutomataNode> (gba.AcceptanceSets [0].Nodes);
                automata.InitialNodes = new HashSet<AutomataNode> (gba.InitialNodes);
                return automata;
            }

            mapping = new Dictionary<int, Dictionary<AutomataNode,AutomataNode>> ();
            for (int i = 0; i < gba.AcceptanceSets.Length; i++) {
                mapping [i] = new Dictionary<AutomataNode, AutomataNode> ();
            }
            
            var ba = new BuchiAutomata ();
            foreach (var qi in gba.InitialNodes) {
                Recur (qi, ba, 0, gba);
            }
            
            foreach (var acceptingNode in gba.AcceptanceSets [0].Nodes) {
                AutomataNode n;
                if (mapping[0].TryGetValue (acceptingNode, out n)) {
                    ba.AcceptanceSet.Add (n);
                }
            }
            
            return ba;
        }
        
        static AutomataNode Recur (AutomataNode root, BuchiAutomata ba, int acceptanceIndex, GeneralizedBuchiAutomata gba)
        {
            if (mapping[acceptanceIndex].ContainsKey(root)) {
                return mapping [acceptanceIndex][root];
            }
            
            var bANode = new AutomataNode (root.Name + " x " + acceptanceIndex);
            mapping [acceptanceIndex].Add(root, bANode);
            ba.AddVertex (bANode);
            if (gba.InitialNodes.Contains (root) & acceptanceIndex == 0) {
                ba.InitialNodes.Add (bANode);
            }
            
            int newAI = acceptanceIndex;
            if (gba.AcceptanceSets[acceptanceIndex].Nodes.Contains (root)) {
                newAI = (acceptanceIndex + 1) % gba.AcceptanceSets.Length;
            }
            
            foreach (var t in gba.OutEdges (root)) {
                var n2 = Recur (t.Target, ba, newAI, gba);
                var t2 = new AutomataTransition (bANode, n2, t.Labels);
                ba.AddEdge (t2);
            }
            
            return bANode;
        }
    }
}

