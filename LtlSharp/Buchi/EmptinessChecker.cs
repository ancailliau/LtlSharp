using System;
using System.Linq;
using System.Collections.Generic;

namespace LittleSharp.Buchi
{
	
	/// <summary>
	/// The emptiness checker.
	/// </summary>
	public class EmptinessChecker
	{
		
		/// <summary>
		/// Gets the automaton to check.
		/// </summary>
		/// <value>
		/// The automaton.
		/// </value>
		public GeneralizedBuchiAutomaton Automaton {
			get;
			private set;
		}
		
		/// <summary>
		/// The stack for the first DFS.
		/// </summary>
		private Stack<GBANode> stackForFirstDFS;
		
		/// <summary>
		/// The stack for the second DFS.
		/// </summary>
		private Stack<GBANode> stackForSecondDFS;
		
		/// <summary>
		/// Initializes a new instance of the <see cref="LittleSharp.Buchi.EmptinessChecker"/> class.
		/// </summary>
		/// <param name='automaton'>
		/// The automaton to check.
		/// </param>
		public EmptinessChecker (GeneralizedBuchiAutomaton automaton)
		{
			Automaton = automaton;
		}
		
		/// <summary>
		/// Determines whether the automaton is empty.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the automaton is empty; otherwise, <c>false</c>.
		/// </returns>
		public bool IsEmpty()
		{
			return !IsNotEmpty();
		}
		
		/// <summary>
		/// Determines whether the automaton is not empty.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the automaton is not empty; otherwise, <c>false</c>.
		/// </returns>
		public bool IsNotEmpty()
		{
			if (Automaton.Nodes.Count <= 1)
				return false;
			
			bool result = true;
			foreach (List<GBANode> set in Automaton.AcceptanceSet) {
				bool localResult = false;
				
				this.stackForFirstDFS = new Stack<GBANode>();
				
				foreach (var node in Automaton.Nodes.Where(n => n.Value.Initial)) {
					if (dfs1(node.Value, set)) {
						localResult = true;
					}
				}
				result &= localResult;					
			}
			
			return result;
		}
		
		/// <summary>
		/// Perform the first DFS.
		/// </summary>
		/// <param name='n'>
		/// The node to start with.
		/// </param>
		private bool dfs1(GBANode n, List<GBANode> set)
		{
			this.stackForFirstDFS.Push(n);
			foreach (Transition t in n.Outgoing) {
				GBANode n2 = t.To;
				if (!this.stackForFirstDFS.Contains(n2)) {
					if (dfs1(n2, set)) {
						return true;
					}
				}
			}
			if (set.Contains(n)) {
				this.stackForSecondDFS = new Stack<GBANode>();
				return dfs2(n);
			}
			return false;
		}
				
		/// <summary>
		/// Perform the second DFS.
		/// </summary>
		/// <param name='n'>
		/// The node to start with.
		/// </param>
		private bool dfs2(GBANode n) {
			this.stackForSecondDFS.Push(n);
			foreach (Transition t in n.Outgoing) {
				GBANode n2 = t.To;
				if (this.stackForFirstDFS.Contains(n2)) {
					return true;
					
				} else if (!this.stackForSecondDFS.Contains(n2)) {
					if(dfs2(n2)) {
						return true;
					}
				}
			}
			return false;
		}
		
	}
}

