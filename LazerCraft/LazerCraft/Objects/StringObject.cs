using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LazerCraft
{
    class StringObject : BaseObject
    {
        public SpriteFont font { get; set; }
        public string message{ get; set;}

        public StringObject(SpriteFont font, string message, Vector2 position, bool useOrigin, Color color, float alpha)
        {
            this.font = font;
            this.message = message;
            this.position = position;
            if (useOrigin == true)
            {
                origin = font.MeasureString(message) / 2;
            }
            else
            {
                origin = Vector2.Zero;
            }
            this.color = color;
            this.alpha = alpha;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(font, message, position, color * alpha, 0f, origin, 1f, SpriteEffects.None, 0f);
        }
        public void ChangeMessage(string newMessage)
        {
            message = newMessage;
            origin = font.MeasureString(message) / 2;
        }

    }
}
