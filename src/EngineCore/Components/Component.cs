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
            GameObject = gameObject;
            Transform = gameObject.Transform;
        }

        protected virtual void Start()
        {
        }
    }
}
