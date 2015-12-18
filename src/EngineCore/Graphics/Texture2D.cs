using ImageProcessor;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace EngineCore.Graphics
{
    public abstract class Texture2D
    {
        public abstract int Width { get; }
        public abstract int Height { get; }
        public abstract IntPtr Pixels { get; }
        public abstract PixelFormat Format { get; }

        public static Texture2D CreateFromFile(string projectRelativePath)
        {
            return new ImageProcessorTexture2D(projectRelativePath);
        }
    }

    internal class ImageProcessorTexture2D : Texture2D
    {
        private GCHandle _handle;
        private readonly Image _image;

        public ImageProcessorTexture2D(string projectRelativePath)
        {
            string fullPath = Path.Combine(AppContext.BaseDirectory, projectRelativePath);
            using (Stream textureFileStream = File.OpenRead(fullPath))
            {
                _image = new Image(textureFileStream);
                AllocHandle();
            }
        }

        public ImageProcessorTexture2D(Stream imageStream)
        {
            _image = new Image(imageStream);
            AllocHandle();
        }

        private void AllocHandle()
        {
            _handle = GCHandle.Alloc(_image.Pixels, GCHandleType.Pinned);
        }

        public override int Height => _image.Height;

        public override IntPtr Pixels => _handle.AddrOfPinnedObject();

        public override int Width => _image.Width;

        public override PixelFormat Format => PixelFormat.R32_G32_B32_A32_Float;
    }

    internal class RawTexture2D : Texture2D
    {
        public RawTexture2D(int width, int height, IntPtr pixels, PixelFormat pixelFormat)
        {
            Width = width;
            Height = height;
            Pixels = pixels;
            Format = pixelFormat;
        }

        public override int Height { get; }

        public override IntPtr Pixels { get; }

        public override int Width { get; }

        public override PixelFormat Format { get; }
    }

    public enum PixelFormat
    {
        /// <summary>
        /// A format where each component (RGBA) is represented by a 32-bit floating point value.
        /// </summary>
        R32_G32_B32_A32_Float,
        /// <summary>
        /// A format where each pixel is represented by a single byte (0-255) greyscale value.
        /// </summary>
        Alpha_Int8,
        /// <summary>
        /// A format where each pixel has four components (RGBA), each represented by an 8-bit integer value.
        /// </summary>
        R8_G8_B8_A8
    }
}
