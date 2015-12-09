using EngineCore.Services;
using System.Numerics;

namespace EngineCore.Input
{
    public abstract class InputSystem : GameSystem, IInputService, IServiceProvider<IInputService>
    {
        public InputSystem(Game game) : base(game) { }

        public abstract bool GetMouseButtonDown(MouseButton button);
        public abstract bool GetMouseButton(MouseButton button);

        public abstract bool GetKeyDown(KeyCode key);
        public abstract bool GetKey(KeyCode key);

        public abstract Vector2 MousePosition { get; }

        IInputService IServiceProvider<IInputService>.GetService() => this;
    }
}
