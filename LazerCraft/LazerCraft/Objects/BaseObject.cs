using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace LazerCraft
{
    public class BaseObject
    {
        #region Position and Size
        public Vector2 position { get; set; }
        public Vector2 origin { get; set; }
        public Rectangle rectangle
        {
            get
            {
                return new Rectangle((int)(position.X - origin.X), (int)(position.Y - origin.Y), (int)sizeX, (int)sizeY);
            }
        }
        public float sizeX
        {
            get
            {
                return origin.X * 2;
            }
        }
        public float sizeY
        {
            get
            {
                return origin.Y * 2;
            }
        } 
        #endregion

        #region Color and Alpha
        public Color color { get; set; }
        public float alpha { get; set; }   
        #endregion

        public BaseObject()
        {
            this.position = Vector2.Zero;
            this.origin = Vector2.Zero;
            this.color = Color.White;
            this.alpha = 1f;
        }

    }
}
