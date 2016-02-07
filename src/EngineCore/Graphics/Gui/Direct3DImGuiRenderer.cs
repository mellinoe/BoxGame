using System;
using ImGuiNET;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D11;
using System.Numerics;
using SharpDX;
using SharpDX.Mathematics.Interop;
using EngineCore.Graphics.Direct3D;

namespace EngineCore.Graphics.Gui
{
    internal unsafe class Direct3DImGuiRenderer : DrawListRendererBase
    {
        private Device _device;
        private VertexShader _vertexShader;
        private InputLayout _inputLayout;
        private PixelShader _pixelShader;
        private BlendState _blendState;
        private RasterizerState _rasterizerState;
        private ShaderResourceView _fontTextureView;
        private SamplerState _fontSampler;

        public Direct3DImGuiRenderer(SharpDxGraphicsSystem graphicsSystem)
            : base(graphicsSystem.Renderer.Window, graphicsSystem.WindowInfo)
        {
            graphicsSystem.AddSelfManagedRenderable(this);
            _device = graphicsSystem.Renderer.Device;

            CreateDeviceObjects();
            ImGui.NewFrame();
        }

        private void CreateDeviceObjects()
        {
            // Create the vertex shader
            {
                const string vertexShaderCode =
@"cbuffer vertexBuffer : register(b0) 
{
    float4x4 ProjectionMatrix; 
};

struct VS_INPUT
{
    float2 pos : POSITION;
    float4 col : COLOR0;
    float2 uv  : TEXCOORD0;
};
            
struct PS_INPUT
{
    float4 pos : SV_POSITION;
    float4 col : COLOR0;
    float2 uv  : TEXCOORD0;
};
            
PS_INPUT main(VS_INPUT input)
{
    PS_INPUT output;
    output.pos = mul(ProjectionMatrix, float4(input.pos.xy, 0.f, 1.f));
    output.col = input.col;
    output.uv  = input.uv;
    return output;
}
";
                var vsCompilation = ShaderBytecode.Compile(vertexShaderCode, "main", "vs_5_0", ShaderFlags.Debug | ShaderFlags.SkipOptimization, EffectFlags.None, "DirectXImGuiRenderer.cs");
                _vertexShader = new VertexShader(_device, vsCompilation.Bytecode);

                //D3DCompile(vertexShader, strlen(vertexShader), NULL, NULL, NULL, "main", "vs_5_0", 0, 0, &g_pVertexShaderBlob, NULL);
                //if (g_pVertexShaderBlob == NULL) // NB: Pass ID3D10Blob* pErrorBlob to D3DCompile() to get error showing in (const string)pErrorBlob->GetBufferPointer(). Make sure to Release() the blob!
                //    return false;
                //if (g_pd3dDevice->CreateVertexShader((DWORD*)g_pVertexShaderBlob->GetBufferPointer(), g_pVertexShaderBlob->GetBufferSize(), NULL, &g_pVertexShader) != S_OK)
                //    return false;

                // Create the input layout
                InputElement[] localLayout =
                {
                    new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32_Float, DrawVert.PosOffset, 0),
                    new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, DrawVert.UVOffset, 0),
                    new InputElement("COLOR", 0, SharpDX.DXGI.Format.R8G8B8A8_UNorm, DrawVert.ColOffset, 0),
                    //{ "POSITION", 0, DXGI_FORMAT_R32G32_FLOAT,   0, (size_t)(&((ImDrawVert*)0)->pos), D3D11_INPUT_PER_VERTEX_DATA, 0 },
                    //{ "TEXCOORD", 0, DXGI_FORMAT_R32G32_FLOAT,   0, (size_t)(&((ImDrawVert*)0)->uv),  D3D11_INPUT_PER_VERTEX_DATA, 0 },
                    //{ "COLOR",    0, DXGI_FORMAT_R8G8B8A8_UNORM, 0, (size_t)(&((ImDrawVert*)0)->col), D3D11_INPUT_PER_VERTEX_DATA, 0 },
                };

                _inputLayout = new InputLayout(_device, vsCompilation.Bytecode, localLayout);

                //if (g_pd3dDevice->CreateInputLayout(localLayout, 3, g_pVertexShaderBlob->GetBufferPointer(), g_pVertexShaderBlob->GetBufferSize(), &g_pInputLayout) != S_OK)
                //    return false;

                // Create the constant buffer
                {
                    BufferDescription cbDesc = new BufferDescription();
                    cbDesc.SizeInBytes = sizeof(Matrix4x4);
                    cbDesc.Usage = ResourceUsage.Dynamic;
                    cbDesc.BindFlags = BindFlags.ConstantBuffer;
                    cbDesc.CpuAccessFlags = CpuAccessFlags.Write;
                    cbDesc.OptionFlags = ResourceOptionFlags.None;

                    Matrix4x4 initialData = Matrix4x4.Identity;
                    g_pVertexConstantBuffer = SharpDX.Direct3D11.Buffer.Create<Matrix4x4>(_device, ref initialData, cbDesc);

                    //D3D11_BUFFER_DESC cbDesc;
                    //cbDesc.ByteWidth = sizeof(VERTEX_CONSTANT_BUFFER);
                    //cbDesc.Usage = D3D11_USAGE_DYNAMIC;
                    //cbDesc.BindFlags = D3D11_BIND_CONSTANT_BUFFER;
                    //cbDesc.CPUAccessFlags = D3D11_CPU_ACCESS_WRITE;
                    //cbDesc.MiscFlags = 0;
                    //g_pd3dDevice->CreateBuffer(&cbDesc, NULL, &g_pVertexConstantBuffer);
                }
            }

            // Create the pixel shader
            {
                const string pixelShaderCode =
@"struct PS_INPUT
{
    float4 pos : SV_POSITION;
    float4 col : COLOR0;
    float2 uv  : TEXCOORD0;
};

sampler sampler0;
Texture2D texture0;
            
float4 main(PS_INPUT input) : SV_Target
{
    float4 out_col = input.col * texture0.Sample(sampler0, input.uv); 
    return out_col;
}
";

                var psCompilation = ShaderBytecode.Compile(pixelShaderCode, "main", "ps_5_0", ShaderFlags.Debug | ShaderFlags.SkipOptimization, EffectFlags.None, "DirectXImGuiRenderer.cs");
                _pixelShader = new PixelShader(_device, psCompilation.Bytecode);

                //D3DCompile(pixelShaderCode, strlen(pixelShaderCode), NULL, NULL, NULL, "main", "ps_5_0", 0, 0, &g_pPixelShaderBlob, NULL);
                //if (g_pPixelShaderBlob == NULL)  // NB: Pass ID3D10Blob* pErrorBlob to D3DCompile() to get error showing in (const string)pErrorBlob->GetBufferPointer(). Make sure to Release() the blob!
                //    return false;
                //if (g_pd3dDevice->CreatePixelShader((DWORD*)g_pPixelShaderBlob->GetBufferPointer(), g_pPixelShaderBlob->GetBufferSize(), NULL, &g_pPixelShader) != S_OK)
                //    return false;
            }

            // Create the blending setup
            {
                BlendStateDescription desc = new BlendStateDescription(); //BlendStateDescription.Default();

                desc.AlphaToCoverageEnable = false;
                desc.RenderTarget[0].IsBlendEnabled = true;
                desc.RenderTarget[0].SourceBlend = BlendOption.SourceAlpha;
                desc.RenderTarget[0].DestinationBlend = BlendOption.InverseSourceAlpha;
                desc.RenderTarget[0].BlendOperation = BlendOperation.Add;
                desc.RenderTarget[0].SourceAlphaBlend = BlendOption.InverseSourceAlpha;
                desc.RenderTarget[0].DestinationAlphaBlend = BlendOption.Zero;
                desc.RenderTarget[0].AlphaBlendOperation = BlendOperation.Add;
                desc.RenderTarget[0].RenderTargetWriteMask = ColorWriteMaskFlags.All;
                _blendState = new BlendState(_device, desc);

                //D3D11_BLEND_DESC desc;
                //ZeroMemory(&desc, sizeof(desc));
                //desc.AlphaToCoverageEnable = false;
                //desc.RenderTarget[0].BlendEnable = true;
                //desc.RenderTarget[0].SrcBlend = D3D11_BLEND_SRC_ALPHA;
                //desc.RenderTarget[0].DestBlend = D3D11_BLEND_INV_SRC_ALPHA;
                //desc.RenderTarget[0].BlendOp = D3D11_BLEND_OP_ADD;
                //desc.RenderTarget[0].SrcBlendAlpha = D3D11_BLEND_INV_SRC_ALPHA;
                //desc.RenderTarget[0].DestBlendAlpha = D3D11_BLEND_ZERO;
                //desc.RenderTarget[0].BlendOpAlpha = D3D11_BLEND_OP_ADD;
                //desc.RenderTarget[0].RenderTargetWriteMask = D3D11_COLOR_WRITE_ENABLE_ALL;
                //g_pd3dDevice->CreateBlendState(&desc, &g_pBlendState);
            }

            // Create the rasterizer state
            {
                RasterizerStateDescription desc = new RasterizerStateDescription();
                desc.FillMode = FillMode.Solid;
                desc.CullMode = CullMode.None;
                //desc.IsScissorEnabled = true;
                desc.IsDepthClipEnabled = true;
                _rasterizerState = new RasterizerState(_device, desc);

                //D3D11_RASTERIZER_DESC desc;
                //ZeroMemory(&desc, sizeof(desc));
                //desc.FillMode = D3D11_FILL_SOLID;
                //desc.CullMode = D3D11_CULL_NONE;
                //desc.ScissorEnable = true;
                //desc.DepthClipEnable = true;
                //g_pd3dDevice->CreateRasterizerState(&desc, &g_pRasterizerState);
            }

            CreateFontsTexture();
        }

        private void CreateFontsTexture()
        {
            IO io = ImGui.GetIO();

            // Build
            var textureData = io.FontAtlas.GetTexDataAsRGBA32();
            //io.Fonts->GetTexDataAsRGBA32(&pixels, &width, &height);

            // Create DX11 texture
            {
                Texture2DDescription texDesc = new Texture2DDescription();
                texDesc.Width = textureData.Width;
                texDesc.Height = textureData.Height;
                texDesc.MipLevels = 1;
                texDesc.ArraySize = 1;
                texDesc.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
                texDesc.SampleDescription.Count = 1;
                texDesc.Usage = ResourceUsage.Default;
                texDesc.BindFlags = BindFlags.ShaderResource;
                texDesc.CpuAccessFlags = CpuAccessFlags.None;

                //D3D11_TEXTURE2D_DESC texDesc;
                //ZeroMemory(&texDesc, sizeof(texDesc));
                //texDesc.Width = width;
                //texDesc.Height = height;
                //texDesc.MipLevels = 1;
                //texDesc.ArraySize = 1;
                //texDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
                //texDesc.SampleDesc.Count = 1;
                //texDesc.Usage = D3D11_USAGE_DEFAULT;
                //texDesc.BindFlags = D3D11_BIND_SHADER_RESOURCE;
                //texDesc.CPUAccessFlags = 0;

                SharpDX.Direct3D11.Texture2D pTexture;
                DataRectangle subResource = new DataRectangle(new IntPtr(textureData.Pixels), texDesc.Width * 4);
                //subResource.SlicePitch = 0;
                pTexture = new SharpDX.Direct3D11.Texture2D(_device, texDesc, subResource);

                //ID3D11Texture2D* pTexture = NULL;
                //D3D11_SUBRESOURCE_DATA subResource;
                //subResource.pSysMem = pixels;
                //subResource.SysMemPitch = texDesc.Width * 4;
                //subResource.SysMemSlicePitch = 0;
                //g_pd3dDevice->CreateTexture2D(&texDesc, &subResource, &pTexture);

                // Create texture view
                ShaderResourceViewDescription srvDesc = new ShaderResourceViewDescription();
                srvDesc.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;
                srvDesc.Dimension = SharpDX.Direct3D.ShaderResourceViewDimension.Texture2D;
                srvDesc.Texture2D.MipLevels = texDesc.MipLevels;
                srvDesc.Texture2D.MostDetailedMip = 0;
                _fontTextureView = new ShaderResourceView(_device, pTexture, srvDesc);
                pTexture.Dispose();

                //D3D11_SHADER_RESOURCE_VIEW_DESC srvDesc;
                //ZeroMemory(&srvDesc, sizeof(srvDesc));
                //srvDesc.Format = DXGI_FORMAT_R8G8B8A8_UNORM;
                //srvDesc.ViewDimension = D3D11_SRV_DIMENSION_TEXTURE2D;
                //srvDesc.Texture2D.MipLevels = texDesc.MipLevels;
                //srvDesc.Texture2D.MostDetailedMip = 0;
                //g_pd3dDevice->CreateShaderResourceView(pTexture, &srvDesc, &g_pFontTextureView);
                //pTexture->Release();
            }

            // Store our identifier
            io.FontAtlas.SetTexID(_fontTextureView.NativePointer);
            //io.Fonts->TexID = (void*)g_pFontTextureView;

            // Create texture sampler
            {
                SamplerStateDescription samplerDesc = SamplerStateDescription.Default();
                samplerDesc.Filter = Filter.MinMagMipLinear;
                samplerDesc.AddressU = TextureAddressMode.Wrap;
                samplerDesc.AddressV = TextureAddressMode.Wrap;
                samplerDesc.AddressW = TextureAddressMode.Wrap;
                samplerDesc.MipLodBias = 0f;
                samplerDesc.ComparisonFunction = Comparison.Always;
                samplerDesc.MinimumLod = 0f;
                samplerDesc.MaximumLod = 0f;
                _fontSampler = new SamplerState(_device, samplerDesc);

                //D3D11_SAMPLER_DESC samplerDesc;
                //ZeroMemory(&samplerDesc, sizeof(samplerDesc));
                //samplerDesc.Filter = D3D11_FILTER_MIN_MAG_MIP_LINEAR;
                //samplerDesc.AddressU = D3D11_TEXTURE_ADDRESS_WRAP;
                //samplerDesc.AddressV = D3D11_TEXTURE_ADDRESS_WRAP;
                //samplerDesc.AddressW = D3D11_TEXTURE_ADDRESS_WRAP;
                //samplerDesc.MipLODBias = 0.f;
                //samplerDesc.ComparisonFunc = D3D11_COMPARISON_ALWAYS;
                //samplerDesc.MinLOD = 0.f;
                //samplerDesc.MaxLOD = 0.f;
                //g_pd3dDevice->CreateSamplerState(&samplerDesc, &g_pFontSampler);
            }

            // Cleanup (don't clear the input data if you want to append new fonts later)
            io.FontAtlas.ClearTexData();
            //io.Fonts->ClearInputData();
            //io.Fonts->ClearTexData();
        }

        private SharpDX.Direct3D11.Buffer g_pVB;
        private SharpDX.Direct3D11.Buffer g_pIB;
        private int g_VertexBufferSize = 5000;
        private int g_IndexBufferSize = 10000;
        private SharpDX.Direct3D11.Buffer g_pVertexConstantBuffer;

        internal override void RenderImDrawData(DrawData* draw_data)
        {
            // Create and grow vertex/index buffers if needed
            if (g_pVB == null || g_VertexBufferSize < draw_data->TotalVtxCount)
            {
                if (g_pVB != null)
                {
                    g_pVB.Dispose();
                    g_pVB = null;
                }

                g_VertexBufferSize = draw_data->TotalVtxCount + 5000;
                BufferDescription desc = new BufferDescription();
                desc.Usage = ResourceUsage.Dynamic;
                desc.SizeInBytes = g_VertexBufferSize * sizeof(DrawVert);
                desc.BindFlags = BindFlags.VertexBuffer;
                desc.CpuAccessFlags = CpuAccessFlags.Write;
                desc.OptionFlags = ResourceOptionFlags.None;
                g_pVB = new SharpDX.Direct3D11.Buffer(_device, desc);
            }

            if (g_pIB == null || g_IndexBufferSize < draw_data->TotalIdxCount)
            {
                if (g_pIB != null) { g_pIB.Dispose(); g_pIB = null; }
                g_IndexBufferSize = draw_data->TotalIdxCount + 10000;
                BufferDescription bufferDesc = new BufferDescription();
                bufferDesc.Usage = ResourceUsage.Dynamic;
                bufferDesc.SizeInBytes = g_IndexBufferSize * sizeof(ushort); // sizeof(ImDrawIdx), ImDrawIdx = typedef unsigned short
                bufferDesc.BindFlags = BindFlags.IndexBuffer;
                bufferDesc.CpuAccessFlags = CpuAccessFlags.Write;
                g_pIB = new SharpDX.Direct3D11.Buffer(_device, bufferDesc);
            }

            // Copy and convert all vertices into a single contiguous buffer
            var deviceContext = _device.ImmediateContext;

            DataBox vtx_resource = deviceContext.MapSubresource(g_pVB, 0, MapMode.WriteDiscard, MapFlags.None);
            DataBox idx_resource = deviceContext.MapSubresource(g_pIB, 0, MapMode.WriteDiscard, MapFlags.None);
            //D3D11_MAPPED_SUBRESOURCE vtx_resource, idx_resource;
            //if (g_pd3dDeviceContext->Map(g_pVB, 0, D3D11_MAP_WRITE_DISCARD, 0, &vtx_resource) != S_OK)
            //    return;
            //if (g_pd3dDeviceContext->Map(g_pIB, 0, D3D11_MAP_WRITE_DISCARD, 0, &idx_resource) != S_OK)
            //    return;

            DrawVert* vtx_dst = (DrawVert*)vtx_resource.DataPointer;
            ushort* idx_dst = (ushort*)idx_resource.DataPointer;
            for (int n = 0; n < draw_data->CmdListsCount; n++)
            {
                DrawList* cmd_list = draw_data->CmdLists[n];
                System.Buffer.MemoryCopy(
                    cmd_list->VtxBuffer.Data,
                    vtx_dst,
                    cmd_list->VtxBuffer.Size * sizeof(DrawVert),
                    cmd_list->VtxBuffer.Size * sizeof(DrawVert));
                //memcpy(vtx_dst, &cmd_list->VtxBuffer[0], cmd_list->VtxBuffer.size() * sizeof(ImDrawVert));

                System.Buffer.MemoryCopy(
                    cmd_list->IdxBuffer.Data,
                    idx_dst,
                    cmd_list->IdxBuffer.Size * sizeof(ushort),
                    cmd_list->IdxBuffer.Size * sizeof(ushort));
                //memcpy(idx_dst, &cmd_list->IdxBuffer[0], cmd_list->IdxBuffer.size() * sizeof(ImDrawIdx));

                vtx_dst += cmd_list->VtxBuffer.Size;
                idx_dst += cmd_list->IdxBuffer.Size;
            }
            deviceContext.UnmapSubresource(g_pVB, 0);
            deviceContext.UnmapSubresource(g_pIB, 0);

            //g_pd3dDeviceContext->Unmap(g_pVB, 0);
            //g_pd3dDeviceContext->Unmap(g_pIB, 0);

            // Setup orthographic projection matrix into our constant buffer
            {
                //D3D11_MAPPED_SUBRESOURCE mappedResource;
                //if (g_pd3dDeviceContext->Map(g_pVertexConstantBuffer, 0, D3D11_MAP_WRITE_DISCARD, 0, &mappedResource) != S_OK)
                //    return;

                // I checked; the below is identical to calling Matrix4x4.CreateOrthographicOffCenter.
                //float L = 0.0f;
                //float R = ImGui.GetIO().DisplaySize.X;
                //float B = ImGui.GetIO().DisplaySize.Y;
                //float T = 0.0f;
                //Matrix4x4 mvp = new Matrix4x4(
                //    2.0f / (R - L), 0.0f, 0.0f, 0.0f,
                //    0.0f, 2.0f / (T - B), 0.0f, 0.0f,
                //    0.0f, 0.0f, 0.5f, 0.0f,
                //    (R + L) / (L - R), (T + B) / (B - T), 0.5f, 1.0f);

                var io = ImGui.GetIO();
                Matrix4x4 mvp = Matrix4x4.CreateOrthographicOffCenter(
                    0.0f,
                    io.DisplaySize.X / io.DisplayFramebufferScale.X,
                    io.DisplaySize.Y / io.DisplayFramebufferScale.Y,
                    0.0f,
                    -1.0f,
                    1.0f);

                DataBox mappedResource = deviceContext.MapSubresource(g_pVertexConstantBuffer, 0, MapMode.WriteDiscard, MapFlags.None);
                Matrix4x4* pConstantBuffer = (Matrix4x4*)mappedResource.DataPointer;

                System.Buffer.MemoryCopy(&mvp, pConstantBuffer, sizeof(Matrix4x4), sizeof(Matrix4x4));
                deviceContext.UnmapSubresource(g_pVertexConstantBuffer, 0);
                //deviceContext.UpdateSubresource(ref mvp, g_pVertexConstantBuffer);

                //memcpy(&pConstantBuffer->mvp, mvp, sizeof(mvp));
                //g_pd3dDeviceContext->Unmap(g_pVertexConstantBuffer, 0);
            }

            // Setup viewport
            {
                RawViewportF vp = new RawViewportF();
                //D3D11_VIEWPORT vp;
                //memset(&vp, 0, sizeof(D3D11_VIEWPORT));
                vp.Width = ImGui.GetIO().DisplaySize.X;
                vp.Height = ImGui.GetIO().DisplaySize.Y;
                vp.MinDepth = 0.0f;
                vp.MaxDepth = 1.0f;
                vp.X = 0;
                vp.Y = 0;
                deviceContext.Rasterizer.SetViewport(0, 0, _windowInfo.Width, _windowInfo.Height, 0, 1);
                //g_pd3dDeviceContext->RSSetViewports(1, &vp);
            }

            // Bind shader and vertex buffers
            int stride = sizeof(DrawVert);
            int offset = 0;
            var ia = deviceContext.InputAssembler;
            ia.InputLayout = _inputLayout;
            VertexBufferBinding vbBinding = new VertexBufferBinding(g_pVB, stride, offset);
            ia.SetVertexBuffers(0, vbBinding);
            ia.SetIndexBuffer(g_pIB, SharpDX.DXGI.Format.R16_UInt, 0);
            ia.PrimitiveTopology = SharpDX.Direct3D.PrimitiveTopology.TriangleList;

            //g_pd3dDeviceContext->IASetInputLayout(g_pInputLayout);
            //g_pd3dDeviceContext->IASetVertexBuffers(0, 1, &g_pVB, &stride, &offset);
            //g_pd3dDeviceContext->IASetIndexBuffer(g_pIB, DXGI_FORMAT_R16_UINT, 0);
            //g_pd3dDeviceContext->IASetPrimitiveTopology(D3D11_PRIMITIVE_TOPOLOGY_TRIANGLELIST);

            deviceContext.VertexShader.SetShader(_vertexShader, null, 0);
            deviceContext.VertexShader.SetConstantBuffer(0, g_pVertexConstantBuffer);
            //g_pd3dDeviceContext->VSSetShader(g_pVertexShader, NULL, 0);
            //g_pd3dDeviceContext->VSSetConstantBuffers(0, 1, &g_pVertexConstantBuffer);

            deviceContext.PixelShader.SetShader(_pixelShader, null, 0);
            deviceContext.PixelShader.SetSamplers(0, 1, _fontSampler);
            //g_pd3dDeviceContext->PSSetShader(g_pPixelShader, NULL, 0);
            //g_pd3dDeviceContext->PSSetSamplers(0, 1, &g_pFontSampler);

            // Setup render state
            RawColor4 blendFactor = new RawColor4(0f, 0f, 0f, 0f);
            //const float[] blendFactor = { 0f, 0f, 0f, 0f };
            deviceContext.OutputMerger.SetBlendState(_blendState, blendFactor, 0xffffffff);
            //g_pd3dDeviceContext->OMSetBlendState(g_pBlendState, blendFactor, 0xffffffff);

            deviceContext.Rasterizer.State = _rasterizerState;
            //g_pd3dDeviceContext->RSSetState(g_pRasterizerState);

            ImGui.ScaleClipRects(draw_data, ImGui.GetIO().DisplayFramebufferScale);

            // Render command lists
            int vtx_offset = 0;
            int idx_offset = 0;
            for (int n = 0; n < draw_data->CmdListsCount; n++)
            {
                DrawList* cmd_list = draw_data->CmdLists[n];
                for (int cmd_i = 0; cmd_i < cmd_list->CmdBuffer.Size; cmd_i++)
                {
                    DrawCmd* pcmd = &(((DrawCmd*)cmd_list->CmdBuffer.Data)[cmd_i]);
                    if (pcmd->UserCallback != IntPtr.Zero)
                    {
                        throw new NotImplementedException();
                    }
                    else
                    {
                        deviceContext.PixelShader.SetShaderResources(0, 1, new ShaderResourceView(pcmd->TextureId));
                        deviceContext.Rasterizer.SetScissorRectangle(
                            (int)pcmd->ClipRect.X,
                            (int)pcmd->ClipRect.Y,
                            (int)pcmd->ClipRect.Z,
                            (int)pcmd->ClipRect.W);
                        deviceContext.DrawIndexed((int)pcmd->ElemCount, idx_offset, vtx_offset);
                    }

                    //const ImDrawCmd* pcmd = &cmd_list->CmdBuffer[cmd_i];
                    //if (pcmd->UserCallback)
                    //{
                    //    pcmd->UserCallback(cmd_list, pcmd);
                    //}
                    //else
                    //{
                    //    const D3D11_RECT r = { (LONG)pcmd->ClipRect.x, (LONG)pcmd->ClipRect.y, (LONG)pcmd->ClipRect.z, (LONG)pcmd->ClipRect.w };
                    //    g_pd3dDeviceContext->PSSetShaderResources(0, 1, (ID3D11ShaderResourceView**)&pcmd->TextureId);
                    //    g_pd3dDeviceContext->RSSetScissorRects(1, &r);
                    //    g_pd3dDeviceContext->DrawIndexed(pcmd->ElemCount, idx_offset, vtx_offset);
                    //}

                    idx_offset += (int)pcmd->ElemCount;
                }
                vtx_offset += cmd_list->VtxBuffer.Size;
            }

            // Restore modified state
            deviceContext.InputAssembler.InputLayout = null;
            deviceContext.PixelShader.SetShader(null, null, 0);
            deviceContext.VertexShader.SetShader(null, null, 0);

            //g_pd3dDeviceContext->IASetInputLayout(NULL);
            //g_pd3dDeviceContext->PSSetShader(NULL, NULL, 0);
            //g_pd3dDeviceContext->VSSetShader(NULL, NULL, 0);
        }

        public override void Dispose()
        {
            if (g_pVB != null)
            {
                g_pVB.Dispose();
            }
            if (g_pIB != null)
            {
                g_pIB.Dispose();
            }
            if (g_pVertexConstantBuffer != null)
            {
                g_pVertexConstantBuffer.Dispose();
            }
            if (_inputLayout != null)
            {
                _inputLayout.Dispose();
            }
            if (_fontSampler != null)
            {
                _fontSampler.Dispose();
            }
            if (_blendState != null)
            {
                _blendState.Dispose();
            }
            if (_fontTextureView != null)
            {
                _fontTextureView.Dispose();
            }
            if (_rasterizerState != null)
            {
                _rasterizerState.Dispose();
            }

            _pixelShader?.Dispose();
            _vertexShader?.Dispose();
        }
    }
}
