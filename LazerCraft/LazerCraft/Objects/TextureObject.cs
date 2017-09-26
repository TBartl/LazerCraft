using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LazerCraft
{
    class TextureObject : BaseObject
    {
        public Texture2D texture { get; set; }
        public bool drawAtScreenPosition {get; set;}

        public TextureObject(Texture2D texture, Vector2 position, bool useOrigin, Color color, float alpha, bool drawAtScreenPosition)
        {
            this.texture = texture;
            this.position = position;
            if (useOrigin == true)
            {
                origin = new Vector2(texture.Width / 2, texture.Height / 2);
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
            if (drawAtScreenPosition == false)
                spriteBatch.Draw(texture, position, null, color * alpha, 0f, origin, 1f, SpriteEffects.None, 0f);
            else
                spriteBatch.Draw(texture, position - Main.screenPosition, null, color * alpha, 0f, origin, 1f, SpriteEffects.None, 0f);
        }
    }
}