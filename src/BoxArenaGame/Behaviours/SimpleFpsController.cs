using EngineCore;
using EngineCore.Input;
using EngineCore.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameApplication.Behaviours
{
    public class SimpleFpsController : Behaviour<InputSystem>
    {
        private CharacterController _controller;
        private InputSystem _input;

        protected override void Update()
        {
            HandleKeyboardMovement();
        }

        private void HandleKeyboardMovement()
        {
            Vector3 movementDirection = new Vector3();
            if (_input.GetKey(KeyCode.W))
            {
                movementDirection += Transform.Forward;
            }
            if (_input.GetKey(KeyCode.S))
            {
                movementDirection += -Transform.Forward;
            }
            if (_input.GetKey(KeyCode.A))
            {
                movementDirection += Transform.Right;
            }
            if (_input.GetKey(KeyCode.D))
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

            if (_input.GetKeyDown(KeyCode.Space))
            {
                JumpButtonPressed();
            }
        }

        private void JumpButtonPressed()
        {
            _controller.BepuController.Jump();
        }

        public float MovementSpeed { get { return 5.0f; } }

        protected override void Initialize(InputSystem system)
        {
            _input = system;
            _controller = GameObject.GetComponent<CharacterController>();
            //_controller.SetMotionDirection(Transform.Forward);
        }

        protected override void Uninitialize(InputSystem system)
        {
            _input = null;
        }
    }
}
