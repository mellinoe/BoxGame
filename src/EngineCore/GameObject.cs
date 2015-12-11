using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using EngineCore.Entities;
using EngineCore.Components;
using EngineCore.Physics;
using EngineCore.Services;

namespace EngineCore
{
    public class GameObject
    {
        internal ServiceRegistry ServiceRegistry { get; set; }

        internal ComponentRegistry ComponentRegistry { get; set; }

        private readonly MultiValueDictionary<Type, Component> _components = new MultiValueDictionary<Type, Component>();

        public GameObject()
        {
            if (GameObjectConstructed != null)
            {
                GameObjectConstructed(this);
            }
            else
            {
                throw new InvalidOperationException(
                    "GameObjectConstructed callback has not been properly initialized before attempting to construct GameObjects.");
            }

            Transform = AddComponent<Transform>();
        }

        public static event Action<GameObject> GameObjectConstructed;

        public Transform Transform { get; }

        public T GetComponent<T>() where T : Component
        {
            return (T)_components[typeof(T)].First();
        }

        public T AddComponent<T>() where T : Component, new() => AddComponent(new T());

        public T AddComponent<T>(T component) where T : Component
        {
            component.CoreInitialize(this);
            _components.Add(typeof(T), component);
            ServiceRegistry.InjectServices(component);
            component.Start();
            ComponentRegistry.RegisterComponent(this, component);
            return component;
        }

        public void RemoveComponent<T>() where T : Component
        {
            T component = GetComponent<T>();
            _components.Remove(typeof(T), component);
            ComponentRegistry.RemoveComponent(this, component);
        }

        public static GameObject CreateBox(float width, float height, float depth, float mass = 1.0f)
        {
            GameObject box = new GameObject();
            var boxRenderer = box.AddComponent<BoxRenderer>();
            boxRenderer.Scale = new Vector3(width, height, depth);
            var collider = box.AddComponent<BoxCollider>();
            collider.Width = width;
            collider.Height = height;
            collider.Length = depth;
            collider.Mass = mass;

            return box;
        }

        public static GameObject CreateStaticBox(float width, float height, float depth)
        {
            GameObject box = new GameObject();
            var boxRenderer = box.AddComponent<BoxRenderer>();
            boxRenderer.Scale = new Vector3(width, height, depth);
            var collider = box.AddComponent<BoxCollider>();
            collider.Width = width;
            collider.Height = height;
            collider.Length = depth;
            collider.Mass = -1.0f;

            return box;
        }
    }
}
