using System;
using NDesk.Options;
using System.Collections.Generic;
using LtlSharp.Utils;
using System.Linq;

namespace LtlSharp.CLI
{
	class MainClass
	{
		public static void Main (string[] args)
        {
            return;
            /*
            bool show_help = false;
            bool dot       = false;
            bool alphabet  = false;
    
            var p = new OptionSet () {
                { "dot", "Print syntaxic tree in dot format.",
                    v => dot = true },
                { "alphabet", "Print alphabet",
                    v => alphabet = true },
                { "h|help",  "show this message and exit", 
                    v => show_help = v != null },
            };
    
            List<string> extra;
            try {
                extra = p.Parse (args);
                
            } catch (OptionException e) {
                PrintError (e.Message);
                return;
            }
    
            if (show_help) {
                ShowHelp (p);
                return;
            }
    
            if (extra.Count == 0) {
                PrintError ("Please provide at least one formula.");
    			return;
            }
    
            foreach (var formula in extra) {
                var parsedFormula = Parser.Parse (formula);
                
                if (parsedFormula == null) {
                    PrintError (string.Format ("Formula '{0}' could not be parsed. Check if it is well-formed.", formula));
                    return;
                }
                
                if (dot) {
                    var dotPrettyPrinter = new DotPrettyPrinter (parsedFormula, Console.Out);
                    dotPrettyPrinter.PrettyPrint ();
                }
                if (alphabet) {
                    var alphabetExtractor = new ExtractAlphabet (parsedFormula);
                    Console.WriteLine (string.Join (", ", alphabetExtractor.Alphabet.ToArray ()));
                }
            }
            */
        }
    
        static void ShowHelp (OptionSet p)
        {
            Console.WriteLine ("Usage: ltlsharp [OPTIONS]+ formula");
            Console.WriteLine ();
            Console.WriteLine ("Options:");
            p.WriteOptionDescriptions (Console.Out);
        }
        
        static void PrintError (string error)
        {  
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.Write ("ltlsharp: ");
            Console.Error.WriteLine (error);
            Console.Error.WriteLine ("Try `ltlsharp --help' for more information.");
            Console.ResetColor ();
        }
	}
}
