using System;
using System.Numerics;
using Vector3 = System.Numerics.Vector3;
using Matrix4x4 = System.Numerics.Matrix4x4;
using EngineCore.Components;
using EngineCore.Utility;

namespace EngineCore.Graphics
{
    public class Camera : Component
    {
        private Matrix4x4 _viewMatrix;
        private Matrix4x4 _projectionMatrix;
        private float _fieldOfViewRadians = 1.05f;
        private ProjectionType _projectionType = ProjectionType.Perspective;

        /// <summary>
        /// Gets or sets the field of view angle, in radians.
        /// </summary>
        public float FieldOfViewRadians
        {
            get { return _fieldOfViewRadians; }
            set
            {
                _fieldOfViewRadians = value;
                if (_projectionType == global::ProjectionType.Perspective)
                {
                    RecalculateProjectionMatrix();
                }
            }
        }

        /// <summary>
        /// Sets the camera's projection type.
        /// </summary>
        public ProjectionType ProjectionType
        {
            get { return _projectionType; }
            set
            {
                _projectionType = value;
                RecalculateProjectionMatrix();
            }
        }

        /// <summary>
        /// Gets the current view matrix, based on the orientation of the camera.
        /// </summary>
        /// <returns></returns>
        public Matrix4x4 GetViewMatrix()
        {
            return _viewMatrix;
        }

        /// <summary>
        /// Gets the current projection matrix, based on camera settings.
        /// </summary>
        /// <returns></returns>
        public Matrix4x4 GetProjectionMatrix()
        {
            return _projectionMatrix;
        }

        public Vector3 ScreenToWorldPoint(Vector2 screenPosition, float screenWidth, float screenHeight)
        {
            float x = 2.0f * screenPosition.X / screenWidth - 1;
            float y = -2.0f * screenPosition.Y / screenHeight + 1;
            Console.WriteLine("Screen X: " + x);
            Console.WriteLine("Screen Y: " + x);

            Matrix4x4 inverseViewProjection;
            Matrix4x4.Invert(_projectionMatrix * _viewMatrix, out inverseViewProjection);

            Vector3 worldPoint = new Vector3(x, y, 0);
            return Vector3.Transform(worldPoint, inverseViewProjection);
        }

        protected override void Start()
        {
            Transform.PositionChanged += OnTransformPositionChanged;
            Transform.RotationChanged += OnTransformRotationChanged;
            RecalculateViewMatrix();
            RecalculateProjectionMatrix();
        }


        private void OnFormResized(object sender, EventArgs e)
        {
            RecalculateProjectionMatrix();
        }

        private void RecalculateViewMatrix()
        {
            var lookAt = Transform.Position + Transform.Forward;
            _viewMatrix = Matrix4x4.CreateLookAt(Transform.Position, lookAt, Transform.Up);
        }

        private void RecalculateProjectionMatrix()
        {
            float windowRatio = //(float)renderForm.ClientRectangle.Width / (float)renderForm.ClientRectangle.Height;
                16f / 9f;

            switch (_projectionType)
            {
                case ProjectionType.Perspective:
                    _projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(_fieldOfViewRadians, windowRatio, 0.1f, 1000.0f);
                    break;
                case ProjectionType.Orthographic:
                    _projectionMatrix = MathUtil.CreateOrthographic(10 * windowRatio, 10, .03f, 1000f);
                    break;
            }
        }

        private void OnTransformRotationChanged(Quaternion obj)
        {
            RecalculateViewMatrix();
        }

        private void OnTransformPositionChanged(Vector3 obj)
        {
            RecalculateViewMatrix();
        }
    }
}

public enum ProjectionType
{
    Perspective,
    Orthographic
}