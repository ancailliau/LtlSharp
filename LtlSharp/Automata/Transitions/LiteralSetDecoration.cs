using System;
using System.Collections.Generic;

namespace LtlSharp.Automata.Transitions
{
    public class LiteralSetDecoration : IAutomatonTransitionDecorator<LiteralSetDecoration>
    {
        private LiteralsSet _literalSet;
        
        public LiteralSetDecoration ()
        {
        }

        public LiteralSetDecoration (IEnumerable<ILiteral> nl)
        {
            _literalSet = new LiteralsSet (nl);
        }
        
        [Obsolete]
        public bool Entails (LiteralSetDecoration l)
        {
            throw new NotImplementedException ();
        }
        
        [Obsolete]
        public IEnumerable<ILiteral> GetAlphabet ()
        {
            throw new NotImplementedException ();
        }

        public LiteralsSet ToLiteralSet ()
        {
            return _literalSet;
        }

        [Obsolete]
        public IEnumerable<LiteralSetDecoration> UnfoldLabels (IEnumerable<ILiteral> enumerable)
        {
            throw new NotImplementedException ();
        }
        
        public override string ToString ()
        {
            return _literalSet.ToString ();
        }
        
        public override bool Equals (object obj)
        {
            if (obj == null)
                return false;
            if (ReferenceEquals (this, obj))
                return true;
            if (obj.GetType () != typeof(LiteralSetDecoration))
                return false;
            var other = (LiteralSetDecoration)obj;
            return _literalSet.Equals (other._literalSet);
        }

        public override int GetHashCode ()
        {
            return _literalSet.GetHashCode ();
        }
    }
}

