using EngineCore.Components;
using System.Numerics;

namespace EngineCore.Graphics
{
    public class MeshRenderer : Component, IRenderable
    {
        public PolyMesh Mesh { get; }
        public Texture2D SurfaceTexture { get; }

        private Matrix4x4 _renderOffsetMatrix = Matrix4x4.Identity;
        public void SetRenderOffset(Vector3 offset)
        {
            _renderOffsetMatrix = Matrix4x4.CreateTranslation(offset);
        }

        public MeshRenderer(PolyMesh mesh, Texture2D surfaceTexture)
        {
            Mesh = mesh;
            SurfaceTexture = surfaceTexture;
        }

        private Vector3 _scale;
        public Vector3 Scale
        {
            get { return _scale; }
            set
            {
                _scale = value;
                _scaleMatrix = Matrix4x4.CreateScale(_scale);
            }
        }

        private Matrix4x4 _scaleMatrix = Matrix4x4.Identity;
        public Matrix4x4 WorldMatrix
        {
            get { return _scaleMatrix * _renderOffsetMatrix * Transform.WorldMatrix; }
        }
    }
}
