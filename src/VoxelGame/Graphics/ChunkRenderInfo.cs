using EngineCore.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using VoxelGame.World;

namespace VoxelGame.Graphics
{
    public class OpenGLChunkRenderInfo : IRenderableObjectInfo
    {
        private Chunk _chunk;
        private Vector3 _chunkCenter;
        private TextureBuffer _textureBuffer;

        public OpenGLChunkRenderInfo(Chunk chunk, TextureBuffer textureBuffer)
        {
            _chunk = chunk;
        }

        public void Render(ref Matrix4x4 lookatMatrix)
        {

        }
    }
}
