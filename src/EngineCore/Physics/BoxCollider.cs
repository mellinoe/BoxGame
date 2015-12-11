using BEPUphysics.Entities.Prefabs;
using EngineCore.Components;

namespace EngineCore.Physics
{
    public class BoxCollider : Collider<Box>
    {
        private float _width, _height, _length;

        public BoxCollider() : this(1.0f, 1.0f, 1.0f) { }

        public BoxCollider(float width, float height, float length)
        {
            _width = width;
            _height = height;
            _length = length;
        }

        public float Width
        {
            get { return _width; }
            set
            {
                _width = value;
                SetPhysicsBoxDimensions();
            }
        }

        public float Height
        {
            get { return _height; }
            set
            {
                _height = value;
                SetPhysicsBoxDimensions();
            }
        }

        public float Length
        {
            get { return _length; }
            set
            {
                _length = value;
                SetPhysicsBoxDimensions();
            }
        }

        private void SetPhysicsBoxDimensions()
        {
            PhysicsEntity.Length = _length * Transform.Scale.Z;
            PhysicsEntity.Width = _width * Transform.Scale.X;
            PhysicsEntity.Height = _height * Transform.Scale.Y;
        }

        protected override Box InitPhysicsEntity()
        {
            return new Box(Transform.Position, _width, _height, _length);
        }

        protected override void OnTransformScaleManuallyChanged(System.Numerics.Vector3 obj)
        {
            SetPhysicsBoxDimensions();
        }
    }
}
