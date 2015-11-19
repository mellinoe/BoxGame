using System;

namespace EngineCore.Graphics
{
    public abstract class GraphicsSystem : GameSystem
    {
        public GraphicsSystem(Game game) : base(game) { }

        public event Action<GameWindowResizedEventArgs> OnScreenResized;

        protected void RaiseOnResized(GameWindowResizedEventArgs args)
        {
            if (OnScreenResized != null)
            {
                OnScreenResized(args);
            }
        }

        public abstract void RegisterSimpleMesh(IRenderable renderable, PolyMesh _cubeMesh, Texture2D texture);

        public abstract void RegisterLight(ILightInfo lightInfo);

        public abstract void SetCamera(Camera camera);

        public abstract IWindowInfo WindowInfo { get; }
    }
}
