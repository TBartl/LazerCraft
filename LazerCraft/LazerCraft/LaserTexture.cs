using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LazerCraft
{
    public class LaserTexture
    {
        Texture2D textureInner;
        Texture2D textureOuter;
        public int Width;
        public int Height;

        public LaserTexture(Texture2D inner, Texture2D outer) 
        {
            textureInner = inner;
            textureOuter = outer;
            Width = inner.Width;
            Height = inner.Height;
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
        {
            spriteBatch.Draw(textureInner, position, Main.GetInnerColor(color));
            spriteBatch.Draw(textureOuter, position, color);
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Rectangle? source, Color color)
        {
            spriteBatch.Draw(textureInner, position, source, Main.GetInnerColor(color));
            spriteBatch.Draw(textureOuter, position, source, color);
        }
        public void Draw(SpriteBatch spriteBatch, Vector2 position, Rectangle? source, Color color, float rotation, Vector2 origin, float scale, SpriteEffects spriteEffects)
        {
            spriteBatch.Draw(textureInner, position, source, Main.GetInnerColor(color), rotation, origin,scale, spriteEffects, 0f);
            spriteBatch.Draw(textureOuter, position, source, color, rotation, origin, scale, spriteEffects, 0f);
        }


    }
}
