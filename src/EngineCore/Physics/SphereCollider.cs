using BEPUphysics.Entities.Prefabs;
using EngineCore.Components;
using System.Numerics;

namespace EngineCore.Physics
{
    public class SphereCollider : Collider<Sphere>
    {
        private float _radius;

        public SphereCollider() : this(1.0f) { }

        public SphereCollider(float radius)
        {
            _radius = radius;
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
        protected override void Start()
        {
            Transform.ScaleChanged += OnScaleChanged;
        }

        private void OnScaleChanged(Vector3 obj)
        {
            Radius = 1.66f * obj.X;
        }

        private void SetPhysicsSphereSize()
        {
            PhysicsEntity.Radius = _radius;
        }

        protected override Sphere InitPhysicsEntity()
        {
            return new Sphere(Transform.Position, _radius * Transform.Scale.X, 1.0f);
        }
    }
}
