using EngineCore.Graphics.Formats;
using System;
using System.Reflection;

namespace EngineCore.Graphics
{
    public static class Primitives
    {
        private static Lazy<PolyMesh> s_sphereMesh = new Lazy<PolyMesh>(() => LoadEmbeddedMesh("Sphere.obj"));
        private static Lazy<PolyMesh> s_teapotMesh = new Lazy<PolyMesh>(() => LoadEmbeddedMesh("Teapot.obj"));
        private static Lazy<Texture2D> s_solidWhiteTexture = new Lazy<Texture2D>(LoadWhiteTexture);

        public static PolyMesh Sphere => s_sphereMesh.Value;

        public static PolyMesh Teapot => s_teapotMesh.Value;

        public static Texture2D SolidWhiteTexture => s_solidWhiteTexture.Value;

        private static Texture2D LoadWhiteTexture()
        {
            Assembly assembly = typeof(Primitives).GetTypeInfo().Assembly;
            using (var stream = assembly.GetManifestResourceStream("EngineCore.Graphics.Models.WhiteSquare.png"))
            {
                return new ImageProcessorTexture2D(stream);
            }
        }

        private static PolyMesh LoadEmbeddedMesh(string name)
        {
            Assembly assembly = typeof(Primitives).GetTypeInfo().Assembly;
            using (var stream = assembly.GetManifestResourceStream($"EngineCore.Graphics.Models.{name}"))
            {
                return ObjImporter.Import(stream).Result;
            }
        }
    }
}
