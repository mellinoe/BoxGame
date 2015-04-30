using BEPUphysics.Entities.Prefabs;
using EngineCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Physics
{
    public class SphereCollider : Collider<Sphere>
    {
        private float radius;

        public SphereCollider() : this(1.0f) { }

        public SphereCollider(float radius)
        {
            this.radius = radius;
        }

        public float Radius
        {
            get { return radius; }
            set
            {
                radius = value;
                SetPhysicsSphereSize();
            }
        }
        protected override void Initialize(BepuPhysicsSystem system)
        {
            base.Initialize(system);
            this.Transform.ScaleChanged += OnScaleChanged;
        }

        private void OnScaleChanged(Vector3 obj)
        {
            this.Radius = 1.66f * obj.X;
        }

        private void SetPhysicsSphereSize()
        {
            PhysicsEntity.Radius = this.radius;
        }

        protected override Sphere InitPhysicsEntity()
        {
            return new Sphere(this.Transform.Position, this.radius * Transform.Scale.X, 1.0f);
        }
    }
}
