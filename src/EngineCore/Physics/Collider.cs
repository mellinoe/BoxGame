using BEPUphysics.Entities;
using EngineCore.Components;
using BEPUphysics;
using System.Numerics;

namespace EngineCore.Physics
{
    public abstract class Collider<T> : PhysicsComponent where T : Entity
    {
        private BepuPhysicsSystem system;
        private T _physicsEntity;

        public T PhysicsEntity
        {
            get { return _physicsEntity; }
            set
            {
                var old = _physicsEntity;
                _physicsEntity = value;
                if (system != null)
                {
                    if (old != null)
                    {
                        system.RemoveObject(old);
                    }
                    system.AddOject(value, GameObject);
                }

                value.PositionUpdated += Transform.OnPhysicsUpdate;
            }
        }

        internal override ISpaceObject GetSpaceObject() => _physicsEntity;

        public float Mass
        {
            get { return _physicsEntity.Mass; }
            set { _physicsEntity.Mass = value; }
        }

        public Vector3 LinearVelocity
        {
            get { return _physicsEntity.LinearVelocity; }
            set { _physicsEntity.LinearVelocity = value; }
        }

        public Vector3 PhysicsPosition
        {
            get { return _physicsEntity.Position; }
            set { _physicsEntity.Position = value; }
        }

        protected internal override void Start()
        {
            Transform.PositionChanged += OnTransformPositionManuallyChanged;
            Transform.RotationChanged += OnTransformRotationManuallyChanged;
            Transform.ScaleChanged += OnTransformScaleManuallyChanged;

            PhysicsEntity = InitPhysicsEntity();

            OnTransformPositionManuallyChanged(Transform.Position);
            OnTransformRotationManuallyChanged(Transform.Rotation);
        }

        protected abstract T InitPhysicsEntity();

        protected virtual void OnTransformPositionManuallyChanged(System.Numerics.Vector3 position)
        {
            _physicsEntity.Position = position;
        }

        protected virtual void OnTransformRotationManuallyChanged(System.Numerics.Quaternion rotation)
        {
            _physicsEntity.Orientation = rotation;
        }

        protected virtual void OnTransformScaleManuallyChanged(System.Numerics.Vector3 obj)
        {
        }
    }
}
