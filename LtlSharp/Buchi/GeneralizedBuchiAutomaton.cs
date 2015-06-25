// 
// GeneralizedBuchiAutomaton.cs
//  
// Author:
//       Antoine Cailliau <antoine.cailliau@uclouvain.be>
// 
// Copyright (c) 2011 UCLouvain
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp;
using LtlSharp.Buchi;

namespace LittleSharp.Buchi
{
	
	/// <summary>
	/// A generalized buchi automaton.
	/// </summary>
	public class GeneralizedBuchiAutomaton
	{
		
		/// <summary>
		/// Gets or sets the list of nodes.
		/// </summary>
		/// <value>
		/// The nodes.
		/// </value>
		public Dictionary<string, GBANode> Nodes {
			get;
			private set;
		}
		
		public List<List<GBANode>> AcceptanceSet {
			get ;
			private set ; 
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="LittleSharp.Buchi.GeneralizedBuchiAutomaton"/> class based
		/// on a set of nodes and an original formula.
		/// </summary>
		/// <param name='nodesSet'>
		/// A set of nodes generated by an ltl to buchi algorithms.
		/// </param>
		/// <param name='originalFormula'>
		/// The original formula, corresponding to the automaton.
		/// </param>
        public GeneralizedBuchiAutomaton (HashSet<Node> nodesSet, ILTLFormula originalFormula)
		{
			Nodes = new Dictionary<string, GBANode>();
			
			// Create an initial node
			GBANode initialNode = new GBANode("init", true);
			Nodes.Add("init", initialNode);
			
			// Build the nodes
			foreach (Node oldNode in nodesSet) {
				GBANode newNode = new GBANode(oldNode.Name);
				Nodes.Add(newNode.Name, newNode);
			}
			
			// Build the transitions
			foreach (Node oldNode in nodesSet) {
				foreach (var incomingNodeName in oldNode.Incoming) {
					Transition transition = new Transition()
						{ From = Nodes[incomingNodeName], 
						  To   = Nodes[oldNode.Name] };
					
                    transition.Literals.AddRange (oldNode.Old.OfType<ILiteral> ());
				}
			}
			
			// The acceptance set contains a separate set of states for
			// each subformula of the form x U y. The set contains the
			// states n such that y in Old(n) or x U y not in Old(n).
			AcceptanceSet = new List<List<GBANode>>();
			
			// Subformulas are processed in a DFS-fashioned way
			Stack<ILTLFormula> formulasToProcess = new Stack<ILTLFormula>();
			formulasToProcess.Push(originalFormula);
			
			int nextAcceptingSetIndex = 0;
			
			while(formulasToProcess.Count > 0) {
				ILTLFormula considered = formulasToProcess.Pop();
				
				if (considered is Until) {
					Until consideredUntil = considered as Until;
					List<GBANode> set = new List<GBANode>();
					
					// Adds all nodes containing right member of until
					// or not the until in their old set.
					set.AddRange(nodesSet.Where(
                        n => n.Old.Contains(consideredUntil.Right)
							| !n.Old.Contains(consideredUntil)
						).ToList().ConvertAll(n => Nodes[n.Name]));
					
					AcceptanceSet.Add(set);
					
					foreach (var node in set) {
						node.AcceptingSet.Add(nextAcceptingSetIndex);
					}
					nextAcceptingSetIndex++;
					
				} else {
					// Recursively continues to process the formula
					
                    if (considered is IBinaryOperator) {
                        formulasToProcess.Push(((IBinaryOperator) considered).Left);
                        formulasToProcess.Push(((IBinaryOperator) considered).Right);
					
					} else if (considered is IUnaryOperator) {
                        formulasToProcess.Push(((IUnaryOperator) considered).Enclosed);
						
					}
				}
			}
		}
	}
}

