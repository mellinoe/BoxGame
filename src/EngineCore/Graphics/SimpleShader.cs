using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Graphics
{
    public class SimpleShader : IDisposable
    {
        private Device device;
        private DeviceContext deviceContext;

        private VertexShader vertexShader;
        private PixelShader pixelShader;
        private InputLayout inputLayout;

        private const ShaderFlags defaultShaderFlags
#if DEBUG
 = ShaderFlags.Debug | ShaderFlags.SkipOptimization;
#else
            = ShaderFlags.None;
#endif

        public SimpleShader(
            Device device,
            DeviceContext deviceContext,
            string fileName,
            string vsEntryPoint,
            string psEntryPoint,
            InputElement[] inputElements)
        {
            var compiledVertexShader = ShaderBytecode.CompileFromFile(fileName, vsEntryPoint, "vs_5_0", defaultShaderFlags);
            var compiledPixelShader = ShaderBytecode.CompileFromFile(fileName, psEntryPoint, "ps_5_0", defaultShaderFlags);
            CoreInitialize(device, deviceContext, compiledVertexShader, compiledPixelShader, inputElements);
        }

        public SimpleShader(
            Device device,
            DeviceContext context,
            string compiledVertexShaderFileName,
            string compiledPixelShaderFileName,
            InputElement[] inputElements)
        {
            var compiledVertexShader = ShaderBytecode.FromFile(compiledVertexShaderFileName);
            var compiledPixelShader = ShaderBytecode.FromFile(compiledPixelShaderFileName);
            CoreInitialize(device, context, compiledVertexShader, compiledPixelShader, inputElements);
        }

        public SimpleShader(
            Device device,
            DeviceContext deviceContext,
            ShaderBytecode compiledVertexShader,
            ShaderBytecode compiledPixelShader,
            InputElement[] inputElements)
        {
            CoreInitialize(device, deviceContext, compiledVertexShader, compiledPixelShader, inputElements);
        }

        private void CoreInitialize(Device device, DeviceContext deviceContext, ShaderBytecode compiledVertexShader, ShaderBytecode compiledPixelShader, InputElement[] inputElements)
        {
            this.device = device;
            this.deviceContext = deviceContext;

            this.vertexShader = new VertexShader(device, compiledVertexShader);
            this.pixelShader = new PixelShader(device, compiledPixelShader);

            var shaderSignature = ShaderSignature.GetInputSignature(compiledVertexShader);
            this.inputLayout = new InputLayout(device, shaderSignature, inputElements);
        }

        /// <summary>
        /// Applies this shader by setting the input layout, vertex shader, and pixel shader.
        /// </summary>
        public void ApplyShader()
        {
            deviceContext.InputAssembler.InputLayout = this.inputLayout;
            deviceContext.VertexShader.Set(this.vertexShader);
            deviceContext.PixelShader.Set(this.pixelShader);
        }

        public void Dispose()
        {
            this.vertexShader.Dispose();
            this.pixelShader.Dispose();
            this.inputLayout.Dispose();
        }
    }
}
