using System;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;
using LtlSharp.Automata;
using LtlSharp.Automata.AcceptanceConditions;
using LtlSharp.Automata.OmegaAutomata;
using LtlSharp.Automata.Nodes.Factories;


namespace LtlSharp.Buchi.Translators
{
    /// <summary>
    /// Provides static methods for transforming a Generalized Büchi Automata to a Büchi Automata
    /// </summary>
    public static class GBA2BA
    {
        // Cache for acceptance condition set x node
        private static Dictionary<int, Dictionary<AutomatonNode,AutomatonNode>> mapping;
        
        /// <summary>
        /// Returns the Büchi Automata corresponding to the specified GBA.
        /// </summary>
        /// <param name="gba">A Generalized Büchi Automata.</param>
        public static BuchiAutomaton<AutomatonNode> Transform (GeneralizedBuchiAutomaton<AutomatonNode> gba)
        {
            // A GBA without acceptance set the same than a GBA with one acceptance set containing all nodes.
            if (gba.GetAcceptanceCondition().IsEmpty) {
                gba.GetAcceptanceCondition ().Add (0, gba.Nodes);
            }
            
            // If the GBA contains only one acceptance set, then it is a BA.
            if (gba.GetAcceptanceCondition().IsBuchi) {
                var automata = new BuchiAutomaton<AutomatonNode> (new AutomatonNodeFactory ());
                automata.AddNodes (gba.Nodes);
                foreach (var e in gba.Edges) {
                    automata.AddTransition (e.Source, e.Target, e.Decoration);
                }
                automata.SetInitialNode (gba.InitialNode);
                automata.SetAcceptanceCondition (gba.GetAcceptanceCondition()[0]);
                return automata;
            }
            
            mapping = new Dictionary<int, Dictionary<AutomatonNode,AutomatonNode>> ();
            var enumerator = gba.GetAcceptanceCondition ().GetEnumerator ();
            for (int i = 0; enumerator.MoveNext (); i ++) {
                mapping [i] = new Dictionary<AutomatonNode, AutomatonNode> ();
            }
            
            var ba = new BuchiAutomaton<AutomatonNode> (new AutomatonNodeFactory ());
            Recur (gba.InitialNode, ba, 0, gba);
            
            foreach (var acceptingNode in gba.GetAcceptanceCondition()[0].GetAcceptingNodes ()) {
                AutomatonNode n;
                if (mapping[0].TryGetValue (acceptingNode, out n)) {
                    ba.AddToAcceptance (n);
                }
            }
            
            return ba;
        }
        
        static AutomatonNode Recur (AutomatonNode node, 
                                    BuchiAutomaton<AutomatonNode> buchiAutomaton, 
                                    int acceptanceIndex, 
                                    GeneralizedBuchiAutomaton<AutomatonNode> generalizedBuchiAutomaton)
        {
            if (mapping[acceptanceIndex].ContainsKey(node)) {
                return mapping [acceptanceIndex][node];
            }
            
            var newNode = new AutomatonNode (node.Name + " x " + acceptanceIndex);
            mapping [acceptanceIndex].Add(node, newNode);
            buchiAutomaton.AddNode (newNode);
            
            if (generalizedBuchiAutomaton.InitialNode.Equals (node) & acceptanceIndex == 0) {
                buchiAutomaton.SetInitialNode (newNode);
            }
            
            int newAcceptanceIndex = acceptanceIndex;
            if (generalizedBuchiAutomaton.GetAcceptanceCondition()[acceptanceIndex].Accept (node)) {
                newAcceptanceIndex = (acceptanceIndex + 1) % generalizedBuchiAutomaton.GetAcceptanceCondition().Count;
            }
            
            foreach (var t in generalizedBuchiAutomaton.GetTransitions (node)) {
                var n2 = Recur (t.Target, buchiAutomaton, newAcceptanceIndex, generalizedBuchiAutomaton);
                //var t2 = new AutomatonTransition<AutomatonNode> (bANode, n2, t.Labels);
                buchiAutomaton.AddTransition (newNode, n2, t.Decoration);
            }
            
            return newNode;
        }
    }
}

