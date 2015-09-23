using System;

namespace LtlSharp.Buchi.Automata
{
    public class AutomataNode
    {
        public int Id;
        public string Name;
        public bool Initial;
        
        public AutomataNode (int id, string name, bool initial)
        {
            Id = id;
            Name = name;
            Initial = initial;
        }
        
        public override string ToString ()
        {
            return string.Format ("[BANode: Id={0}, Name=\"{1}\", Initial={2}]", Id, Name, Initial);
        }
    }
}

