using EngineCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EngineCore.Components
{
    internal interface IComponentStore { }

    internal class ComponentStore<TComponent> : IComponentStore
    {
        // TODO: Come up with real numbers.
        private const double MinimumCompressionEfficiency = 0.65;
        private const double CompressionTargetEfficiency = 0.85;
        private const int InitialStorageSize = 10;

        private TComponent[] _components = new TComponent[InitialStorageSize];
        private readonly Dictionary<EntityID, int> _entityComponentMap = new Dictionary<EntityID, int>();

        private int _currentOpenSlot = 0;
        private int _emptyInnerSlots = 0;

        public TComponent GetComponent(EntityID entity)
        {
            int index;
            if (!_entityComponentMap.TryGetValue(entity, out index))
            {
                throw new InvalidOperationException(
                    $"Entity {entity} doesn't have a component of type {typeof(TComponent).Name} attached.");
            }

            return _components[index];
        }

        public void AddComponent(EntityID entity, TComponent component)
        {
            ExpandIfNecessary();
            _components[_currentOpenSlot] = component;
            _entityComponentMap.Add(entity, _currentOpenSlot);
            _currentOpenSlot += 1;
        }

        public void AddComponent(EntityID entity, ComponentInitializationFunc<TComponent> initFunc)
        {
            if (_entityComponentMap.ContainsKey(entity))
            {
                throw new InvalidOperationException($"Can't add duplicate components of type {typeof(TComponent).Name} on entity {entity}.");
            }

            ExpandIfNecessary();

            TComponent component = Activator.CreateInstance<TComponent>();
            _components[_currentOpenSlot] = (TComponent)(object)component;
            _entityComponentMap.Add(entity, _currentOpenSlot);
            initFunc(ref _components[_currentOpenSlot]);
            _currentOpenSlot += 1;
        }

        internal IndexedComponentView<TComponent> GetIndexedView(List<EntityID> entities)
        {
            // TODO: Pool this.
            int[] indices = entities.Select(id => _entityComponentMap[id]).ToArray();
            return new IndexedComponentView<TComponent>(_components, indices);
        }

        public void RemoveComponent(EntityID entity, TComponent component)
        {
            if (!_entityComponentMap.Remove(entity))
            {
                throw new InvalidOperationException($"Can't remove component on entity {entity}, doesn't exist.");
            }

            _emptyInnerSlots += 1; // Technically we might have removed the last component in the array, but I don't check that.
            CompressIfNecessary();
        }

        public void ExecutePerEntityOperation() { }

        private void ExpandIfNecessary()
        {
            if (_currentOpenSlot == _components.Length)
            {
                Array.Resize(ref _components, _components.Length * 2);
            }
        }

        private void CompressIfNecessary()
        {
            int count = _entityComponentMap.Count;
            if (count > InitialStorageSize
                && (double)_emptyInnerSlots / _components.Length < MinimumCompressionEfficiency)
            {
                int usedSlots = _components.Length - _emptyInnerSlots;
                int newCapacity = Math.Max(
                    InitialStorageSize, // Only consolidate down to initial size.
                    (int)(usedSlots * (1 / CompressionTargetEfficiency)));

                TComponent[] newStore = new TComponent[newCapacity];

                int currentIndex = 0;
                foreach (var kvp in _entityComponentMap)
                {
                    int oldIndex = kvp.Value;
                    newStore[currentIndex] = _components[oldIndex];
                    _entityComponentMap[kvp.Key] = currentIndex;
                    currentIndex += 1;
                }

                _components = newStore;
            }
        }
    }
}
