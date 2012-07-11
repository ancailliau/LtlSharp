using System;
using Antlr.Runtime;

namespace LtlSharp
{
	public class Parser
	{
		public static Expression Parse (string expression)
		{
			var input = new ANTLRStringStream(expression); 
			var lexer = new LTLLexer(input);
			var token = new CommonTokenStream(lexer);
			var parser = new LTLParser(token);
			return parser.parse ().value;
		}
	}
}

