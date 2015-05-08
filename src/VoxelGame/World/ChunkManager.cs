using EngineCore.Graphics.OpenGL;
using Noise;
using System.Numerics;
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
        private SpatialStorageBuffer<Tuple<Chunk, OpenGLChunkRenderInfo>> _chunks;

        private static readonly Bitmap s_cubeFaceTextures = new Bitmap("Textures/CubeFaceTextures.png");
        private TextureBuffer _textureBuffer = new TextureBuffer(s_cubeFaceTextures);


        public ChunkManager()
        {
            _chunks = new SpatialStorageBuffer<Tuple<Chunk, OpenGLChunkRenderInfo>>(_loadedChunkDistance);
            for (int x = 0; x < _loadedChunkDistance; x++)
                for (int y = 0; y < _loadedChunkDistance; y++)
                    for (int z = 0; z < _loadedChunkDistance; z++)
                    {
                        int worldX = x * Chunk.ChunkLength;
                        int worldY = y * Chunk.ChunkLength;
                        int worldZ = z * Chunk.ChunkLength;
                        Chunk chunk = GenChunk(worldX, worldY, worldZ);
                        OpenGLChunkRenderInfo renderInfo = new OpenGLChunkRenderInfo(chunk);
                        _chunks[x, y, z] = Tuple.Create(chunk, renderInfo);
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
                        float noiseVal = _noiseGen.GetNoise(((float)(worldX + x)) * frequency, (((float)(worldY + y)) * frequency), (((float)(worldZ + z)) * frequency));
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

        public void Render(ref Matrix4x4 viewMatrix)
        {
            BindTexture();

            // Set the Matrix Mode before loop
            GL.MatrixMode(MatrixMode.Modelview);

            for (int x = 0; x < _loadedChunkDistance; x++)
                for (int y = 0; y < _loadedChunkDistance; y++)
                    for (int z = 0; z < _loadedChunkDistance; z++)
                    {
                        RenderSingleChunk(_chunks[x, y, z].Item2, new Vector3(x, y, z) * Chunk.ChunkLength, ref viewMatrix);
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

        private void RenderSingleChunk(OpenGLChunkRenderInfo renderInfo, Vector3 chunkCenter, ref Matrix4x4 viewMatrix)
        {
            Matrix4x4 chunkWorldMatrix = Matrix4x4.CreateTranslation(chunkCenter);
            Matrix4x4 transformMatrix = chunkWorldMatrix * viewMatrix;
            GLEx.LoadMatrix(ref transformMatrix);
            renderInfo.DrawMeshElements();
        }
    }
}
