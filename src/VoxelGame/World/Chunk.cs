using System;

namespace VoxelGame.World
{
    public class Chunk
    {
        /// <summary>
        /// Chunk length, in blocks, in the X-axis.
        /// </summary>
        public const int ChunkWidth = 16;

        /// <summary>
        /// Chunk length, in blocks, in the Y-axis.
        /// </summary>
        public const int ChunkHeight = 256;

        /// <summary>
        /// Chunk length, in blocks, in the Z-axis.
        /// </summary>
        public const int ChunkDepth = 16;

        /// <summary>
        /// Length of a single block, in units.
        /// </summary>
        public const float BlockLength = 1f;

        /// <summary>
        /// Total number of blocks per chunk.
        /// </summary>
        public const int BlocksPerChunk = ChunkWidth * ChunkHeight * ChunkDepth;

        private readonly SpatialStorageBuffer<BlockData> _blockData;

        public Chunk()
        {
            _blockData = new SpatialStorageBuffer<BlockData>(ChunkWidth, ChunkHeight, ChunkDepth);
        }

        public BlockData this[int x, int y, int z]
        {
            get
            {
                if (x > ChunkWidth || y > ChunkHeight || z > ChunkDepth)
                {
                    throw new ArgumentOutOfRangeException("A provided coordinate was not valid.");
                }
                else
                {
                    return _blockData[x, y, z];
                }
            }
            set
            {
                if (x > ChunkWidth || y > ChunkHeight || z > ChunkDepth)
                {
                    throw new ArgumentOutOfRangeException("A provided coordinate was not valid.");
                }
                else
                {
                    _blockData[x, y, z] = value;
                }
            }
        }
    }
}
