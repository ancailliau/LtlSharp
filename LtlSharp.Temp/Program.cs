using System;
using LittleSharp.Buchi;
using LtlSharp.Buchi.LTL2Buchi;

namespace LtlSharp.Temp
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            Console.WriteLine ("Hello World!");
            
            ILTLFormula f = new StrongImplication (new Proposition ("a"), new Proposition ("b"));
            Console.WriteLine (f);
            
            f = f.Normalize ();
            Console.WriteLine (f);
            
            ILTL2Buchi t = new Gia01 ();
            var nset = t.CreateGraph (f.Normalize ());
            
            var buchi = new GeneralizedBuchiAutomaton (nset, f);
            Console.WriteLine (buchi);
            
            var ec = new EmptinessChecker (buchi);
            Console.WriteLine (ec.IsEmpty ());
        }
    }
}
