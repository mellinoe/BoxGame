using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;

namespace EngineCore.Graphics.Direct3D
{
    public class ShaderCache : IDisposable
    {
        private readonly Dictionary<string, SimpleShader> _shaders = new Dictionary<string, SimpleShader>();

        public SimpleShader GetNewOrCachedShader(
            Device device,
            DeviceContext context,
            string filename,
            string vsEntryPoint,
            string psEntryPoint,
            InputElement[] inputElements)
        {
            SimpleShader shader;
            if (!_shaders.TryGetValue(filename, out shader))
            {
                shader = new SimpleShader(device, context, filename, vsEntryPoint, psEntryPoint, inputElements);
                _shaders.Add(filename, shader);
            }
            return shader;
        }

        public void Dispose()
        {
            foreach (SimpleShader shader in this._shaders.Values)
            {
                shader.Dispose();
            }
        }
    }
}
