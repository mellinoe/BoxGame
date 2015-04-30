using BEPUphysics.Entities;
using BEPUphysics.Entities.Prefabs;
using EngineCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Physics
{
    public class BoxCollider : Collider<Box>
    {
        private float width, height, length;

        public BoxCollider() : this(1.0f, 1.0f, 1.0f) { }

        public BoxCollider(float width, float height, float length)
        {
            this.width = width;
            this.height = height;
            this.length = length;
        }

        public float Width
        {
            get { return width; }
            set
            {
                width = value;
                SetPhysicsBoxDimensions();
            }
        }

        public float Height
        {
            get { return height; }
            set
            {
                height = value;
                SetPhysicsBoxDimensions();
            }
        }

        public float Length
        {
            get { return length; }
            set
            {
                length = value;
                SetPhysicsBoxDimensions();
            }
        }

        private void SetPhysicsBoxDimensions()
        {
            PhysicsEntity.Length = length * Transform.Scale.Z;
            PhysicsEntity.Width = width * Transform.Scale.X;
            PhysicsEntity.Height = height * Transform.Scale.Y;
        }

        protected override Box InitPhysicsEntity()
        {
            return new Box(this.Transform.Position, width, height, length);
        }

        protected override void OnTransformScaleManuallyChanged(System.Numerics.Vector3 obj)
        {
            SetPhysicsBoxDimensions();
        }
    }
}
