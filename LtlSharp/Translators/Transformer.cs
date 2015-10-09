using System;

namespace LtlSharp.Translators
{
    public abstract class Transformer<T1, T2>
    {
        public abstract T2 Transform (T1 t);
    }
}

