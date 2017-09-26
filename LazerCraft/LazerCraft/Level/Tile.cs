using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LazerCraft
{
    public enum TileType
    {
        None, Basic, UpLeft, UpRight, DownRight, DownLeft
    }
    public class Tile
    {
        public SpriteSheetObject properties { get; set; }
        public SpriteSheetObject outline { get; set; }
        int outlineFrame;
        public TileType type { get; set; }

        public Tile(Texture2D texture, Vector2 position, TileType type,int totalSourceFrames, int sourceFrame)
        {
            this.properties = new SpriteSheetObject(texture, position, true, Color.White, 1f, true, totalSourceFrames, sourceFrame);
            if (sourceFrame == 0)
            {
                properties.alpha = 0f;
            }
            this.type = type;
        }

        public void SetSides(bool up, bool down, bool left, bool right)
        {
            if (type == TileType.None && properties.frame == 0)
                outlineFrame = 1;

            else if (type == TileType.None && properties.frame != 0)
                outlineFrame = 17;

            else if (up && !down && !left && !right)
                outlineFrame = 2;

            else if (!up && down && !left && !right)
                outlineFrame = 3;

            else if (!up && !down && left && !right)
                outlineFrame = 4;

            else if (!up && !down && !left && right)
                outlineFrame = 5;

            else if (up && down && !left && !right)
                outlineFrame = 6;

            else if (up && !down && left && !right)
                outlineFrame = 7;

            else if (up && !down && !left && right)
                outlineFrame = 8;

            else if (!up && down && left && !right)
                outlineFrame = 9;

            else if (!up && down && !left && right)
                outlineFrame = 10;

            else if (!up && !down && left && right)
                outlineFrame = 11;

            else if (up && down && left && !right)
                outlineFrame = 12;

            else if (up && down && !left && right)
                outlineFrame = 13;

            else if (up && !down && left && right)
                outlineFrame = 14;

            else if (!up && down && left && right)
                outlineFrame = 15;

            else if (up && down && left && right)
                outlineFrame = 16;


            outline = new SpriteSheetObject(Main.outline, properties.position, true, Color.White, 1f, true, 17, outlineFrame);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            properties.Draw(spriteBatch);
            outline.Draw(spriteBatch);
        }
    }
}
