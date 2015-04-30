using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace EngineCore.Graphics
{
    public class DirectionalLight
    {
        private Device device;
        private DeviceContext deviceContext;
        private Vector3 direction;
        public Vector3 Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        private Color4f diffuseColor;
        private SharpDX.Direct3D11.Buffer lightMatrixBuffer;

        public DirectionalLight(Device device, DeviceContext deviceContext, Vector3 direction, Color4f color)
        {
            this.device = device;
            this.deviceContext = deviceContext;
            this.direction = Vector3.Normalize(direction);
            this.diffuseColor = color;
            this.lightMatrixBuffer = new SharpDX.Direct3D11.Buffer(
                device,
                Marshal.SizeOf(typeof(LightBuffer)),
                ResourceUsage.Default,
                BindFlags.ConstantBuffer,
                CpuAccessFlags.None,
                ResourceOptionFlags.None,
                0);
        }

        public void SetLightBuffer()
        {
            var lightBuffer = new LightBuffer(diffuseColor, direction);
            deviceContext.UpdateSubresource<LightBuffer>(ref lightBuffer, lightMatrixBuffer);
            deviceContext.PixelShader.SetConstantBuffer(0, lightMatrixBuffer);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 16)]
    internal struct LightBuffer
    {
        Color4f diffuseColor;
        Vector3 lightDirection;
        float __packing;

        public LightBuffer(Color4f diffuseColor, Vector3 direction)
        {
            this.diffuseColor = diffuseColor;
            this.lightDirection = direction;
            this.__packing = 0;
        }
    }
}
