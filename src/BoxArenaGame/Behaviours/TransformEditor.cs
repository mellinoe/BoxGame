using BEPUphysics;
using EngineCore;
using EngineCore.Graphics;
using EngineCore.Input;
using EngineCore.Physics;
using EngineCore.Services;
using System;
using EngineCore.Components;
using ImGuiNET;
using System.Numerics;
using EngineCore.Utility;

namespace BoxArenaGame.Behaviours
{
    public class TransformEditor : Behaviour
    {
        private Transform _objectBeingEdited;

        [AutoInject]
        public IInputService InputService { get; set; }

        [AutoInject]
        public IGraphicsService GraphicsService { get; set; }

        [AutoInject]
        public BepuPhysicsSystem Physics { get; set; }

        protected override void Update()
        {
            if (InputService.GetKeyDown(KeyCode.E))
            {
                RayCastResult rcr;
                if (Physics.RayCast(new BEPUutilities.Ray(Transform.Position + Transform.Forward * 2.25f, Transform.Forward), out rcr))
                {
                    GameObject hitGo;
                    if (rcr.HitObject.TryGetGameObject(out hitGo))
                    {
                        Console.WriteLine("RayCast hit object " + hitGo);
                        _objectBeingEdited = hitGo.Transform;
                    }
                }
            }

            DrawTransformEditor();
        }

        private void DrawTransformEditor()
        {
            if (ImGui.BeginWindow("Transform Editor"))
            {
                if (_objectBeingEdited != null)
                {
                    Vector3 position = _objectBeingEdited.Position;
                    ImGui.DragVector3("Position", ref position, -1000, 1000, dragSpeed: 0.5f, displayFormat: "%.3f");
                    if (position != _objectBeingEdited.Position)
                    {
                        _objectBeingEdited.Position = position;
                    }

                    Vector3 eulerAngles = MathUtil.RadiansToDegrees(_objectBeingEdited.Rotation.GetEulerAngles());
                    Vector3 editedEulerAngles = eulerAngles;
                    ImGui.DragVector3("Rotation", ref editedEulerAngles, -360, 360, dragSpeed: 0.5f, displayFormat: "%.3f");
                    if (eulerAngles != editedEulerAngles)
                    {
                        editedEulerAngles = MathUtil.DegreesToRadians(editedEulerAngles);
                        _objectBeingEdited.Rotation = Quaternion.CreateFromYawPitchRoll(editedEulerAngles.Y, editedEulerAngles.X, editedEulerAngles.Z);
                    }

                    Vector3 scale = _objectBeingEdited.Scale;
                    ImGui.DragVector3("Scale", ref scale, -100, 100, dragSpeed: 0.5f, displayFormat: "%.3f");
                    if (scale != _objectBeingEdited.Scale)
                    {
                        _objectBeingEdited.Scale = scale;
                    }
                }
            }
            ImGui.EndWindow();
        }
    }
}
