namespace EngineCore.Graphics.Direct3D
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
