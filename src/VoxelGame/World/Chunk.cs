using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VoxelGame.World
{
    public class Chunk
    {
        /// <summary>
        /// Length of a chunk, in number of blocks.
        /// </summary>
        public const int ChunkLength = 16;
        /// <summary>
        /// Length of a single block, in units.
        /// </summary>
        public const float BlockLength = 1f;
        /// <summary>
        /// Total number of blocks per chunk.
        /// </summary>
        public const int BlocksPerChunk = ChunkLength * ChunkLength * ChunkLength;

        private readonly BlockData[] _blockData;

        public Chunk()
        {
            _blockData = new BlockData[ChunkLength * ChunkLength * ChunkLength];
        }

        public Chunk(BlockData[] blockData)
        {
            _blockData = blockData;
        }

        public BlockData this[int x, int y, int z]
        {
            get
            {
                if (x > ChunkLength || y > ChunkLength || z > ChunkLength)
                {
                    throw new ArgumentOutOfRangeException("A provided coordinate was not within the range [0 - " + (ChunkLength - 1) + "]");
                }
                else
                {
                    return _blockData[
                    x
                    + (y * ChunkLength)
                    + (z * ChunkLength * ChunkLength)
                    ];
                }
            }
            set
            {
                if (x > ChunkLength || y > ChunkLength || z > ChunkLength)
                {
                    throw new ArgumentOutOfRangeException("A provided coordinate was not within the range [0 - " + ChunkLength + "]");
                }
                else
                {
                    _blockData[
                        x
                        + (y * ChunkLength)
                        + (z * ChunkLength * ChunkLength)
                        ] = value;
                }
            }
        }
    }
}
