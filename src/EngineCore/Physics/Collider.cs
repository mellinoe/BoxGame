using BEPUphysics.Entities;
using EngineCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Physics
{
    public abstract class Collider<T> : Component<BepuPhysicsSystem> where T : Entity
    {
        private BepuPhysicsSystem system;
        private T physicsEntity;

        public T PhysicsEntity
        {
            get { return physicsEntity; }
            set
            {
                var old = physicsEntity;
                physicsEntity = value;
                if (system != null)
                {
                    if (old != null)
                    {
                        system.RemoveObject(old);
                    }
                    system.AddOject(value, this.GameObject);
                }

                value.PositionUpdated += Transform.OnPhysicsUpdate;
            }
        }

        public float Mass
        {
            get { return physicsEntity.Mass; }
            set { physicsEntity.Mass = value; }
        }

        protected override void Initialize(BepuPhysicsSystem system)
        {

            this.Transform.PositionChanged += OnTransformPositionManuallyChanged;
            this.Transform.RotationChanged += OnTransformRotationManuallyChanged;
            this.Transform.ScaleChanged += OnTransformScaleManuallyChanged;

            this.system = system;
            this.PhysicsEntity = InitPhysicsEntity();

            OnTransformPositionManuallyChanged(this.Transform.Position);
            OnTransformRotationManuallyChanged(this.Transform.Rotation);
        }

        protected abstract T InitPhysicsEntity();

        protected override void Uninitialize(BepuPhysicsSystem system)
        {
            this.Transform.PositionChanged -= OnTransformPositionManuallyChanged;
            this.Transform.RotationChanged -= OnTransformRotationManuallyChanged;

            if (this.physicsEntity != null)
            {
                system.RemoveObject(this.physicsEntity);
            }
        }

        protected virtual void OnTransformPositionManuallyChanged(System.Numerics.Vector3 position)
        {
            this.physicsEntity.Position = position;
        }

        protected virtual void OnTransformRotationManuallyChanged(System.Numerics.Quaternion rotation)
        {
            this.physicsEntity.Orientation = rotation;
        }

        protected virtual void OnTransformScaleManuallyChanged(System.Numerics.Vector3 obj)
        {
        }
    }
}
