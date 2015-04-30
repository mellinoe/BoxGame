using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Graphics
{
    public class ShaderCache : IDisposable
    {
        private Dictionary<string, SimpleShader> shaders = new Dictionary<string, SimpleShader>();

        public SimpleShader GetNewOrCachedShader(
            Device device,
            DeviceContext context,
            string filename,
            string vsEntryPoint,
            string psEntryPoint,
            InputElement[] inputElements)
        {
            SimpleShader shader;
            if (!shaders.TryGetValue(filename, out shader))
            {
                shader = new SimpleShader(device, context, filename, vsEntryPoint, psEntryPoint, inputElements);
                shaders.Add(filename, shader);
            }
            return shader;
        }

        public void Dispose()
        {
            foreach (SimpleShader shader in this.shaders.Values)
            {
                shader.Dispose();
            }
        }
    }
}
