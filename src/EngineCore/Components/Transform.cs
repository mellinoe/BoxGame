using EngineCore.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Components
{
    public class Transform : Component
    {
        public Transform() : base() { }

        private Vector3 position;
        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                position = value;
                OnPositionChanged();
            }
        }

        internal event Action<Vector3> PositionChanged;
        private void OnPositionChanged()
        {
            if (PositionChanged != null)
            {
                this.PositionChanged(this.position);
            }
        }

        private Quaternion rotation = Quaternion.Identity;
        public Quaternion Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                OnRotationChanged();
            }
        }

        internal event Action<Quaternion> RotationChanged;
        private void OnRotationChanged()
        {
            if (RotationChanged != null)
            {
                this.RotationChanged(this.rotation);
            }
        }

        private Vector3 scale = Vector3.One;
        public Vector3 Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                this.OnScalechanged();
            }
        }

        internal event Action<Vector3> ScaleChanged;
        private void OnScalechanged()
        {
            if (ScaleChanged != null)
            {
                this.ScaleChanged(this.scale);
            }
        }

        public Matrix4x4 WorldMatrix
        {
            get
            {
                return Matrix4x4.CreateScale(scale)
                    * Matrix4x4.CreateFromQuaternion(rotation)
                    * Matrix4x4.CreateTranslation(position);
            }
        }

        public Vector3 Forward
        {
            get
            {
                return Vector3.Transform(Vector3.UnitZ, this.rotation);
            }
        }

        public Vector3 Up
        {
            get
            {
                return Vector3.Transform(Vector3.UnitY, this.rotation);
            }
        }

        public Vector3 Right
        {
            get
            {
                return Vector3.Transform(Vector3.UnitX, this.rotation);
            }
        }

        internal void OnPhysicsUpdate(BEPUphysics.Entities.Entity obj)
        {
            this.position = obj.Position;
            this.rotation = obj.Orientation;
        }

        protected internal override void Initialize(IEnumerable<GameSystem> systems) { }
        protected internal override void Uninitialize(IEnumerable<GameSystem> systems) { }
    }
}
