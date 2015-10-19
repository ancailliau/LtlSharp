using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Automata.Transitions;
using LtlSharp.Automata.Transitions.Decorations;
using LtlSharp.Language;

namespace LtlSharp.Automata.Utils
{
    public static class LiteralSetTransitionExtensions
    {
        /// <summary>
        /// Simplifies the transitions of the automaton.
        /// </summary>
        /// <param name="automaton">Automaton.</param>
        /// <typeparam name="T">Type of the nodes of the automaton.</typeparam>
        public static void SimplifyTransitions<T> (this Automaton<T, LiteralSetDecoration> automaton)
            where T : IAutomatonNode
        {
            foreach (var trans in automaton.Edges) {
                var sameTarget = automaton.Edges.Where (t => t.Target.Equals (trans.Target)).ToList ();
                var labels = sameTarget.Select (x => x.Decoration.LiteralSet);
                    var lf = new LiteralFormula (labels);
                    var newLabels = lf.Simplify ();
                    foreach (var e in sameTarget) {
                    automaton.RemoveTransition (e.Source, e.Target, e.Decoration);
                    }
                    foreach (var nl in newLabels) {
                    automaton.AddTransition (trans.Source, trans.Target, new LiteralSetDecoration (nl));
                    }
                }
        }

        /// <summary>
        /// Unfold the transitions.
        /// </summary>
        /// <remarks>
        /// Some transitions might only contains a subset of literals of the alphabet of the automaton. With some
        /// algorithms, this might be a problem. This method replace the transition decorations with new decorations
        /// (and transitions if needed) to explicitly refer to all literal of the alphabet.
        /// 
        /// For example, consider a transition with {b} but the alphabet of the automaton is {a,b}. The transition
        /// will be duplicated in {a,b} and {!a,b}.
        /// </remarks>
        /// <param name="automaton">Automaton.</param>
        /// <typeparam name="T">Type of the nodes of the automaton.</typeparam>
        public static void UnfoldTransitions<T> (this Automaton<T, LiteralSetDecoration> automaton)
            where T : IAutomatonNode
        {
            var alphabet = automaton.Alphabet ().ToList ();
            automaton.MapLabel ((arg) => arg.LiteralSet.Expand (alphabet).Select (x => new LiteralSetDecoration (x)));
        }

        /// <summary>
        /// Returns the set of literals that correspond to (at least) a transition.
        /// </summary>
        /// <param name="node">Node.</param>
        public static IEnumerable<ILiteral> Alphabet<T> (this Automaton<T, LiteralSetDecoration> automaton)
            where T : IAutomatonNode
        {
            return automaton.Edges.SelectMany (e => e.Decoration.LiteralSet.GetAlphabet ()).Distinct ();
        }
        
        /// <summary>
        /// Returns whether the omega automaton is deterministic.
        /// </summary>
        /// <returns><c>True</c> if deterministic, <c>False</c> otherwise.</returns>
        public static bool IsDeterministic<T> (this Automaton<T, LiteralSetDecoration> automaton, T initialNode)
            where T : IAutomatonNode
        {
            var pending = new Stack<T> (new [] { initialNode });
            var visited = new HashSet<T> ();

            while (pending.Count > 0) {
                var s0 = pending.Pop ();
                visited.Add (s0);
                foreach (var c in automaton.GetOutDecorations (s0)) {
                    var succs = automaton.Post (s0, c);
                    if (succs.Count () > 1)
                        return false;
                    
                    foreach (var s in succs.Except (visited))
                        pending.Push (s);
                }
            }

            return true;
        }
        
        /// <summary>
        /// Returns all the successor nodes of the specified node for the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public static IEnumerable<T> Post<T> (this Automaton<T, LiteralSetDecoration> automaton,
                                              T node, 
                                              LiteralSet labels)
            where T : IAutomatonNode
        {
            return automaton.Post (node, (l, target) => labels.Entails(l.LiteralSet));
        }

        /// <summary>
        /// Returns all the successor nodes of the specified nodes for the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public static IEnumerable<T> Post<T> (this Automaton<T, LiteralSetDecoration> automaton, 
                                              IEnumerable<T> nodes, 
                                              LiteralSet labels)
            where T : IAutomatonNode
        {
            return nodes.SelectMany (node => automaton.Post(node, labels));
        }

        /// <summary>
        /// Returns all the successor nodes of the specified node for all the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public static IEnumerable<T> Post<T> (this Automaton<T, LiteralSetDecoration> automaton, 
                                              T node, IEnumerable<LiteralSet> labels)
            where T : IAutomatonNode
        {
            return labels.SelectMany (l => automaton.Post(node, l));
        }

        /// <summary>
        /// Returns all the successor nodes of the specified nodes for all the specified transition label.
        /// </summary>
        /// <param name="node">Node.</param>
        public static IEnumerable<T> Post<T> (this Automaton<T, LiteralSetDecoration> automaton, 
                                              IEnumerable<T> nodes, 
                                              IEnumerable<LiteralSet> labels)
            where T : IAutomatonNode
        {
            return nodes.SelectMany (node => automaton.Post(node, labels));
        }
    }
}

