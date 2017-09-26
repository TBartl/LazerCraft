using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LazerCraft
{
    public class SpriteSheetObject: BaseObject
    {
        public bool drawAtScreenPosition { get; set; }
        public int frames { get; set; }
        public int frame { get; set; }
        public Texture2D texture { get; set; }
        Rectangle sourceRectangle;
        public SpriteSheetObject(Texture2D texture, Vector2 position, bool useOrigin, Color color, float alpha, bool drawAtScreenPosition, int frames, int firstFrame)
        {
            this.texture = texture;
            this.position = position;
            this.frames = frames;
            this.frame = firstFrame;
            if (useOrigin == true)
            {
                origin = new Vector2((texture.Width / 2) / frames, texture.Height / 2);
            }
            else
            {
                origin = Vector2.Zero;
            }   
            this.color = color;
            this.alpha = alpha;
            this.drawAtScreenPosition = drawAtScreenPosition;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sourceRectangle = new Rectangle((int)((frame - 1) * sizeX), 0, (int)sizeX, (int)sizeY);
            if (drawAtScreenPosition == false)
                spriteBatch.Draw(texture, position, sourceRectangle, color * alpha, 0f, origin, 1f, SpriteEffects.None, 0f);
            else
                spriteBatch.Draw(texture, position - Main.screenPosition, sourceRectangle, color * alpha, 0f, origin, 1f, SpriteEffects.None, 0f);
        }

    }
}
