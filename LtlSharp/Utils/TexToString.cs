using System;
using System.Linq;
using System.Collections.Generic;

namespace LtlSharp.Utils
{
    public class TexToString : Traversal
    {
        public string String {
            get;
            private set;
        }
        
        public TexToString (ILTLFormula formula)
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
            String += " \\rightarrow ";
            Visit (implication.Right);
        }

        protected override void VisitEquivalence (Equivalence           equivalence)
        {
            Visit (equivalence.Left);
            String += " \\leftrightarrow ";
            Visit (equivalence.Right);
        }

        protected override void VisitConjunction (Conjunction           conjunction)
        {
            Visit (conjunction.Left);
            String += " \\wedge ";
            Visit (conjunction.Right);
        }

        protected override void VisitDisjunction (Disjunction           disjunction)
        {
            Visit (disjunction.Left);
            String += " \\vee ";
            Visit (disjunction.Right);
        }

        protected override void VisitNegation (Negation              negation)
        {
            String += "\\neg ";
            Visit (negation.Enclosed);
        }

        protected override void VisitNext (Next                  next)
        {
            String += "\\bigcirc\\ ";
            Visit (next.Enclosed);
        }

        protected override void VisitFinally (Finally               @finally)
        {
            String += "\\Diamond\\ ";
            Visit (@finally.Enclosed);   
        }

        protected override void VisitGlobally (Globally              globally)
        {
            String += "\\Box\\ ";
            Visit (globally.Enclosed);   
        }

        protected override void VisitUntil (Until                 until)
        {
            Visit (until.Left);
            String += "\\ U\\ ";
            Visit (until.Right);
        }

        protected override void VisitRelease (Release               release)
        {
            Visit (release.Left);
            String += "\\ V\\ ";
            Visit (release.Right);   
        }

        protected override void VisitUnless (Unless                unless)
        {
            Visit (unless.Left);
            String += "\\ W\\ ";  
            Visit (unless.Right);
        }

        protected override void VisitStrongEquivalence (StrongEquivalence     strongEquivalence)
        {
            Visit (strongEquivalence.Left);
            String += " \\Leftrightarrow ";
            Visit (strongEquivalence.Right);
        }

        protected override void VisitStrongImplication (StrongImplication     strongImplication)
        {
            Visit (strongImplication.Left);
            String += " \\Rightarrow ";
            Visit (strongImplication.Right);
        }

        protected override void VisitParenthesedExpression (ParenthesedExpression expression)
        {
            String += "(";
            Visit (expression.Enclosed);
            String += ")";
        }
        
        protected override void VisitBinaryOperator    (IBinaryOperator    binaryoperator) {}
        protected override void VisitUnaryOperator     (IUnaryOperator     unaryoperator)  {}
    }
}
