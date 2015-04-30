using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Numerics;
using Vector2 = System.Numerics.Vector2;
using Vector3 = System.Numerics.Vector3;
using Matrix4x4 = System.Numerics.Matrix4x4;
using EngineCore.Entities;
using System.Collections.Immutable;

namespace EngineCore.Graphics
{
    public class SimpleRenderer
    {
        #region Private Fields
        // Direct3D Device and State Objects
        private SharpDX.Direct3D11.Device device;
        private DeviceContext deviceContext;
        private SharpDX.Toolkit.Graphics.GraphicsDevice __2dGraphicsDevice;
        private SwapChain swapChain;
        private RenderTargetView backBufferView;
        private DepthStencilView depthStencilView;
        private DepthStencilState depthState;
        private SamplerState samplerState;
        private BlendState blendState;
        private RasterizerState rasterizerState;

        // Matrix Objects
        private SharpDX.Direct3D11.Buffer worldViewProjectionMatrixBuffer;
        private Matrix4x4 projectionMatrix;
        private Matrix4x4 viewMatrix;

        // Ambient Color Properties
        private SharpDX.Direct3D11.Buffer ambientLightBuffer;
        private Color4f ambientColor;

        // Misc
        private ImmutableArray<IRenderable> renderables;
        private Camera camera;
        private RenderForm renderForm;
        private bool needsResizing = false;

        #endregion Private Fields

        #region Public Accessors And Methods
        public RenderForm Form { get { return renderForm; } }

        public DeviceContext DeviceContext { get { return deviceContext; } }

        public SwapChain SwapChain { get { return swapChain; } }

        public SharpDX.Direct3D11.Device Device { get { return device; } }

        public Color4f AmbientColor
        {
            get { return ambientColor; }
            set
            {
                ambientColor = value;
                OnAmbientColorChanged();
            }
        }

        public Camera MainCamera
        {
            get { return camera; }
            set { camera = value; }
        }

        public void AddRenderable(IRenderable renderable)
        {
            this.renderables = this.renderables.Add(renderable);
        }

        public void RemoveRenderable(IRenderable renderable)
        {
            this.renderables = this.renderables.Remove(renderable);
        }

        public IReadOnlyCollection<IRenderable> Renderables { get { return renderables; } }
        public DirectionalLight Light { get; set; }

#if TEXT_RENDERER
        public SimpleText TextRenderer { get; private set; }
#endif
        #endregion Public Accessors And Methods

        #region Constructor
        public SimpleRenderer()
        {
            string title = "SharpDX Renderer (SIMD " + (Vector.IsHardwareAccelerated ? "Enabled" : "Disabled") + ")";
            title += Environment.Is64BitProcess ? " 64-bit" : " 32-bit";
            this.renderForm = new RenderForm(title);
            this.renderables = ImmutableArray<IRenderable>.Empty;
            CreateAndInitializeDevice();
            renderForm.Show();
            AmbientColor = new Color4f(.25f, .25f, .25f, 1);
#if TEXT_RENDERER
            this.TextRenderer = new SimpleText(this.Get2DGraphicsDevice(), "Fonts/textfont.dds");
#endif
        }
        private void CreateAndInitializeDevice()
        {
            var swapChainDescription = new SwapChainDescription()
            {
                BufferCount = 1,
                IsWindowed = true,
                ModeDescription = new ModeDescription(renderForm.ClientSize.Width, renderForm.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                OutputHandle = renderForm.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };
            DeviceCreationFlags flags = DeviceCreationFlags.None;
#if DEBUG
            flags |= DeviceCreationFlags.Debug;
#endif
            SharpDX.Direct3D11.Device.CreateWithSwapChain(SharpDX.Direct3D.DriverType.Hardware, flags, swapChainDescription, out device, out swapChain);
            deviceContext = device.ImmediateContext;
#if TEXT_RENDERER
            __2dGraphicsDevice = SharpDX.Toolkit.Graphics.GraphicsDevice.New(device);
#endif
            var factory = SwapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(renderForm.Handle, WindowAssociationFlags.IgnoreAll);

            SetRasterizerState();
            SetDepthBufferState();
            SetSamplerState();
            SetBlendState();
            CreateConstantBuffers();
            renderForm.Resize += OnFormResized;
            PerformResizing();
            SetRegularTargets();
        }
        #endregion Constructor

        #region Private/Internal Implementation
        private void OnAmbientColorChanged()
        {
            var bufferStruct = new AmbientLightBuffer(ambientColor);
            deviceContext.UpdateSubresource<AmbientLightBuffer>(ref bufferStruct, ambientLightBuffer);
            deviceContext.PixelShader.SetConstantBuffer(1, ambientLightBuffer);
        }

        private void CreateConstantBuffers()
        {
            this.worldViewProjectionMatrixBuffer = new SharpDX.Direct3D11.Buffer(
                device,
                Marshal.SizeOf(typeof(MatricesBuffer)),
                ResourceUsage.Default,
                BindFlags.ConstantBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0);

            this.ambientLightBuffer = new SharpDX.Direct3D11.Buffer(
                device,
                Marshal.SizeOf(typeof(AmbientLightBuffer)),
                ResourceUsage.Default,
                BindFlags.ConstantBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0);
        }

        private void SetRegularTargets()
        {
            // Setup targets and viewport for rendering
            DeviceContext.Rasterizer.SetViewport(0, 0, renderForm.ClientSize.Width, renderForm.ClientSize.Height);
            DeviceContext.OutputMerger.SetTargets(depthStencilView, backBufferView);
        }

        private void SetBlendState()
        {
            blendState = new BlendState(Device, BlendStateDescription.Default());
        }

        private void SetDepthBufferState()
        {
            DepthStencilStateDescription description = DepthStencilStateDescription.Default();
            description.DepthComparison = Comparison.LessEqual;
            description.IsDepthEnabled = true;

            depthState = new DepthStencilState(device, description);
        }

        private void SetRasterizerState()
        {
            rasterizerState = new RasterizerState(device, RasterizerStateDescription.Default());
            var desc = rasterizerState.Description;
            desc.CullMode = CullMode.None;
            rasterizerState = new RasterizerState(device, desc);
            deviceContext.Rasterizer.State = rasterizerState;
        }

        public void SetSamplerState()
        {
            SamplerStateDescription description = SamplerStateDescription.Default();
            description.Filter = Filter.MinMagMipLinear;
            description.AddressU = TextureAddressMode.Wrap;
            description.AddressV = TextureAddressMode.Wrap;
            samplerState = new SamplerState(Device, description);
        }

        private void SetAllDeviceStates()
        {
            DeviceContext.Rasterizer.State = rasterizerState;
            DeviceContext.OutputMerger.SetBlendState(blendState);
            DeviceContext.OutputMerger.SetDepthStencilState(depthState);
            DeviceContext.PixelShader.SetSampler(0, samplerState);
        }

        private void OnRendering()
        {
            if (needsResizing)
            {
                PerformResizing();
            }

            SetAllDeviceStates();
            Clear(Color.CornflowerBlue);

            UpdateViewProjectionBuffers();
            if (Light != null)
            {
                Light.SetLightBuffer();
            }

            var statisticsQuery = new Query(device, new QueryDescription()
            {
                Type = QueryType.PipelineStatistics
            });
            deviceContext.Begin(statisticsQuery);
#if TEXT_RENDERER
            TextRenderer.BeginDraw();
#endif
            foreach (var renderable in renderables)
            {
                renderable.Render(this);
            }
            deviceContext.End(statisticsQuery);
            QueryDataPipelineStatistics result;
            while (!deviceContext.GetData(statisticsQuery, out result)) { }
#if TEXT_RENDERER
            TextRenderer.DrawText("VS Invocations: " + result.VSInvocationCount, new Vector2(0, 50));
            TextRenderer.DrawText("PS Invocations: " + result.PSInvocationCount, new Vector2(0, 75));
            TextRenderer.DrawText("Input Vertices: " + result.IAVerticeCount, new Vector2(0, 100));

            TextRenderer.EndDraw();
#endif

            swapChain.Present(0, PresentFlags.None);
        }

        private void UpdateViewProjectionBuffers()
        {
            projectionMatrix = camera.GetProjectionMatrix();
            viewMatrix = camera.GetViewMatrix();
        }

        private void Clear(Color4 color)
        {
            // Clear the back buffer
            deviceContext.ClearRenderTargetView(backBufferView, color);

            // Clear the depth buffer
            DeviceContext.ClearDepthStencilView(depthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);
        }

        private void OnFormResized(object sender, EventArgs e)
        {
            this.needsResizing = true;
        }

        private void PerformResizing()
        {
            if (backBufferView != null)
            {
                backBufferView.Dispose();
            }
            if (depthStencilView != null)
            {
                depthStencilView.Dispose();
            }

            swapChain.ResizeBuffers(1, renderForm.ClientSize.Width, renderForm.ClientSize.Height, Format.R8G8B8A8_UNorm, SwapChainFlags.AllowModeSwitch);

            // Get the backbuffer from the swapchain
            using (var backBufferTexture = SwapChain.GetBackBuffer<Texture2D>(0))
            {
                // Backbuffer
                backBufferView = new RenderTargetView(Device, backBufferTexture);
            }

            // Depth buffer

            using (var zbufferTexture = new Texture2D(Device, new Texture2DDescription()
            {
                Format = Format.D16_UNorm,
                ArraySize = 1,
                MipLevels = 1,
                Width = Math.Max(1, renderForm.ClientSize.Width),
                Height = Math.Max(1, renderForm.ClientSize.Height),
                SampleDescription = new SampleDescription(1, 0),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            }))
            {
                // Create the depth buffer view
                depthStencilView = new DepthStencilView(Device, zbufferTexture);
            }

            SetRegularTargets();

            this.needsResizing = false;
        }

        internal void SetWorldMatrix(Matrix4x4 worldMatrix)
        {
            var worldTransposed = Matrix4x4.Transpose(worldMatrix);
            var viewTransposed = Matrix4x4.Transpose(viewMatrix);
            var projectionTransposed = Matrix4x4.Transpose(projectionMatrix);

            MatricesBuffer matricesBuffer = new MatricesBuffer(ref worldTransposed, ref viewTransposed, ref projectionTransposed);
            deviceContext.UpdateSubresource<MatricesBuffer>(ref matricesBuffer, this.worldViewProjectionMatrixBuffer);
            deviceContext.VertexShader.SetConstantBuffer(0, this.worldViewProjectionMatrixBuffer);
        }

        internal void RenderFrame()
        {
            this.OnRendering();
        }

        internal SharpDX.Toolkit.Graphics.GraphicsDevice Get2DGraphicsDevice()
        {
            return this.__2dGraphicsDevice;
        }
        #endregion Private/Internal Implementation
    }

    #region Shader Constant Buffer Definitions
    [StructLayout(LayoutKind.Sequential)]
    internal struct MatricesBuffer
    {
        Matrix4x4 worldMatrix;
        Matrix4x4 viewMatrix;
        Matrix4x4 projectionMatrix;

        public MatricesBuffer(ref Matrix4x4 world, ref Matrix4x4 view, ref Matrix4x4 projection)
        {
            this.worldMatrix = world;
            this.viewMatrix = view;
            this.projectionMatrix = projection;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct AmbientLightBuffer
    {
        public readonly Color4f AmbientColor;
        public AmbientLightBuffer(Color4f color)
        {
            this.AmbientColor = color;
        }
    }
    #endregion Shader Constant Buffer Definitions
}