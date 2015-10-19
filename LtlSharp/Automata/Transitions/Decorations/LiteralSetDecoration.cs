using System;
using System.Collections.Generic;

namespace LtlSharp.Automata.Transitions.Decorations
{
    /// <summary>
    /// Defines a decoration with a literal set.
    /// </summary>
    public class LiteralSetDecoration : ITransitionDecoration<LiteralSetDecoration>
    {
        private LiteralSet _literalSet;
        
        public LiteralSet LiteralSet {
            get {
                return _literalSet;
            }
        }
        
        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:LtlSharp.Automata.Transitions.Decorations.LiteralSetDecoration"/> class with an empty set of
        /// literals.
        /// </summary>
        public LiteralSetDecoration ()
        {
            _literalSet = new LiteralSet ();
        }

        /// <summary>
        /// Initializes a new instance of the
        /// <see cref="T:LtlSharp.Automata.Transitions.Decorations.LiteralSetDecoration"/> class with the specified
        /// literals.
        /// </summary>
        /// <param name="nl">Nl.</param>
        public LiteralSetDecoration (IEnumerable<ILiteral> nl)
        {
            _literalSet = new LiteralSet (nl);
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

