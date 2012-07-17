using System;
using CheckMyModels;
using NDesk.Options;
using System.Collections.Generic;

namespace LtlSharp.CLI
{
	class MainClass
	{
		public static void Main (string[] args)
        {
            bool show_help = false;
            bool dot = false;
    
            var p = new OptionSet () {
                { "dot", "Print syntaxic tree in dot format.",
                    v => dot = true },
                { "h|help",  "show this message and exit", 
                    v => show_help = v != null },
            };
    
            List<string> extra;
            try {
                extra = p.Parse (args);
            } catch (OptionException e) {
                Console.Write ("ltlsharp: ");
                Console.WriteLine (e.Message);
                Console.WriteLine ("Try `greet --help' for more information.");
                return;
            }
    
            if (show_help) {
                ShowHelp (p);
                return;
            }
    
            if (extra.Count == 0) {
                Console.Write ("ltlsharp: ");
                Console.WriteLine ("Please give at least one formula");
                Console.WriteLine ("Try `ltlsharp --help' for more information.");
    			return;
            }
    
            foreach (var formula in extra) {
                var parsedFormula = Parser.Parse (formula);
                if (dot) {
                    var dotPrettyPrinter = new Dot (parsedFormula);
                    dotPrettyPrinter.PrettyPrint ();
                }
            }
        }
    
        static void ShowHelp (OptionSet p)
        {
            Console.WriteLine ("Usage: ltlsharp [OPTIONS]+ formula");
            Console.WriteLine ();
            Console.WriteLine ("Options:");
            p.WriteOptionDescriptions (Console.Out);
        }
	}
}
