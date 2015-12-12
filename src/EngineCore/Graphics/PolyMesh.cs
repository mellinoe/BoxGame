using System.Collections.Generic;

namespace EngineCore.Graphics
{
    public class PolyMesh
    {
        private IList<SimpleVertex> _vertices;
        private IList<int> _indices;

        /// <summary>
        /// Returns the list of positions of this mesh's vertices.
        /// </summary>
        public IList<SimpleVertex> Vertices { get { return this._vertices; } }

        /// <summary>
        /// Returns the list of indices of this mesh.
        /// </summary>
        public IList<int> Indices { get { return this._indices; } }

        public PolyMesh(IList<SimpleVertex> vertices, IList<int> indices)
        {
            _vertices = vertices;
            _indices = indices;
        }
    }
}
