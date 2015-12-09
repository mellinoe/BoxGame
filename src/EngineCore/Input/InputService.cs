using System.Numerics;

namespace EngineCore.Input
{
    public interface IInputService
    {
        bool GetMouseButtonDown(MouseButton button);
        bool GetMouseButton(MouseButton button);

        bool GetKeyDown(KeyCode key);
        bool GetKey(KeyCode key);

        Vector2 MousePosition { get; }
    }
}
