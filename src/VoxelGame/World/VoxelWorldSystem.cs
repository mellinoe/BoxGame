using EngineCore;
using EngineCore.Graphics;
using EngineCore.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoxelGame.World
{
    public class VoxelWorldSystem : GameSystem
    {
        private ChunkManager _chunkManager;
        private OpenGLGraphicsSystem _graphicsSystem;

        public VoxelWorldSystem(Game game) : base(game) { }

        public override void Start()
        {
            _graphicsSystem = (OpenGLGraphicsSystem)Game.GraphicsSystem;
            _chunkManager = new ChunkManager(_graphicsSystem);
            _graphicsSystem.AddSelfManagedRenderable(_chunkManager);
        }

        public override void Stop()
        {

        }

        public override void Update()
        {

        }
    }
}
