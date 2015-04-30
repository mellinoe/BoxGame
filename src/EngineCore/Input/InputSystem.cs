using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace EngineCore.Input
{
    public abstract class InputSystem : GameSystem
    {
        public InputSystem(Game game) : base(game) { }

        public abstract bool GetMouseButtonDown(MouseButton button);
        public abstract bool GetMouseButton(MouseButton button);

        public abstract bool GetKeyDown(KeyCode key);
        public abstract bool GetKey(KeyCode key);

        public abstract Vector2 MousePosition { get; }
    }
}
