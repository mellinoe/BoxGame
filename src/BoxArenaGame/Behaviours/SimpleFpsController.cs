using EngineCore;
using EngineCore.Input;
using EngineCore.Physics;
using EngineCore.Services;
using System.Numerics;

namespace GameApplication.Behaviours
{
    public class SimpleFpsController : Behaviour
    {
        [AutoInject]
        public IInputService InputService { get; set; }

        private CharacterController _controller => GameObject.GetComponent<CharacterController>();

        protected override void Update()
        {
            HandleKeyboardMovement();
        }

        private void HandleKeyboardMovement()
        {
            Vector3 movementDirection = new Vector3();
            if (InputService.GetKey(KeyCode.W))
            {
                movementDirection += Transform.Forward;
            }
            if (InputService.GetKey(KeyCode.S))
            {
                movementDirection += -Transform.Forward;
            }
            if (InputService.GetKey(KeyCode.A))
            {
                movementDirection += Transform.Right;
            }
            if (InputService.GetKey(KeyCode.D))
            {
                movementDirection += -Transform.Right;
            }
            if (movementDirection != Vector3.Zero)
            {
                var normalized = Vector3.Normalize(movementDirection);
                normalized.Y = 0f;
                var motionDirection = new Vector2(-normalized.X, normalized.Z);
                _controller.SetMotionDirection(motionDirection * MovementSpeed * Time.DeltaTime);
            }
            else
            {
                _controller.SetMotionDirection(Vector2.Zero);
            }

            if (InputService.GetKeyDown(KeyCode.Space))
            {
                JumpButtonPressed();
            }
        }

        private void JumpButtonPressed()
        {
            _controller.BepuController.Jump();
        }

        public float MovementSpeed { get { return 5.0f; } }
    }
}
