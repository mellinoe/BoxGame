using EngineCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Physics
{
    public class CharacterController : Component<BepuPhysicsSystem>
    {
        private BEPUphysics.Character.CharacterController bepuController
            = new BEPUphysics.Character.CharacterController(jumpSpeed: 7.5f, mass: 1.0f);

        public BEPUphysics.Character.CharacterController BepuController
        {
            get { return bepuController; }
        }

        protected override void Initialize(BepuPhysicsSystem system)
        {
            system.AddOject(bepuController, this.GameObject);
            bepuController.Body.PositionUpdated += Transform.OnPhysicsUpdate;
        }

        public void SetMotionDirection(Vector2 motion)
        {
            bepuController.HorizontalMotionConstraint.MovementDirection = motion;
        }

        protected override void Uninitialize(BepuPhysicsSystem system)
        {
            system.RemoveObject(bepuController);
            bepuController.Body.PositionUpdated -= Transform.OnPhysicsUpdate;
        }
    }
}
