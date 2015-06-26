// 
// Transition.cs
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
using LtlSharp;

namespace LittleSharp.Buchi
{
	
	/// <summary>
	/// A transition between two nodes for a generalized Buchi automaton
	/// </summary>
	public class Transition
	{
		/// <summary>
		/// Gets the list of literals.
		/// </summary>
		/// <value>
		/// The literals attached to the transition.
		/// </value>
        public HashSet<ILiteral> Literals {
			get;
			private set;
		}
		
		private GBANode head = null;
		public GBANode From {
			get {
				return head;
			}
			set {
				if (head != null) {
					head.Outgoing.Remove(this);
				}
				value.Outgoing.Add(this);
				head = value;
			}
		}
		
		private GBANode tail;
		public GBANode To {
			get {
				return tail;
			}
			set {
				if (tail != null) {
					tail.Incoming.Remove(this);
				}
				value.Incoming.Add(this);
				tail = value;
			}
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="LittleSharp.Buchi.Transition"/> class.
		/// </summary>
		public Transition ()
		{
            Literals = new HashSet<ILiteral>();
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="LittleSharp.Buchi.Transition"/> class.
		/// </summary>
		/// <param name='root'>
		/// Root.
		/// </param>
		public Transition (GBANode from, GBANode to)
			: this ()
		{
			this.From = from;
			this.To = to;
		}

	}
}

