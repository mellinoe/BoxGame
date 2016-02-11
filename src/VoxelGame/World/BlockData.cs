namespace VoxelGame.World
{
    public struct BlockData
    {
        public readonly BlockType Type;

        public BlockData(BlockType type)
        {
            Type = type;
        }

        public override string ToString() => Type.ToString();
    }
}
