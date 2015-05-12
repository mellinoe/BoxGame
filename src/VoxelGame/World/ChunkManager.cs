using EngineCore.Graphics.OpenGL;
using Noise;
using System.Numerics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using VoxelGame.Graphics;
using System.IO;
using EngineCore.Graphics;
using System.Threading.Tasks;
using System.Diagnostics;
using OpenTK.Graphics;

namespace VoxelGame.World
{
    public class ChunkManager : IRenderableObjectInfo
    {
        private int _loadedChunkDistance = 10;
        private NoiseGen _noiseGen = new NoiseGen(1f, 1f, 4);
        private SpatialStorageBuffer<Tuple<Chunk, OpenGLChunkRenderInfo>> _chunks;

        private static Texture2D s_cubeFaceTextures = new Texture2D("Textures/CubeFaceTextures.png");
        private TextureBuffer _textureBuffer = new TextureBuffer(s_cubeFaceTextures);

        public ChunkManager(OpenGLGraphicsSystem graphicsSystem)
        {
            Stopwatch sw = Stopwatch.StartNew();

            GenChunksSimpleThreaded(graphicsSystem);

            sw.Stop();
            Console.WriteLine("Total elapsed chunk generation time: " + sw.Elapsed.TotalSeconds);
        }

        private void GenChunksSimple()
        {
            _chunks = new SpatialStorageBuffer<Tuple<Chunk, OpenGLChunkRenderInfo>>(_loadedChunkDistance);
            for (int x = 0; x < _loadedChunkDistance; x++)
                for (int y = 0; y < _loadedChunkDistance; y++)
                    for (int z = 0; z < _loadedChunkDistance; z++)
                    {
                        int worldX = x * Chunk.ChunkLength;
                        int worldY = y * Chunk.ChunkLength;
                        int worldZ = z * Chunk.ChunkLength;
                        Chunk chunk = GenChunk(worldX, worldY, worldZ);
                        OpenGLChunkRenderInfo renderInfo = new OpenGLChunkRenderInfo(chunk, new Vector3(worldX, worldY, worldZ));
                        _chunks[x, y, z] = Tuple.Create(chunk, renderInfo);
                    }
        }

        private void GenChunksSimpleThreaded(OpenGLGraphicsSystem graphicsSystem)
        {
            _chunks = new SpatialStorageBuffer<Tuple<Chunk, OpenGLChunkRenderInfo>>(_loadedChunkDistance);
            var tempChunks = new Chunk[_chunks.NumItems];
            List<Task> tasks = new List<Task>();

            for (int x = 0; x < _loadedChunkDistance; x++)
                for (int y = 0; y < _loadedChunkDistance; y++)
                    for (int z = 0; z < _loadedChunkDistance; z++)
                    {
                        int localX = x;
                        int localY = y;
                        int localZ = z;

                        tasks.Add(Task.Run(() =>
                        {
                            try
                            {
                                int worldX = localX * Chunk.ChunkLength;
                                int worldY = localY * Chunk.ChunkLength;
                                int worldZ = localZ * Chunk.ChunkLength;

                                int chunkIndex = localX + (localY * _loadedChunkDistance) + (localZ * _loadedChunkDistance * _loadedChunkDistance);
                                tempChunks[chunkIndex] = GenChunk(worldX, worldY, worldZ);
                            }
                            catch (Exception e)
                            {

                                if (Debugger.IsAttached)
                                    Debugger.Break();
                                else
                                    Console.WriteLine("Error encountered in worker thread: " + e);
                                throw;
                            }
                        }));
                    }

            Task.WaitAll(tasks.ToArray());

            for (int x = 0; x < _loadedChunkDistance; x++)
                for (int y = 0; y < _loadedChunkDistance; y++)
                    for (int z = 0; z < _loadedChunkDistance; z++)
                    {
                        int worldX = x * Chunk.ChunkLength;
                        int worldY = y * Chunk.ChunkLength;
                        int worldZ = z * Chunk.ChunkLength;
                        int chunkIndex = x + (y * _loadedChunkDistance) + (z * _loadedChunkDistance * _loadedChunkDistance);

                        Chunk chunk = tempChunks[chunkIndex];
                        OpenGLChunkRenderInfo renderInfo = new OpenGLChunkRenderInfo(chunk, new Vector3(worldX, worldY, worldZ));
                        _chunks[chunkIndex] = Tuple.Create(chunk, renderInfo);
                    }
        }

        // Experimental version using buffer mapping. Doesn't work well.
        private unsafe void GenChunksThreaded(OpenGLGraphicsSystem graphicsSystem)
        {
            _chunks = new SpatialStorageBuffer<Tuple<Chunk, OpenGLChunkRenderInfo>>(_loadedChunkDistance);
            List<Task> tasks = new List<Task>();

            int numChunks = _chunks.NumItems;
            uint* vertexBufferIds = stackalloc uint[numChunks];
            GL.GenBuffers(numChunks, vertexBufferIds);
            uint* indexBufferIds = stackalloc uint[numChunks];
            GL.GenBuffers(numChunks, indexBufferIds);

            IntPtr* vertexBufferMappings = stackalloc IntPtr[numChunks];
            IntPtr* indexBufferMappings = stackalloc IntPtr[numChunks];

            for (int i = 0; i < _chunks.NumItems; i++)
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, vertexBufferIds[i]);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(Chunk.BlocksPerChunk * 4 * 6 * SimpleVertex.SizeInBytes), IntPtr.Zero, BufferUsageHint.DynamicDraw);
                Console.WriteLine("Reserving " + Chunk.BlocksPerChunk * 4 * 6 * SimpleVertex.SizeInBytes + " bytes for vertexbuffer id " + vertexBufferIds[i]);

                while (vertexBufferMappings[i] == IntPtr.Zero)
                {
                    vertexBufferMappings[i] = GL.MapBuffer(BufferTarget.ArrayBuffer, BufferAccess.WriteOnly);
                }
                GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBufferIds[i]);
                GL.BufferData(BufferTarget.ElementArrayBuffer, new IntPtr(Chunk.BlocksPerChunk * 6 * 6 * 8), IntPtr.Zero, BufferUsageHint.DynamicDraw);
                Console.WriteLine("Reserving " + Chunk.BlocksPerChunk * 6 * 6 * 8 + " bytes for indexbuffer id " + indexBufferIds[i]);
                indexBufferMappings[i] = GL.MapBuffer(BufferTarget.ElementArrayBuffer, BufferAccess.WriteOnly);
            }

            for (int x = 0; x < _loadedChunkDistance; x++)
                for (int y = 0; y < _loadedChunkDistance; y++)
                    for (int z = 0; z < _loadedChunkDistance; z++)
                    {
                        int localX = x;
                        int localY = y;
                        int localZ = z;

                        tasks.Add(Task.Run(() =>
                        {
                            try
                            {
                                int worldX = localX * Chunk.ChunkLength;
                                int worldY = localY * Chunk.ChunkLength;
                                int worldZ = localZ * Chunk.ChunkLength;

                                int chunkIndex = x + (y * _loadedChunkDistance) + (z * _loadedChunkDistance * _loadedChunkDistance);

                                uint vertexBufferId = vertexBufferIds[chunkIndex];
                                uint indexBufferId = indexBufferIds[chunkIndex];
                                IntPtr vertexBufferMapping = vertexBufferMappings[chunkIndex];
                                IntPtr indexBufferMapping = indexBufferMappings[chunkIndex];

                                Chunk chunk = GenChunk(worldX, worldY, worldZ);

                                OpenGLChunkRenderInfo renderInfo = new OpenGLChunkRenderInfo(
                                    chunk,
                                    vertexBufferId,
                                    indexBufferId,
                                    vertexBufferMapping,
                                    indexBufferMapping,
                                    new Vector3(worldX, worldY, worldZ));
                                _chunks[localX, localY, localZ] = Tuple.Create(chunk, renderInfo);

                                Console.WriteLine("Chunk finished generating: " + renderInfo.ChunkCenter);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Error encountered in worker thread: " + e);
                                throw;
                            }
                        }));

                        Task.WaitAll(tasks.ToArray());
                    }
        }

        public int CurrentLength { get { return _chunks.Length; } }

        private Chunk GenChunk(int worldX, int worldY, int worldZ)
        {
            Chunk chunk = new Chunk();
            float frequency = 1.0f / Chunk.ChunkLength;

            for (int x = 0; x < Chunk.ChunkLength; x++)
            {
                for (int y = 0; y < Chunk.ChunkLength; y++)
                {
                    for (int z = 0; z < Chunk.ChunkLength; z++)
                    {
                        float noiseVal = _noiseGen.GetNoise(((float)(worldX + x)) * frequency, (((float)(worldY + y)) * frequency), (((float)(worldZ + z)) * frequency));
                        if (noiseVal > .61f)
                        {
                            chunk[x, y, z] = new BlockData(BlockType.Stone);
                        }
                        else
                        {
                            chunk[x, y, z] = new BlockData(BlockType.Air);
                        }
                    }
                }
            }

            return chunk;
        }

        public void Render(ref Matrix4x4 viewMatrix)
        {
            BindTexture();

            // Set the Matrix Mode before loop
            GL.MatrixMode(MatrixMode.Modelview);

            // Set the client state used by the render infos
            GL.EnableClientState(ArrayCap.NormalArray);
            GL.EnableClientState(ArrayCap.TextureCoordArray);
            GL.EnableClientState(ArrayCap.VertexArray);

            for (int index = 0; index < _chunks.NumItems; index++)
            {
                if (_chunks[index] != null)
                {
                    OpenGLChunkRenderInfo renderInfo = _chunks[index].Item2;
                    RenderSingleChunk(_chunks[index].Item2, ref viewMatrix);
                }
            }

            UnbindTexture();
        }

        protected void BindTexture()
        {
            GL.Enable(EnableCap.Texture2D);
            _textureBuffer.Bind();
        }

        private void UnbindTexture()
        {
            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private void RenderSingleChunk(OpenGLChunkRenderInfo renderInfo, ref Matrix4x4 viewMatrix)
        {
            Matrix4x4 chunkWorldMatrix = Matrix4x4.CreateTranslation(renderInfo.ChunkCenter);
            Matrix4x4 transformMatrix = chunkWorldMatrix * viewMatrix;
            GLEx.LoadMatrix(ref transformMatrix);
            renderInfo.DrawMeshElements();
        }
    }
}
