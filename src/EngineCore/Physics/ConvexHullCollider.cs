using BEPUphysics.Entities.Prefabs;
using EngineCore.Graphics;
using System;
using System.Linq;

namespace EngineCore.Physics
{
    public class ConvexHullCollider : Collider<ConvexHull>
    {
        private PolyMesh _mesh;

        public PolyMesh Mesh
        {
            get { return _mesh; }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException("Mesh");
                }

                _mesh = value;
                SetPhysicsEntity();
            }
        }

        private void SetPhysicsEntity()
        {
            PhysicsEntity = InitPhysicsEntity();
        }

        public ConvexHullCollider(PolyMesh mesh)
        {
            _mesh = mesh;
        }

        protected override ConvexHull InitPhysicsEntity()
        {
            return new ConvexHull(Transform.Position, _mesh.Vertices.Select(sv => sv.Position).ToArray());
        }
    }
}
