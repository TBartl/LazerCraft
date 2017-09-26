using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace LazerCraft
{
    public class Level
    {
        public string folderName;
        public Tile[,] Map { get; set; }
        public int xSize { get; set; }
        public int ySize { get; set; }

        Texture2D spriteSheet {get; set;}
        int spriteSheetFrames { get; set; }

        TextureObject[] backgrounds = new TextureObject[3];

        List<string> lines;
        
        int yPos;
        int xPos;
        Vector2 realPosition;
        Tile currentTile;
        string currentTileType;
        TileType enum_currentTileType;
        string currentSheetLocation;
        int int_currentSheetLocation;
        bool typeOrLocation;

        int xSideIndex;
        int ySideIndex;
        bool up;
        bool down;
        bool left;
        bool right;
        
        public Level(string folderName)
        {
            using (FileStream fileStream = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LazerCraft/" + folderName + "/tiles.png"), FileMode.Open))
            {
                spriteSheet = Texture2D.FromStream(Main.graphicsDevice, fileStream);
            }
            spriteSheetFrames = spriteSheet.Width / 32;

            for (int index = 0; index < backgrounds.GetLength(0); index += 1)
            {
                using (FileStream fileStream = new FileStream(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LazerCraft/" + folderName + "/background" + index.ToString() + ".png"), FileMode.Open))
                {
                    backgrounds[index] = new TextureObject(Texture2D.FromStream(Main.graphicsDevice, fileStream), Vector2.Zero, true, Color.White, 1f, false);
                }
            }
            lines = new List<string>(File.ReadAllLines(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "LazerCraft/" + folderName + "/level.txt")));
            for (int index = 0; index < lines[0].Length; index++)
            {
                if (lines[0][index] == '(')
                {
                    xSize++;
                }   
            }
            Map = new Tile[xSize, lines.Count];
            xSize = xSize * 32;
            ySize = Map.GetLength(1) * 32;
            yPos = 0;
            foreach (string line in lines)
            {
                xPos = 0;
                foreach (char character in line)
                {
                    switch (character)
                    {
                        case ' ':
                            break;
                        case '(':
                            typeOrLocation = true;
                            currentTileType = String.Empty;
                            currentSheetLocation = String.Empty;
                            break;
                        case ',':
                            typeOrLocation = false;
                            break;
                        case ')':
                            int_currentSheetLocation = Convert.ToInt16(currentSheetLocation);
                            if (currentTileType == "0")
                            {
                                enum_currentTileType = TileType.None;
                            }
                            else if (currentTileType == "1")
                            {
                                enum_currentTileType = TileType.Basic;
                            }
                            else
                            {
                                enum_currentTileType = TileType.Basic;
                            }
                            realPosition = new Vector2(xPos * 32 + 16, yPos * 32 + 16);
                            currentTile = new Tile(spriteSheet, realPosition, enum_currentTileType, spriteSheetFrames, int_currentSheetLocation);
                            Map[xPos, yPos] = currentTile;
                            xPos++;
                            break;
                        default:
                            if (typeOrLocation == true)
                                currentTileType += character;
                            else
                                currentSheetLocation += character;
                            break;
                    }
                }
                yPos++;
            }
            while (xSideIndex < Map.GetLength(0))
            {
                ySideIndex = 0;
                while (ySideIndex < Map.GetLength(1))
                {
                    if (ySideIndex == 0)
                        up = false;
                    else
                        up = checkForLine(0, -1);

                    if (ySideIndex == Map.GetLength(1) -1)
                        down = false;
                    else
                        down = checkForLine(0, 1);

                    if (xSideIndex == 0)
                        left = false;
                    else
                        left = checkForLine(-1, 0);

                    if (xSideIndex == Map.GetLength(0) - 1)
                        down = false;
                    else
                        right = checkForLine(1, 0);

                    Map[xSideIndex, ySideIndex].SetSides(up, down, left, right);
                    ySideIndex += 1;
                }
                xSideIndex += 1;
            }
        }

        public bool checkForLine(int xCompare,int yCompare)
        {
            if (Map[xSideIndex + xCompare, ySideIndex + yCompare].type == TileType.Basic && Map[xSideIndex, ySideIndex].properties.frame == Map[xSideIndex + xCompare, ySideIndex + yCompare].properties.frame)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for(int xIndex = 0; xIndex < Map.GetLength(0); xIndex += 1)
            {
                for (int yIndex = 0; yIndex < Map.GetLength(1); yIndex += 1)
                {
                    Map[xIndex, yIndex].Draw(spriteBatch);
                }
            }
        }
    }
}
