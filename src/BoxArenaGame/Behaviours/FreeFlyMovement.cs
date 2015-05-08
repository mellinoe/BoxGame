using EngineCore;
using EngineCore.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameApplication.Behaviours
{
    public class FreeFlyMovement : Behaviour<InputSystem>
    {
        private float previousMouseX;
        private float previousMouseY;
        private float currentYaw;
        private float currentPitch;

        private InputSystem _input;
        private float _speed = 5f;

        private float _turboMultiplier = 3f;

        protected override void Update()
        {
            Vector3 moveDirection = Vector3.Zero;

            if (_input.GetKey(KeyCode.W))
            {
                moveDirection += Transform.Forward;
            }
            if (_input.GetKey(KeyCode.S))
            {
                moveDirection -= Transform.Forward;
            }
            if (_input.GetKey(KeyCode.A))
            {
                moveDirection += Transform.Right;
            }
            if (_input.GetKey(KeyCode.D))
            {
                moveDirection -= Transform.Right;
            }
            if (_input.GetKey(KeyCode.E))
            {
                moveDirection += Transform.Up;
            }
            if (_input.GetKey(KeyCode.Q))
            {
                moveDirection -= Transform.Up;
            }

            if (moveDirection != Vector3.Zero)
            {
                float totalSpeed = _speed * (_input.GetKey(KeyCode.ShiftLeft) ? _turboMultiplier : 1.0f);
                Transform.Position += Vector3.Normalize(moveDirection) * totalSpeed * Time.DeltaTime;
            }

            HandleMouseMovement();
        }


        void HandleMouseMovement()
        {
            float newMouseX = _input.MousePosition.X;
            float newMouseY = _input.MousePosition.Y;

            float xDelta = newMouseX - previousMouseX;
            float yDelta = newMouseY - previousMouseY;

            if (_input.GetMouseButton(MouseButton.Left) || _input.GetMouseButton(MouseButton.Right))
            {
                currentYaw += -xDelta * 0.01f;
                currentPitch += yDelta * 0.01f;

                this.Transform.Rotation = Quaternion.CreateFromYawPitchRoll(currentYaw, currentPitch, 0f);
            }

            this.previousMouseX = newMouseX;
            this.previousMouseY = newMouseY;
        }

        protected override void Initialize(InputSystem system)
        {
            _input = system;
        }

        protected override void Uninitialize(InputSystem system)
        {
            _input = null;
        }
    }
}
