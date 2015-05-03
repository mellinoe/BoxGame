using EngineCore.Components;
using EngineCore.Graphics;
using EngineCore.Graphics.OpenGL;
using EngineCore.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;

namespace EngineCore.Entities
{
    public class BoxRenderer : Component<GraphicsSystem>, IRenderable
    {
        private readonly PolyMesh _cubeMesh;
        private readonly Bitmap _bitmap;

        private static readonly string s_defaultBitmapPath = Path.Combine(AppContext.BaseDirectory, "Textures", "Stone.png");
        private static readonly Bitmap s_defaultBitmap = new Bitmap(s_defaultBitmapPath);

        public BoxRenderer()
        {
            _bitmap = s_defaultBitmap;
            _cubeMesh = s_cachedMeshInstance;
        }

        public BoxRenderer(Bitmap bitmap)
            : this()
        {
            _cubeMesh = s_cachedMeshInstance;
            _bitmap = bitmap;
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
        public System.Numerics.Matrix4x4 WorldMatrix
        {
            get { return this._scaleMatrix * Transform.WorldMatrix; }
        }

        protected override void Initialize(GraphicsSystem system)
        {
            system.RegisterSimpleMesh(this, _cubeMesh, _bitmap);
        }

        protected override void Uninitialize(GraphicsSystem system)
        {
            throw new NotImplementedException();
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
            new SimpleVertex(new Vector3(-.5f,-.5f,.5f),    new Vector3(1,0,0), Color4f.Blue,       new Vector2(0, 0)),
            new SimpleVertex(new Vector3(-.5f,.5f,.5f),     new Vector3(1,0,0), Color4f.Blue,       new Vector2(1, 0)),
            new SimpleVertex(new Vector3(-.5f,.5f,-.5f),    new Vector3(1,0,0), Color4f.Blue,       new Vector2(1, 1)),
            new SimpleVertex(new Vector3(-.5f,-.5f,-.5f),   new Vector3(1,0,0), Color4f.Blue,       new Vector2(0, 1)),
            // Right
            new SimpleVertex(new Vector3(.5f,-.5f,.5f),     new Vector3(-1,0,0), Color4f.Green,     new Vector2(0, 0)),
            new SimpleVertex(new Vector3(.5f,.5f,.5f),      new Vector3(-1,0,0), Color4f.Green,     new Vector2(1, 0)),
            new SimpleVertex(new Vector3(.5f,.5f,-.5f),     new Vector3(-1,0,0), Color4f.Green,     new Vector2(1, 1)),
            new SimpleVertex(new Vector3(.5f,-.5f,-.5f),    new Vector3(-1,0,0), Color4f.Green,     new Vector2(0, 1)),
            // Front
            new SimpleVertex(new Vector3(-.5f,.5f,.5f),     new Vector3(0,0,-1), Color4f.Yellow,     new Vector2(0, 0)),
            new SimpleVertex(new Vector3(.5f,.5f,.5f),      new Vector3(0,0,-1), Color4f.Yellow,     new Vector2(1, 0)),
            new SimpleVertex(new Vector3(.5f,-.5f,.5f),     new Vector3(0,0,-1), Color4f.Yellow,     new Vector2(1, 1)),
            new SimpleVertex(new Vector3(-.5f,-.5f,.5f),    new Vector3(0,0,-1), Color4f.Yellow,     new Vector2(0, 1)),
            // Back
            new SimpleVertex(new Vector3(-.5f,.5f,-.5f),    new Vector3(0,0,1), Color4f.Orange,     new Vector2(0, 0)),
            new SimpleVertex(new Vector3(.5f,.5f,-.5f),     new Vector3(0,0,1), Color4f.Orange,     new Vector2(1, 0)),
            new SimpleVertex(new Vector3(.5f,-.5f,-.5f),    new Vector3(0,0,1), Color4f.Orange,     new Vector2(1, 1)),
            new SimpleVertex(new Vector3(-.5f,-.5f,-.5f),   new Vector3(0,0,1), Color4f.Orange,     new Vector2(0, 1))
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
