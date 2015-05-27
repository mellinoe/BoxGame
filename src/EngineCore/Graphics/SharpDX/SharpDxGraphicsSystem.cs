using EngineCore.Entities;
using EngineCore.Graphics;
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
        private ShaderCache shaderCache = new ShaderCache();
        public ShaderCache ShaderCache { get { return shaderCache; } }

        SimpleRenderer renderer;
        public SimpleRenderer Renderer
        {
            get { return renderer; }
            set { renderer = value; }
        }

        private EngineCore.Graphics.OpenGL.NativeWindowInputSystem _inputSystem;
        internal EngineCore.Graphics.OpenGL.NativeWindowInputSystem InputSystem { get { return _inputSystem; } }

        private Thread thread;
        private bool active;

        public SharpDxGraphicsSystem(Game game)
            : base(game)
        {
            this.thread = new Thread(ThreadStartFunc);
        }

        private void ThreadStartFunc(object obj)
        {
            renderer = new SimpleRenderer();
            renderer.Window.Closing += OnWindowClosing;

            _inputSystem = new OpenGL.NativeWindowInputSystem(Game, renderer.Window);

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
                renderer.RenderFrame();
                renderer.Window.ProcessEvents();
            }
        }

        public override void Start()
        {
            this.thread.Start();
            while (this.renderer == null)
            {
                Thread.Sleep(0);
            }
        }

        public override void Stop()
        {
            Debug.WriteLine("Stopping SharpDxGraphicsSystem");
            this.active = false;

        }

        public override void SetCamera(Camera camera)
        {
            renderer.MainCamera = camera;
        }

        public override void RegisterSimpleMesh(IRenderable renderable, PolyMesh _cubeMesh, System.Drawing.Bitmap bitmap)
        {
            Console.WriteLine("I should be registering a simple mesh now...");
        }

        public override System.Drawing.Size WindowSize
        {
            get
            {
                return renderer.Window.ClientSize;
            }
            set
            {
                renderer.Window.ClientSize = value;
            }
        }
    }
}
