using EngineCore;
using EngineCore.Input;
using EngineCore.Services;
using System.Numerics;

namespace GameApplication.Behaviours
{
    public class FreeFlyMovement : Behaviour
    {
        [AutoInject]
        public IInputService InputService { get; set; }

        private float previousMouseX;
        private float previousMouseY;
        private float currentYaw;
        private float currentPitch;

        private float _speed = 5f;

        private float _turboMultiplier = 3f;

        protected override void Update()
        {
            Vector3 moveDirection = Vector3.Zero;

            if (InputService.GetKey(KeyCode.W))
            {
                moveDirection += Transform.Forward;
            }
            if (InputService.GetKey(KeyCode.S))
            {
                moveDirection -= Transform.Forward;
            }
            if (InputService.GetKey(KeyCode.A))
            {
                moveDirection += Transform.Right;
            }
            if (InputService.GetKey(KeyCode.D))
            {
                moveDirection -= Transform.Right;
            }
            if (InputService.GetKey(KeyCode.E))
            {
                moveDirection += Transform.Up;
            }
            if (InputService.GetKey(KeyCode.Q))
            {
                moveDirection -= Transform.Up;
            }

            if (moveDirection != Vector3.Zero)
            {
                float totalSpeed = _speed * (InputService.GetKey(KeyCode.ShiftLeft) ? _turboMultiplier : 1.0f);
                Transform.Position += Vector3.Normalize(moveDirection) * totalSpeed * Time.DeltaTime;
            }

            HandleMouseMovement();
        }


        void HandleMouseMovement()
        {
            float newMouseX = InputService.MousePosition.X;
            float newMouseY = InputService.MousePosition.Y;

            float xDelta = newMouseX - previousMouseX;
            float yDelta = newMouseY - previousMouseY;

            if (InputService.GetMouseButton(MouseButton.Left) || InputService.GetMouseButton(MouseButton.Right))
            {
                currentYaw += -xDelta * 0.01f;
                currentPitch += yDelta * 0.01f;

                this.Transform.Rotation = Quaternion.CreateFromYawPitchRoll(currentYaw, currentPitch, 0f);
            }

            this.previousMouseX = newMouseX;
            this.previousMouseY = newMouseY;
        }
    }
}
