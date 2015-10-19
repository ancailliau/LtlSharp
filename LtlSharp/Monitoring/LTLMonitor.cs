using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Buchi;
using LtlSharp.Buchi.LTL2Buchi;

using QuickGraph.Graphviz;
using QuickGraph;
using LtlSharp.Language;
using LtlSharp.Automata;
using LtlSharp.Automata.FiniteAutomata;
using LtlSharp.Automata.Utils;

namespace LtlSharp.Monitoring
{
    public enum MonitorStatus {
        True, False, Inconclusive
    }
    
    public class LTLMonitor
    {
        public PowerSetAutomatonNode<AutomatonNode> currentNegative;
        public PowerSetAutomatonNode<AutomatonNode> currentPositive;
        public NFA<PowerSetAutomatonNode<AutomatonNode>> negativeNFA;
        public NFA<PowerSetAutomatonNode<AutomatonNode>> positiveNFA;
        Gia02 translator = new Gia02 ();

        public MonitorStatus Status { get; private set; }
        
        public LTLMonitor (ITLFormula formula)
        {
            var tpositiveNFA = translator.GetBuchiAutomaton (formula).ToNFA();
            var tnegativeNFA = translator.GetBuchiAutomaton (formula.Negate ()).ToNFA();

            positiveNFA = tpositiveNFA.Determinize ();
            negativeNFA = tnegativeNFA.Determinize ();
            
            currentNegative = negativeNFA.InitialNode;
            currentPositive = positiveNFA.InitialNode;
            
            UpdateStatus ();
        }

        public void Step (MonitoredState state)
        {
            if (currentNegative == null | currentPositive == null)
                return;

            var successors = positiveNFA.Post (currentPositive, (l, t) => state.Evaluate (l.LiteralSet));
            if (successors.Count () == 1) {
                currentPositive = successors.Single ();

            } else if (successors.Count () == 0) {
                // There is no way to satisfy the formula
                currentPositive = null;
                Status = MonitorStatus.False;
                return;

            } else {
                throw new NotImplementedException ("Non deterministic automaton not supported.");
            }

            successors = negativeNFA.Post (currentNegative,(l, t) => state.Evaluate (l.LiteralSet));
            if (successors.Count () == 1) {
                currentNegative = successors.Single ();

            } else if (successors.Count () == 0) {
                // There is no way to dissatisfy the formula
                currentNegative = null;
                Status = MonitorStatus.True;
                return;

            } else {
                throw new NotImplementedException ("Non deterministic automaton not supported.");
            }

            UpdateStatus ();
        }

        void UpdateStatus ()
        {
            var negativeAcceptance = negativeNFA.AcceptingNodes.Contains (currentNegative);
            var positiveAcceptance = positiveNFA.AcceptingNodes.Contains (currentPositive);
            if (negativeAcceptance & positiveAcceptance) {
                Status = MonitorStatus.Inconclusive;
            } else if (!negativeAcceptance) {
                Status = MonitorStatus.True;
            } else if (!positiveAcceptance) {
                Status = MonitorStatus.False;
            } else {
                throw new NotImplementedException ();
            }
        }
    }
}

