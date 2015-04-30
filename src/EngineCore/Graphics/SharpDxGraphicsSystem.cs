using EngineCore.Entities;
using EngineCore.Graphics;
using EngineCore.Physics;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

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
            renderer.Form.MouseDown += OnMouseDown;
            renderer.Form.FormClosing += OnFormClosing;
            this.active = true;
            Application.Run(renderer.Form);
        }

        private void OnFormClosing(object sender, FormClosingEventArgs e)
        {
            Game.Exit();
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            Debug.WriteLine("Clicked at " + e.X + ", " + e.Y);
            if (e.Button == MouseButtons.Left)
            {
                renderer.Form.Text += "+";
            }
            else if (e.Button == MouseButtons.Right)
            {
                renderer.Form.Text = renderer.Form.Text.Length != 0 ? renderer.Form.Text.Substring(0, renderer.Form.Text.Length - 1) : "Empty";
            }
        }

        public override void Update()
        {
            if (this.active)
            {
                renderer.RenderFrame();
            }
        }

        public override void Start()
        {
            this.thread.Start();
            while (this.renderer == null)
            {
                Thread.Yield();
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
