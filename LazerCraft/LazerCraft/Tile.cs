using System;
using Microsoft.Xna.Framework;
using System.IO;
using System.Text;
namespace LazerCraft
{
    public class Tile
    {
        public byte texturePosition;
        public byte type;
        public byte typeRotation;
        public byte textureRotation;
        static public int tileSize = 32;
        static public int halfTileSize = 16;
        public Tile(byte type, byte texturePosition, byte typeRotation, byte textureRotation)
        {
            this.type = type;
            this.texturePosition = texturePosition;
            this.typeRotation = typeRotation;
            this.textureRotation = textureRotation;
        }
    }
}
