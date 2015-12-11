using System;

namespace EngineCore.Graphics
{
    public class GameWindowResizedEventArgs : EventArgs
    {
        public int Width { get; }

        public int Height { get; }

        public GameWindowResizedEventArgs(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
