using ImageProcessor;
using OpenTK.Graphics.OpenGL;
using System;

namespace EngineCore.Graphics.OpenGL
{
    public class TextureBuffer : IDisposable
    {
        private int _textureBufferId;

        public TextureBuffer(Texture2D texture2D)
        {
            GenerateTexture(texture2D);
        }

        private void GenerateTexture(Texture2D image)
        {
            GL.GenTextures(1, out _textureBufferId);
            GL.BindTexture(TextureTarget.Texture2D, _textureBufferId);

            //the following code sets certian parameters for the texture
            GL.TexEnv(TextureEnvTarget.TextureEnv,
                   TextureEnvParameter.TextureEnvMode, (float)TextureEnvMode.Modulate);
            GL.TexParameter(TextureTarget.Texture2D,
                   TextureParameterName.TextureMinFilter, (float)TextureMinFilter.LinearMipmapLinear);
            GL.TexParameter(TextureTarget.Texture2D,
                   TextureParameterName.TextureMagFilter, (float)TextureMagFilter.Linear);

            // tell OpenGL to build mipmaps out of the bitmap data
            GL.TexParameter(TextureTarget.Texture2D,
                   TextureParameterName.GenerateMipmap, (float)1.0f);

            var pixelFormat = MapPixelFormat(image.Format);
            var pixelType = MapPixelType(image.Format);

            // load the texture
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0, // level
                PixelInternalFormat.Four,
                image.Width, image.Height,
                0, // border
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
                pixelType,
                image.Pixels
                );

            GL.BindTexture(TextureTarget.Texture2D, 0);
        }

        private OpenTK.Graphics.OpenGL.PixelFormat MapPixelFormat(PixelFormat format)
        {
            switch (format)
            {
                case PixelFormat.R32_G32_B32_A32_Float:
                    return OpenTK.Graphics.OpenGL.PixelFormat.Bgra;
                case PixelFormat.Alpha_Int8:
                    return OpenTK.Graphics.OpenGL.PixelFormat.Alpha;
                case PixelFormat.R8_G8_B8_A8:
                    return OpenTK.Graphics.OpenGL.PixelFormat.Bgra;
                default:
                    throw new InvalidOperationException("Invalid pixel format: " + format);
            }
        }

        private PixelType MapPixelType(PixelFormat format)
        {
            switch (format)
            {
                case PixelFormat.R32_G32_B32_A32_Float:
                    return PixelType.Float;
                case PixelFormat.Alpha_Int8:
                    return PixelType.UnsignedByte;
                case PixelFormat.R8_G8_B8_A8:
                    return PixelType.Int;
                default:
                    throw new InvalidOperationException("Invalid pixel format: " + format);
            }
        }

        public void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2D, _textureBufferId);
        }

        public void Dispose()
        {
            if (_textureBufferId != 0)
            {
                GL.DeleteTexture(_textureBufferId);
                _textureBufferId = 0;
            }
        }

        public TextureBufferBinding BeginBindingBlock()
        {
            return new TextureBufferBinding(_textureBufferId);
        }

        public struct TextureBufferBinding : IDisposable
        {
            public TextureBufferBinding(int id)
            {
                GL.BindTexture(TextureTarget.Texture2D, id);
            }

            public void Dispose()
            {
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
        }

    }
}
