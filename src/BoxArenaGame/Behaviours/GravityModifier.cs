using EngineCore;
using EngineCore.Input;
using EngineCore.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace BoxArenaGame.Behaviours
{
    public class GravityModifier : Behaviour<BepuPhysicsSystem, InputSystem>
    {
        private InputSystem _input;
        private BepuPhysicsSystem _physics;

        protected override void Initialize(BepuPhysicsSystem system1, InputSystem system2)
        {
            _physics = system1;
            _input = system2;
        }

        protected override void Uninitialize(BepuPhysicsSystem system1, InputSystem system2)
        {
            _physics = null;
            _input = null;
        }

        protected override void Update()
        {
            if (_input.GetKeyDown(KeyCode.F10))
            {
                ChangeGravity(-1.0f);
            }
            else if (_input.GetKeyDown(KeyCode.F9))
            {
                ChangeGravity(1.0f);
            }
        }

        private void ChangeGravity(float amount)
        {
            _physics.Gravity = _physics.Gravity + (Vector3.UnitY * amount);
        }
    }
}
