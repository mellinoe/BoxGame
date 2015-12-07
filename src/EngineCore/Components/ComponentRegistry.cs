using EngineCore.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EngineCore.Components
{
    public delegate void ComponentInitializationFunc<T>(ref T component);

    public class ComponentRegistry
    {
        private const int MaxComponentTypes = 64;

        private static Dictionary<Type, int> s_componentTypeIDs = new Dictionary<Type, int>();
        private static Type[] s_componentTypes = new Type[MaxComponentTypes];
        private static int s_lastAssignedID;

        private readonly Dictionary<Type, IComponentStore> _stores = new Dictionary<Type, IComponentStore>();
        private readonly Dictionary<EntityID, ComponentMask> _entityComponentMasks = new Dictionary<EntityID, ComponentMask>();
        private readonly List<ComponentListenerRegistration> _registrations = new List<ComponentListenerRegistration>();

        public void AddComponent<T>(EntityID entity, T component)
        {
            ComponentStore<T> store = GetStore<T>();
            store.AddComponent(entity, component);
            AddComponentMask<T>(entity);
        }

        public void AddComponent<T>(EntityID entity, ComponentInitializationFunc<T> initFunc) where T : new()
        {
            ComponentStore<T> store = GetStore<T>();
            store.AddComponent(entity, initFunc);

            AddComponentMask<T>(entity);
        }

        internal List<EntityID> GetEntitiesWithComponents(ComponentMask mask)
        {
            List<EntityID> ids = new List<EntityID>();
            foreach (var kvp in _entityComponentMasks)
            {
                if ((kvp.Value & mask) == mask)
                {
                    ids.Add(kvp.Key);
                }
            }

            return ids;
        }

        public IndexedComponentView<T> GetIndexedComponentView<T>(List<EntityID> entities)
        {
            ComponentStore<T> store = GetStore<T>();
            return store.GetIndexedView(entities);
        }

        public void AddComponentRegistration<T>(Action<T> addedAction, Action<T> removedAction)
        {
            ComponentMask mask = GetComponentMask(typeof(T));
            var registration = new ComponentListenerRegistration(
                mask,
                (o) => addedAction((T)o),
                (o) => removedAction((T)o));
            _registrations.Add(registration);
        }

        public static int GetComponentID(Type type)
        {
            int id;
            if (!s_componentTypeIDs.TryGetValue(type, out id))
            {
                id = AssignNextID(type);
                s_componentTypeIDs.Add(type, id);
                s_componentTypes[id] = type;
            }

            return id;
        }

        internal static ComponentMask GetComponentMask(Type type)
        {
            int id = GetComponentID(type);
            Debug.Assert(id > 0 && id < MaxComponentTypes);
            return ComponentMask.GetForID(id);
        }

        internal static IEnumerable<Type> GetTypesFromMask(ComponentMask mask)
        {
            for (int i = 0; i < s_lastAssignedID; i++)
            {
                ComponentMask maskI = ComponentMask.GetForID(i);
                if ((mask & maskI) == maskI)
                {
                    yield return s_componentTypes[i];
                }
            }
        }

        private static int AssignNextID(Type type)
        {
            return s_lastAssignedID++;
        }

        private ComponentStore<T> GetStore<T>()
        {
            IComponentStore store;
            if (!_stores.TryGetValue(typeof(T), out store))
            {
                store = new ComponentStore<T>();
            }

            return (ComponentStore<T>)store;
        }

        private void AddComponentMask<T>(EntityID entity)
        {
            ComponentMask mask = ComponentMask.None;
            _entityComponentMasks.TryGetValue(entity, out mask);
            _entityComponentMasks[entity] = mask & GetComponentMask(typeof(T));
        }

        private void RemoveComponentMask<T>(EntityID entity)
        {
            ComponentMask mask = ComponentMask.None;
            _entityComponentMasks.TryGetValue(entity, out mask);
            _entityComponentMasks[entity] = mask ^ GetComponentMask(typeof(T));
        }
    }
}
