using ImGuiNET;
using System;

namespace EngineCore.Graphics.Gui
{
    internal class ImGuiSystem : GameSystem
    {
        private PolyMesh _mesh;
        private GameObject _proxy;

        public ImGuiSystem(Game game) : base(game)
        {
        }

        public override unsafe void Start()
        {
            _mesh = new PolyMesh(Array.Empty<SimpleVertex>(), Array.Empty<int>());
            ImGui.LoadDefaultFont();
            var fontTexData = ImGui.GetIO().FontAtlas.GetTexDataAsAlpha8();
            Texture2D fontTexture = new RawTexture2D(fontTexData.Width, fontTexData.Height, new IntPtr(fontTexData.Pixels), PixelFormat.Alpha_Int8);
            _proxy = new GameObject();
            _proxy.AddComponent(new MeshRenderer(_mesh, fontTexture));


            ImGui.NewFrame();
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

        public unsafe override void Update()
        {
            ImGui.Render();
            var drawData = ImGui.GetDrawData();
            for (int i = 0; i < drawData->CmdListsCount; i++)
            {
                var cmdList = drawData->CmdLists[i];
                var vertexBufferPtr = (DrawVert*)cmdList->VtxBuffer.Data;
            }
            ImGui.NewFrame();
        }
    }

    internal unsafe interface IDrawListRenderer
    {
        void Render(DrawData* drawData);
    }
}
