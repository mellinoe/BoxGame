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
    public class SharpDxGraphicsSystem : GameSystem
    {
        private ShaderCache shaderCache = new ShaderCache();
        public ShaderCache ShaderCache { get { return shaderCache; } }

        SimpleRenderer renderer;
        public SimpleRenderer Renderer
        {
            get { return renderer; }
            set { renderer = value; }
        }

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

        internal void SetCamera(Camera camera)
        {
            renderer.MainCamera = camera;
        }
    }
}
