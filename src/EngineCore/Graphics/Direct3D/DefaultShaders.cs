using SharpDX.Direct3D11;
using System;
using System.IO;
using System.Reflection;

namespace EngineCore.Graphics.Direct3D
{
    public class DefaultShaders
    { 
        private DeviceContext _context;
        private Device _device;
        private Lazy<Assembly> _thisAssembly = new Lazy<Assembly>(() => typeof(DefaultShaders).GetTypeInfo().Assembly);
        private readonly string _embeddedResourceNamePrefix = "EngineCore.Graphics.Direct3D.DefaultShaders";

        private SimpleShader _lightShader;

        private static readonly InputElement[] _lightShaderInputElements = SimpleVertex.VertexInputLayout;

        public SimpleShader LightShader { get { return _lightShader ?? (_lightShader = LoadFromEmbeddedResource("LightShader.hlsl", "VS", "PS", _lightShaderInputElements)); } }

        public DefaultShaders(Device device, DeviceContext context)
        {
            _device = device;
            _context = context;
        }

        private SimpleShader LoadFromEmbeddedResource(string shaderName, string vsEntryPoint, string psEntryPoint, InputElement[] inputElements)
        {
            string resourceName = string.Format("{0}.{1}", _embeddedResourceNamePrefix, shaderName);
            using (var stream = _thisAssembly.Value.GetManifestResourceStream(resourceName))
            {
                string allText = new StreamReader(stream).ReadToEnd();
                return SimpleShader.CreateFromSource(_device, _context, allText, vsEntryPoint, psEntryPoint, inputElements);
            }
        }
    }
}
