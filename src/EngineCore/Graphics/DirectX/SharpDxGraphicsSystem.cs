using EngineCore.Entities;
using EngineCore.Graphics;
using EngineCore.Graphics.DirectX;
using EngineCore.Physics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace EngineCore.Graphics
{
    public class SharpDxGraphicsSystem : GraphicsSystem
    {
        private bool _supportsBatching = true;
        private Dictionary<PolyMesh, Direct3DBatchedMeshInfo> _batchedMeshInfos = new Dictionary<PolyMesh, Direct3DBatchedMeshInfo>();

        private ShaderCache shaderCache = new ShaderCache();
        public ShaderCache ShaderCache { get { return shaderCache; } }

        SimpleRenderer _renderer;
        public SimpleRenderer Renderer
        {
            get { return _renderer; }
            set { _renderer = value; }
        }

        private EngineCore.Graphics.OpenGL.NativeWindowInputSystem _inputSystem;
        internal EngineCore.Graphics.OpenGL.NativeWindowInputSystem InputSystem { get { return _inputSystem; } }

        private bool active;

        public SharpDxGraphicsSystem(Game game)
            : base(game)
        {
            _renderer = new SimpleRenderer();
            _renderer.Window.Closing += OnWindowClosing;

            _inputSystem = new OpenGL.NativeWindowInputSystem(Game, _renderer.Window);

            this.active = true;
        }

        private void OnWindowClosing(object sender, EventArgs e)
        {
            Game.Exit();
        }

        public override void Update()
        {
            if (this.active)
            {
                _renderer.RenderFrame();
                _renderer.Window.ProcessEvents();
            }
        }

        public override void Start()
        {

        }

        public override void Stop()
        {
            Debug.WriteLine("Stopping SharpDxGraphicsSystem");
            this.active = false;
        }

        public override void SetCamera(Camera camera)
        {
            _renderer.MainCamera = camera;
        }

        public override void RegisterSimpleMesh(IRenderable renderable, PolyMesh mesh, System.Drawing.Bitmap bitmap)
        {
            if (_supportsBatching)
            {
                Direct3DBatchedMeshInfo batchedInfo;
                if (!_batchedMeshInfos.TryGetValue(mesh, out batchedInfo))
                {
                    batchedInfo = new Direct3DBatchedMeshInfo(Renderer, renderable, mesh.Vertices.ToArray(), mesh.Indices.ToArray(), bitmap);
                    _batchedMeshInfos.Add(mesh, batchedInfo);
                    _renderer.AddRenderable(batchedInfo);
                }

                batchedInfo.AddRenderable(renderable);
            }
            else
            {
                Direct3DMeshInfo meshInfo = new Direct3DMeshInfo(_renderer, renderable, mesh.Vertices.ToArray(), mesh.Indices.ToArray(), bitmap);
                _renderer.AddRenderable(meshInfo);
            }
        }

        public override OpenTK.Size WindowSize
        {
            get
            {
                return _renderer.Window.ClientSize;
            }
            set
            {
                _renderer.Window.ClientSize = value;
            }
        }

        public override void RegisterLight(ILightInfo lightInfo)
        {
            _renderer.AddLight(lightInfo);
        }
    }
}
