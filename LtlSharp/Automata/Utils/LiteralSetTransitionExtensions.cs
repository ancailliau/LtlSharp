using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Automata.Transitions;
using LtlSharp.Language;

namespace LtlSharp.Automata.Utils
{
    public static class LiteralSetTransitionExtensions
    {
        public static void SimplifyTransitions<T> (this Automata<T, LiteralSetDecoration> automata)
            where T : IAutomatonNode
        {
            foreach (var trans in automata.Edges) {
                var sameTarget = automata.Edges.Where (t => t.Item3.Equals (trans.Item3)).ToList ();
                var labels = sameTarget.Select (x => x.Item2.ToLiteralSet ());
                    var lf = new LiteralFormula (labels);
                    var newLabels = lf.Simplify ();
                    foreach (var e in sameTarget) {
                    automata.RemoveTransition (e.Item1, e.Item3, e.Item2);
                    }
                    foreach (var nl in newLabels) {
                    automata.AddTransition (trans.Item1, trans.Item3, new LiteralSetDecoration (nl));
                    }
                }
        }

        public static void UnfoldTransitions<T> (this Automata<T, LiteralSetDecoration> automata)
            where T : IAutomatonNode
        {
            var alphabet = automata.Alphabet ().ToList ();
            Console.WriteLine ("Alphabet = {" + string.Join (",", alphabet) + "}");
            automata.MapLabel ((arg) => arg.ToLiteralSet().Expand (alphabet).Select (x => new LiteralSetDecoration (x)));
        }

        /// <summary>
        /// Returns the set of literals that correspond to (at least) a transition.
        /// </summary>
        /// <param name="node">Node.</param>
        public static IEnumerable<ILiteral> Alphabet<T> (this Automata<T, LiteralSetDecoration> automata)
            where T : IAutomatonNode
        {
            return automata.Edges.SelectMany (e => e.Item2.ToLiteralSet ().GetAlphabet ()).Distinct ();
        }
        
        
        

        /// <summary>
        /// Returns whether the omega automaton is deterministic.
        /// </summary>
        /// <returns><c>True</c> if deterministic, <c>False</c> otherwise.</returns>
        public static bool IsDeterministic<T> (this Automata<T, LiteralSetDecoration> automata, T initialNode)
            where T : IAutomatonNode
        {
            var pending = new Stack<T> (new [] { initialNode });
            var visited = new HashSet<T> ();

            while (pending.Count > 0) {
                var s0 = pending.Pop ();
                visited.Add (s0);

                var transitions = automata.GetTransitions (s0);
                // TODO Simpler expression MUST exist !
                foreach (var c in transitions.Select (x => x.Item2)) {
                    // Better use POST with a predicate
                    var succ = transitions.Where (t => c.ToLiteralSet ().Entails(t.Item2.ToLiteralSet ())).Select (t => t.Item1);
                    if (succ.Count () > 1)
                        return false;

                    foreach (var s in succ.Except (visited)) {
                        pending.Push (s);
                    }
                }
            }

            return true;
        }
        
        
        
        
        /// <summary>
        /// Returns all the successor nodes of the specified node for the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public static IEnumerable<T> Post<T> (this Automata<T, LiteralSetDecoration> automata, T node, LiteralsSet labels)
            where T : IAutomatonNode
        {
            return automata.Post (node, (l, target) => labels.Entails(l.ToLiteralSet ()));
        }

        /// <summary>
        /// Returns all the successor nodes of the specified nodes for the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public static IEnumerable<T> Post<T> (this Automata<T, LiteralSetDecoration> automata, IEnumerable<T> nodes, LiteralsSet labels)
            where T : IAutomatonNode
        {
            return nodes.SelectMany (node => automata.Post(node, labels));
        }

        /// <summary>
        /// Returns all the successor nodes of the specified node for all the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public static IEnumerable<T> Post<T> (this Automata<T, LiteralSetDecoration> automata, T node, IEnumerable<LiteralsSet> labels)
            where T : IAutomatonNode
        {
            return labels.SelectMany (l => automata.Post(node, l));
        }

        /// <summary>
        /// Returns all the successor nodes of the specified nodes for all the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public static IEnumerable<T> Post<T> (this Automata<T, LiteralSetDecoration> automata, IEnumerable<T> nodes, IEnumerable<LiteralsSet> labels)
            where T : IAutomatonNode
        {
            return nodes.SelectMany (node => automata.Post(node, labels));
        }
    }
}

