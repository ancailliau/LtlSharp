﻿using System;
using System.Collections.Generic;

namespace LtlSharp.Buchi.LTL2Buchi
{
    public interface ILTL2Buchi
    {
        HashSet<Node> CreateGraph (ILTLFormula phi);
        GeneralizedBuchiAutomata GetAutomaton (ILTLFormula phi);
    }
}

