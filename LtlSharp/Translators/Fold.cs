using System;
using QuickGraph;
using LtlSharp.Buchi.Automata;
using System.Linq;
using LtlSharp.Language;
using LtlSharp.Automata;
using LtlSharp.Automata.OmegaAutomata;

namespace LtlSharp.Translators
{
    public class Fold<T,T2> : Transformer<T,T>
        where T : OmegaAutomaton<T2>
        where T2 : IAutomatonNode
    {
        public Fold ()
        {
        }
        
        public override T Transform (T target)
        {
            target.SimplifyTransitions ();
            return target;
        }
        
    }
}

