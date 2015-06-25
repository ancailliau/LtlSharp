using System;

namespace LtlSharp
{
    public abstract class Traversal
    {
        protected ILTLFormula formula;
        
        public Traversal (ILTLFormula formula)
        {
            this.formula = formula;
        }

        protected void Visit ()
        {
            Visit (formula);
        }
        
        protected virtual void Visit (ILTLFormula formula)
        {
            if (formula is Proposition) {
                VisitProposition (formula as Proposition);
                
            } else if (formula is Implication) {
                VisitImplication (formula as Implication);
                VisitBinaryOperator (formula as IBinaryOperator);
                
            } else if (formula is Equivalence) {
                VisitEquivalence (formula as Equivalence);
                VisitBinaryOperator (formula as IBinaryOperator);
                
            } else if (formula is Conjunction) {
                VisitConjunction (formula as Conjunction);
                
            } else if (formula is Disjunction) {
                VisitDisjunction (formula as Disjunction);
                
            } else if (formula is Negation) {
                VisitNegation (formula as Negation);
                VisitUnaryOperator (formula as IUnaryOperator);
                
            } else if (formula is Next) {
                VisitNext (formula as Next);
                VisitUnaryOperator (formula as IUnaryOperator);
                
            } else if (formula is Finally) {
                VisitFinally (formula as Finally);
                VisitUnaryOperator (formula as IUnaryOperator);
                
            } else if (formula is Globally) {
                VisitGlobally (formula as Globally);
                VisitUnaryOperator (formula as IUnaryOperator);
                
            } else if (formula is Until) {
                VisitUntil (formula as Until);
                VisitBinaryOperator (formula as IBinaryOperator);
                
            } else if (formula is Release) {
                VisitRelease (formula as Release);
                VisitBinaryOperator (formula as IBinaryOperator);
                
            } else if (formula is Unless) {
                VisitUnless (formula as Unless);
                VisitBinaryOperator (formula as IBinaryOperator);
                
            } else if (formula is StrongEquivalence) {
                VisitStrongEquivalence (formula as StrongEquivalence);
                VisitBinaryOperator (formula as IBinaryOperator);
                
            } else if (formula is StrongImplication) {
                VisitStrongImplication (formula as StrongImplication);
                VisitBinaryOperator (formula as IBinaryOperator);
                
            } else if (formula is ParenthesedExpression) {
                VisitParenthesedExpression (formula as ParenthesedExpression);
                VisitUnaryOperator (formula as IUnaryOperator);
                
            }
        }
        
        protected virtual void VisitProposition           (Proposition           proposition)       {}
        protected virtual void VisitImplication           (Implication           implication)       {}
        protected virtual void VisitEquivalence           (Equivalence           equivalence)       {}
        protected virtual void VisitConjunction           (Conjunction           conjunction)       {}
        protected virtual void VisitDisjunction           (Disjunction           disjunction)       {}
        protected virtual void VisitNegation              (Negation              negation)          {}
        protected virtual void VisitNext                  (Next                  next)              {}
        protected virtual void VisitFinally               (Finally               @finally)          {}
        protected virtual void VisitGlobally              (Globally              globally)          {}
        protected virtual void VisitUntil                 (Until                 until)             {}
        protected virtual void VisitRelease               (Release               release)           {}
        protected virtual void VisitUnless                (Unless                unless)            {}
        protected virtual void VisitStrongEquivalence     (StrongEquivalence     strongEquivalence) {}
        protected virtual void VisitStrongImplication     (StrongImplication     strongImplication) {}
        protected virtual void VisitParenthesedExpression (ParenthesedExpression strongImplication) {}
        
        protected virtual void VisitBinaryOperator    (IBinaryOperator    binaryoperator)
        {
            Visit (binaryoperator.Left);
            Visit (binaryoperator.Right);
        }
        
        protected virtual void VisitUnaryOperator     (IUnaryOperator     unaryoperator)
        {
            Visit (unaryoperator.Enclosed);   
        }
    }
}

