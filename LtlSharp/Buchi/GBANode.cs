using System;
using System.Collections.Generic;

namespace LittleSharp.Buchi
{
	public class GBANode
	{
		
		public List<Transition> Incoming {
			get;
			private set;
		}
		
		public List<Transition> Outgoing {
			get;
			private set;
		}
		
		public string Name {
			get;
			set;
		}
		
		public bool Initial {
			get;
			set;
		}
		
		public List<int> AcceptingSet {
			get ;
			set ; 
		}
		
		public GBANode (string name)
		{
			Name = name;
			Incoming = new List<Transition>();
			Outgoing = new List<Transition>();
			Initial = false;
			AcceptingSet = new List<int>();
		}
		
		public GBANode (string name, bool initial)
			: this(name)
		{
			Initial = initial;
		}
		
	}
}

