using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System;
using System.IO;

namespace EngineCore.Graphics
{
    public class SimpleShader : IDisposable
    {
        private DeviceContext _deviceContext;

        private VertexShader _vertexShader;
        private PixelShader _pixelShader;
        private InputLayout _inputLayout;

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
            string shaderSource = File.ReadAllText(fileName);

            CompilationResult compiledVertexShader = ShaderBytecode.Compile(shaderSource, vsEntryPoint, "vs_5_0", defaultShaderFlags, EffectFlags.None, fileName);
            CompilationResult compiledPixelShader = ShaderBytecode.Compile(shaderSource, psEntryPoint, "ps_5_0", defaultShaderFlags, EffectFlags.None, fileName);

            CoreInitialize(device, deviceContext, compiledVertexShader, compiledPixelShader, inputElements);
        }

        public static SimpleShader CreateFromSource(
            Device device,
            DeviceContext deviceContext,
            string shaderSource,
            string vsEntryPoint,
            string psEntryPoint,
            InputElement[] inputElements)
        {
            CompilationResult compiledVertexShader = ShaderBytecode.Compile(shaderSource, vsEntryPoint, "vs_5_0", defaultShaderFlags, EffectFlags.None);
            CompilationResult compiledPixelShader = ShaderBytecode.Compile(shaderSource, psEntryPoint, "ps_5_0", defaultShaderFlags, EffectFlags.None);

            SimpleShader shader = new SimpleShader();
            shader.CoreInitialize(device, deviceContext, compiledVertexShader, compiledPixelShader, inputElements);
            return shader;
        }

        private SimpleShader() { }

        private void CoreInitialize(Device device, DeviceContext deviceContext, ShaderBytecode compiledVertexShader, ShaderBytecode compiledPixelShader, InputElement[] inputElements)
        {
            _deviceContext = deviceContext;

            _vertexShader = new VertexShader(device, compiledVertexShader);
            _pixelShader = new PixelShader(device, compiledPixelShader);
            _inputLayout = new InputLayout(device, compiledVertexShader.Data, inputElements);
        }

        /// <summary>
        /// Applies this shader by setting the input layout, vertex shader, and pixel shader.
        /// </summary>
        public void ApplyShader()
        {
            _deviceContext.InputAssembler.InputLayout = _inputLayout;
            _deviceContext.VertexShader.Set(_vertexShader);
            _deviceContext.PixelShader.Set(_pixelShader);
        }

        public void Dispose()
        {
            _vertexShader.Dispose();
            _pixelShader.Dispose();
            _inputLayout.Dispose();
        }
    }
}
