using EngineCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace EngineCore.Components
{
    public abstract class Component
    {
        /// <summary>
        /// Retrieves the GameObject holding this Component.
        /// </summary>
        public GameObject GameObject { get; private set; }

        /// <summary>
        /// Retrieves the Transform component from the GameObject this Component is attached to.
        /// A Transform is always present on GameObjects.
        /// </summary>
        public Transform Transform { get; private set; }

        /// <summary>
        /// Obtains the system types which this Component is dependent on.
        /// </summary>
        /// <returns></returns>
        internal virtual IEnumerable<Type> GetDependencies() { return Array.Empty<Type>(); }

        internal void CoreInitialize(GameObject gameObject, IEnumerable<GameSystem> systems)
        {
            this.GameObject = gameObject;
            this.Transform = gameObject.Transform;

            this.Initialize(systems);
        }

        protected internal abstract void Initialize(IEnumerable<GameSystem> systems);
        protected internal abstract void Uninitialize(IEnumerable<GameSystem> systems);
    }

    /// <summary>
    /// An abstract Component type declaring one GameSystem dependency.
    /// </summary>
    /// <typeparam name="TSystem">The type of dependency.</typeparam>
    public abstract class Component<TSystem> : Component where TSystem : GameSystem
    {
        private static readonly HashSet<Type> s_dependencyTypes = new HashSet<Type>() { typeof(TSystem) };

        internal override IEnumerable<Type> GetDependencies() { return s_dependencyTypes; }

        protected internal override sealed void Initialize(IEnumerable<GameSystem> systems)
        {
            this.Initialize((TSystem)systems.First());
        }

        protected internal override sealed void Uninitialize(IEnumerable<GameSystem> systems)
        {
            this.Uninitialize((TSystem)systems.First());
        }

        protected abstract void Initialize(TSystem system);
        protected abstract void Uninitialize(TSystem system);
    }

    public abstract class Component<TSystem1, TSystem2> : Component
        where TSystem1 : GameSystem
        where TSystem2 : GameSystem
    {
        private static readonly HashSet<Type> s_dependencyTypes = new HashSet<Type>() { typeof(TSystem1), typeof(TSystem2) };

        internal override IEnumerable<Type> GetDependencies()
        {
            return s_dependencyTypes;
        }

        protected internal override void Initialize(IEnumerable<GameSystem> systems)
        {
            TSystem1 system1 = (TSystem1)systems.Single(gs => typeof(TSystem1).GetTypeInfo().IsAssignableFrom(gs.GetType().GetTypeInfo()));
            TSystem2 system2 = (TSystem2)systems.Single(gs => typeof(TSystem2).GetTypeInfo().IsAssignableFrom(gs.GetType().GetTypeInfo()));

            this.Initialize(system1, system2);
        }

        protected internal override void Uninitialize(IEnumerable<GameSystem> systems)
        {
            TSystem1 system1 = (TSystem1)systems.Single(gs => typeof(TSystem1).GetTypeInfo().IsAssignableFrom(gs.GetType().GetTypeInfo()));
            TSystem2 system2 = (TSystem2)systems.Single(gs => typeof(TSystem2).GetTypeInfo().IsAssignableFrom(gs.GetType().GetTypeInfo()));

            this.Uninitialize(system1, system2);
        }

        protected abstract void Initialize(TSystem1 system1, TSystem2 system2);
        protected abstract void Uninitialize(TSystem1 system1, TSystem2 system2);
    }

    public abstract class Component<TSystem1, TSystem2, TSystem3> : Component
        where TSystem1 : GameSystem
        where TSystem2 : GameSystem
        where TSystem3 : GameSystem
    {
        private static readonly HashSet<Type> s_dependencyTypes = new HashSet<Type>() { typeof(TSystem1), typeof(TSystem2), typeof(TSystem3) };

        internal override IEnumerable<Type> GetDependencies()
        {
            return s_dependencyTypes;
        }

        protected internal override void Initialize(IEnumerable<GameSystem> systems)
        {
            TSystem1 system1 = (TSystem1)systems.Single(gs => typeof(TSystem1).GetTypeInfo().IsAssignableFrom(gs.GetType().GetTypeInfo()));
            TSystem2 system2 = (TSystem2)systems.Single(gs => typeof(TSystem2).GetTypeInfo().IsAssignableFrom(gs.GetType().GetTypeInfo()));
            TSystem3 system3 = (TSystem3)systems.Single(gs => typeof(TSystem3).GetTypeInfo().IsAssignableFrom(gs.GetType().GetTypeInfo()));

            this.Initialize(system1, system2, system3);
        }

        protected internal override void Uninitialize(IEnumerable<GameSystem> systems)
        {
            TSystem1 system1 = (TSystem1)systems.Single(gs => typeof(TSystem1).GetTypeInfo().IsAssignableFrom(gs.GetType().GetTypeInfo()));
            TSystem2 system2 = (TSystem2)systems.Single(gs => typeof(TSystem2).GetTypeInfo().IsAssignableFrom(gs.GetType().GetTypeInfo()));
            TSystem3 system3 = (TSystem3)systems.Single(gs => typeof(TSystem3).GetTypeInfo().IsAssignableFrom(gs.GetType().GetTypeInfo()));

            this.Uninitialize(system1, system2, system3);
        }

        protected abstract void Initialize(TSystem1 system1, TSystem2 system2, TSystem3 system3);
        protected abstract void Uninitialize(TSystem1 system1, TSystem2 system2, TSystem3 system3);
    }
}
