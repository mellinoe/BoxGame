using ImageProcessor;
using OpenTK.Graphics.OpenGL;
using System;

namespace EngineCore.Graphics.OpenGL
{
    public class TextureBuffer : IDisposable
    {
        private int _textureBufferId;

        public TextureBuffer(Image image)
        {
            GenerateTexture(image);
        }

        public TextureBuffer(Texture2D texture2D)
        {
            GenerateTexture(texture2D.Image);
        }

        private void GenerateTexture(Image image)
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

            // load the texture
            GL.TexImage2D(
                TextureTarget.Texture2D,
                0, // level
                PixelInternalFormat.Four,
                image.Width, image.Height,
                0, // border
                PixelFormat.Bgra,
                PixelType.Float,
                image.Pixels
                );

            GL.BindTexture(TextureTarget.Texture2D, 0);
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
