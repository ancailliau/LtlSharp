using System;
using LittleSharp.Buchi;
using LtlSharp.Buchi.LTL2Buchi;
using System.Linq;
using System.Collections.Generic;
using LtlSharp.Buchi;
using LtlSharp.Buchi.Translators;
using LtlSharp.Buchi.Automata;
using LtlSharp.Monitors;

namespace LtlSharp.Temp
{
    class MainClass
    {
        public static void Main (string[] args)
        {
            var f = new Until (new Proposition ("spawn").Negate (), new Proposition ("init"));
            var m = new LTLMonitor (f);
            
        }
    }
}
