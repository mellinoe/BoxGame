using EngineCore.Components;
using EngineCore.Entities;
using EngineCore.Graphics;
using EngineCore.Graphics.Gui;
using EngineCore.Graphics.OpenGL;
using EngineCore.Physics;
using EngineCore.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;

#if USE_SLEEP0 && USE_THREADYIELD
#error USE_SLEEP0 and USE_THREADYIELD are mutually exclusive, define only one.
#endif

namespace EngineCore
{
    public abstract class Game
    {
        private double desiredFrameLength = 1.0 / 100000000;
        private bool running = false;
        private GraphicsSystem _graphicsSystem;

        private readonly ServiceRegistry _serviceRegistry = new ServiceRegistry();
        private readonly ComponentRegistry _componentRegistry = new ComponentRegistry();
        private readonly List<GameObject> _gameObjects = new List<GameObject>();

        private void AddGameObject(GameObject go)
        {
            _gameObjects.Add(go);
            go.ServiceRegistry = _serviceRegistry;
            go.ComponentRegistry = _componentRegistry;
        }

        public GameSystemCollection Systems { get; set; }

        public GraphicsSystem GraphicsSystem => _graphicsSystem;

        public ComponentRegistry ComponentRegistry => _componentRegistry;

        public Game()
        {
            Systems = new GameSystemCollection();
            AddInitialGameSystems();

            GameObject.GameObjectConstructed += AddGameObject;  // Todo: Make this less ridiculous

            previousFrameStartTime = DateTime.UtcNow;
        }

        protected virtual void AddInitialGameSystems()
        {
            AddGameSystem(new EntityUpdateSystem(this));
            AddGameSystem(new BepuPhysicsSystem(this));

            bool useDirectX = true;
            if (useDirectX)
            {
                _graphicsSystem = new SharpDxGraphicsSystem(this);
                AddGameSystem(_graphicsSystem);
                AddGameSystem(((SharpDxGraphicsSystem)_graphicsSystem).InputSystem);
            }
            else
            {
                _graphicsSystem = new OpenGLGraphicsSystem(this);
                AddGameSystem(_graphicsSystem);
                AddGameSystem(((OpenGLGraphicsSystem)_graphicsSystem).InputSystem);
            }

           AddGameSystem(new ImGuiSystem(this));
        }

        protected void AddGameSystem(GameSystem system)
        {
            Systems.Add(system);
            var serviceProviderTypes = system.GetType().GetInterfaces()
                .Where(t => t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() == typeof(IServiceProvider<>));

            foreach (var t in serviceProviderTypes)
            {
                Type serviceType = t.GetGenericArguments()[0];
                var method = t.GetRuntimeMethod(nameof(IServiceProvider<object>.GetService), Array.Empty<Type>());
                var service = method.Invoke(system, null);
                Console.WriteLine($"System {system.GetType().Name} provided a {serviceType.Name} service.");
                _serviceRegistry.RegisterService(serviceType, service);
            }
        }

        public T GetService<T>()
        {
            return (T)_serviceRegistry.GetService(typeof(T));
        }

        public void Start()
        {
            Debug.WriteLine("Starting main game loop.");
            running = true;
            PerformCustomInitialization();
            StartSystems();
            PostSystemsStart();
            RunMainLoop();
            return;
        }

        protected abstract void PerformCustomInitialization();
        protected virtual void PostSystemsStart() { }

        private void StartSystems()
        {
            foreach (var system in Systems)
            {
                system.Start();
            }
        }

        private void StopSystems()
        {
            Console.WriteLine("Stopping systems");
            foreach (var system in Systems)
            {
                system.Stop();
            }
        }

        private void RunMainLoop()
        {
            while (running)
            {
                RunSingleFrame();
            }

            StopSystems();
        }

        private DateTime previousFrameStartTime;
        private void RunSingleFrame()
        {
            DateTime beforeFrameTime = DateTime.UtcNow;
            float elapsedSinceLastFrame = (float)(beforeFrameTime - previousFrameStartTime).TotalSeconds;
            Time.SetDeltaTime(elapsedSinceLastFrame);
            previousFrameStartTime = beforeFrameTime;
            foreach (GameSystem system in Systems)
            {
                system.Update();
            }
            DateTime afterFrameTime = DateTime.UtcNow;
            double elapsed = (afterFrameTime - beforeFrameTime).TotalSeconds;
            double sleepTime = desiredFrameLength - elapsed;
            if (sleepTime > 0.0)
            {
#if USE_THREADYIELD || USE_SLEEP0
                DateTime finishTime = afterFrameTime + TimeSpan.FromSeconds(sleepTime);
                while (DateTime.UtcNow < finishTime)
                {
#if USE_THREADYIELD
                    Thread.Yield();
#elif USE_SLEEP0
                    Thread.Sleep(0);
#endif
                }
#else
                Thread.Sleep((int)(sleepTime * 1000));
#endif
            }
#if MONITOR_SLOWRUNNING
            else
            {
                Console.WriteLine("Running slowly, no sleep time.");
            }
#endif
        }

        internal void Exit()
        {
            running = false;
        }
    }
}
