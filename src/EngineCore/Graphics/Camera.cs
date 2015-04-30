using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Vector3 = System.Numerics.Vector3;
using Matrix4x4 = System.Numerics.Matrix4x4;
using EngineCore.Components;
using EngineCore.Utility;
using EngineCore.Graphics.OpenGL;

namespace EngineCore.Graphics
{
    public class Camera : Component<GraphicsSystem>
    {
        private Matrix4x4 viewMatrix;
        private Matrix4x4 projectionMatrix;
        private float fieldOfViewRadians = 1.05f;
        private ProjectionType projectionType = ProjectionType.Perspective;

        /// <summary>
        /// Gets or sets the field of view angle, in radians.
        /// </summary>
        public float FieldOfViewRadians
        {
            get { return fieldOfViewRadians; }
            set
            {
                fieldOfViewRadians = value;
                if (projectionType == global::ProjectionType.Perspective)
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
            get { return projectionType; }
            set
            {
                projectionType = value;
                RecalculateProjectionMatrix();
            }
        }

        /// <summary>
        /// Gets the current view matrix, based on the orientation of the camera.
        /// </summary>
        /// <returns></returns>
        public Matrix4x4 GetViewMatrix()
        {
            return viewMatrix;
        }

        /// <summary>
        /// Gets the current projection matrix, based on camera settings.
        /// </summary>
        /// <returns></returns>
        public Matrix4x4 GetProjectionMatrix()
        {
            return projectionMatrix;
        }

        public Vector3 ScreenToWorldPoint(Vector2 screenPosition, float screenWidth, float screenHeight)
        {
            float x = 2.0f * screenPosition.X / screenWidth - 1;
            float y = -2.0f * screenPosition.Y / screenHeight + 1;
            Console.WriteLine("Screen X: " + x);
            Console.WriteLine("Screen Y: " + x);

            Matrix4x4 inverseViewProjection;
            Matrix4x4.Invert(projectionMatrix * viewMatrix, out inverseViewProjection);

            Vector3 worldPoint = new Vector3(x, y, 0);
            return Vector3.Transform(worldPoint, inverseViewProjection);
        }

        protected override void Initialize(GraphicsSystem system)
        {
            this.Transform.PositionChanged += OnTransformPositionChanged;
            this.Transform.RotationChanged += OnTransformRotationChanged;
            this.RecalculateViewMatrix();
            this.RecalculateProjectionMatrix();

            system.SetCamera(this);
            //system.OnScreenResized += OnScreenResized;
        }

        protected override void Uninitialize(GraphicsSystem system) { }

        private void OnFormResized(object sender, EventArgs e)
        {
            RecalculateProjectionMatrix();
        }

        private void RecalculateViewMatrix()
        {
            var lookAt = this.Transform.Position + this.Transform.Forward;
            this.viewMatrix = Matrix4x4.CreateLookAt(this.Transform.Position, lookAt, this.Transform.Up);
        }

        private void RecalculateProjectionMatrix()
        {
            float windowRatio = //(float)renderForm.ClientRectangle.Width / (float)renderForm.ClientRectangle.Height;
                16f / 9f;

            switch (this.projectionType)
            {
                case ProjectionType.Perspective:
                    this.projectionMatrix = Matrix4x4.CreatePerspectiveFieldOfView(fieldOfViewRadians, windowRatio, 0.1f, 1000.0f);
                    break;
                case ProjectionType.Orthographic:
                    this.projectionMatrix = MathUtil.CreateOrthographic(10 * windowRatio, 10, .03f, 1000f);
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