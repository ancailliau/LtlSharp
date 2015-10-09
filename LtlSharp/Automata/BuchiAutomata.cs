using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LtlSharp.Buchi.Automata;
using QuickGraph;

namespace LtlSharp.Buchi
{
    public class BuchiAutomata : AdjacencyGraph<AutomataNode, LabeledAutomataTransition<AutomataNode>> 
    {
        public HashSet<AutomataNode> InitialNodes;
        public HashSet<AutomataNode> AcceptanceSet;
        
        public BuchiAutomata () : base ()
        {
            InitialNodes = new HashSet<AutomataNode> ();
            AcceptanceSet = new HashSet<AutomataNode> ();
        }
        
        /// <summary>
        /// Returns the set of literals that correspond to (at least) a transition from the specified node.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<HashSet<ILiteral>> OutAlphabet (AutomataNode node)
        {
            return OutEdges (node).Select (e => e.Labels).Distinct ();
        }
        
        /// <summary>
        /// Returns the set of literals that correspond to (at least) a transition for (at least) a specified node.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<HashSet<ILiteral>> OutAlphabet (IEnumerable<AutomataNode> nodes)
        {
            return nodes.SelectMany (OutAlphabet).Distinct ();
        }
        
        /// <summary>
        /// Returns all the successor nodes of the specified node
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<AutomataNode> Post (AutomataNode node)
        {
            return OutEdges (node).Select (e => e.Target);
        }
        
        /// <summary>
        /// Returns all the successor nodes of the specified nodes
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<AutomataNode> Post (IEnumerable<AutomataNode> nodes)
        {
            return nodes.SelectMany (node => Post(node));
        }

        /// <summary>
        /// Returns all the successor nodes of the specified node for the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<AutomataNode> Post (AutomataNode node, HashSet<ILiteral> labels)
        {
            return OutEdges (node).Where (e => e.Labels.SetEquals (labels)).Select (e => e.Target);
        }

        /// <summary>
        /// Returns all the successor nodes of the specified nodes for the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<AutomataNode> Post (IEnumerable<AutomataNode> nodes, HashSet<ILiteral> labels)
        {
            return nodes.SelectMany (node => Post(node, labels));
        }

        /// <summary>
        /// Returns all the successor nodes of the specified node for all the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<AutomataNode> Post (AutomataNode node, IEnumerable<HashSet<ILiteral>> labels)
        {
            return labels.SelectMany (l => Post(node, l));
        }

        /// <summary>
        /// Returns all the successor nodes of the specified nodes for all the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public IEnumerable<AutomataNode> Post (IEnumerable<AutomataNode> nodes, IEnumerable<HashSet<ILiteral>> labels)
        {
            return nodes.SelectMany (node => Post(node, labels));
        }
        
    }
    
    public class DegeneralizerBuchiAutomata : AdjacencyGraph<AutomataNode, DegeneralizerAutomataTransition<AutomataNode>> 
    {
        public HashSet<AutomataNode> InitialNodes;
        public HashSet<AutomataNode> AcceptanceSet;

        public DegeneralizerBuchiAutomata () : base ()
        {
            InitialNodes = new HashSet<AutomataNode> ();
            AcceptanceSet = new HashSet<AutomataNode> ();
        }
    }
}

