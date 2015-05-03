using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace VoxelGame.World
{
    public class Chunk : IDictionary<Vector3, BlockData>
    {
        public const int ChunkLength = 16;
        public const float BlockLength = 1f;

        private readonly BlockData[] _blockData;

        public Chunk()
        {
            _blockData = new BlockData[ChunkLength * ChunkLength * ChunkLength];
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
