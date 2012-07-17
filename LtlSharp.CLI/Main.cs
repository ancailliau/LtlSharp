using System;
using CheckMyModels;

namespace LtlSharp.CLI
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			var pp = new Dot (Parser.Parse (args[0]));
			Console.WriteLine (pp.PrettyPrint ());
		}
	}
}
