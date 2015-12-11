using System;
using System.Numerics;

namespace EngineCore.Components
{
    public class Transform : Component
    {
        public Transform() : base() { }

        private Vector3 _position;
        public Vector3 Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;
                OnPositionChanged();
            }
        }

        internal event Action<Vector3> PositionChanged;
        private void OnPositionChanged()
        {
            if (PositionChanged != null)
            {
                PositionChanged(_position);
            }
        }

        private Quaternion _rotation = Quaternion.Identity;
        public Quaternion Rotation
        {
            get { return _rotation; }
            set
            {
                _rotation = value;
                OnRotationChanged();
            }
        }

        internal event Action<Quaternion> RotationChanged;
        private void OnRotationChanged()
        {
            if (RotationChanged != null)
            {
                RotationChanged(_rotation);
            }
        }

        private Vector3 _scale = Vector3.One;
        public Vector3 Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                OnScalechanged();
            }
        }

        internal event Action<Vector3> ScaleChanged;
        private void OnScalechanged()
        {
            if (ScaleChanged != null)
            {
                ScaleChanged(_scale);
            }
        }

        public Matrix4x4 WorldMatrix
        {
            get
            {
                return Matrix4x4.CreateScale(_scale)
                    * Matrix4x4.CreateFromQuaternion(_rotation)
                    * Matrix4x4.CreateTranslation(_position);
            }
        }

        public Vector3 Forward
        {
            get
            {
                return Vector3.Transform(Vector3.UnitZ, _rotation);
            }
        }

        public Vector3 Up
        {
            get
            {
                return Vector3.Transform(Vector3.UnitY, _rotation);
            }
        }

        public Vector3 Right
        {
            get
            {
                return Vector3.Transform(Vector3.UnitX, _rotation);
            }
        }

        internal void OnPhysicsUpdate(BEPUphysics.Entities.Entity obj)
        {
            _position = obj.Position;
            _rotation = obj.Orientation;
        }
    }
}
