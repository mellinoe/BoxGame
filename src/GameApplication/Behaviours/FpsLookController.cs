using EngineCore;
using EngineCore.Components;
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
    public class FpsLookController : Behaviour<InputSystem>
    {
        public Transform Tracked { get; set; }
        private CharacterController cc;

        private float previousMouseX;
        private float previousMouseY;
        private float currentYaw;
        private float currentPitch;
        private InputSystem _input;
        protected override void Update()
        {
            this.Transform.Position = Tracked.Position;
            this.cc = Tracked.GameObject.GetComponent<CharacterController>();
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
                this.cc.BepuController.ViewDirection = this.Transform.Forward;
            }

            this.previousMouseX = newMouseX;
            this.previousMouseY = newMouseY;
        }

        protected override void Initialize(InputSystem system)
        {
            _input = system;
            //cc = Tracked.GameObject.GetComponent<CharacterController>();
        }

        protected override void Uninitialize(InputSystem system)
        {
            _input = null;
        }
    }
}
