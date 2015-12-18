using System;
using System.Collections.Generic;

namespace EngineCore.Graphics
{
    public class PolyMesh
    {
        private SimpleVertex[] _vertices;
        private int[] _indices;

        /// <summary>
        /// Returns the list of positions of this mesh's vertices.
        /// </summary>
        public SimpleVertex[] Vertices { get { return _vertices; } }

        /// <summary>
        /// Returns the list of indices of this mesh.
        /// </summary>
        public int[] Indices { get { return _indices; } }

        public event Action<PolyMesh> MeshChanged;

        public PolyMesh(SimpleVertex[] vertices, int[] indices)
        {
            _vertices = vertices;
            _indices = indices;
        }

        public void UpdateMesh(SimpleVertex[] vertices, int[] indices)
        {
            _vertices = vertices;
            _indices = indices;
            MeshChanged(this);
        }
    }
}
