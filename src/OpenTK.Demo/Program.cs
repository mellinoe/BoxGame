// This code was written for the OpenTK library and has been released
// to the Public Domain.
// It is provided "as is" without express or implied warranty of any kind.

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace OpenTK.Demo
{
    public class Program
    {
        public static void Main(string[] args)
        {

            using (DemoWindow demoWindow = new DemoWindow(900, 600))
            {
                demoWindow.Run(30.0, 0.0);
            }
        }
    }

    public class DemoWindow : GameWindow
    {
        public DemoWindow(int width, int height) : base(width, height)
        {
            Title = "Simple OpenTK Window";
        }

        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(Color.CornflowerBlue);
        }

        protected override void OnResize(EventArgs e)
        {
            SetDefaultProperties();
        }

        private void SetDefaultProperties()
        {
            GL.Viewport(0, 0, Width, Height);

            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(-1.0, 1.0, -1.0, 1.0, 0.0, 4.0);
        }

        /// <summary>
        /// Add your game rendering code here.
        /// </summary>
        /// <param name="e">Contains timing information.</param>
        /// <remarks>There is no need to call the base implementation.</remarks>
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.Begin(BeginMode.Triangles);

            GL.Color3(Color.MidnightBlue);
            GL.Vertex2(-1.0f, 1.0f);
            GL.Color3(Color.SpringGreen);
            GL.Vertex2(0.0f, -1.0f);
            GL.Color3(Color.Ivory);
            GL.Vertex2(1.0f, 1.0f);

            GL.End();

            this.SwapBuffers();
        }
    }
}