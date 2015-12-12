using EngineCore.Graphics.Formats;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace EngineCore.Graphics
{
    public static class PrimitiveShapes
    {
        private static Lazy<PolyMesh> s_sphereMesh = new Lazy<PolyMesh>(LoadSphereMesh);

        public static PolyMesh Sphere => s_sphereMesh.Value;

        private static PolyMesh LoadSphereMesh()
        {
            Assembly assembly = typeof(PrimitiveShapes).GetTypeInfo().Assembly;
            using (var stream = assembly.GetManifestResourceStream("EngineCore.Graphics.Models.Sphere.obj"))
            {
                return ObjImporter.Import(stream).Result;
            }
        }
    }
}
