using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Text;

namespace LazerCraft
{
    public class Level
    {
        public string levelName;
        public int numOfBackgrounds=5;
        public int xSize;
        public int ySize;
        public Tile[,] tiles;
        public Vector2 smallAdjustment;


        public Level()
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {

            for (int index = numOfBackgrounds - 1; index >= 0; index--)
            {
                //spriteBatch.Draw(Main.levelBackgrounds[index], (new Vector2(Main.levelBackgrounds[index].Width, Main.levelBackgrounds[index].Height) - Main.screenDimensions) / (new Vector2(xSize, ySize) * Tile.tileSize - Main.screenDimensions) * -Main.screenPosition, Color.White);
                spriteBatch.Draw(Main.levelBackgrounds[index], Vector2.Zero, new Rectangle((int)Main.screenPosition.X, (int)Main.screenPosition.Y, Math.Min(xSize * Tile.tileSize, (int)Main.screenDimensions.X), Math.Min(ySize * Tile.tileSize, (int)Main.screenDimensions.Y)), Color.White);
            }

            for (int xIndex = Math.Max(0,(int)(Main.screenPosition.X / Tile.tileSize)); xIndex < Math.Min((Main.screenPosition.X + Main.screenDimensions.X) / Tile.tileSize, xSize); xIndex += 1)
            {
                for (int yIndex = Math.Max(0,(int)(Main.screenPosition.Y / Tile.tileSize)); yIndex < Math.Min((Main.screenPosition.Y + Main.screenDimensions.Y) / Tile.tileSize, ySize); yIndex += 1)
                {
                    //Not a blank texture.
                    if (tiles[xIndex, yIndex].texturePosition != 0)
                    {
                        Tile t = tiles[xIndex, yIndex];
                        int matrixModifier = (t.typeRotation + t.textureRotation) % 4;
                        if (matrixModifier == (1 + t.textureRotation) % 4)
                        {
                            matrixModifier = (3 + t.textureRotation) % 4;
                        }
                        else if (matrixModifier == (3 + t.textureRotation) % 4)
                        {
                            matrixModifier = (1 + t.textureRotation) % 4;
                        }
                        if (t.type == 0)
                            spriteBatch.Draw(Main.tileTextureFull, new Vector2(xIndex * Tile.tileSize, yIndex * Tile.tileSize) - new Vector2((int)Main.screenPosition.X, (int)Main.screenPosition.Y) + new Vector2(Tile.halfTileSize, Tile.halfTileSize), new Rectangle((t.texturePosition - 1) * Tile.tileSize + Main.tileTexture.Width * (matrixModifier), (tiles[xIndex, yIndex].type) * Tile.tileSize, Tile.tileSize, Tile.tileSize), Color.White, t.typeRotation * (float)(Math.PI / 2), new Vector2(Tile.halfTileSize, Tile.halfTileSize), 1f, SpriteEffects.None, 0f);
                        else
                            spriteBatch.Draw(Main.tileTextureFull, new Vector2(xIndex * Tile.tileSize, yIndex * Tile.tileSize) - new Vector2((int)Main.screenPosition.X, (int)Main.screenPosition.Y) + new Vector2(Tile.halfTileSize, Tile.halfTileSize), new Rectangle((t.texturePosition - 1) * Tile.tileSize + Main.tileTexture.Width * (matrixModifier), (tiles[xIndex, yIndex].type - 1) * Tile.tileSize, Tile.tileSize, Tile.tileSize), Color.White, t.typeRotation * (float)(Math.PI / 2), new Vector2(Tile.halfTileSize, Tile.halfTileSize), 1f, SpriteEffects.None, 0f);
                    }
                }
            }
        }

        public void DrawEditor(SpriteBatch spriteBatch)
        {
            for (int index = numOfBackgrounds - 1; index >= 0; index--)
            {
                //TODO: Fix Backgrounds
                //spriteBatch.Draw(Main.levelBackgrounds[index], (new Vector2(Main.levelBackgrounds[index].Width, Main.levelBackgrounds[index].Height) - Main.screenDimensions) / (new Vector2(xSize, ySize) * Tile.tileSize - Main.screenDimensions) * -Main.screenPosition + new Vector2(248, 0), Color.White);
            }

            //Draw types
            for (int xIndex = Math.Max(0,(int)(Main.screenPosition.X / Tile.tileSize)); xIndex < Math.Min((Main.screenPosition.X + Main.screenDimensions.X) / Tile.tileSize, xSize); xIndex += 1)
            {
                for (int yIndex = Math.Max(0,(int)(Main.screenPosition.Y / Tile.tileSize)); yIndex < Math.Min((Main.screenPosition.Y + Main.screenDimensions.Y) / Tile.tileSize, ySize); yIndex += 1)
                {
                    //Blank texture
                    if (tiles[xIndex, yIndex].type != 0)
                    {
                        if (tiles[xIndex, yIndex].type == 10)
                        {
                            if (tiles[xIndex, yIndex].typeRotation == 0)
                                spriteBatch.Draw(Main.editorTypes, new Vector2((xIndex * Tile.tileSize + Tile.halfTileSize), (yIndex * Tile.tileSize + Tile.halfTileSize)) - Main.screenPosition, new Rectangle((int)((tiles[xIndex, yIndex].type - 1) * Tile.tileSize), 0, Tile.tileSize, Tile.tileSize), Color.Red, 0, Vector2.One * Tile.tileSize / 2f, 1f, SpriteEffects.None, 0f);
                            else if (tiles[xIndex, yIndex].typeRotation == 1)
                                spriteBatch.Draw(Main.editorTypes, new Vector2((xIndex * Tile.tileSize + Tile.halfTileSize), (yIndex * Tile.tileSize + Tile.halfTileSize)) - Main.screenPosition, new Rectangle((int)((tiles[xIndex, yIndex].type - 1) * Tile.tileSize), 0, Tile.tileSize, Tile.tileSize), Color.Green, 0, Vector2.One * Tile.tileSize / 2f, 1f, SpriteEffects.None, 0f);
                            else if (tiles[xIndex, yIndex].typeRotation == 2)
                                spriteBatch.Draw(Main.editorTypes, new Vector2((xIndex * Tile.tileSize + Tile.halfTileSize), (yIndex * Tile.tileSize + Tile.halfTileSize)) - Main.screenPosition, new Rectangle((int)((tiles[xIndex, yIndex].type - 1) * Tile.tileSize), 0, Tile.tileSize, Tile.tileSize), Color.Blue, 0, Vector2.One * Tile.tileSize / 2f, 1f, SpriteEffects.None, 0f);
                            else if (tiles[xIndex, yIndex].typeRotation == 3)
                                spriteBatch.Draw(Main.editorTypes, new Vector2((xIndex * Tile.tileSize + Tile.halfTileSize), (yIndex * Tile.tileSize + Tile.halfTileSize)) - Main.screenPosition, new Rectangle((int)((tiles[xIndex, yIndex].type - 1) * Tile.tileSize), 0, Tile.tileSize, Tile.tileSize), Color.Yellow, 0, Vector2.One * Tile.tileSize / 2f, 1f, SpriteEffects.None, 0f);
                             
                        }
                        else if (tiles[xIndex, yIndex].texturePosition == 0)
                        {
                            spriteBatch.Draw(Main.editorTypes, new Vector2((xIndex * Tile.tileSize + Tile.halfTileSize), (yIndex * Tile.tileSize + Tile.halfTileSize)) - Main.screenPosition, new Rectangle((int)((tiles[xIndex, yIndex].type - 1) * Tile.tileSize), 0, Tile.tileSize, Tile.tileSize), Color.White, tiles[xIndex, yIndex].typeRotation * (float)(Math.PI / 2), Vector2.One * Tile.tileSize / 2f, 1f, SpriteEffects.None, 0f);
                             
                        }
                    }
                    //Not a blank texture.
                    if (tiles[xIndex, yIndex].texturePosition != 0)
                    {
                        Tile t = tiles[xIndex, yIndex];
                        int matrixModifier = (t.typeRotation + t.textureRotation) % 4;
                        if (matrixModifier == (1 + t.textureRotation) % 4)
                        {
                            matrixModifier = (3 + t.textureRotation) % 4;
                        }
                        else if (matrixModifier == (3 + t.textureRotation) % 4)
                        {
                            matrixModifier = (1 + t.textureRotation) % 4;
                        }
                        if (t.type == 0)
                            spriteBatch.Draw(Main.tileTextureFull, new Vector2(xIndex * Tile.tileSize, yIndex * Tile.tileSize) - new Vector2((int)Main.screenPosition.X, (int)Main.screenPosition.Y) + new Vector2(Tile.halfTileSize, Tile.halfTileSize), new Rectangle((t.texturePosition - 1) * Tile.tileSize + Main.tileTexture.Width * (matrixModifier), (tiles[xIndex, yIndex].type) * Tile.tileSize, Tile.tileSize, Tile.tileSize), Color.White * .6f, t.typeRotation * (float)(Math.PI / 2), new Vector2(Tile.halfTileSize, Tile.halfTileSize), 1f, SpriteEffects.None, 0f);
                        else
                            spriteBatch.Draw(Main.tileTextureFull, new Vector2(xIndex * Tile.tileSize, yIndex * Tile.tileSize) - new Vector2((int)Main.screenPosition.X, (int)Main.screenPosition.Y) + new Vector2(Tile.halfTileSize, Tile.halfTileSize), new Rectangle((t.texturePosition - 1) * Tile.tileSize + Main.tileTexture.Width * (matrixModifier), (tiles[xIndex, yIndex].type - 1) * Tile.tileSize, Tile.tileSize, Tile.tileSize), Color.White, t.typeRotation * (float)(Math.PI / 2), new Vector2(Tile.halfTileSize, Tile.halfTileSize), 1f, SpriteEffects.None, 0f);
                    }
                }
            }
        }

        public void DefaultLevel()
        {
            //string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/LazerCraft/Levels/_LVLEDIT/";
            levelName = "untitled";
            string path = @"Content/Levels/_LVLEDIT/";
            using (FileStream fileStream = new FileStream(path + "/tiles.png", FileMode.Open))
            {
                Main.tileTexture = Texture2D.FromStream(Main.graphicsDevice, fileStream);
                SetFullTileTexture();
            }
            Main.levelBackgrounds = new Texture2D[numOfBackgrounds];
            for (int index = 1; index <= numOfBackgrounds; index += 1)
            {
                using (FileStream fileStream = new FileStream(path + "/background" + index.ToString() + ".png", FileMode.Open))
                {
                    Main.levelBackgrounds[index - 1] = Texture2D.FromStream(Main.graphicsDevice, fileStream);
                }
            }
            xSize = 72;
            ySize = 56;
            tiles = new Tile[xSize, ySize];
            for (int xIndex = 0; xIndex < xSize; xIndex++)
            {
                for (int yIndex = 0; yIndex < ySize; yIndex++)
                {
                    if (xIndex == 0 || yIndex == 0 || xIndex == xSize - 1 || yIndex == ySize - 1)
                        tiles[xIndex, yIndex] = new Tile(1, 0, 0, 0);
                    else
                        tiles[xIndex, yIndex] = new Tile(0, 0, 0, 0);
                }
            }
            smallAdjustment = new Vector2(Math.Max(0, Main.screenDimensions.X - xSize * Tile.tileSize), Math.Max(0, Main.screenDimensions.Y - ySize * Tile.tileSize)) / 2;
        }

        public void SaveLevel()
        {
            //string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/LazerCraft/Levels/";
            string path = @"Content/Levels/";
            if (!Directory.Exists(path + levelName + "/"))
            {
                Directory.CreateDirectory(path + levelName + "/");
                for (int index = 1; index <= numOfBackgrounds; index += 1)
                {
                    File.Copy(path + "/_LVLEDIT/background" + index + ".png", path + levelName + "/background" + index + ".png");
                }
                File.Copy(path + "/_LVLEDIT/tiles.png", path + levelName + "/tiles.png");
            }
            using (BinaryWriter writer = new BinaryWriter(File.Open(path + levelName + "/data.data", FileMode.OpenOrCreate)))
            {
                writer.Write(xSize);
                writer.Write(ySize);
                for (int yIndex = 0; yIndex < ySize; yIndex += 1)
                {
                    for (int xIndex = 0; xIndex < xSize; xIndex += 1)
                    {
                        writer.Write(tiles[xIndex, yIndex].type);
                        writer.Write(tiles[xIndex, yIndex].texturePosition);
                        writer.Write(tiles[xIndex, yIndex].typeRotation);
                        writer.Write(tiles[xIndex, yIndex].textureRotation);
                    }
                }
            }
        }

        public void LoadLevel()
        {
            //string path = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/LazerCraft/Levels/"+levelName+"/";
            string path = @"Content/Levels/" + levelName + "/";
            using (BinaryReader reader = new BinaryReader(File.Open(path + "data.data", FileMode.Open)))
            {
                xSize = reader.ReadInt32();
                ySize = reader.ReadInt32();
                tiles = new Tile[xSize, ySize];
                for (int yIndex = 0; yIndex < ySize; yIndex += 1)
                {
                    for (int xIndex = 0; xIndex < xSize; xIndex += 1)
                    {
                        tiles[xIndex, yIndex] = new Tile(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), reader.ReadByte());
                    }
                }
                smallAdjustment = new Vector2(Math.Max(0, Main.screenDimensions.X - xSize * Tile.tileSize), Math.Max(0, Main.screenDimensions.Y - ySize * Tile.tileSize)) / 2;
            }
            using (FileStream fileStream = new FileStream(path + "tiles.png", FileMode.Open))
            {
                Main.tileTexture = Texture2D.FromStream(Main.graphicsDevice, fileStream);
                SetFullTileTexture();
            }


            Main.levelBackgrounds = new Texture2D[numOfBackgrounds];
            for (int index = 1; index <= numOfBackgrounds; index += 1)
            {
                using (FileStream fileStream = new FileStream(path + "background" + index.ToString() + ".png", FileMode.Open))
                {
                    Main.levelBackgrounds[index - 1] = Texture2D.FromStream(Main.graphicsDevice, fileStream);
                }
            }
        }

        public void SetFullTileTexture()
        {
            Main.tileTextureFull = new Texture2D(Main.graphicsDevice, Main.tileTexture.Width * 4, Main.editorTypes.Width);
            Color[,] textureFullMatrix = GetColorMatrix(Main.tileTextureFull);
            Color[,] textureNormalMatrix = GetColorMatrix(Main.tileTexture);
            Color[,] typeMatrix = GetColorMatrix(Main.editorTypes);
            for (int xIndex = 0; xIndex < Main.tileTextureFull.Width; xIndex += 1)
            {
                for (int yIndex = 0; yIndex < Main.tileTextureFull.Height; yIndex += 1)
                {
                    textureFullMatrix[xIndex, yIndex] = textureNormalMatrix[xIndex % Main.tileTexture.Width, yIndex % Tile.tileSize];
                }
            }
            Color[,] originalMatrix = new Color[Main.tileTextureFull.Width, Main.tileTextureFull.Height];
            for (int xIndex = 0; xIndex < Main.tileTextureFull.Width; xIndex += 1)
            {
                for (int yIndex = 0; yIndex < Main.tileTextureFull.Height; yIndex += 1)
                {
                    originalMatrix[xIndex, yIndex] = new Color(textureFullMatrix[xIndex, yIndex].R, textureFullMatrix[xIndex, yIndex].G, textureFullMatrix[xIndex, yIndex].B, textureFullMatrix[xIndex, yIndex].A);
                }
            }
            for (int rotation = 1; rotation < 4; rotation += 1)
            {
                for (int xSection = 0; xSection < Main.tileTextureFull.Width / Tile.tileSize / 4; xSection += 1)
                {
                    for (int ySection = 0; ySection < Main.tileTextureFull.Height / Tile.tileSize; ySection += 1)
                    {
                        for (int xIndex = 0; xIndex < Tile.tileSize; xIndex += 1)
                        {
                            for (int yIndex = 0; yIndex < Tile.tileSize; yIndex += 1)
                            {
                                int x = rotation * Main.tileTexture.Width + xSection * Tile.tileSize + xIndex;
                                int y = ySection * Tile.tileSize + yIndex;
                                int newX = xIndex - Tile.halfTileSize;
                                int newY = yIndex - Tile.halfTileSize;
                                int temp = newX;
                                if (rotation == 1)
                                {
                                    newX = newY;
                                    newY = -temp;
                                    newX += rotation * Main.tileTexture.Width + xSection * Tile.tileSize + Tile.halfTileSize;
                                    newY += ySection * Tile.tileSize + Tile.halfTileSize - 1;
                                    textureFullMatrix[x, y] = originalMatrix[newX, newY];
                                }
                                else if (rotation == 2)
                                {
                                    newX = -newX;
                                    newY = -newY;
                                    newX += rotation * Main.tileTexture.Width + xSection * Tile.tileSize + Tile.halfTileSize - 1;
                                    newY += ySection * Tile.tileSize + Tile.halfTileSize - 1;
                                    textureFullMatrix[x, y] = originalMatrix[newX, newY];
                                }
                                else if (rotation == 3)
                                {
                                    newX = -newY;
                                    newY = temp;
                                    newX += rotation * Main.tileTexture.Width + xSection * Tile.tileSize + Tile.halfTileSize - 1;
                                    newY += ySection * Tile.tileSize + Tile.halfTileSize;
                                    textureFullMatrix[x, y] = originalMatrix[newX, newY];
                                }

                            }
                        }
                    }
                }
            }
            for (int xSection = 0; xSection < Main.tileTextureFull.Width / Tile.tileSize; xSection += 1)
            {
                for (int ySection = 0; ySection < Main.tileTextureFull.Height / Tile.tileSize; ySection += 1)
                {
                    for (int xIndex = 0; xIndex < Tile.tileSize; xIndex += 1)
                    {
                        for (int yIndex = 0; yIndex < Tile.tileSize; yIndex += 1)
                        {
                            textureFullMatrix[xSection * Tile.tileSize + xIndex, ySection * Tile.tileSize + yIndex].R = (byte)(textureFullMatrix[xSection * Tile.tileSize + xIndex, ySection * Tile.tileSize + yIndex].R * (typeMatrix[ySection * Tile.tileSize + xIndex, yIndex].A / 255f));
                            textureFullMatrix[xSection * Tile.tileSize + xIndex, ySection * Tile.tileSize + yIndex].G = (byte)(textureFullMatrix[xSection * Tile.tileSize + xIndex, ySection * Tile.tileSize + yIndex].G * (typeMatrix[ySection * Tile.tileSize + xIndex, yIndex].A / 255f));
                            textureFullMatrix[xSection * Tile.tileSize + xIndex, ySection * Tile.tileSize + yIndex].B = (byte)(textureFullMatrix[xSection * Tile.tileSize + xIndex, ySection * Tile.tileSize + yIndex].B * (typeMatrix[ySection * Tile.tileSize + xIndex, yIndex].A / 255f));
                            textureFullMatrix[xSection * Tile.tileSize + xIndex, ySection * Tile.tileSize + yIndex].A = (byte)(textureFullMatrix[xSection * Tile.tileSize + xIndex, ySection * Tile.tileSize + yIndex].A * (typeMatrix[ySection * Tile.tileSize + xIndex, yIndex].A / 255f));

                        }
                    }
                }
            }
            Main.tileTextureFull.SetData<Color>(ConvertColorToArray(textureFullMatrix));

        }

        public Color[,] GetColorMatrix(Texture2D texture)
        {
            Color[] array = new Color[texture.Width * texture.Height];
            texture.GetData<Color>(array);
            Color[,] matrix = new Color[texture.Width, texture.Height];
            for (int index = 0; index < texture.Width * texture.Height; index += 1)
            {
                matrix[index % texture.Width, index / texture.Width] = array[index];
            }
            return matrix;
        }

        public Color[] ConvertColorToArray(Color[,] matrix)
        {
            Color[] array = new Color[matrix.GetLength(0) * matrix.GetLength(1)];
            for (int yIndex = 0; yIndex < matrix.GetLength(1); yIndex += 1)
            {
                for (int xIndex = 0; xIndex < matrix.GetLength(0); xIndex += 1)
                {
                    array[xIndex + yIndex * matrix.GetLength(0)] = matrix[xIndex, yIndex];
                }
            }
            return array;
        }

        public bool CheckTile(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < xSize && y < ySize)
                return true;
            else
                return false;
        }

        public Vector2 GetSpawn(Player p)
        {
            List<Vector2> possibleSpawns = new List<Vector2>();
            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    if (tiles[x, y].type == 10)
                    {
                        if (p.team == -1 || p.team == 0 || p.team == tiles[x, y].typeRotation)
                        {
                            possibleSpawns.Add(new Vector2(x+.5f,y+.5f)*Tile.tileSize);
                        }
                    }
                }
            }
            if (possibleSpawns.Count > 0)
                return possibleSpawns[Main.random.Next(0,possibleSpawns.Count)];
            return Vector2.Zero;
        }
    }
}
