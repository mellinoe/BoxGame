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

            //_chunks = new SpatialStorageBuffer<Tuple<Chunk, OpenGLChunkRenderInfo>>(_loadedChunkDistance);
            //for (int x = 0; x < _loadedChunkDistance; x++)
            //    for (int y = 0; y < _loadedChunkDistance; y++)
            //        for (int z = 0; z < _loadedChunkDistance; z++)
            //        {
            //            int worldX = x * Chunk.ChunkLength;
            //            int worldY = y * Chunk.ChunkLength;
            //            int worldZ = z * Chunk.ChunkLength;
            //            Chunk chunk = GenChunk(worldX, worldY, worldZ);
            //            OpenGLChunkRenderInfo renderInfo = new OpenGLChunkRenderInfo(chunk);
            //            _chunks[x, y, z] = Tuple.Create(chunk, renderInfo);
            //        }

            _chunks = new SpatialStorageBuffer<Tuple<Chunk, OpenGLChunkRenderInfo>>(_loadedChunkDistance);
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

                                    Chunk chunk = GenChunk(worldX, worldY, worldZ);

                                    if (GraphicsContext.CurrentContext == null)
                                    {
                                        GraphicsContext context = new GraphicsContext(GraphicsMode.Default, graphicsSystem.Window.WindowInfo);
                                    }

                                    OpenGLChunkRenderInfo renderInfo = new OpenGLChunkRenderInfo(chunk, new Vector3(worldX, worldY, worldZ));
                                    _chunks[localX, localY, localZ] = Tuple.Create(chunk, renderInfo);
                                }
                                catch (Exception e)
                                {
                                    Console.WriteLine("Error encountered in worker thread: " + e);
                                    throw;
                                }
                            }));
                    }

            //Task.WaitAll(tasks.ToArray());

            sw.Stop();
            Console.WriteLine("Total elapsed chunk generation time: " + sw.Elapsed.TotalSeconds);
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

            for (int index = 0; index < _chunks.NumItems; index++)
            {
                if (_chunks[index] != null)
                {
                    OpenGLChunkRenderInfo renderInfo = _chunks[index].Item2;
                    RenderSingleChunk(_chunks[index].Item2, renderInfo.ChunkCenter * Chunk.ChunkLength, ref viewMatrix);
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

        private void RenderSingleChunk(OpenGLChunkRenderInfo renderInfo, Vector3 chunkCenter, ref Matrix4x4 viewMatrix)
        {
            Matrix4x4 chunkWorldMatrix = Matrix4x4.CreateTranslation(chunkCenter);
            Matrix4x4 transformMatrix = chunkWorldMatrix * viewMatrix;
            GLEx.LoadMatrix(ref transformMatrix);
            renderInfo.DrawMeshElements();
        }
    }
}
