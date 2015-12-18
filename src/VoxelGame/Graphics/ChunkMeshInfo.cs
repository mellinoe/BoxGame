using System;
using System.Collections.Generic;
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
            int numBlocks = Chunk.ChunkLength * Chunk.ChunkLength * Chunk.ChunkLength;
            float density = .7f;
            int verticesPerFace = 4;
            int indicesPerFace = 6;
            int estimatedNumFaces = (int)(numBlocks * density) * 6;
            List<SimpleVertex> vertices = new List<SimpleVertex>(estimatedNumFaces * verticesPerFace);
            List<int> indices = new List<int>(estimatedNumFaces * indicesPerFace);

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
            if (blockType != BlockType.Air)
            {
                AddFace(vertices, indices, blockType, BlockFace.Back, center);
                AddFace(vertices, indices, blockType, BlockFace.Bottom, center);
                AddFace(vertices, indices, blockType, BlockFace.Front, center);
                AddFace(vertices, indices, blockType, BlockFace.Left, center);
                AddFace(vertices, indices, blockType, BlockFace.Right, center);
                AddFace(vertices, indices, blockType, BlockFace.Top, center);
            }
        }

        private void AddFace(List<SimpleVertex> vertices, List<int> indices, BlockType blockType, BlockFace face, Vector3 center)
        {
            SimpleVertex v0 = new SimpleVertex(GetFacePosition(face, 0) + center, GetFaceNormal(face, 0), Color4f.Blue, GetTexCoord(blockType, face, 0));
            SimpleVertex v1 = new SimpleVertex(GetFacePosition(face, 1) + center, GetFaceNormal(face, 1), Color4f.Blue, GetTexCoord(blockType, face, 1));
            SimpleVertex v2 = new SimpleVertex(GetFacePosition(face, 2) + center, GetFaceNormal(face, 2), Color4f.Blue, GetTexCoord(blockType, face, 2));
            SimpleVertex v3 = new SimpleVertex(GetFacePosition(face, 3) + center, GetFaceNormal(face, 3), Color4f.Blue, GetTexCoord(blockType, face, 3));

            int previousIndex = vertices.Count;
            vertices.Add(v0);
            vertices.Add(v1);
            vertices.Add(v2);
            vertices.Add(v3);

            indices.Add(previousIndex + 0);
            indices.Add(previousIndex + 1);
            indices.Add(previousIndex + 2);
            indices.Add(previousIndex + 0);
            indices.Add(previousIndex + 2);
            indices.Add(previousIndex + 3);
        }

        private Vector3 GetFacePosition(BlockFace face, int vertexNum)
        {
            switch (face)
            {
                case BlockFace.Left:
                    return s_leftFacePositions[vertexNum];
                case BlockFace.Right:
                    return s_rightFacePositions[vertexNum];
                case BlockFace.Front:
                    return s_frontFacePositions[vertexNum];
                case BlockFace.Back:
                    return s_backFacePositions[vertexNum];
                case BlockFace.Top:
                    return s_topFacePositions[vertexNum];
                case BlockFace.Bottom:
                    return s_bottomFacePositions[vertexNum];
                default:
                    throw new InvalidOperationException("Invalid face: " + face);
            }
        }

        private Vector3 GetFaceNormal(BlockFace face, int vertexNum)
        {
            switch (face)
            {
                case BlockFace.Left:
                    return s_leftFaceNormals[vertexNum];
                case BlockFace.Right:
                    return s_rightFaceNormals[vertexNum];
                case BlockFace.Front:
                    return s_frontFaceNormals[vertexNum];
                case BlockFace.Back:
                    return s_backFaceNormals[vertexNum];
                case BlockFace.Top:
                    return s_topFaceNormals[vertexNum];
                case BlockFace.Bottom:
                    return s_bottomFaceNormals[vertexNum];
                default:
                    throw new InvalidOperationException("Invalid face: " + face);
            }
        }

        private Vector2 GetTexCoord(BlockType blockType, BlockFace blockFace, int vertexNum)
        {
            return s_cubeFaceTexCoords[vertexNum];
        }

        private enum BlockFace { Left, Right, Front, Back, Top, Bottom }

        private static readonly Vector3[] s_frontFacePositions = new Vector3[]
        {
            // Front
            new Vector3(-.5f,.5f,.5f),
            new Vector3(.5f,.5f,.5f),
            new Vector3(.5f,-.5f,.5f),
            new Vector3(-.5f,-.5f,.5f)
        };

        private static readonly Vector3[] s_topFacePositions = new Vector3[]
        {
            // Top
            new Vector3(-.5f,.5f,.5f),
            new Vector3(.5f,.5f,.5f),
            new Vector3(.5f,.5f,-.5f),
            new Vector3(-.5f,.5f,-.5f)
        };

        private static readonly Vector3[] s_bottomFacePositions = new Vector3[]
        {
            // Bottom
            new Vector3(-.5f,-.5f,.5f),
            new Vector3(.5f,-.5f,.5f),
            new Vector3(.5f,-.5f,-.5f),
            new Vector3(-.5f,-.5f,-.5f)
        };

        private static readonly Vector3[] s_leftFacePositions = new Vector3[]
        {
            // Left
            new Vector3(-.5f,-.5f,.5f),
            new Vector3(-.5f,.5f,.5f),
            new Vector3(-.5f,.5f,-.5f),
            new Vector3(-.5f,-.5f,-.5f)
        };

        private static readonly Vector3[] s_rightFacePositions = new Vector3[]
        {
            // Right
            new Vector3(.5f,-.5f,.5f),
            new Vector3(.5f,.5f,.5f),
            new Vector3(.5f,.5f,-.5f),
            new Vector3(.5f,-.5f,-.5f)
        };

        private static readonly Vector3[] s_backFacePositions = new Vector3[]
        {
            // Back
            new Vector3(-.5f,.5f,-.5f),
            new Vector3(.5f,.5f,-.5f),
            new Vector3(.5f,-.5f,-.5f),
            new Vector3(-.5f,-.5f,-.5f),
        };

        private static readonly Vector3[] s_topFaceNormals = new Vector3[]
        {
            // Top
            new Vector3(0,1,0),
            new Vector3(0,1,0),
            new Vector3(0,1,0),
            new Vector3(0,1,0)
        };

        private static readonly Vector3[] s_bottomFaceNormals = new Vector3[]
        {
            // Bottom
            new Vector3(0,-1,0),
            new Vector3(0,-1,0),
            new Vector3(0,-1,0),
            new Vector3(0,-1,0)
        };

        private static readonly Vector3[] s_leftFaceNormals = new Vector3[]
        {
            // Left
            new Vector3(1,0,0),
            new Vector3(1,0,0),
            new Vector3(1,0,0),
            new Vector3(1,0,0)
        };

        private static readonly Vector3[] s_rightFaceNormals = new Vector3[]
        {
            // Right
            new Vector3(-1,0,0),
            new Vector3(-1,0,0),
            new Vector3(-1,0,0),
            new Vector3(-1,0,0)
        };

        private static readonly Vector3[] s_frontFaceNormals = new Vector3[]
        {
            // Front
            new Vector3(0,0,-1),
            new Vector3(0,0,-1),
            new Vector3(0,0,-1),
            new Vector3(0,0,-1)
        };

        private static readonly Vector3[] s_backFaceNormals = new Vector3[]
        {
            // Back
            new Vector3(0,0,1),
            new Vector3(0,0,1),
            new Vector3(0,0,1),
            new Vector3(0,0,1),
        };

        private static readonly Vector2[] s_cubeFaceTexCoords = new Vector2[]
        {
            new Vector2(0,0),
            new Vector2(1,0),
            new Vector2(1,1),
            new Vector2(0,1)
        };
    }
}
