using ImageProcessor;
using System;
using System.IO;

namespace EngineCore.Graphics
{
    public class Texture2D
    {
        private readonly Image _image;

        public Texture2D(string projectRelativePath)
        {
            string fullPath = Path.Combine(AppContext.BaseDirectory, projectRelativePath);
            using (Stream textureFileStream = File.OpenRead(fullPath))
            {
                _image = new Image(textureFileStream);
            }
        }

        public Texture2D(Stream imageStream)
        {
            _image = new Image(imageStream);
        }

        public Image Image
        {
            get { return _image; }
        }
    }
}
