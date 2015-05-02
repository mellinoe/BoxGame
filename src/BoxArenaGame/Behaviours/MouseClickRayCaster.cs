using BEPUutilities;
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
    public class MouseClickRayCaster : Behaviour<BepuPhysicsSystem, InputSystem>
    {
        private InputSystem _input;
        private BepuPhysicsSystem _physicsSystem;

        protected override void Update()
        {
            if (_input.GetMouseButtonDown(MouseButton.Button1))
            {
                Vector2 clickPos = _input.MousePosition;
            }
        }

        protected override void Initialize(BepuPhysicsSystem system1, InputSystem system2)
        {
            _physicsSystem = system1;
            _input = system2;
        }

        protected override void Uninitialize(BepuPhysicsSystem system1, InputSystem system2)
        {
            _physicsSystem = null;
            _input = null;
        }
    }
}
