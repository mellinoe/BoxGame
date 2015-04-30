using BEPUphysics.Entities.Prefabs;
using EngineCore.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Physics
{
    public class ConvexHullCollider : Collider<ConvexHull>
    {
        private PolyMesh mesh;

        public PolyMesh Mesh
        {
            get { return mesh; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Mesh");
                }

                mesh = value;
                SetPhysicsEntity();
            }
        }

        private void SetPhysicsEntity()
        {
            this.PhysicsEntity = InitPhysicsEntity();
        }

        public ConvexHullCollider(PolyMesh mesh)
        {
            this.mesh = mesh;
        }

        protected override ConvexHull InitPhysicsEntity()
        {
            return new ConvexHull(this.Transform.Position, mesh.Vertices);
        }
    }
}
