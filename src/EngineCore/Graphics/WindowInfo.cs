namespace EngineCore.Graphics
{
    public interface IWindowInfo
    {
        string Title { get; set; }
        bool IsFullscreen { get; set; }
        int Width { get; set; }
        int Height { get; set; }
        float ScaleFactor { get; }
    }
}
