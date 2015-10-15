﻿using System;
using System.Collections.Generic;
using LtlSharp.Utils;
using System.Linq;

namespace LtlSharp.Automata.Transitions
{
    public class DegeneralizerAutomataTransition
        : IAutomatonTransitionDecorator<DegeneralizerAutomataTransition>
    {   
        public HashSet<int> Labels;
        public bool Else;
        
        public DegeneralizerAutomataTransition () : this (false, Enumerable.Empty<int> ())
        {}
        
        public DegeneralizerAutomataTransition (bool @else) : this (@else, Enumerable.Empty<int> ())
        {}

        public DegeneralizerAutomataTransition (IEnumerable<int> labels): this (false, labels)
        {}
        
        public DegeneralizerAutomataTransition (bool @else, IEnumerable<int> labels)
        {
            Labels = new HashSet<int> (labels);
            Else = @else;
        }

        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(DegeneralizerAutomataTransition))
                return false;
            var other = (DegeneralizerAutomataTransition)obj;
            return Labels.SetEquals (other.Labels) & Else == other.Else;
        }

        public override int GetHashCode ()
        {
            return 17 + 23 * (base.GetHashCode () + 23 * (Labels.GetHashCodeForElements () + 23 * Else.GetHashCode ()));
        }

        public IEnumerable<ILiteral> GetAlphabet ()
        {
            throw new NotImplementedException ();
        }

        IEnumerable<ILiteral> IAutomatonTransitionDecorator<DegeneralizerAutomataTransition>.GetAlphabet ()
        {
            throw new NotImplementedException ();
        }

        LiteralsSet IAutomatonTransitionDecorator<DegeneralizerAutomataTransition>.ToLiteralSet ()
        {
            throw new NotImplementedException ();
        }

        bool IAutomatonTransitionDecorator<DegeneralizerAutomataTransition>.Entails (DegeneralizerAutomataTransition l)
        {
            throw new NotImplementedException ();
        }

        IEnumerable<DegeneralizerAutomataTransition> IAutomatonTransitionDecorator<DegeneralizerAutomataTransition>.UnfoldLabels (IEnumerable<ILiteral> enumerable)
        {
            throw new NotImplementedException ();
        }
    }
}

