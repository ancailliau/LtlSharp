using System;
using LtlSharp;
using System.Collections.Generic;

namespace CheckMyModels
{
	public class Dot
	{
		public Dot (LTLFormula formula)
		{
			this.formula = formula;
		}

		private LTLFormula formula;
		private int i = 0;
		private Dictionary<LTLFormula, string> mapping;

		private string pp (LTLFormula e)
		{
			if (e is BinaryOperator) {
				var element = (BinaryOperator) e;

				mapping.Add (element.Left, "node"+(++i));
				mapping.Add (element.Right, "node"+(++i));

				return string.Format ("\t{0} -> {2};\n"
				                      + "\t{0} -> {4};\n",
				                      mapping[element], element, mapping[element.Left], element.Left, mapping[element.Right], element.Right)
					+ pp (element.Left) + pp (element.Right);


			} else if (e is UnaryOperator) {
				var element = (UnaryOperator) e;
				mapping.Add (element.Enclosed, "node"+(++i));

				return string.Format ("\t{0} -> {2};\n",
				                      mapping[element], element, mapping[element.Enclosed], element.Enclosed)
					+ pp (element.Enclosed);

			} else if (e is NaryOperator) {
				var element = (NaryOperator) e;
				string output = "";
				foreach (var el in element.Expressions) {
					mapping.Add (el, "node"+(++i));
					output += string.Format ("\t{0} -> {2};\n",
				                      mapping[element], element, mapping[el], el) + pp (el);
				}
				return output;

			} else if (e is Proposition) {

			}
			return "";
		}

		private string Name (LTLFormula f)
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

		public string PrettyPrint ()
		{
			mapping = new Dictionary<LTLFormula, string> ();

			i++;
			mapping.Add (formula, "node"+i);

			string output = "digraph G { \n";
			output += pp (formula);

			foreach (var v in mapping.Keys) {
				output += "\t" + mapping[v] + "[label=\"" + Name(v) + "\"];\n";
			}

			return output + "}";
		}
	}
}

