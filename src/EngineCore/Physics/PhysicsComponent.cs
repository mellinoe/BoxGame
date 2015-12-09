using BEPUphysics;
using EngineCore.Components;

namespace EngineCore.Physics
{
    public abstract class PhysicsComponent : Component
    {
        internal abstract ISpaceObject GetSpaceObject();
    }
}
