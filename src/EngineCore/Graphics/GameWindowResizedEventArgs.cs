using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Graphics
{
    public class GameWindowResizedEventArgs : EventArgs
    {
        private readonly int _width;
        private readonly int _height;

        public int Height
        {
            get { return _height; }
        }

        public int Width
        {
            get { return _width; }
        }

        public GameWindowResizedEventArgs(int width, int height)
        {
            _width = width;
            _height = height;
        }
    }
}
