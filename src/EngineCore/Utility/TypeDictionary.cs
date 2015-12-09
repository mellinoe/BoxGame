using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EngineCore.Utility
{
    internal class TypeDictionary<T>
    {
        private readonly Dictionary<Type, T> _dict;

        public TypeDictionary() : this(10) { }

        public TypeDictionary(int capacity)
        {
            _dict = new Dictionary<Type, T>(capacity);
        }

        public void Add(Type key, T value)
        {

        }
    }
}
