using System;

namespace EngineCore.Components
{
    internal class ComponentListenerRegistration
    {
        internal ComponentMask Mask { get; }
        internal Action<object> ComponentAddedAction { get; }
        internal Action<object> ComponentRemovedAction { get; }

        public ComponentListenerRegistration(ComponentMask componentMask, Action<object> componentAddedAction, Action<object> componentRemovedAction)
        {
            Mask = componentMask;
            ComponentAddedAction = componentAddedAction;
            ComponentRemovedAction = componentRemovedAction;
        }
    }
}