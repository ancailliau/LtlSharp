using System;
using LtlSharp.Buchi.LTL2Buchi;
using LtlSharp.Buchi.Translators;

namespace LtlSharp.Monitors
{
    public class LTLMonitor
    {

        public LTLMonitor (ILTLFormula formula)
        {
            var a = new GPVW ();
            var positiveGBA = a.GetAutomaton (formula);
            var negativeGBA = a.GetAutomaton (formula.Negate ());

            var positiveBA = GBA2BA.Transform (positiveGBA);
            var negativeBA = GBA2BA.Transform (negativeGBA);

            var positiveNFA = BA2NFA.Transform (positiveBA);
            var negativeNFA = BA2NFA.Transform (negativeBA);

            positiveNFA.ToSingleInitialState ();
            negativeNFA.ToSingleInitialState ();
            
            Console.WriteLine ("--");
            Console.WriteLine (string.Join ("\n", positiveNFA.Nodes));
            Console.WriteLine ("--");
            Console.WriteLine (string.Join ("\n", negativeNFA.Nodes));
            Console.WriteLine ("--");
            
            Console.WriteLine (positiveNFA.IsDeterministic ());
            Console.WriteLine (negativeNFA.IsDeterministic ());
            
            Console.WriteLine ("--");
            
        }
    }
}

