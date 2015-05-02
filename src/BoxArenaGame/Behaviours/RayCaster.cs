using EngineCore.Components;
using EngineCore.Physics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApplication.Behaviours
{
    public class RayCaster : Component<BepuPhysicsSystem>
    {
        private BepuPhysicsSystem system;
        protected override void Initialize(BepuPhysicsSystem system)
        {
            this.system = system;
        }

        protected override void Uninitialize(BepuPhysicsSystem system)
        {
            throw new NotImplementedException();
        }
    }
}
