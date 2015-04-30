using EngineCore.Utility;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EngineCore.Graphics.OpenGL
{
    public unsafe class OpenGLGraphicsSystem : GraphicsSystem
    {
        private OpenTK.NativeWindow _window;
        private GraphicsContext _graphicsContext;
        private Matrix4x4 _viewMatrix;
        private NativeWindowInputSystem _inputSystem;
        private float fieldOfViewRadians = 1.05f;
        private Camera _camera;
        private static readonly string s_windowTitle = "Hold F to Fire Boxes, Y to Place, +/- to Resize";

        private bool _supportsMeshBatching = true;

        private List<IRenderableObjectInfo> _renderableObjects = new List<IRenderableObjectInfo>();
        private Dictionary<PolyMesh, BatchedOpenGLMeshInfo> _batchedModels = new Dictionary<PolyMesh, BatchedOpenGLMeshInfo>();

        public OpenGLGraphicsSystem(Game game)
            : base(game)
        {
            _window = new OpenTK.NativeWindow(960, 600, s_windowTitle, OpenTK.GameWindowFlags.Default, GraphicsMode.Default, OpenTK.DisplayDevice.Default);
            _graphicsContext = new GraphicsContext(GraphicsMode.Default, _window.WindowInfo, 3, 0, GraphicsContextFlags.Default);
            _graphicsContext.MakeCurrent(_window.WindowInfo);
            ((IGraphicsContextInternal)_graphicsContext).LoadAll(); // wtf is this?

            _inputSystem = new NativeWindowInputSystem(Game, _window);

            SetInitialStates();
            SetViewport();

            _window.Resize += OnGameWindowResized;
            _window.Closing += OnWindowClosing;
        }

        public EngineCore.Input.InputSystem InputSystem { get { return _inputSystem; } }

        public override void Start()
        {
            _window.Visible = true;
        }

        private void OnWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _graphicsContext.Dispose();
            this.Game.Exit();
        }

        private void SetInitialStates()
        {
            GL.ClearColor(System.Drawing.Color.CornflowerBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.PolygonSmooth);
        }

        private void SetViewport()
        {
            _graphicsContext.Update(_window.WindowInfo);
            GL.Viewport(0, 0, _window.Width, _window.Height);

            float aspectRatio = _window.Width / (float)_window.Height;
            Matrix4x4 perspective = Matrix4x4.CreatePerspectiveFieldOfView(fieldOfViewRadians, aspectRatio, 0.1f, 1000.0f);
        }

        private void OnGameWindowResized(object sender, EventArgs e)
        {
            GameWindowResizedEventArgs args = new GameWindowResizedEventArgs(_window.Width, _window.Height);
            RaiseOnResized(args);

            SetViewport();
        }

        public override void Stop()
        {

        }

        public override void Update()
        {
            RenderFrame();

            _window.ProcessEvents();
        }

        private void RenderFrame()
        {
            // render graphics
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            _viewMatrix = _camera.GetViewMatrix();
            SetProjectionMatrix(_camera.GetProjectionMatrix());

            foreach (IRenderableObjectInfo roi in this._renderableObjects)
            {
                roi.Render(ref _viewMatrix, _graphicsContext);
            }
            _graphicsContext.SwapBuffers();
        }

        private void SetProjectionMatrix(Matrix4x4 projectionMatrix)
        {
            GL.MatrixMode(MatrixMode.Projection);
            GLEx.LoadMatrix(ref projectionMatrix);
        }

        public override void RegisterSimpleMesh(IRenderable renderable, PolyMesh mesh, Bitmap bitmap)
        {
            BatchedOpenGLMeshInfo batchedMeshInfo;

            if (_supportsMeshBatching)
            {
                if (_batchedModels.TryGetValue(mesh, out batchedMeshInfo))
                {
                    batchedMeshInfo.AddRenderable(renderable);
                }
                else
                {
                    batchedMeshInfo = new BatchedOpenGLMeshInfo(mesh, bitmap);
                    batchedMeshInfo.AddRenderable(renderable);
                    _batchedModels.Add(mesh, batchedMeshInfo);
                    _renderableObjects.Add(batchedMeshInfo);
                }
            }
            else // Mesh batching not supported
            {
                _renderableObjects.Add(new OpenGLMeshInfo(renderable, mesh, bitmap));
            }
        }

        public override void SetCamera(Camera camera)
        {
            _camera = camera;
        }

        // Shouldn't use this for many things
        internal void AddSelfManagedRenderable(IRenderableObjectInfo info)
        {
            _renderableObjects.Add(info);
        }

        // Shouldn't use this for many things
        internal unsafe void RemoveSelfManagedRenderable(IRenderableObjectInfo info)
        {
            _renderableObjects.Remove(info);
        }

        public override Size WindowSize
        {
            get
            {
                return _window.Size;
            }
            set
            {
                _window.Size = value;
            }
        }
    }
}
