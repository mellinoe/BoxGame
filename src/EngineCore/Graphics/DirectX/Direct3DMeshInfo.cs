using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Graphics.DirectX
{
    public class Direct3DMeshInfo : RenderableObjectBase, IRenderableObjectInfo, IDisposable
    {
        /// <summary>
        /// The vertex buffer of the mesh
        /// </summary>
        public SharpDX.Direct3D11.Buffer VertexBuffer { get; set; }

        /// <summary>
        /// The normal buffer of the mesh
        /// </summary>
        public SharpDX.Direct3D11.Buffer NormalBuffer { get; set; }

        /// <summary>
        /// The index buffer of the mesh
        /// </summary>
        public SharpDX.Direct3D11.Buffer IndexBuffer { get; set; }

        private int _indexCount;
        private SimpleShader _shader;
        private IRenderable _renderable;
        private ShaderResourceView _shaderTextureResourceView;

        public Direct3DMeshInfo(SimpleRenderer simpleRenderer, IRenderable renderable, SimpleVertex[] vertices, int[] indices, Bitmap bitmap)
            : base(simpleRenderer)
        {
            VertexBuffer = SharpDX.Direct3D11.Buffer.Create<SimpleVertex>(simpleRenderer.Device, BindFlags.VertexBuffer, vertices);
            IndexBuffer = SharpDX.Direct3D11.Buffer.Create<int>(simpleRenderer.Device, BindFlags.IndexBuffer, indices);
            _indexCount = indices.Length;
            _shader = simpleRenderer.DefaultShaders.LightShader;
            _renderable = renderable;

            Texture2DDescription desc;
            desc.Width = bitmap.Size.Width;
            desc.Height = bitmap.Size.Height;
            desc.ArraySize = 1;
            desc.BindFlags = BindFlags.ShaderResource;
            desc.Usage = ResourceUsage.Default;
            desc.CpuAccessFlags = CpuAccessFlags.None;
            desc.Format = Format.R8G8B8A8_UNorm;
            desc.MipLevels = 1;
            desc.OptionFlags = ResourceOptionFlags.None;
            desc.SampleDescription.Count = 1;
            desc.SampleDescription.Quality = 0;

            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            DataRectangle dataRectangle = new DataRectangle(bitmapData.Scan0, bitmapData.Stride);
            bitmap.UnlockBits(bitmapData);

            SharpDX.Direct3D11.Texture2D texture = new SharpDX.Direct3D11.Texture2D(simpleRenderer.Device, desc, dataRectangle);
            _shaderTextureResourceView = new ShaderResourceView(simpleRenderer.Device, texture);
        }

        public void Dispose()
        {
            VertexBuffer.Dispose();
            NormalBuffer.Dispose();
            IndexBuffer.Dispose();
        }

        public virtual void Render(ref System.Numerics.Matrix4x4 lookatMatrix)
        {
            ApplyShaderSettings();

            var inputAssembler = SimpleRenderer.Device.ImmediateContext.InputAssembler;
            ApplyVertexSettings(inputAssembler);
            SetWorldMatrix(_renderable);
            SetIndexBuffer(inputAssembler);
            DrawIndexedElements(inputAssembler);
        }

        private void SetIndexBuffer(InputAssemblerStage inputAssembler)
        {
            inputAssembler.SetIndexBuffer(IndexBuffer, SharpDX.DXGI.Format.R32_UInt, 0);
        }

        protected void SetWorldMatrix(IRenderable renderable)
        {
            SimpleRenderer.SetWorldMatrix(renderable.WorldMatrix);
        }

        protected virtual void ApplyVertexSettings(InputAssemblerStage inputAssembler)
        {
            inputAssembler.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;
            inputAssembler.SetVertexBuffers(0, new VertexBufferBinding(VertexBuffer, SimpleVertex.SizeInBytes, 0));
        }

        protected virtual void ApplyShaderSettings()
        {
            SimpleRenderer.DeviceContext.PixelShader.SetShaderResource(0, _shaderTextureResourceView);
            _shader.ApplyShader();
        }

        protected virtual void DrawIndexedElements(InputAssemblerStage inputAssembler)
        {
            SimpleRenderer.Device.ImmediateContext.DrawIndexed(_indexCount, 0, 0);
        }
    }

    public class Direct3DBatchedMeshInfo : Direct3DMeshInfo
    {
        private List<IRenderable> _renderables = new List<IRenderable>();

        public Direct3DBatchedMeshInfo(SimpleRenderer simpleRenderer, IRenderable renderable, SimpleVertex[] vertices, int[] indices, Bitmap bitmap)
            : base(simpleRenderer, renderable, vertices, indices, bitmap)
        {
        }

        internal void AddRenderable(IRenderable renderable)
        {
            _renderables.Add(renderable);
        }

        internal void RemoveRenderable(IRenderable renderable)
        {
            _renderables.Remove(renderable);
        }

        public override void Render(ref System.Numerics.Matrix4x4 lookatMatrix)
        {
            ApplyShaderSettings();
            var inputAssembler = SimpleRenderer.Device.ImmediateContext.InputAssembler;

            ApplyVertexSettings(inputAssembler);

            inputAssembler.SetIndexBuffer(IndexBuffer, SharpDX.DXGI.Format.R32_UInt, 0);
            foreach (IRenderable renderable in _renderables)
            {
                SetWorldMatrix(renderable);
                DrawIndexedElements(inputAssembler);
            }
        }
    }
}
