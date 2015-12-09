using BEPUphysics;
using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using EngineCore.Entities;
using EngineCore.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;

namespace EngineCore.Physics
{
    public class BepuPhysicsSystem : GameSystem, IServiceProvider<BepuPhysicsSystem>
    {
        private Space _space;
        private BEPUutilities.Threading.ParallelLooper _looper;

        public BepuPhysicsSystem(Game game) : base(game)
        {
            _looper = new BEPUutilities.Threading.ParallelLooper();
            for (int g = 0; g < Environment.ProcessorCount - 1; g++)
            {
                _looper.AddThread();
            }
            _space = new Space(_looper);
        }

        public override void Update()
        {
            _space.Update(Time.DeltaTime);
        }

        public void AddOject(ISpaceObject entity, GameObject gameObject)
        {
            entity.Tag = gameObject;
            _space.Add(entity);
        }

        public void RemoveObject(ISpaceObject entity)
        {
            _space.Remove(entity);
        }

        public override void Start()
        {
            _space.ForceUpdater.Gravity = new Vector3(0f, -9.81f, 0f);
        }

        public override void Stop()
        {
        }

        public bool RayCast(BEPUutilities.Ray ray, out RayCastResult result)
        {
            return _space.RayCast(ray, out result);
        }

        BepuPhysicsSystem IServiceProvider<BepuPhysicsSystem>.GetService() => this;

        public Vector3 Gravity
        {
            get
            {
                return _space.ForceUpdater.Gravity;
            }
            set
            {
                _space.ForceUpdater.Gravity = value;
            }
        }
    }
}
