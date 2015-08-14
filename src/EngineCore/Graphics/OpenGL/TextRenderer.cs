using EngineCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Numerics;

namespace EngineCore.Graphics.OpenGL
{
    public class TextRenderer : Component<OpenGLGraphicsSystem>, IRenderableObjectInfo2D, IDisposable
    {
        private int _width, _height;
        private Bitmap _bitmap;
        private int _textTextureId;
        private Font _font;
        private Brush _brush;

        // Not very good yet
        private string _text;
        private float _textPosX;
        private float _textPosY;

        public void DrawText(string text, float x, float y)
        {
            _text = text;
            _textPosX = x;
            _textPosY = y;

            UpdateBitmap();
        }

        private void UpdateBitmap()
        {
            // Render text using System.Drawing.
            // Do this only when text changes.
            using (System.Drawing.Graphics graphics = System.Drawing.Graphics.FromImage(_bitmap))
            {
                graphics.Clear(Color.Transparent);
                graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
                graphics.DrawString(_text, _font, _brush, _textPosX, _textPosY); // Draw as many strings as you need
            }

            // Upload the Bitmap to OpenGL.
            // Do this only when text changes.
            BitmapData data = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _width, _height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
            _bitmap.UnlockBits(data);
        }

        public void Render()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.PushMatrix();
            GL.LoadIdentity();

            GL.MatrixMode(MatrixMode.Projection);
            GL.PushMatrix();
            Matrix4x4 ortho_projection = Matrix4x4.CreateOrthographicOffCenter(0, _width, _height, 0, -1, 1);
            GLEx.LoadMatrix(ref ortho_projection);

            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
            GL.Enable(EnableCap.Texture2D);
            GL.BindTexture(TextureTarget.Texture2D, _textTextureId);

            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0);
            GL.Vertex2(0, 0);
            GL.TexCoord2(1, 0);
            GL.Vertex2(_bitmap.Width, 0);
            GL.TexCoord2(1, 1);
            GL.Vertex2(_bitmap.Width, _bitmap.Height);
            GL.TexCoord2(0, 1);
            GL.Vertex2(0, _bitmap.Height);
            GL.End();
            GL.PopMatrix();

            GL.Disable(EnableCap.Blend);
            GL.Disable(EnableCap.Texture2D);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.PopMatrix();
        }

        protected override void Initialize(OpenGLGraphicsSystem system)
        {
            system.OnScreenResized += OnScreenResized;
            _width = system.WindowSize.Width;
            _height = system.WindowSize.Height;

            system.AddSelfManagedRenderable(this);

            CreateInitialBitmap();

            _font = new Font("Segoe UI", 18, FontStyle.Bold);
            _brush = new SolidBrush(Color.White);

            this.DrawText("Hello World", 0, 0);
        }

        private void CreateInitialBitmap()
        {
            // Create Bitmap and OpenGL texture
            _bitmap = new Bitmap(_width, _height); // match window size

            _textTextureId = GL.GenTexture();
            GL.BindTexture(TextureTarget.Texture2D, _textTextureId);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)All.Linear);
            GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)All.Linear);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _bitmap.Width, _bitmap.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero); // just allocate memory, so we can update efficiently using TexSubImage2D
        }

        private void OnScreenResized(GameWindowResizedEventArgs args)
        {
            _width = args.Width;
            _height = args.Height;

            // Ensure Bitmap and texture match window size
            _bitmap.Dispose();
            _bitmap = new Bitmap(Math.Max(1, _width), Math.Max(1, _height));

            GL.BindTexture(TextureTarget.Texture2D, _textTextureId);
            //GL.TexSubImage2D(TextureTarget.Texture2D, 0, 0, 0, _bitmap.Width, _bitmap.Height,
            //    OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero);
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, _bitmap.Width, _bitmap.Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero); // just allocate memory, so we can update efficiently using TexSubImage2D

            UpdateBitmap();
        }

        protected override void Uninitialize(OpenGLGraphicsSystem system)
        {
            system.OnScreenResized -= OnScreenResized;
            system.RemoveSelfManagedRenderable(this);
            Dispose();
        }

        public void Dispose()
        {
            if (_textTextureId != 0)
            {
                GL.DeleteTexture(_textTextureId);
                _textTextureId = 0;
            }
        }
    }
}
