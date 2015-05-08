using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace EngineCore.Graphics.OpenGL
{
    public class OpenGLMeshInfo : IRenderableObjectInfo, IDisposable
    {
        private IRenderable _renderable;
        private PolyMesh _mesh;

        // Buffers
        private TextureBuffer _textureBuffer;
        private uint _vertexBufferId;
        private uint _indexBufferId;

        private int _numElements;

        public OpenGLMeshInfo(IRenderable renderable, PolyMesh mesh, System.Drawing.Bitmap texture)
        {
            this._renderable = renderable;
            this._mesh = mesh;
            this._textureBuffer = new TextureBuffer(texture);

            GenerateBuffers();
        }

        private unsafe void GenerateBuffers()
        {
            GenerateCombinedVertexBuffer();
            GenerateIndicesBuffer();
        }

        private void RenderImmediateMode(ref Matrix4x4 viewMatrix)
        {
            GL.MatrixMode(MatrixMode.Modelview);

            Matrix4x4 transformMatrix = _renderable.WorldMatrix * viewMatrix;
            GLEx.LoadMatrix(ref transformMatrix);

            GL.Begin(PrimitiveType.Triangles);
            for (int i = 0; i <= _mesh.Indices.Count - 3; i += 3)
            {
                SimpleVertex vertex0 = _mesh.Vertices[_mesh.Indices[i]];
                SimpleVertex vertex1 = _mesh.Vertices[_mesh.Indices[i + 1]];
                SimpleVertex vertex2 = _mesh.Vertices[_mesh.Indices[i + 2]];

                GLEx.Color4f(vertex0.Color);
                GLEx.Vertex3(vertex0.Position);
                GLEx.Normal3(vertex0.Normal);

                GLEx.Color4f(vertex1.Color);
                GLEx.Vertex3(vertex1.Position);
                GLEx.Normal3(vertex1.Normal);

                GLEx.Color4f(vertex2.Color);
                GLEx.Vertex3(vertex2.Position);
                GLEx.Normal3(vertex2.Normal);
            }
            GL.End();
        }

        public virtual unsafe void Render(ref Matrix4x4 viewMatrix)
        {
            //RenderImmediateMode(ref viewMatrix);
            RenderWithVbo(ref viewMatrix);
        }

        private unsafe void RenderWithVbo(ref Matrix4x4 viewMatrix)
        {
            GL.MatrixMode(MatrixMode.Modelview);

            Matrix4x4 transformMatrix = _renderable.WorldMatrix * viewMatrix;
            GLEx.LoadMatrix(ref transformMatrix);

            // Push current Array Buffer state so we can restore it later
            GL.PushClientAttrib(ClientAttribMask.ClientVertexArrayBit);

            if (_vertexBufferId == 0)
                return;
            if (_indexBufferId == 0)
                return;

            BindTexture();
            BindAllBuffers();
            DrawElements();
            UnbindTexture();

            // Restore the state
            GL.PopClientAttrib();
        }

        protected void UnbindTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        protected void BindTexture()
        {
            GL.Enable(EnableCap.Texture2D);
            _textureBuffer.Bind();
        }

        protected unsafe void BindAllBuffers()
        {
            // Normal Array Buffer
            // Bind to the Array Buffer ID
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferId);
            // Set the Pointer to the current bound array describing how the data ia stored
            GL.NormalPointer(NormalPointerType.Float, SimpleVertex.SizeInBytes, SimpleVertex.NormalOffset);
            // Enable the client state so it will use this array buffer pointer
            GL.EnableClientState(ArrayCap.NormalArray);

            // Texture coordinate array
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferId);
            GL.TexCoordPointer(2, TexCoordPointerType.Float, SimpleVertex.SizeInBytes, SimpleVertex.TexCoordOffset);

            // Enable texture coordinate array
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            // Position Array Buffer

            // Bind to the Array Buffer ID
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferId);
            // Set the Pointer to the current bound array describing how the data ia stored
            GL.VertexPointer(3, VertexPointerType.Float, SimpleVertex.SizeInBytes, SimpleVertex.PositionOffset);
            // Enable the client state so it will use this array buffer pointer
            GL.EnableClientState(ArrayCap.VertexArray);

            // Element Array Buffer
            // Bind to the Array Buffer ID
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBufferId);
        }

        protected void DrawElements()
        {
            // Draw the elements in the element array buffer
            // Draws up items in the Color, Vertex, TexCoordinate, and Normal Buffers using indices in the ElementArrayBuffer
            GL.DrawElements(PrimitiveType.Triangles, _numElements, DrawElementsType.UnsignedInt, IntPtr.Zero);
            // Could also call GL.DrawArrays which would ignore the ElementArrayBuffer and just use primitives
            // Of course we would have to reorder our data to be in the correct primitive order
        }

        /// <summary>
        /// 
        /// </summary>
        private void PrintCurrentArrayBufferSize()
        {
            int value;
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out value);
            Console.WriteLine("Size of current ArrayBuffer: " + value);
        }

        private void PrintCurrentElementBufferSize()
        {
            int value;
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out value);
            Console.WriteLine("Size of current ElementArrayBuffer: " + value);
        }

        #region Buffer Binding and Initialization
        private void GenerateCombinedVertexBuffer()
        {
            int bufferSize;
            SimpleVertex[] vertices = _mesh.Vertices.ToArray();
            // Generate Array Buffer Id
            GL.GenBuffers(1, out _vertexBufferId);

            // Bind current context to Array Buffer ID
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferId);

            // Send data to buffer
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * SimpleVertex.SizeInBytes), vertices, BufferUsageHint.DynamicDraw);

            // Validate that the buffer is the correct size
            GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
            if (vertices.Length * SimpleVertex.SizeInBytes != bufferSize)
                throw new InvalidOperationException("Vertex array not uploaded correctly");

            // Clear the buffer Binding
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        private void GenerateIndicesBuffer()
        {
            int bufferSize;

            // Generate Array Buffer Id
            GL.GenBuffers(1, out _indexBufferId);

            // Bind current context to Array Buffer ID
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBufferId);

            // Send data to buffer
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(_mesh.Indices.Count * sizeof(int)), _mesh.Indices.ToArray(), BufferUsageHint.StaticDraw);

            // Validate that the buffer is the correct size
            GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
            if (_mesh.Indices.Count * sizeof(int) != bufferSize)
                throw new InvalidOperationException("Element array not uploaded correctly");

            // Clear the buffer Binding
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

            // Store the number of elements for the DrawElements call
            _numElements = _mesh.Indices.Count;
        }
        #endregion Buffer Binding and Initialization

        public virtual void Dispose()
        {
            _textureBuffer.Dispose();

            if (_vertexBufferId != 0)
            {
                GL.DeleteBuffer(_vertexBufferId);
                _vertexBufferId = 0;
            }

            if (_indexBufferId != 0)
            {
                GL.DeleteBuffer(_indexBufferId);
                _indexBufferId = 0;
            }
        }
    }
}
