using EngineCore.Entities;
using EngineCore.Services;
using System;

namespace EngineCore.Graphics
{
    public abstract class GraphicsSystem : GameSystem, IGraphicsService, IServiceProvider<IGraphicsService>
    {
        public GraphicsSystem(Game game) : base(game)
        {
            var registry = game.ComponentRegistry;
            registry.AddComponentRegistration<BoxRenderer>(
                (br) => RegisterSimpleMesh(br, br._cubeMesh, br._surfaceTexture),
                (br) => { });
            registry.AddComponentRegistration<MeshRenderer>(
                (br) => RegisterSimpleMesh(br, br.Mesh, br.SurfaceTexture),
                (br) => { });
            registry.AddComponentRegistration<LightComponent>
                ((lc) => RegisterLight(lc),
                (lc) => { });
            registry.AddComponentRegistration<Camera>((cam) =>
            {
                SetCamera(cam);
            }, (cam) => { });
        }

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

        IGraphicsService IServiceProvider<IGraphicsService>.GetService()
        {
            return this;
        }

        public abstract IWindowInfo WindowInfo { get; }
    }

    public interface IGraphicsService
    {
        IWindowInfo WindowInfo { get; }
    }
}
