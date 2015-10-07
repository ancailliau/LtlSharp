using System;
using LtlSharp;
using System.Collections.Generic;
using System.IO;

namespace LtlSharp.Utils
{
	public class DotPrettyPrinter : Traversal
	{
		private int i = 0;
        private Dictionary<ITLFormula, string> mapping;
        private TextWriter writer;

        public DotPrettyPrinter (ITLFormula formula, TextWriter writer)
            : base (formula)
		{
            this.writer = writer;
        }
        
        public void PrettyPrint ()
        {
            mapping = new Dictionary<ITLFormula, string> ();
            mapping.Add (formula, GetNextId ());

            writer.WriteLine ("digraph G {");
            Visit ();

            foreach (var element in mapping.Keys) {
                writer.WriteLine ("\t{0}[label=\"{1}\"]", mapping[element], GetNameFor(element));
            }

            writer.WriteLine ("}");
        }        

		private string GetNextId ()
        {
            return "node" + (++i);
        }
        
        protected override void VisitBinaryOperator (IBinaryOperator @operator)
        {
            mapping.Add (@operator.Left,  GetNextId ());
            mapping.Add (@operator.Right, GetNextId ());
                
            writer.WriteLine ("\t{0} -> {1};", mapping[@operator], mapping[@operator.Left]);
            writer.WriteLine ("\t{0} -> {1};", mapping[@operator], mapping[@operator.Right]);
            
            Visit (@operator.Left);
            Visit (@operator.Right);
        }
        
        protected override void VisitUnaryOperator (IUnaryOperator @operator)
        {
            mapping.Add (@operator.Enclosed, GetNextId ());
            writer.WriteLine ("\t{0} -> {1};", mapping[@operator], mapping[@operator.Enclosed]);
            
            Visit (@operator.Enclosed);
        }
		private string GetNameFor (ITLFormula f)
		{
			if (f is Proposition) {
				return (f as Proposition).Name;
			} else if (f is Implication) {
				return "->";
			} else if (f is Equivalence) {
				return "<->";
			} else if (f is Conjunction) {
				return @"and";
			} else if (f is Disjunction) {
				return @"or";
			} else if (f is Negation) {
				return "!";
			} else if (f is Next) {
				return "X";
			} else if (f is Finally) {
				return "F";
			} else if (f is Globally) {
				return "G";
			} else if (f is Until) {
				return "U";
			} else if (f is Release) {
				return "R";
			} else if (f is Unless) {
				return "W";
			} else if (f is StrongEquivalence) {
				return "<=>";
			} else if (f is StrongImplication) {
				return "=>";
			}

			return f.ToString ();
		}
	}
}

