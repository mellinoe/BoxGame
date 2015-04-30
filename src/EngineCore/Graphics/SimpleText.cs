using SharpDX.Direct3D11;
using SharpDX.DirectWrite;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace EngineCore.Graphics
{
    public class SimpleText : IDisposable
    {
        private Device device;
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;
        private GraphicsDevice graphicsDevice;

        public SimpleText(GraphicsDevice device, string fontFilename)
        {
            this.device = device;
            this.graphicsDevice = device;
            this.spriteFont = SpriteFont.Load(graphicsDevice, fontFilename);
            var sfd = SpriteFontData.Load(fontFilename);
            this.spriteBatch = new SpriteBatch(graphicsDevice);
        }

        public void BeginDraw() { spriteBatch.Begin(); }
        public void EndDraw() { spriteBatch.End(); }

        public void DrawText(string text, Vector2 screenPosition)
        {
            DrawText(text, screenPosition, Color4f.White);
        }

        public void DrawText(string text, Vector2 screenPosition, Color4f color)
        {
            //spriteBatch.Begin();
            spriteBatch.DrawString(spriteFont, text, ToSharpVector(screenPosition), ToSharpColor(color));
            //spriteBatch.End();
        }

        private SharpDX.Vector2 ToSharpVector(Vector2 v2f)
        {
            return new SharpDX.Vector2(v2f.X, v2f.Y);
        }

        private SharpDX.Color ToSharpColor(Color4f color4f)
        {
            return new SharpDX.Color(color4f.R, color4f.G, color4f.B, color4f.A);
        }

        public void Dispose()
        {
            this.spriteBatch.Dispose();
            this.spriteFont.Dispose();
        }
    }
}
