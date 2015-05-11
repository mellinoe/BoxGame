using EngineCore.Graphics;
using EngineCore.Graphics.OpenGL;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VoxelGame.World;

namespace VoxelGame.Graphics
{
    public class OpenGLChunkRenderInfo
    {
        private Chunk _chunk;
        private ChunkMeshInfo _meshInfo;
        private Vector3 _chunkCenter;

        private uint _vertexBufferId;
        private uint _indexBufferId;
        private int _numElements;
        private Chunk chunk;

        public OpenGLChunkRenderInfo(Chunk chunk, Vector3 center)
        {
            _chunk = chunk;
            _meshInfo = new ChunkMeshInfo(chunk);
            _chunkCenter = center;
            GenerateMeshBuffers(_meshInfo.Mesh, out _vertexBufferId, out _indexBufferId, out _numElements);
        }

        public OpenGLChunkRenderInfo(Chunk chunk, ChunkMeshInfo meshInfo, Vector3 center)
        {
            _chunk = chunk;
            _meshInfo = meshInfo;
            _chunkCenter = center;
            GenerateMeshBuffers(_meshInfo.Mesh, out _vertexBufferId, out _indexBufferId, out _numElements);
        }

        public OpenGLChunkRenderInfo(Chunk chunk, uint vertexBufferId, uint indexBufferId, IntPtr vertexBufferMapping, IntPtr indexBufferMapping, Vector3 chunkCenter)
        {
            _chunk = chunk;
            _chunkCenter = chunkCenter;

            _meshInfo = new ChunkMeshInfo(chunk);

            _vertexBufferId = vertexBufferId;
            _indexBufferId = indexBufferId;

            SendMeshDataToExistingMappings(vertexBufferMapping, indexBufferMapping);
        }

        private unsafe void SendMeshDataToExistingMappings(IntPtr vertexBufferMapping, IntPtr indexBufferMapping)
        {
            Console.WriteLine("vertex buffer mapping: " + vertexBufferMapping);
            Console.WriteLine("index buffer mapping: " + indexBufferMapping);

            SimpleVertex* vertexBufferBasePtr = (SimpleVertex*)vertexBufferMapping;
            int* indexBufferBasePtr = (int*)indexBufferMapping;

            IList<SimpleVertex> vertices = _meshInfo.Mesh.Vertices;
            Console.WriteLine("Will buffer " + vertices.Count * SimpleVertex.SizeInBytes + " bytes for vertex mapping loc " + vertexBufferMapping);
            IList<int> indices = _meshInfo.Mesh.Indices;
            Console.WriteLine("Will buffer " + indices.Count * sizeof(Int32) + " bytes for index mapping loc " + indexBufferMapping);

            for (int i = 0; i < vertices.Count; i++)
            {
                vertexBufferBasePtr[i] = vertices[i];
            }

            for (int i = 0; i < indices.Count; i++)
            {
                indexBufferBasePtr[i] = indices[i];
            }
        }

        public Vector3 ChunkCenter { get { return _chunkCenter; } }

        public void DrawMeshElements()
        {
            BindBuffers();
            DrawElements();
        }

        private void BindBuffers()
        {
            // Normal Array Buffer
            // Bind to the Array Buffer ID
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferId);
            // Set the Pointer to the current bound array describing how the data ia stored
            GL.NormalPointer(NormalPointerType.Float, SimpleVertex.SizeInBytes, SimpleVertex.NormalOffset);
            // Enable the client state so it will use this array buffer pointer
            GL.EnableClientState(ArrayCap.NormalArray);

            // Texture coordinate array
            GL.TexCoordPointer(2, TexCoordPointerType.Float, SimpleVertex.SizeInBytes, SimpleVertex.TexCoordOffset);

            // Enable texture coordinate array
            GL.EnableClientState(ArrayCap.TextureCoordArray);

            // Position Array Buffer
            GL.VertexPointer(3, VertexPointerType.Float, SimpleVertex.SizeInBytes, SimpleVertex.PositionOffset);
            // Enable the client state so it will use this array buffer pointer
            GL.EnableClientState(ArrayCap.VertexArray);

            // Element Array Buffer
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, _indexBufferId);
        }

        private void DrawElements()
        {
            // Draw the elements in the element array buffer
            // Draws up items in the Color, Vertex, TexCoordinate, and Normal Buffers using indices in the ElementArrayBuffer
            GL.DrawElements(PrimitiveType.Triangles, _numElements, DrawElementsType.UnsignedInt, IntPtr.Zero);
            // Could also call GL.DrawArrays which would ignore the ElementArrayBuffer and just use primitives
            // Of course we would have to reorder our data to be in the correct primitive order
        }

        private void GenerateMeshBuffers(PolyMesh polyMesh, out uint vertexBufferId, out uint indexBufferId, out int numElements)
        {
            // Vertex Buffer
            {
                int bufferSize;
                SimpleVertex[] vertices = polyMesh.Vertices.ToArray();
                // Generate Array Buffer Id
                GL.GenBuffers(1, out vertexBufferId);

                // Bind current context to Array Buffer ID
                GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferId);

                // Send data to buffer
                GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(vertices.Length * SimpleVertex.SizeInBytes), vertices, BufferUsageHint.DynamicDraw);

#if DEBUG
                // Validate that the buffer is the correct size
                GL.GetBufferParameter(BufferTarget.ArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
                if (vertices.Length * SimpleVertex.SizeInBytes != bufferSize)
                    throw new InvalidOperationException("Vertex array not uploaded correctly");
#endif

                // Clear the buffer Binding
                GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            }

            // Index Buffer
            {
                int bufferSize;

                // Generate Array Buffer Id
                GL.GenBuffers(1, out indexBufferId);

                // Bind current context to Array Buffer ID
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferId);

                // Send data to buffer
                GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(polyMesh.Indices.Count * sizeof(int)), polyMesh.Indices.ToArray(), BufferUsageHint.StaticDraw);

#if DEBUG
                // Validate that the buffer is the correct size
                GL.GetBufferParameter(BufferTarget.ElementArrayBuffer, BufferParameterName.BufferSize, out bufferSize);
                if (polyMesh.Indices.Count * sizeof(int) != bufferSize)
                    throw new InvalidOperationException("Element array not uploaded correctly");
#endif

                // Clear the buffer Binding
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);

                // Store the number of elements for the DrawElements call
                numElements = polyMesh.Indices.Count;
            }
        }
    }
}
