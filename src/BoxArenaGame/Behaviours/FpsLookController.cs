﻿using EngineCore;
using EngineCore.Components;
using EngineCore.Input;
using EngineCore.Physics;
using EngineCore.Services;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameApplication.Behaviours
{
    public class FpsLookController : Behaviour
    {
        private CharacterController cc;
        private float previousMouseX;
        private float previousMouseY;
        private float currentYaw;
        private float currentPitch;

        public FpsLookController(Transform trackedTransform)
        {
            Tracked = trackedTransform;
        }

        [AutoInject]
        public IInputService Input { get; set; }

        public Transform Tracked { get; set; }

        protected override void Start()
        {
            cc = Tracked.GameObject.GetComponent<CharacterController>();
            cc.BepuController.ViewDirection = Transform.Forward;
        }

        protected override void Update()
        {
            Transform.Position = Tracked.Position;
            HandleMouseMovement();
        }

        void HandleMouseMovement()
        {
            float newMouseX = Input.MousePosition.X;
            float newMouseY = Input.MousePosition.Y;

            float xDelta = newMouseX - previousMouseX;
            float yDelta = newMouseY - previousMouseY;

            if ((Input.GetMouseButton(MouseButton.Left) || Input.GetMouseButton(MouseButton.Right)) && !ImGui.IsMouseHoveringAnyWindow())
            {
                currentYaw += -xDelta * 0.01f;
                currentPitch += yDelta * 0.01f;

                Transform.Rotation = Quaternion.CreateFromYawPitchRoll(currentYaw, currentPitch, 0f);
                cc.BepuController.ViewDirection = Transform.Forward;
            }

            previousMouseX = newMouseX;
            previousMouseY = newMouseY;
        }
    }
}
