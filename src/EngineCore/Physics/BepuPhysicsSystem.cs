using BEPUphysics;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using EngineCore.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;

namespace EngineCore.Physics
{
    public class BepuPhysicsSystem : GameSystem
    {
        private Space space;
        private BEPUutilities.Threading.ParallelLooper looper;
        public BepuPhysicsSystem(Game game)
            : base(game)
        {
            this.looper = new BEPUutilities.Threading.ParallelLooper();
            for (int g = 0; g < Environment.ProcessorCount - 1; g++)
            {
                this.looper.AddThread();
            }
            this.space = new Space(this.looper);
        }

        public override void Update()
        {
            space.Update(Time.DeltaTime);
        }

        public void AddOject(ISpaceObject entity, GameObject gameObject)
        {
            entity.Tag = gameObject;
            space.Add(entity);
        }

        public void RemoveObject(ISpaceObject entity)
        {
            space.Remove(entity);
        }

        public override void Start()
        {
            space.ForceUpdater.Gravity = new Vector3(0f, -9.81f, 0f);
        }

        public override void Stop()
        {
        }

        public bool RayCast(BEPUutilities.Ray ray, out RayCastResult result)
        {
            return space.RayCast(ray, out result);
        }
    }
}
