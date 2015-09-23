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
                    new GBAAcceptanceSet (0, gba.Vertices.ToArray ())
                };
            }
            
            // If the GBA contains only one acceptance set, then it is a BA.
            if (gba.AcceptanceSets.Length == 1) {
                var automata = new BuchiAutomata ();
                foreach (var n in gba.Vertices.Select (x => new AutomataNode (x.Id, x.Name, x.Initial))) {
                    automata.AddVertex (n);
                    foreach (var t in gba.OutEdges (n)) {
                        automata.AddEdge (new AutomataTransition (n, t.Target, t.Labels));
                    }
                }
                automata.AcceptanceSet = new HashSet<AutomataNode> (gba.AcceptanceSets [0].Nodes);
                return automata;
            }
            
            mapping = new AutomataNode[gba.AcceptanceSets.Length, gba.Edges.Count()];
            
            var ba = new BuchiAutomata ();
            foreach (var qi in gba.Vertices.Where (x => x.Initial)) {
                Recur (qi, ba, 0, gba);
            }
            
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
        
        static AutomataNode Recur (AutomataNode root, BuchiAutomata ba, int acceptanceIndex, GeneralizedBuchiAutomata gba)
        {
            if (mapping[acceptanceIndex,root.Id] != null) {
                return mapping [acceptanceIndex, root.Id];
            }
            
            var bANode = new AutomataNode (0, root.Name + " x " + acceptanceIndex, root.Initial & acceptanceIndex == 0);
            mapping [acceptanceIndex, root.Id] = bANode;
            ba.AddVertex (bANode);
            
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

