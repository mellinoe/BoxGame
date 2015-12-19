using BEPUphysics.Entities.Prefabs;
using EngineCore.Components;
using System;
using System.Numerics;

namespace EngineCore.Physics
{
    public class SphereCollider : Collider<Sphere>
    {
        private float _unscaledRadius;
        private float _radius;
        private float _massPerVolume;

        public SphereCollider() : this(1.0f, 1.0f) { }

        public SphereCollider(float radius, float massPerVolume)
        {
            _unscaledRadius = radius;
            _massPerVolume = massPerVolume;
        }

        public float Radius
        {
            get { return _radius; }
            set
            {
                _radius = value;
                SetPhysicsSphereSize();
            }
        }

        protected override void OnTransformScaleManuallyChanged(Vector3 newScale)
        {
            base.OnTransformScaleManuallyChanged(newScale);
            if (newScale.X != newScale.Y || newScale.X != newScale.Z || newScale.Y != newScale.Z)
            {
                Console.WriteLine("Warning: sphere collider has non-uniform scale. This will not behave properly.");
            }

            Radius = _unscaledRadius * newScale.X;
        }

        private void SetPhysicsSphereSize()
        {
            PhysicsEntity.Radius = CalculatedScaledRadius();
            PhysicsEntity.Mass = CalculateScaledMass();
        }

        private float CalculatedScaledRadius() => _unscaledRadius * Transform.Scale.X;

        protected override Sphere InitPhysicsEntity()
        {
            float scaledRadius = CalculatedScaledRadius();
            return new Sphere(Transform.Position, CalculatedScaledRadius(), CalculateScaledMass());
        }

        private float CalculateScaledMass()
        {
            float radius = CalculatedScaledRadius();
            return _massPerVolume * (float)Math.PI * (radius * radius);
        }
    }
}
