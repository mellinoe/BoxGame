using BEPUphysics;
using BEPUphysics.Entities;
using EngineCore.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using EngineCore;
using BEPUphysics.Entities.Prefabs;
using EngineCore.Entities;
using EngineCore.Components;
using EngineCore.Physics;
using EngineCore.Services;

namespace EngineCore
{
    public class GameObject
    {
        internal ServiceRegistry ServiceRegistry { get; set; }

        MultiValueDictionary<Type, Component> _components = new MultiValueDictionary<Type, Component>();

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

            this.Transform = AddComponent<Transform>();
        }

        public static event Action<GameObject> GameObjectConstructed;

        public Transform Transform { get; }

        public T GetComponent<T>() where T : Component
        {
            return (T)_components[typeof(T)].First();
        }

        public T AddComponent<T>() where T : Component, new()
        {
            T component = new T();
            _components.Add(typeof(T), component);
            ServiceRegistry.InjectServices(component);

            return component;
        }

        public T AddComponent<T>(T component) where T : Component
        {
            _components.Add(typeof(T), component);
            ServiceRegistry.InjectServices(component);

            return component;
        }

        public void RemoveComponent<T>() where T : Component
        {
            T component = GetComponent<T>();
            _components.Remove(typeof(T), component);
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
