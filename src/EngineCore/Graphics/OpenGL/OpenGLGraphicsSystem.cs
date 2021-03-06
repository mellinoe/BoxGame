﻿using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace EngineCore.Graphics.OpenGL
{
    public unsafe class OpenGLGraphicsSystem : GraphicsSystem
    {
        private OpenTK.NativeWindow _window;
        private OpenTKNativeWindowInfo _windowInfo;
        private GraphicsContext _graphicsContext;
        private Matrix4x4 _viewMatrix;
        private NativeWindowInputSystem _inputSystem;
        private float fieldOfViewRadians = 1.05f;
        private Camera _camera;

        private bool _supportsMeshBatching = true;

        private List<IRenderableObjectInfo> _renderableObjects = new List<IRenderableObjectInfo>();
        private List<IRenderableObjectInfo2D> _renderableObjects2D = new List<IRenderableObjectInfo2D>();
        private Dictionary<PolyMesh, BatchedOpenGLMeshInfo> _batchedModels = new Dictionary<PolyMesh, BatchedOpenGLMeshInfo>();

        public OpenGLGraphicsSystem(Game game)
            : base(game)
        {
            _window = new OpenTK.NativeWindow(960, 600, "EngineCore", OpenTK.GameWindowFlags.Default, GraphicsMode.Default, OpenTK.DisplayDevice.Default);
            _windowInfo = new OpenTKNativeWindowInfo(_window, _window.Width / 960f);
            GraphicsContextFlags flags = GraphicsContextFlags.Default;
#if DEBUG
            //flags |= GraphicsContextFlags.Debug;
#endif
            _graphicsContext = new GraphicsContext(GraphicsMode.Default, _window.WindowInfo, 3, 0, flags);
            _graphicsContext.MakeCurrent(_window.WindowInfo);
            ((IGraphicsContextInternal)_graphicsContext).LoadAll(); // wtf is this?

            _inputSystem = new NativeWindowInputSystem(Game, _window);

            SetInitialStates();
            SetViewport();

            _window.Resize += OnGameWindowResized;
            _window.Closing += OnWindowClosing;
        }

        public EngineCore.Input.InputSystem InputSystem { get { return _inputSystem; } }
        public override IWindowInfo WindowInfo => _windowInfo;

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
            GL.ClearColor(OpenTK.Color.CornflowerBlue);
            GL.Enable(EnableCap.DepthTest);
            GL.Enable(EnableCap.PolygonSmooth);
            GL.Enable(EnableCap.CullFace);
            GL.FrontFace(FrontFaceDirection.Cw);
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

            foreach (IRenderableObjectInfo roi in _renderableObjects)
            {
                roi.Render(ref _viewMatrix);
            }

            foreach (IRenderableObjectInfo2D roi2D in _renderableObjects2D)
            {
                roi2D.Render();
            }

            _graphicsContext.SwapBuffers();
        }

        private void SetProjectionMatrix(Matrix4x4 projectionMatrix)
        {
            GL.MatrixMode(MatrixMode.Projection);
            GLEx.LoadMatrix(ref projectionMatrix);
        }

        public override void RegisterSimpleMesh(IRenderable renderable, PolyMesh mesh, Texture2D texture)
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
                    batchedMeshInfo = new BatchedOpenGLMeshInfo(mesh, texture);
                    batchedMeshInfo.AddRenderable(renderable);
                    _batchedModels.Add(mesh, batchedMeshInfo);
                    _renderableObjects.Add(batchedMeshInfo);
                }
            }
            else // Mesh batching not supported
            {
                _renderableObjects.Add(new OpenGLMeshInfo(renderable, mesh, texture));
            }
        }

        public override void SetCamera(Camera camera)
        {
            _camera = camera;
        }

        // Shouldn't use this for many things
        public void AddSelfManagedRenderable(IRenderableObjectInfo info)
        {
            _renderableObjects.Add(info);
        }

        // Shouldn't use this for many things
        public void RemoveSelfManagedRenderable(IRenderableObjectInfo info)
        {
            _renderableObjects.Remove(info);
        }

        public void AddSelfManagedRenderable(IRenderableObjectInfo2D info)
        {
            _renderableObjects2D.Add(info);
        }

        // Shouldn't use this for many things
        public void RemoveSelfManagedRenderable(IRenderableObjectInfo2D info)
        {
            _renderableObjects2D.Remove(info);
        }

        public OpenTK.NativeWindow Window { get { return _window; } }

        public override void RegisterLight(ILightInfo lightInfo)
        {
            Console.WriteLine("OpenGLGraphicsSystem does not support lights yet.");
        }
    }
}
