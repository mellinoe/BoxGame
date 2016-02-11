using System.Numerics;
using BEPUphysics;

namespace EngineCore.Physics
{
    public class CharacterController : PhysicsComponent
    {
        private BEPUphysics.Character.CharacterController _bepuController
            = new BEPUphysics.Character.CharacterController(jumpSpeed: 7.5f, mass: 1.0f);

        public BEPUphysics.Character.CharacterController BepuController => _bepuController;

        protected internal override void Start()
        {
            _bepuController.Tag = this;
            _bepuController.Body.Position = Transform.Position;
            _bepuController.Body.PositionUpdated += Transform.OnPhysicsUpdate;
        }

        internal override ISpaceObject GetSpaceObject() => _bepuController;

        public void SetMotionDirection(Vector2 motion)
        {
            _bepuController.HorizontalMotionConstraint.MovementDirection = motion;
        }
    }
}
