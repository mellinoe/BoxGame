using EngineCore.Graphics.OpenGL;
using Noise;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using VoxelGame.Graphics;

namespace VoxelGame.World
{
    public class ChunkManager : IRenderableObjectInfo
    {
        private int _loadedChunkDistance = 10;
        private NoiseGen _noiseGen = new NoiseGen(1f, 1f, 4);
        private SpatialStorageBuffer<Tuple<Chunk, ChunkMeshInfo>> _chunks;

        private static readonly Bitmap s_cubeFaceTextures = new Bitmap("Textures/CubeFaceTextures.png");
        private TextureBuffer _textureBuffer = new TextureBuffer(s_cubeFaceTextures);


        public ChunkManager()
        {
            _chunks = new SpatialStorageBuffer<Tuple<Chunk, ChunkMeshInfo>>(_loadedChunkDistance);
            for (int x = 0; x < _loadedChunkDistance; x++)
                for (int y = 0; y < _loadedChunkDistance; y++)
                    for (int z = 0; z < _loadedChunkDistance; z++)
                    {
                        int worldX = x * Chunk.ChunkLength;
                        int worldY = y * Chunk.ChunkLength;
                        int worldZ = z * Chunk.ChunkLength;
                        Chunk chunk = GenChunk(worldX, worldY, worldZ);
                        ChunkMeshInfo mesh = new ChunkMeshInfo(chunk);
                        _chunks[x, y, z] = Tuple.Create(chunk, mesh);
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
                        float noiseVal = _noiseGen.GetNoise((worldX + x) * frequency, ((worldY + y) * frequency), ((worldZ + z) * frequency));
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

        public void Render(ref System.Numerics.Matrix4x4 lookatMatrix)
        {
            for (int x = 0; x < _loadedChunkDistance; x++)
                for (int y = 0; y < _loadedChunkDistance; y++)
                    for (int z = 0; z < _loadedChunkDistance; z++)
                    {

                    }
        }
    }
}
