using System;
using System.Collections.Generic;

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

        internal void CoreInitialize(GameObject gameObject)
        {
            GameObject = gameObject;
            Transform = gameObject.Transform;
        }

        protected internal virtual void Start()
        {
        }
    }
}
