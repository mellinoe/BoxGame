using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EngineCore.Graphics;
using VoxelGame.World;
using System.Numerics;

namespace VoxelGame.Graphics
{
    public class ChunkMeshInfo
    {
        private Chunk _chunk;
        private PolyMesh _mesh;

        public ChunkMeshInfo(Chunk chunk)
        {
            _chunk = chunk;
            _mesh = RegenerateMesh();
        }

        public PolyMesh Mesh { get { return _mesh; } }

        private PolyMesh RegenerateMesh()
        {
            List<SimpleVertex> vertices = new List<SimpleVertex>();
            List<int> indices = new List<int>();

            for (int x = 0; x < Chunk.ChunkLength; x++)
            {
                for (int y = 0; y < Chunk.ChunkLength; y++)
                {
                    for (int z = 0; z < Chunk.ChunkLength; z++)
                    {
                        BlockData data = _chunk[x, y, z];
                        AddBlock(vertices, indices, data.Type, new Vector3(x, y, z) * Chunk.BlockLength);

                    }
                }
            }

            return new PolyMesh(vertices.ToArray(), indices.ToArray());
        }

        private void AddBlock(List<SimpleVertex> vertices, List<int> indices, BlockType blockType, Vector3 center)
        {
            // Front
            SimpleVertex v0 = new SimpleVertex(s_cubeFacePositions[0] + center, s_cubeFaceNormals[0], Color4f.Blue, GetTexCoordForSide
        }

        private static readonly Vector3[] s_cubeFacePositions = new Vector3[]
        {
            // Top
            new Vector3(-.5f,.5f,.5f),
            new Vector3(.5f,.5f,.5f),
            new Vector3(.5f,.5f,-.5f),
            new Vector3(-.5f,.5f,-.5f),
            // Bottom
            new Vector3(-.5f,-.5f,.5f),
            new Vector3(.5f,-.5f,.5f),
            new Vector3(.5f,-.5f,-.5f),
            new Vector3(-.5f,-.5f,-.5f),
            // Left
            new Vector3(-.5f,-.5f,.5f),
            new Vector3(-.5f,.5f,.5f),
            new Vector3(-.5f,.5f,-.5f),
            new Vector3(-.5f,-.5f,-.5f),
            // Right
            new Vector3(.5f,-.5f,.5f),
            new Vector3(.5f,.5f,.5f),
            new Vector3(.5f,.5f,-.5f),
            new Vector3(.5f,-.5f,-.5f),
            // Front
            new Vector3(-.5f,.5f,.5f),
            new Vector3(.5f,.5f,.5f),
            new Vector3(.5f,-.5f,.5f),
            new Vector3(-.5f,-.5f,.5f),
            // Back
            new Vector3(-.5f,.5f,-.5f),
            new Vector3(.5f,.5f,-.5f),
            new Vector3(.5f,-.5f,-.5f),
            new Vector3(-.5f,-.5f,-.5f),
        };

        private static readonly Vector3[] s_cubeFaceNormals = new Vector3[]
        {
            // Top
            new Vector3(0,1,0),
            new Vector3(0,1,0),
            new Vector3(0,1,0),
            new Vector3(0,1,0),
            // Bottom
            new Vector3(0,-1,0),
            new Vector3(0,-1,0),
            new Vector3(0,-1,0),
            new Vector3(0,-1,0),
            // Left
            new Vector3(1,0,0),
            new Vector3(1,0,0),
            new Vector3(1,0,0),
            new Vector3(1,0,0),
            // Right
            new Vector3(-1,0,0),
            new Vector3(-1,0,0),
            new Vector3(-1,0,0),
            new Vector3(-1,0,0),
            // Front
            new Vector3(0,0,-1),
            new Vector3(0,0,-1),
            new Vector3(0,0,-1),
            new Vector3(0,0,-1),
            // Back
            new Vector3(0,0,1),
            new Vector3(0,0,1),
            new Vector3(0,0,1),
            new Vector3(0,0,1),
        };

        private static readonly int[] s_cubeIndices = new int[]
        {
            0,1,2,0,2,3,
            4,5,6,4,6,7,
            8,9,10,8,10,11,
            12,13,14,12,14,15,
            16,17,18,16,18,19,
            20,21,22,20,22,23
        };
    }
}
