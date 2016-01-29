using BEPUphysics.BroadPhaseEntries;

namespace EngineCore.Physics
{
    public static class PhysicsExtensions
    {
        public static bool TryGetGameObject(this BroadPhaseEntry bpe, out GameObject go)
        {
            if (bpe.Tag is PhysicsComponent)
            {
                go = ((PhysicsComponent)bpe.Tag).GameObject;
                return true;
            }

            go = null;
            return false;
        }
    }
}
