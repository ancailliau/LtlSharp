using System;
using LtlSharp;
using System.Collections.Generic;
using System.IO;

namespace LtlSharp.PrettyPrinters
{
	public class Dot
	{
		private int i = 0;
        private LTLFormula formula;
        private Dictionary<LTLFormula, string> mapping;

        public Dot (LTLFormula formula)
		{
			this.formula = formula;
		}

		private string GetNextId ()
        {
            return "node" + (++i);
        }
        
		private void PrettyPrint (LTLFormula formula, TextWriter writer)
		{
			if (formula is BinaryOperator) {
				var @operator = (BinaryOperator) formula;

				mapping.Add (@operator.Left,  GetNextId ());
				mapping.Add (@operator.Right, GetNextId ());
                
                writer.WriteLine ("\t{0} -> {1};", mapping[@operator], mapping[@operator.Left]);
                writer.WriteLine ("\t{0} -> {1};", mapping[@operator], mapping[@operator.Right]);
               
                PrettyPrint (@operator.Left,  writer);
                PrettyPrint (@operator.Right, writer);

			} else if (formula is UnaryOperator) {
				var @operator = (UnaryOperator) formula;
                mapping.Add (@operator.Enclosed, GetNextId ());
				writer.WriteLine ("\t{0} -> {1};", mapping[@operator], mapping[@operator.Enclosed]);               
                PrettyPrint (@operator.Enclosed, writer);
                
			} else if (formula is NaryOperator) {
				var @operator = (NaryOperator) formula;                
				foreach (var expression in @operator.Expressions) {
					mapping.Add (expression, GetNextId ());
                    writer.WriteLine ("\t{0} -> {1};", mapping[@operator], mapping[expression]);               
                    PrettyPrint (expression, writer);
				}
            }
		}

		private string GetNameFor (LTLFormula f)
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
			} else if (f is ParenthesedExpression) {
				return "(...)";
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

		public void PrettyPrint ()
		{
            PrettyPrint (Console.Out);
        }
        
        public void PrettyPrint (TextWriter writer)
        {
            mapping = new Dictionary<LTLFormula, string> ();
			mapping.Add (formula, GetNextId ());

			writer.WriteLine ("digraph G {");
			PrettyPrint (formula, writer);

			foreach (var element in mapping.Keys) {
                writer.WriteLine ("\t{0}[label=\"\"]", mapping[element], GetNameFor(element));
			}

			writer.WriteLine ("}");
		}
	}
}

