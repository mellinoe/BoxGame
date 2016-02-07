using EngineCore.Graphics.Direct3D;
using EngineCore.Graphics.OpenGL;
using System;

namespace EngineCore.Graphics.Gui
{
    internal class ImGuiSystem : GameSystem
    {
        private GraphicsSystem _graphicsSystem;
        private IDrawListRenderer _renderer;

        public ImGuiSystem(Game game) : base(game)
        {
            _graphicsSystem = game.GraphicsSystem;
        }

        public override unsafe void Start()
        {
            if (_graphicsSystem is OpenGLGraphicsSystem)
            {
                _renderer = new OpenGLImGuiRenderer((OpenGLGraphicsSystem)_graphicsSystem);
            }
            else if (_graphicsSystem is SharpDxGraphicsSystem)
            {
                _renderer = new Direct3DImGuiRenderer((SharpDxGraphicsSystem)_graphicsSystem);
            }
        }

        public override void Stop()
        {
            _renderer.Dispose();
        }

        public unsafe override void Update()
        {
        }
    }

    internal unsafe interface IDrawListRenderer : IRenderableObjectInfo2D, IDisposable
    {
    }
}
