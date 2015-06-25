// 
// GeneralizedAutomaton.cs
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
using System.Linq;
using System.Collections.Generic;

namespace LtlSharp.Buchi
{
	
	/// <summary>
	/// Automaton used for the generation of a graph that will be used
	/// to build a generalized buchi automaton. See  <see cref="LittleSharp.Buchi.AuGeneralizedBuchiAutomatontomaton"/>
	/// for more information.
	/// </summary>
	public class Automaton
	{
		
		/// <summary>
		/// Gets the nodes contained in the automaton.
		/// </summary>
		/// <value>
		/// The nodes.
		/// </value>
		public Queue<Node> Nodes {
			get;
			private set;
		}
		
		/// <summary>
		/// Gets the formula represented by the automaton.
		/// </summary>
		/// <value>
		/// The formula.
		/// </value>
        public ILTLFormula Formula {
			get;
			private set;
		}
		
		/// <summary>
		/// Gets a value indicating whether this <see cref="LittleSharp.Buchi.Automaton"/> generation has failed.
		/// </summary>
		/// <value>
		/// <c>true</c> if it failed; otherwise, <c>false</c>.
		/// </value>
		public bool Failed {
			get { return failed; }
		}
		private bool failed = true;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="LittleSharp.Buchi.Automaton"/> class.
		/// </summary>
		/// <param name='formula'>
		/// The formula to represents.
		/// </param>
        public Automaton (ILTLFormula formula)
		{
			Nodes = new Queue<Node>();
			Formula = formula;
			failed = true;
			Build();
		}
		
		/// <summary>
		/// Whether a similar node exists in the automaton. A node is similar if Old
		/// and Next obligations are the same.
		/// </summary>
		/// <returns>
		/// The exists.
		/// </returns>
		/// <param name='comparable'>
		/// Comparable.
		/// </param>
		public Node SimilarExists (Node comparable)
		{
			foreach (Node n in Nodes) {
                if (new HashSet<ILTLFormula>(n.Old).SetEquals (comparable.Old)  ) {
					return n;
				}
			}
						
			return default(Node);
		}
		
		/// <summary>
		/// Build this instance.
		/// </summary>
		private void Build ()
		{
//			Node initialNode = new Node();
//            initialNode.Incoming.Add("init");
//            initialNode.New.Add(Formula);
//			
//			if (initialNode.Expand(this) != null) {
//				failed = false;
//			}
		}
	}
}