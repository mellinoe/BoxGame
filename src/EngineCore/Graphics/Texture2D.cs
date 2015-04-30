using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Graphics
{
    public class Texture2D
    {
        private readonly Bitmap _bitmap;

        public Texture2D(string filename)
        {
            _bitmap = new Bitmap(filename);
        }

        public Bitmap Bitmap
        {
            get { return _bitmap; }
        }
    }
}
