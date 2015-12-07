using System.Collections.Generic;

namespace EngineCore.Components
{
    public struct IndexedComponentView<T>
    {
        private readonly T[] _components;
        private readonly IReadOnlyList<int> _indices;

        public T[] Components => _components;
        public IReadOnlyList<int> Indices => _indices;

        public IndexedComponentView(T[] components, IReadOnlyList<int> indices)
        {
            _components = components;
            _indices = indices;
        }
    }
}
