using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelGame.World
{
    public struct BlockData
    {
        public readonly BlockType Type;

        public BlockData(BlockType type)
        {
            Type = type;
        }
    }
}
