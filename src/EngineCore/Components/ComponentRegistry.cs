using System;
using System.Collections.Generic;
using System.Reflection;

namespace EngineCore.Components
{
    public delegate void ComponentInitializationFunc<T>(ref T component);

    public class ComponentRegistry
    {
        private const int MaxComponentTypes = 64;

        private static Dictionary<Type, int> s_componentTypeIDs = new Dictionary<Type, int>();
        private static Dictionary<Type, ComponentMask> s_typeComponentMasks = new Dictionary<Type, ComponentMask>();
        private static Type[] s_componentTypes = new Type[MaxComponentTypes];
        private static int s_lastAssignedID;

        private readonly Dictionary<GameObject, ComponentMask> _entityComponentMasks = new Dictionary<GameObject, ComponentMask>();
        private readonly List<ComponentListenerRegistration> _registrations = new List<ComponentListenerRegistration>();

        public void RegisterComponent<T>(GameObject go, T component)
        {
            AddComponentMask<T>(go);
            ComponentMask maskOfT = GetComponentMask(typeof(T));

            foreach (var listenerRegistration in _registrations)
            {
                if ((listenerRegistration.Mask & maskOfT) == maskOfT)
                {
                    listenerRegistration.ComponentAddedAction(component);
                }
            }
        }

        public void RemoveComponent<T>(GameObject go, T component)
        {
            var mask = RemoveComponentMask<T>(go);

            ComponentMask maskOfT = GetComponentMask(typeof(T));
            foreach (var listenerRegistration in _registrations)
            {
                if ((listenerRegistration.Mask & maskOfT) == maskOfT)
                {
                    listenerRegistration.ComponentRemovedAction(component);
                }
            }
        }

        internal List<GameObject> GetEntitiesWithComponents(ComponentMask mask)
        {
            List<GameObject> gameObjects = new List<GameObject>();
            foreach (var kvp in _entityComponentMasks)
            {
                if ((kvp.Value & mask) == mask)
                {
                    gameObjects.Add(kvp.Key);
                }
            }

            return gameObjects;
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
            ComponentMask mask;
            if (!s_typeComponentMasks.TryGetValue(type, out mask))
            {
                foreach (RegistrationTypeAttribute rta in GetRegistrationTypes(type))
                {
                    int id = GetComponentID(rta.Type);
                    mask |= ComponentMask.GetForID(id);
                }
                if (mask == ComponentMask.None)
                {
                    mask = ComponentMask.GetForID(GetComponentID(type));
                }

                s_typeComponentMasks.Add(type, mask);
            }

            return mask;
        }

        private static IEnumerable<RegistrationTypeAttribute> GetRegistrationTypes(Type type)
        {
            return type.GetTypeInfo().GetCustomAttributes<RegistrationTypeAttribute>(inherit: true);
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
            return ++s_lastAssignedID;
        }

        private ComponentMask AddComponentMask<T>(GameObject go)
        {
            ComponentMask entityMask = ComponentMask.None;
            _entityComponentMasks.TryGetValue(go, out entityMask);
            entityMask |= GetComponentMask(typeof(T));
            _entityComponentMasks[go] = entityMask;
            return entityMask;
        }

        private ComponentMask RemoveComponentMask<T>(GameObject go)
        {
            ComponentMask entityMask = ComponentMask.None;
            _entityComponentMasks.TryGetValue(go, out entityMask);
            _entityComponentMasks[go] = entityMask ^ GetComponentMask(typeof(T));
            return entityMask;
        }
    }

    public class RegistrationTypeAttribute : Attribute
    {
        public Type Type { get; }
        public RegistrationTypeAttribute(Type type)
        {
            Type = type;
        }
    }
}
