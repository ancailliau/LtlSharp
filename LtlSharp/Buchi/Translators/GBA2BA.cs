using System;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;

namespace LtlSharp.Buchi.Translators
{
    public class GBA2BA
    {
        private static BANode[,] mapping;
        
        public static BuchiAutomata Transform (GeneralizedBuchiAutomata gba)
        {
            mapping = new BANode[gba.AcceptanceSets.Length,gba.Nodes.Length];
            var nodes = new List<BANode> ();
            var transitions = new Dictionary<int, List<BATransition>> ();
            foreach (var qi in gba.Nodes.Where (x => x.Initial)) {
                Recur (qi, nodes, transitions, 0, gba);
            }
            
            foreach (var n in nodes) {
                Console.WriteLine (n);
            }
            
            foreach (var item in transitions) {
                foreach (var item2 in item.Value) {
                    Console.WriteLine (item.Key + ": " + item2);
                }
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
                Console.WriteLine (nodes2[i]);
                var bANode = mapping [0, i];
                if (bANode != null) {
                    ba.AcceptanceSet [i] = bANode.Id;
                }
            }
            
            return ba;
        }
        
        static BANode Recur (GBANode root, List<BANode> nodes, Dictionary<int, List<BATransition>> transitions, int acceptanceIndex, GeneralizedBuchiAutomata gba)
        {
            Console.WriteLine ("Expanding node : " + root.Name + " x " + acceptanceIndex);
            
            if (mapping[acceptanceIndex,root.Id] != null) {
                Console.WriteLine ("Already processed");
                return mapping [acceptanceIndex, root.Id];
            }
            
            var bANode = new BANode (nodes.Count (), root.Name + " x " + acceptanceIndex, root.Initial & acceptanceIndex == 0);
            mapping [acceptanceIndex, root.Id] = bANode;
            nodes.Add (bANode);
            
            int newAI = acceptanceIndex;
            if (gba.AcceptanceSets[acceptanceIndex].Nodes.Contains (root.Id)) {
                newAI = (acceptanceIndex + 1) % gba.AcceptanceSets.Length;
            }
            
            foreach (var t in gba.Transitions[root.Id]) {
                var n2 = Recur (gba.Nodes [t.To], nodes, transitions, newAI, gba);
                var t2 = new BATransition (n2.Id, t.Labels);

                Console.WriteLine ("Adding transition : " + (root.Name + " x " + acceptanceIndex) + " to " + (gba.Nodes [t.To].Name + " x " + newAI) );

                if (!transitions.ContainsKey (bANode.Id)) {
                    transitions.Add (bANode.Id, new List<BATransition> ());
                }
                transitions[bANode.Id].Add (t2);
            }
            
            return bANode;
        }
    }
}

