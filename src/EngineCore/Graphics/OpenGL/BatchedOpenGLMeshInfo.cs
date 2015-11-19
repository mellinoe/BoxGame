using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using System.Numerics;

namespace EngineCore.Graphics.OpenGL
{
    class BatchedOpenGLMeshInfo : OpenGLMeshInfo
    {
        public BatchedOpenGLMeshInfo(PolyMesh mesh, Texture2D texture) : base(null, mesh, texture) { }

        private List<IRenderable> _renderables = new List<IRenderable>();

        public override unsafe void Render(ref Matrix4x4 viewMatrix)
        {
            // Push current Array Buffer state so we can restore it later
            // (is this necessary...?)
            GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);
            BindAllBuffers();
            BindTexture();

            GL.MatrixMode(MatrixMode.Modelview);
            foreach (IRenderable renderable in _renderables)
            {
                Matrix4x4 modelViewMatrix = renderable.WorldMatrix * viewMatrix;
                GLEx.LoadMatrix(ref modelViewMatrix);

                this.DrawElements();
            }
            UnbindTexture();

            // Restore the state
            GL.PopClientAttrib();
        }

        internal void AddRenderable(IRenderable renderable)
        {
            _renderables.Add(renderable);
        }

        internal void RemoveRenderable(IRenderable renderable)
        {
            _renderables.Remove(renderable);
        }
    }
}
