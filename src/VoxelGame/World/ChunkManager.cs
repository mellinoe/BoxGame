using Noise;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VoxelGame.World
{
    public class ChunkManager
    {
        private int _loadedChunkDistance = 10;
        private NoiseGen _noiseGen = new NoiseGen(1f, 1f, 4);
        private SpatialStorageBuffer<Chunk> _chunks;

        public ChunkManager()
        {
            _chunks = new SpatialStorageBuffer<Chunk>(_loadedChunkDistance);
            for (int x = 0; x < _loadedChunkDistance; x++)
                for (int y = 0; y < _loadedChunkDistance; y++)
                    for (int z = 0; z < _loadedChunkDistance; z++)
                    {
                        int worldX = x * Chunk.ChunkLength;
                        int worldY = y * Chunk.ChunkLength;
                        int worldZ = z * Chunk.ChunkLength;
                        _chunks[x, y, z] = GenChunk(worldX, worldY, worldZ);
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
                    }
                }
            }

            return chunk;
        }
    }
}
