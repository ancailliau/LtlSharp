using System;
using LtlSharp.Models;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.ProbabilisticSystems;

namespace LtlSharp.ModelCheckers
{
    public class PCTLModelChecker
    {
        MarkovChain mc;
        ITLFormula phi;
        double epsilon;
        
        public PCTLModelChecker (MarkovChain mc, ITLFormula phi, double epsilon)
        {
            this.mc = mc;
            this.phi = phi.Normalize ();
            this.epsilon = epsilon;
        }
        
        public HashSet<MarkovNode> Check ()
        {
            var sat = new Dictionary<ITLFormula, HashSet<MarkovNode>> ();
            
            Console.WriteLine ("Initial formula " + phi);
            
            foreach (var f in EnumerateSubFormulae (this.phi)) {
                Console.WriteLine ("Enumerate : " + f);
                if (sat.ContainsKey (f))
                    continue;
                
                var satSet = ComputeSatisfactionSet (f, sat);
                sat.Add (f, satSet);
            }
            
            Console.WriteLine (phi);
            
            return sat[phi];
        }
        
        IEnumerable<ITLFormula> EnumerateSubFormulae (ITLFormula phi)
        {   
            if (phi is IBinaryOperator) {
                var binary = (IBinaryOperator)phi;
                foreach (var s in EnumerateSubFormulae(binary.Left)) {
                    yield return s;
                }
                foreach (var s in EnumerateSubFormulae(binary.Right)) {
                    yield return s;
                }
                if (phi is Conjunction )
                    yield return phi;
                
            } else if (phi is IUnaryOperator) {
                var unary = (IUnaryOperator)phi;
                foreach (var s in EnumerateSubFormulae(unary.Enclosed)) {
                    yield return s;
                }
                if (phi is Negation | phi is ProbabilisticOperator)
                    yield return phi;
                
            } else if (phi is ILiteral) {
                yield return phi;
            }
        }
        
        HashSet<MarkovNode> ComputeSatisfactionSet (ITLFormula phi, Dictionary<ITLFormula, HashSet<MarkovNode>> sat)
        {
            // See "Principles of Model-Checking", p343 and p774ff
            
            if (phi is True)
                return new HashSet<MarkovNode> (mc.Nodes);
            
            if (phi is False)
                return new HashSet<MarkovNode> ();
            
            if (phi is Proposition)
                return new HashSet<MarkovNode> (mc.Nodes.Where (n => n.Labels.Contains (phi)));
            
            if (phi is Conjunction) {
                var c = (Conjunction)phi;
                if (!sat.ContainsKey (c.Left) | !sat.ContainsKey(c.Right)) {
                    throw new InvalidProgramException (c.Left + " or " + c.Right + " was not precomputed.");
                }
                return new HashSet<MarkovNode> (sat [c.Left].Intersect (sat [c.Right]));
            }
            
            if (phi is Disjunction) {
                var c = (Disjunction)phi;
                if (!sat.ContainsKey (c.Left) | !sat.ContainsKey(c.Right)) {
                    throw new InvalidProgramException (c.Left + " or " + c.Right + " was not precomputed.");
                }
                return new HashSet<MarkovNode> (sat [c.Left].Union (sat [c.Right]).Distinct ());
            }
            
            if (phi is Negation) {
                var e = (Negation)phi;
                if (!sat.ContainsKey (e.Enclosed)) {
                    throw new InvalidProgramException (e.Enclosed + " was not precomputed.");
                }
                return new HashSet<MarkovNode> (mc.ExceptNodes (sat[e.Enclosed]));
            }
            
            if (phi is ProbabilisticOperator) {
                var po = (ProbabilisticOperator)phi;
                var enclosed = po.Enclosed;
                if (enclosed is Next) {
                    var nodes = mc.Nodes.Where (n => po.IsSatisfied (sat [enclosed].Sum (s => mc.GetEdge (n, s)?.Probability ?? 0), epsilon));
                    return new HashSet<MarkovNode> (nodes);
                    
                } else if (enclosed is Until) {
                    var u = (Until)enclosed;
                    var n2 = mc.ConstrainedReachability (sat [u.Left], sat [u.Right]);
                    var nodes = n2.Where (kv => po.IsSatisfied (kv.Value, epsilon)).Select (kv => kv.Key);
                    return new HashSet<MarkovNode> (nodes);
                    
                } else if (enclosed is Release) { // A R B is !(!A U !B)
                    var u = (Release)enclosed;
                    var n2 = mc.ConstrainedReachability (mc.ExceptNodes (sat [u.Left]), mc.ExceptNodes (sat [u.Right]));
                    var nodes = n2.Where (kv => po.IsSatisfied (kv.Value, epsilon)).Select (kv => kv.Key);
                    return new HashSet<MarkovNode> (mc.ExceptNodes (nodes));
                
                } else {
                    var result = mc.QuantitativeLinearProperty (enclosed);
                    var nodes = result.Where (kv => po.IsSatisfied (kv.Value, epsilon)).Select (kv => kv.Key);
                    return new HashSet<MarkovNode> (nodes);
                }
            }
            
            throw new NotImplementedException ("Operator " + phi.GetType () + " is not supported.");
        }
    }
}

