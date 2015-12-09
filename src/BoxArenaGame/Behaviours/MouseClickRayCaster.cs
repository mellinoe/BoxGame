using EngineCore;
using EngineCore.Input;
using EngineCore.Physics;
using EngineCore.Services;
using System.Numerics;

namespace GameApplication.Behaviours
{
    public class MouseClickRayCaster : Behaviour
    {
        [AutoInject]
        public IInputService InputService { get; set; }

        [AutoInject]
        public BepuPhysicsSystem Physics { get; set; }

        protected override void Update()
        {
            if (InputService.GetMouseButtonDown(MouseButton.Button1))
            {
                Vector2 clickPos = InputService.MousePosition;
            }
        }
    }
}
