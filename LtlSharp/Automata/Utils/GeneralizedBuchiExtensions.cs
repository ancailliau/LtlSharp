using System;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;
using LtlSharp.Automata;
using LtlSharp.Automata.AcceptanceConditions;
using LtlSharp.Automata.OmegaAutomata;
using LtlSharp.Automata.Nodes.Factories;


namespace LtlSharp.Automata.Utils
{
    /// <summary>
    /// Provides static methods for transforming a Generalized Büchi Automata to a Büchi Automata
    /// </summary>
    public static class GBA2BA
    {
        /// <summary>
        /// Returns the Büchi Automata corresponding to the specified GBA.
        /// </summary>
        /// <param name="gba">A Generalized Büchi Automata.</param>
        public static BuchiAutomaton<AutomatonNode> ToBA (this GeneralizedBuchiAutomaton<AutomatonNode> gba)
        {
            // A GBA without acceptance set the same than a GBA with one acceptance set containing all nodes.
            if (gba.AcceptanceCondition.IsEmpty) {
                gba.AcceptanceCondition.Add (0, gba.Nodes);
            }
            
            // If the GBA contains only one acceptance set, then it is a BA.
            if (gba.AcceptanceCondition.IsBuchi) {
                var automaton = new BuchiAutomaton<AutomatonNode> (new AutomatonNodeFactory ());
                automaton.AddNodes (gba.Nodes);
                foreach (var e in gba.Edges) {
                    automaton.AddTransition (e.Source, e.Target, e.Decoration);
                }
                automaton.SetInitialNode (gba.InitialNode);
                automaton.SetAcceptanceCondition (gba.AcceptanceCondition[0]);
                return automaton;
            }
            
            var mapping = new Dictionary<int, Dictionary<AutomatonNode,AutomatonNode>> ();
            var enumerator = gba.AcceptanceCondition.GetEnumerator ();
            for (int i = 0; enumerator.MoveNext (); i ++) {
                mapping [i] = new Dictionary<AutomatonNode, AutomatonNode> ();
            }
            
            var ba = new BuchiAutomaton<AutomatonNode> (new AutomatonNodeFactory ());
            Recur (gba.InitialNode, ba, 0, gba, mapping);
            
            foreach (var acceptingNode in gba.AcceptanceCondition[0].GetAcceptingNodes ()) {
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
                                    GeneralizedBuchiAutomaton<AutomatonNode> generalizedBuchiAutomaton,
                                    Dictionary<int, Dictionary<AutomatonNode,AutomatonNode>> mapping)
        {
            if (mapping[acceptanceIndex].ContainsKey(node)) {
                return mapping [acceptanceIndex][node];
            }
            
            var newNode = buchiAutomaton.AddNode (node.Name + " x " + acceptanceIndex);
            mapping [acceptanceIndex].Add(node, newNode);
            
            if (generalizedBuchiAutomaton.InitialNode.Equals (node) & acceptanceIndex == 0) {
                buchiAutomaton.SetInitialNode (newNode);
            }
            
            int newAcceptanceIndex = acceptanceIndex;
            if (generalizedBuchiAutomaton.AcceptanceCondition[acceptanceIndex].Accept (node)) {
                newAcceptanceIndex = (acceptanceIndex + 1) % generalizedBuchiAutomaton.AcceptanceCondition.Count;
            }
            
            foreach (var t in generalizedBuchiAutomaton.GetTransitions (node)) {
                var n2 = Recur (t.Target, buchiAutomaton, newAcceptanceIndex, generalizedBuchiAutomaton, mapping);
                //var t2 = new AutomatonTransition<AutomatonNode> (bANode, n2, t.Labels);
                buchiAutomaton.AddTransition (newNode, n2, t.Decoration);
            }
            
            return newNode; 
        }
    }
}

