using System;
using System.Collections.Generic;
using System.Linq;
using LtlSharp.Automata.Transitions;

namespace LtlSharp.Monitoring
{
    public class MonitoredState
    {
        Dictionary<Proposition, bool> state;
        
        public MonitoredState ()
        {
            state = new Dictionary<Proposition, bool> ();
        }

        internal bool Evaluate (LiteralsSet labels)
        {
            //Console.WriteLine (string.Join (",", labels));
            //Console.WriteLine (string.Join (",", state.Select (kv => kv.Key + "=" + kv.Value)));
            
            foreach (var l in labels) {
                if (l is False) return false;
                if (l is Proposition) {
                    if (!state [(Proposition)l]) return false;
                }
                if (l is Negation) {
                    if (state [((Proposition)((Negation)l).Enclosed)]) return false;
                }
            }
            //Console.WriteLine (">> true");
            return true;
        }

        public void Set (Proposition proposition, bool value)
        {
            if (state.ContainsKey (proposition)) {
                state [proposition] = value;
            } else {
                state.Add (proposition, value);
            }
        }
        
        public override string ToString ()
        {
            return string.Format ("[MonitoredState: state={0}]", string.Join (";", state.Select ((kv) => "(" + kv.Key + "," + kv.Value + ")")));
        }
    }
}

