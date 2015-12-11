namespace EngineCore.Graphics.DirectX
{
    public abstract class RenderableObjectBase
    {
        public SimpleRenderer SimpleRenderer { get; private set; }

        public RenderableObjectBase(SimpleRenderer simpleRenderer)
        {
            SimpleRenderer = simpleRenderer;
        }
    }
}
