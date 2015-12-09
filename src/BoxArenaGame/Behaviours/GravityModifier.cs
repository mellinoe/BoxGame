using EngineCore;
using EngineCore.Input;
using EngineCore.Physics;
using EngineCore.Services;
using System.Numerics;

namespace BoxArenaGame.Behaviours
{
    public class GravityModifier : Behaviour
    {
        [AutoInject]
        public IInputService InputService { get; set; }

        [AutoInject]
        public BepuPhysicsSystem Physics { get; set; }

        protected override void Update()
        {
            if (InputService.GetKeyDown(KeyCode.F10))
            {
                ChangeGravity(-1.0f);
            }
            else if (InputService.GetKeyDown(KeyCode.F9))
            {
                ChangeGravity(1.0f);
            }
        }

        private void ChangeGravity(float amount)
        {
            Physics.Gravity = Physics.Gravity + (Vector3.UnitY * amount);
        }
    }
}
