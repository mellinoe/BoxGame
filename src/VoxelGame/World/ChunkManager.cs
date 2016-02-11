using EngineCore.Graphics.OpenGL;
using Noise;
using System.Numerics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using VoxelGame.Graphics;
using EngineCore.Graphics;
using System.Threading.Tasks;
using System.Diagnostics;
using EngineCore.Physics;
using BEPUphysics.Entities.Prefabs;
using BEPUphysics.CollisionShapes;
using BEPUphysics.CollisionShapes.ConvexShapes;

namespace VoxelGame.World
{
    public class ChunkManager : IRenderableObjectInfo
    {
        private int _loadedChunkDistance = 6;
        private NoiseGen _noiseGen = new NoiseGen(1f, 1f, 4);
        private SpatialStorageBuffer<Tuple<Chunk, OpenGLChunkRenderInfo>> _chunks;

        private static Texture2D s_cubeFaceTextures = Texture2D.CreateFromFile("Textures/CubeFaceTextures.png");
        private TextureBuffer _textureBuffer = new TextureBuffer(s_cubeFaceTextures);

        public ChunkManager(OpenGLGraphicsSystem graphicsSystem, BepuPhysicsSystem physicsSystem)
        {
            Stopwatch sw = Stopwatch.StartNew();

            GenChunksSimpleThreaded(graphicsSystem, physicsSystem);
            AddPhysicsColliders(physicsSystem);

            sw.Stop();
            Console.WriteLine("Total elapsed chunk generation time: " + sw.Elapsed.TotalSeconds);
        }

        private void GenChunksSimple()
        {
            _chunks = new SpatialStorageBuffer<Tuple<Chunk, OpenGLChunkRenderInfo>>(_loadedChunkDistance, 1, _loadedChunkDistance);
            int y = 0;
            for (int x = 0; x < _loadedChunkDistance; x++)
                for (int z = 0; z < _loadedChunkDistance; z++)
                {
                    int worldX = x * Chunk.ChunkWidth;
                    int worldY = y * Chunk.ChunkHeight;
                    int worldZ = z * Chunk.ChunkDepth;
                    Chunk chunk = GenChunk(worldX, worldY, worldZ);
                    OpenGLChunkRenderInfo renderInfo = new OpenGLChunkRenderInfo(chunk, new Vector3(worldX, worldY, worldZ));
                    _chunks[x, y, z] = Tuple.Create(chunk, renderInfo);
                }
        }

        private void GenChunksSimpleThreaded(OpenGLGraphicsSystem graphicsSystem, BepuPhysicsSystem physicsSystem)
        {
            _chunks = new SpatialStorageBuffer<Tuple<Chunk, OpenGLChunkRenderInfo>>(_loadedChunkDistance, 1, _loadedChunkDistance);
            var tempChunks = new SpatialStorageBuffer<Chunk>(_loadedChunkDistance, 1, _loadedChunkDistance);
            List<Task> tasks = new List<Task>();

            int y = 0;
            for (int x = 0; x < _loadedChunkDistance; x++)
                for (int z = 0; z < _loadedChunkDistance; z++)
                {
                    int localX = x;
                    int localY = y;
                    int localZ = z;

                    tasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            int worldX = localX * Chunk.ChunkWidth;
                            int worldY = localY * Chunk.ChunkHeight;
                            int worldZ = localZ * Chunk.ChunkDepth;

                            tempChunks[localX, localY, localZ] = GenChunk(worldX, worldY, worldZ);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Error encountered in worker thread: " + e);
                            throw;
                        }
                    }));
                }

            Task.WaitAll(tasks.ToArray());

            for (int x = 0; x < _loadedChunkDistance; x++)
                for (int z = 0; z < _loadedChunkDistance; z++)
                {
                    int worldX = x * Chunk.ChunkWidth;
                    int worldY = y * Chunk.ChunkHeight;
                    int worldZ = z * Chunk.ChunkDepth;

                    Chunk chunk = tempChunks[x, y, z];
                    OpenGLChunkRenderInfo renderInfo = new OpenGLChunkRenderInfo(chunk, new Vector3(worldX, worldY, worldZ));
                    _chunks[x, y, z] = Tuple.Create(chunk, renderInfo);
                }
        }

        private void AddPhysicsColliders(BepuPhysicsSystem physicsSystem)
        {
            int y = 0;
            for (int x = 0; x < _loadedChunkDistance; x++)
                for (int z = 0; z < _loadedChunkDistance; z++)
                {
                    int worldX = x * Chunk.ChunkWidth;
                    int worldY = y * Chunk.ChunkHeight;
                    int worldZ = z * Chunk.ChunkDepth;

                    Chunk chunk = _chunks[x, y, z].Item1;
                    AddChunkPhysics(physicsSystem, chunk, new Vector3(worldX, worldY, worldZ));
                }
        }

        private void AddChunkPhysics(BepuPhysicsSystem physicsSystem, Chunk chunk, Vector3 chunkOrigin)
        {
            CompoundBody chunkBody;
            List<CompoundShapeEntry> shapes = new List<CompoundShapeEntry>();

            for (int x = 0; x < Chunk.ChunkWidth; x++)
                for (int y = 0; y < Chunk.ChunkHeight; y++)
                    for (int z = 0; z < Chunk.ChunkDepth; z++)
                    {
                        BlockData block = chunk[x, y, z];
                        if (block.Type != BlockType.Air)
                        {
                            shapes.Add(
                                new CompoundShapeEntry(
                                    new BoxShape(
                                        Chunk.BlockLength, Chunk.BlockLength, Chunk.BlockLength),
                                        chunkOrigin + new Vector3(x, y, z) * Chunk.BlockLength));
                        }
                    }

            if (shapes.Count != 0)
            {
                chunkBody = new CompoundBody(shapes);
                physicsSystem.AddOject(chunkBody);
            }
        }

        private Chunk GenChunk(int worldX, int worldY, int worldZ)
        {
            Chunk chunk = new Chunk();
            const float xzFrequency = (1.0f / Chunk.ChunkWidth); ;
            const float yFrequency = xzFrequency;

            for (int x = 0; x < Chunk.ChunkWidth; x++)
            {
                for (int y = 0; y < Chunk.ChunkHeight; y++)
                {
                    for (int z = 0; z < Chunk.ChunkDepth; z++)
                    {
                        float noiseVal = _noiseGen.GetNoise(
                            (worldX + x) * xzFrequency,
                            (worldY + y) * yFrequency,
                            (worldZ + z) * xzFrequency);
                        float secondaryNoise = _noiseGen.GetNoise(
                            (worldX + x) * (xzFrequency / 4f),
                            (worldY + y) * (yFrequency / 4f),
                            (worldZ + z) * (xzFrequency / 4f));

                        BlockType type;
                        if (noiseVal > 0.9f)
                        {
                            type = BlockType.Gravel;
                        }
                        else if (noiseVal > .61f)
                        {
                            if (secondaryNoise > 0.82f)
                            {
                                type = BlockType.Grass;
                            }
                            else if (secondaryNoise > 0.6f)
                            {
                                type = BlockType.Dirt;
                            }
                            else
                            {
                                type = BlockType.Stone;
                            }
                        }
                        else
                        {
                            type = BlockType.Air;
                        }

                        chunk[x, y, z] = new BlockData(type);
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
                    RenderSingleChunk(renderInfo, ref viewMatrix);
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
