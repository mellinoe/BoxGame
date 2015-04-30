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

namespace EngineCore
{
    public class GameObject
    {
        // Hold a back-reference to the game until I figure out how to encapsulate things
        Game game;

        MultiValueDictionary<Type, Component> components = new MultiValueDictionary<Type, Component>();

        public GameObject()
        {
            this.Transform = AddComponent<Transform>();

            if (GameObjectConstructed != null)
            {
                GameObjectConstructed(this);
            }
            else
            {
                throw new InvalidOperationException(
                    "GameObjectConstructed callback has not been properly initialized before attempting to construct GameObjects.");
            }
        }

        public static event Action<GameObject> GameObjectConstructed;

        public Transform Transform { get; private set; }

        public T GetComponent<T>() where T : Component
        {
            return (T)components[typeof(T)].First();
        }

        public T AddComponent<T>() where T : Component, new()
        {
            T component = new T();
            this.components.Add(typeof(T), component);
            if (game != null)
            {
                InitializeSingleComponent(this.game, component);
            }
            return component;
        }

        public void AddComponent<T>(T component) where T : Component
        {
            this.components.Add(typeof(T), component);
            if (game != null)
            {
                InitializeSingleComponent(this.game, component);
            }
        }

        public void RemoveComponent<T>() where T : Component
        {
            T component = this.GetComponent<T>();
            if (game != null)
            {
                UninitializeSingleComponent(this.game, component);
            }
            this.components.Remove(typeof(T), component);
        }

        public void InitializeComponents(Game game)
        {
            this.game = game;
            foreach (var componentList in components.Values)
            {
                foreach (var component in componentList)
                {
                    InitializeSingleComponent(game, component);
                }
            }
        }

        private void InitializeSingleComponent(Game game, Component component)
        {
            IEnumerable<GameSystem> systems = null;
            IEnumerable<Type> dependencyTypes = component.GetDependencies();
            if (dependencyTypes != null)
            {
                systems = game.Systems.GetSystemsByTypes(dependencyTypes);
            }
            component.CoreInitialize(this, systems);
        }

        private void UninitializeSingleComponent(Game game, Component component)
        {
            IEnumerable<GameSystem> systems = null;
            IEnumerable<Type> dependencyTypes = component.GetDependencies();
            if (dependencyTypes != null)
            {
                systems = game.Systems.GetSystemsByTypes(dependencyTypes);
            }
            component.Uninitialize(systems);
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
