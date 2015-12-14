using EngineCore.Components;
using EngineCore.Graphics;
using System.IO;
using System.Numerics;

namespace EngineCore.Entities
{
    public class BoxRenderer : Component, IRenderable
    {
        internal readonly PolyMesh _cubeMesh;
        internal readonly Texture2D _surfaceTexture;

        private static readonly string s_defaultImagePath = Path.Combine("Textures", "StoneTile.png");
        private static readonly Texture2D s_defaultImage = new Texture2D(s_defaultImagePath);

        public BoxRenderer()
        {
            _surfaceTexture = s_defaultImage;
            _cubeMesh = s_cachedMeshInstance;
        }

        public BoxRenderer(Texture2D texture)
            : this()
        {
            _cubeMesh = s_cachedMeshInstance;
            _surfaceTexture = texture;
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
            get { return this._scaleMatrix * Transform.WorldMatrix; }
        }

        private static readonly SimpleVertex[] s_cubeVertices = new SimpleVertex[]
        {
            // Top
            new SimpleVertex(new Vector3(-.5f,.5f,.5f),     new Vector3(0,1,0), Color4f.Red,        new Vector2(0, 0)),
            new SimpleVertex(new Vector3(.5f,.5f,.5f),      new Vector3(0,1,0), Color4f.Red,        new Vector2(1, 0)),
            new SimpleVertex(new Vector3(.5f,.5f,-.5f),     new Vector3(0,1,0), Color4f.Red,        new Vector2(1, 1)),
            new SimpleVertex(new Vector3(-.5f,.5f,-.5f),    new Vector3(0,1,0), Color4f.Red,        new Vector2(0, 1)),
            // Bottom
            new SimpleVertex(new Vector3(-.5f,-.5f,.5f),    new Vector3(0,-1,0), Color4f.Grey,       new Vector2(0, 0)),
            new SimpleVertex(new Vector3(.5f,-.5f,.5f),     new Vector3(0,-1,0), Color4f.Grey,       new Vector2(1, 0)),
            new SimpleVertex(new Vector3(.5f,-.5f,-.5f),    new Vector3(0,-1,0), Color4f.Grey,       new Vector2(1, 1)),
            new SimpleVertex(new Vector3(-.5f,-.5f,-.5f),   new Vector3(0,-1,0), Color4f.Grey,       new Vector2(0, 1)),
            // Left
            new SimpleVertex(new Vector3(-.5f,-.5f,.5f),    new Vector3(-1,0,0), Color4f.Blue,       new Vector2(0, 0)),
            new SimpleVertex(new Vector3(-.5f,.5f,.5f),     new Vector3(-1,0,0), Color4f.Blue,       new Vector2(1, 0)),
            new SimpleVertex(new Vector3(-.5f,.5f,-.5f),    new Vector3(-1,0,0), Color4f.Blue,       new Vector2(1, 1)),
            new SimpleVertex(new Vector3(-.5f,-.5f,-.5f),   new Vector3(-1,0,0), Color4f.Blue,       new Vector2(0, 1)),
            // Right
            new SimpleVertex(new Vector3(.5f,-.5f,.5f),     new Vector3(1,0,0), Color4f.Green,     new Vector2(0, 0)),
            new SimpleVertex(new Vector3(.5f,.5f,.5f),      new Vector3(1,0,0), Color4f.Green,     new Vector2(1, 0)),
            new SimpleVertex(new Vector3(.5f,.5f,-.5f),     new Vector3(1,0,0), Color4f.Green,     new Vector2(1, 1)),
            new SimpleVertex(new Vector3(.5f,-.5f,-.5f),    new Vector3(1,0,0), Color4f.Green,     new Vector2(0, 1)),
            // Back
            new SimpleVertex(new Vector3(-.5f,.5f,.5f),     new Vector3(0,0,1), Color4f.Yellow,     new Vector2(0, 0)),
            new SimpleVertex(new Vector3(.5f,.5f,.5f),      new Vector3(0,0,1), Color4f.Yellow,     new Vector2(1, 0)),
            new SimpleVertex(new Vector3(.5f,-.5f,.5f),     new Vector3(0,0,1), Color4f.Yellow,     new Vector2(1, 1)),
            new SimpleVertex(new Vector3(-.5f,-.5f,.5f),    new Vector3(0,0,1), Color4f.Yellow,     new Vector2(0, 1)),
            // Front
            new SimpleVertex(new Vector3(-.5f,.5f,-.5f),    new Vector3(0,0,-1), Color4f.Orange,     new Vector2(0, 0)),
            new SimpleVertex(new Vector3(.5f,.5f,-.5f),     new Vector3(0,0,-1), Color4f.Orange,     new Vector2(1, 0)),
            new SimpleVertex(new Vector3(.5f,-.5f,-.5f),    new Vector3(0,0,-1), Color4f.Orange,     new Vector2(1, 1)),
            new SimpleVertex(new Vector3(-.5f,-.5f,-.5f),   new Vector3(0,0,-1), Color4f.Orange,     new Vector2(0, 1))
        };

        private static readonly int[] s_cubeIndices = new int[]
        {
            0,1,2,0,2,3,
            4,5,6,4,6,7,
            8,9,10,8,10,11,
            12,13,14,12,14,15,
            16,17,18,16,18,19,
            20,21,22,20,22,23
        };

        private static PolyMesh s_cachedMeshInstance = new PolyMesh(s_cubeVertices, s_cubeIndices);
    }
}
