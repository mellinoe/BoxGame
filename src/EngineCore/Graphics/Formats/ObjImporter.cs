using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;

namespace EngineCore.Graphics.Formats
{
    public class ObjImporter
    {
        public static async Task<PolyMesh> Import(Stream stream)
        {
            StreamReader sr = new StreamReader(stream);

            List<Vector3> positions = new List<Vector3>();
            List<Vector3> normals = new List<Vector3>();
            List<Vector2> textureCoords = new List<Vector2>();
            List<SimpleVertex> vertices = new List<SimpleVertex>();
            List<int> indices = new List<int>();

            int lastIndexUsed = -1;
            Dictionary<ObjVertex, int> objVertexIndices = new Dictionary<ObjVertex, int>();

            while (!sr.EndOfStream)
            {
                string line = await sr.ReadLineAsync();
                if (line.StartsWith("#"))
                {
                    continue;
                }
                else if (line.StartsWith("v "))
                {
                    string[] split = line.Split(' ');
                    positions.Add(ParseVector3FromLine(split));
                }
                else if (line.StartsWith("vt "))
                {
                    string[] split = line.Split(' ');
                    textureCoords.Add(ParseVector2FromLine(split));
                }
                else if (line.StartsWith("vn "))
                {
                    string[] split = line.Split(' ');
                    normals.Add(ParseVector3FromLine(split));
                }
                else if (line.StartsWith("f"))
                {
                    string[] words = line.Split(' ');
                    var v1Split = words[1].Split('/');
                    var v2Split = words[2].Split('/');
                    var v3Split = words[3].Split('/');

                    ObjVertex v1 = ParseObjVertexFromElements(v1Split, positions, normals, textureCoords);
                    ObjVertex v2 = ParseObjVertexFromElements(v2Split, positions, normals, textureCoords);
                    ObjVertex v3 = ParseObjVertexFromElements(v3Split, positions, normals, textureCoords);
                    ObjVertex[] objVertices = new[] { v1, v2, v3 };

                    foreach (ObjVertex objV in objVertices)
                    {
                        int vertexIndex;
                        if (!objVertexIndices.TryGetValue(objV, out vertexIndex))
                        {
                            vertexIndex = ++lastIndexUsed;
                            objVertexIndices.Add(objV, vertexIndex);
                            SimpleVertex vertex = new SimpleVertex(
                                positions[objV.Position],
                                normals[objV.Normal],
                                Color4f.White,
                                textureCoords[objV.TextureCoord]);
                            vertices.Add(vertex);
                            Debug.Assert(vertices.Count == lastIndexUsed + 1);
                        }

                        indices.Add(vertexIndex);
                    }
                }
            }

            return new PolyMesh(vertices, indices);
        }

        private static ObjVertex ParseObjVertexFromElements(string[] elements, List<Vector3> positions, List<Vector3> normals, List<Vector2> textureCoords)
        {
            if (elements.Length != 3)
            {
                throw new NotSupportedException("Can't parse this obj file");
            }

            int posIndex = int.Parse(elements[0]);
            int texIndex = int.Parse(elements[1]);
            int normalIndex = int.Parse(elements[2]);

            return new ObjVertex(posIndex, normalIndex, texIndex);
        }

        private static Vector3 ParseVector3FromLine(string[] words)
        {
            return new Vector3(
                float.Parse(words[1]),
                float.Parse(words[2]),
                float.Parse(words[3]));
        }

        private static Vector2 ParseVector2FromLine(string[] words)
        {
            return new Vector2(
                float.Parse(words[1]),
                float.Parse(words[2]));
        }
    }

    internal struct ObjVertex
    {
        public readonly int Position;
        public readonly int Normal;
        public readonly int TextureCoord;

        public ObjVertex(int pos, int normal, int texCoord)
        {
            Position = pos;
            Normal = normal;
            TextureCoord = texCoord;
        }
    }
}
