using System;
using System.Linq;
using System.Collections.Generic;

namespace LtlSharp.Utils
{
    public class CanonicToString : Traversal
    {
        public string String {
            get;
            private set;
        }
        
        public CanonicToString (LTLFormula formula)
            : base (formula)
        {
            Visit (formula);
        }
        
        protected override void VisitProposition (Proposition           proposition)
        {
            String += proposition.Name;
        }
        
        protected override void VisitImplication (Implication           implication)
        {
            Visit (implication.Left);
            String += " -> ";
            Visit (implication.Right);
        }

        protected override void VisitEquivalence (Equivalence           equivalence)
        {
            Visit (equivalence.Left);
            String += " <-> ";
            Visit (equivalence.Right);
        }

        protected override void VisitConjunction (Conjunction           conjunction)
        {
            Visit (conjunction.Expressions.First());
            for (int i = 1; i < conjunction.Expressions.Count; i++) {
                String += " & ";
                Visit (conjunction.Expressions[i]);
            }
        }

        protected override void VisitDisjunction (Disjunction           disjunction)
        {
            Visit (disjunction.Expressions.First());
            for (int i = 1; i < disjunction.Expressions.Count; i++) {
                String += " | ";
                Visit (disjunction.Expressions[i]);
            }
        }

        protected override void VisitNegation (Negation              negation)
        {
            String += "! ";
            Visit (negation.Enclosed);
        }

        protected override void VisitNext (Next                  next)
        {
            String += "X ";
            Visit (next.Enclosed);
        }

        protected override void VisitFinally (Finally               @finally)
        {
            String += "F ";
            Visit (@finally.Enclosed);   
        }

        protected override void VisitGlobally (Globally              globally)
        {
            String += "G ";
            Visit (globally.Enclosed);   
        }

        protected override void VisitUntil (Until                 until)
        {
            Visit (until.Left);
            String += " U ";
            Visit (until.Right);
        }

        protected override void VisitRelease (Release               release)
        {
            Visit (release.Left);
            String += " V ";
            Visit (release.Right);   
        }

        protected override void VisitUnless (Unless                unless)
        {
            Visit (unless.Right);
            String += " V (";   
            Visit (unless.Right);
            String += " | ";  
            Visit (unless.Left);
            String += ")";
        }

        protected override void VisitStrongEquivalence (StrongEquivalence     strongEquivalence)
        {
            String += "G (";
            Visit (strongEquivalence.Left);
            String += " <-> ";
            Visit (strongEquivalence.Right);
            String += ")";
        }

        protected override void VisitStrongImplication (StrongImplication     strongImplication)
        {
            String += "G (";
            Visit (strongImplication.Left);
            String += " -> ";
            Visit (strongImplication.Right);
            String += ")";
        }

        protected override void VisitParenthesedExpression (ParenthesedExpression expression)
        {
            String += "(";
            Visit (expression.Enclosed);
            String += ")";
        }
        
        protected override void VisitBinaryOperator    (BinaryOperator    binaryoperator) {}
        protected override void VisitUnaryOperator     (UnaryOperator     unaryoperator)  {}
        protected override void VisitNaryOperator      (NaryOperator      naryoperator)   {}
    }
}

