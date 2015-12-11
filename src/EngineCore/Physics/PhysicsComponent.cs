using BEPUphysics;
using EngineCore.Components;

namespace EngineCore.Physics
{
    [RegistrationType(typeof(PhysicsComponent))]
    public abstract class PhysicsComponent : Component
    {
        internal abstract ISpaceObject GetSpaceObject();
    }
}
