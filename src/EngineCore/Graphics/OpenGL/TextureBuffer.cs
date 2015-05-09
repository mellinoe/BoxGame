using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineCore.Graphics.OpenGL
{
    public class TextureBuffer : IDisposable
    {
        private int _textureBufferId;

        public TextureBuffer(Bitmap bitmap)
        {
            GenerateTexture(bitmap);
        }

        public TextureBuffer(Texture2D texture2D)
        {
            GenerateTexture(texture2D.Bitmap);
        }

        private void GenerateTexture(Bitmap bitmap)
        {
            BitmapData bitmapData = bitmap.LockBits(
                new Rectangle(0, 0, bitmap.Width, bitmap.Height),
                ImageLockMode.ReadOnly,
                System.Drawing.Imaging.PixelFormat.Format24bppRgb);

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
                PixelInternalFormat.Three,
                bitmap.Width, bitmap.Height,
                0, // border
                OpenTK.Graphics.OpenGL.PixelFormat.Rgb,
                PixelType.UnsignedByte,
                bitmapData.Scan0
                );

            //free the bitmap data (we dont need it anymore because it has been passed to the OpenGL driver
            bitmap.UnlockBits(bitmapData);

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
