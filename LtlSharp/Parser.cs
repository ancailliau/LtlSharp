using System;
using Antlr.Runtime;

namespace LtlSharp
{
	public class Parser
	{
		public static LTLFormula Parse (string expression)
		{
			var input = new ANTLRStringStream(expression); 
			var lexer = new LTLLexer(input);
			var token = new CommonTokenStream(lexer);
			var parser = new LTLParser(token);
            var a = parser.parse ();
			return parser.NumberOfSyntaxErrors > 0 ? null : a.value;
		}
	}
}

