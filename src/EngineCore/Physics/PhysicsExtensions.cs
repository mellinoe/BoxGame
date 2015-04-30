using BEPUphysics.BroadPhaseEntries;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Physics
{
    public static class PhysicsExtensions
    {
        public static GameObject GetGameObject(this BroadPhaseEntry bpe)
        {
            Debug.Assert(bpe.Tag is GameObject, "BroadPhaseEntry.Tag was not set to a valid GameObject.");
            return (GameObject)bpe.Tag;
        }
    }
}
