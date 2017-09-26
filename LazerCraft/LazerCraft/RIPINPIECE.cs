using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;
using System.Text;

namespace LazerCraft
{
    class RIPINPIECE
    {
        /*
        protected Tuple<Vector2, Vector2> MapCollision(float size, Vector2 position, Vector2 velocity, bool special)
        {
            float distanceRemaining = velocity.Length();
            float smallestDistance;
            Vector2 newVelocity;
            Vector2 lastPlane;
            while (distanceRemaining > 0f)
            {
                smallestDistance = distanceRemaining;
                newVelocity = velocity;
                lastPlane = Vector2.Zero;
                for (int xIndex = (int)(Math.Min(position.X - size, (position + velocity).X - size) / tileSize); xIndex <= (int)(Math.Max(position.X + size, (position + velocity).X + size) / tileSize); xIndex++)
                {
                    for (int yIndex = (int)(Math.Min(position.Y - size, (position + velocity).Y - size) / tileSize); yIndex <= (int)(Math.Max(position.Y + size, (position + velocity).Y + size) / tileSize); yIndex++)
                    {
                        if (xIndex >= 0 && xIndex < xSize && yIndex >= 0 && yIndex < ySize)
                        {
                            #region Top Axis
                            if (levelTileTypes[xIndex, yIndex] == 1 || levelTileTypes[xIndex, yIndex] == 4 || levelTileTypes[xIndex, yIndex] == 5)
                            {
                                Vector2 slopeToCheck = new Vector2(1, 0);
                                Vector2 collisionPoint = new Vector2(position.X, position.Y + size);
                                Vector2 hitPoint = GetIntersection(velocity, collisionPoint, slopeToCheck, new Vector2(xIndex * tileSize, yIndex * tileSize));
                                if (hitPoint.X >= xIndex * 32f && hitPoint.X <= (xIndex + 1) * 32f)
                                {
                                    float distanceToCheck = Vector2.Distance(collisionPoint, hitPoint);
                                    Tuple<float, Vector2, Vector2> newComponents = FinalizeHit(velocity, slopeToCheck, special, distanceToCheck, smallestDistance, newVelocity, lastPlane);
                                    smallestDistance = newComponents.Item1;
                                    newVelocity = newComponents.Item2;
                                    lastPlane = newComponents.Item3;
                                }
                            }
                            #endregion
                            #region Bottom Axis
                            if (levelTileTypes[xIndex, yIndex] == 1 || levelTileTypes[xIndex, yIndex] == 2 || levelTileTypes[xIndex, yIndex] == 3)
                            {
                                Vector2 slopeToCheck = new Vector2(-1, 0);
                                Vector2 collisionPoint = new Vector2(position.X, position.Y - size);
                                Vector2 hitPoint = GetIntersection(velocity, collisionPoint, slopeToCheck, new Vector2((xIndex + 1) * tileSize, (yIndex + 1) * tileSize));
                                if (hitPoint.X >= xIndex * 32f && hitPoint.X <= (xIndex + 1) * 32f)
                                {
                                    float distanceToCheck = Vector2.Distance(collisionPoint, hitPoint);
                                    Tuple<float, Vector2, Vector2> newComponents = FinalizeHit(velocity, slopeToCheck, special, distanceToCheck, smallestDistance, newVelocity, lastPlane);
                                    smallestDistance = newComponents.Item1;
                                    newVelocity = newComponents.Item2;
                                    lastPlane = newComponents.Item3;
                                }
                            }
                            #endregion
                            #region Left Axis
                            if (levelTileTypes[xIndex, yIndex] == 1 || levelTileTypes[xIndex, yIndex] == 3 || levelTileTypes[xIndex, yIndex] == 5)
                            {
                                Vector2 slopeToCheck = new Vector2(0, -1);
                                Vector2 collisionPoint = new Vector2(position.X + size, position.Y);
                                Vector2 hitPoint = GetIntersection(velocity, collisionPoint, slopeToCheck, new Vector2(xIndex * tileSize, yIndex * tileSize));
                                if (hitPoint.Y >= yIndex * 32f && hitPoint.Y <= (yIndex + 1) * 32f)
                                {
                                    float distanceToCheck = Vector2.Distance(collisionPoint, hitPoint);
                                    Tuple<float, Vector2, Vector2> newComponents = FinalizeHit(velocity, slopeToCheck, special, distanceToCheck, smallestDistance, newVelocity, lastPlane);
                                    smallestDistance = newComponents.Item1;
                                    newVelocity = newComponents.Item2;
                                    lastPlane = newComponents.Item3;
                                }
                            }
                            #endregion
                            #region Right Axis
                            if (levelTileTypes[xIndex, yIndex] == 1 || levelTileTypes[xIndex, yIndex] == 2 || levelTileTypes[xIndex, yIndex] == 4)
                            {
                                Vector2 slopeToCheck = new Vector2(0, 1);
                                Vector2 collisionPoint = new Vector2(position.X - size, position.Y);
                                Vector2 hitPoint = GetIntersection(velocity, collisionPoint, slopeToCheck, new Vector2((xIndex + 1) * tileSize, yIndex * tileSize));
                                if (hitPoint.Y >= yIndex * 32f && hitPoint.Y <= (yIndex + 1) * 32f)
                                {
                                    float distanceToCheck = Vector2.Distance(collisionPoint, hitPoint);
                                    Tuple<float, Vector2, Vector2> newComponents = FinalizeHit(velocity, slopeToCheck, special, distanceToCheck, smallestDistance, newVelocity, lastPlane);
                                    smallestDistance = newComponents.Item1;
                                    newVelocity = newComponents.Item2;
                                    lastPlane = newComponents.Item3;
                                }
                            }
                            #endregion

                            #region Top Left Axis
                            if (levelTileTypes[xIndex, yIndex] == 2)
                            {
                                Vector2 collisionPoint = new Vector2(position.X + size / (float)Math.Sqrt(2), position.Y + size / (float)Math.Sqrt(2));
                                Vector2 hitPoint;
                                if (velocity.X == 0)
                                {
                                    hitPoint.X = collisionPoint.X;
                                }
                                else
                                {
                                    hitPoint.X = ((velocity.Y / velocity.X) * collisionPoint.X - collisionPoint.Y + (xIndex + yIndex + 1) * tileSize) / (velocity.Y / velocity.X + 1);
                                }
                                hitPoint.Y = -hitPoint.X + (xIndex + yIndex + 1) * tileSize;
                                if (hitPoint.X >= xIndex * 32f && hitPoint.X <= (xIndex + 1) * 32f)
                                {
                                    float distanceToCheck = Vector2.Distance(collisionPoint, hitPoint);
                                    Tuple<float, Vector2, Vector2> newComponents = FinalizeHit(velocity, new Vector2(1, -1), special, distanceToCheck, smallestDistance, newVelocity, lastPlane);
                                    smallestDistance = newComponents.Item1;
                                    newVelocity = newComponents.Item2;
                                    lastPlane = newComponents.Item3;
                                }
                            }
                            #endregion
                            #region Top Right Axis
                            if (levelTileTypes[xIndex, yIndex] == 3)
                            {
                                Vector2 collisionPoint = new Vector2(position.X - size / (float)Math.Sqrt(2), position.Y + size / (float)Math.Sqrt(2));
                                Vector2 hitPoint;
                                if (velocity.X == 0)
                                {
                                    hitPoint.X = collisionPoint.X;
                                }
                                else
                                {
                                    hitPoint.X = ((velocity.Y / velocity.X) * collisionPoint.X - collisionPoint.Y + (-xIndex + yIndex) * tileSize) / (velocity.Y / velocity.X - 1);
                                }
                                hitPoint.Y = hitPoint.X + (-xIndex + yIndex) * tileSize;
                                if (collisionPoint.Y <= collisionPoint.X + (-xIndex + yIndex) * tileSize && collisionPoint.Y + velocity.Y >= collisionPoint.X + velocity.X + (-xIndex + yIndex) * tileSize && hitPoint.X >= xIndex * 32f && hitPoint.X <= (xIndex + 1) * 32f && (special == true || Vector2.Distance(collisionPoint, hitPoint) > Vector2.Distance(collisionPoint + velocity * .001f, hitPoint)))
                                {
                                    float distanceToCheck = Vector2.Distance(collisionPoint, hitPoint);
                                    Tuple<float, Vector2, Vector2> newComponents = FinalizeHit(velocity, new Vector2(1, 1), special, distanceToCheck, smallestDistance, newVelocity, lastPlane);
                                    smallestDistance = newComponents.Item1;
                                    newVelocity = newComponents.Item2;
                                    lastPlane = newComponents.Item3;
                                }
                            }
                            #endregion
                            #region Bottom Left Axis

                            if (levelTileTypes[xIndex, yIndex] == 4)
                            {
                                Vector2 collisionPoint = new Vector2(position.X + size / (float)Math.Sqrt(2), position.Y - size / (float)Math.Sqrt(2));
                                Vector2 hitPoint;
                                if (velocity.X == 0)
                                {
                                    hitPoint.X = collisionPoint.X;
                                }
                                else
                                {
                                    hitPoint.X = ((velocity.Y / velocity.X) * collisionPoint.X - collisionPoint.Y + (-xIndex + yIndex) * tileSize) / (velocity.Y / velocity.X - 1);
                                }
                                hitPoint.Y = hitPoint.X + (-xIndex + yIndex) * tileSize;
                                if (collisionPoint.Y >= collisionPoint.X + (-xIndex + yIndex) * tileSize && collisionPoint.Y + velocity.Y <= collisionPoint.X + velocity.X + (-xIndex + yIndex) * tileSize && hitPoint.X >= xIndex * 32f && hitPoint.X <= (xIndex + 1) * 32f && (special == true || Vector2.Distance(collisionPoint, hitPoint) > Vector2.Distance(collisionPoint + velocity * .001f, hitPoint)))
                                {
                                    float distanceToCheck = Vector2.Distance(collisionPoint, hitPoint);
                                    Tuple<float, Vector2, Vector2> newComponents = FinalizeHit(velocity, new Vector2(-1, -1), special, distanceToCheck, smallestDistance, newVelocity, lastPlane);
                                    smallestDistance = newComponents.Item1;
                                    newVelocity = newComponents.Item2;
                                    lastPlane = newComponents.Item3;
                                }
                            }
                            #endregion
                            #region Bottom Right Axis
                            if (levelTileTypes[xIndex, yIndex] == 5)
                            {
                                Vector2 collisionPoint = new Vector2(position.X - size / (float)Math.Sqrt(2), position.Y - size / (float)Math.Sqrt(2));
                                Vector2 hitPoint;
                                if (velocity.X == 0)
                                {
                                    hitPoint.X = collisionPoint.X;
                                }
                                else
                                {
                                    hitPoint.X = ((velocity.Y / velocity.X) * collisionPoint.X - collisionPoint.Y + (xIndex + yIndex + 1) * tileSize) / (velocity.Y / velocity.X + 1);
                                }
                                hitPoint.Y = -hitPoint.X + (xIndex + yIndex + 1) * tileSize;
                                if (collisionPoint.Y >= -collisionPoint.X + (xIndex + yIndex + 1) * tileSize && collisionPoint.Y + velocity.Y <= -collisionPoint.X - velocity.X + (xIndex + yIndex + 1) * tileSize && hitPoint.X >= xIndex * 32f && hitPoint.X <= (xIndex + 1) * 32f && (special == true || Vector2.Distance(collisionPoint, hitPoint) > Vector2.Distance(collisionPoint + velocity * .001f, hitPoint)))
                                {
                                    float distanceToCheck = Vector2.Distance(collisionPoint, hitPoint);
                                    Tuple<float, Vector2, Vector2> newComponents = FinalizeHit(velocity, new Vector2(-1, 1), special, distanceToCheck, smallestDistance, newVelocity, lastPlane);
                                    smallestDistance = newComponents.Item1;
                                    newVelocity = newComponents.Item2;
                                    lastPlane = newComponents.Item3;
                                }
                            }
                            #endregion


                            #region Points
                            for (int xChange = 0; xChange <= 1; xChange += 1)
                            {
                                for (int yChange = 0; yChange <= 1; yChange += 1)
                                {
                                    if (((xChange == 0 && yChange == 0) && (levelTileTypes[xIndex, yIndex] == 1 || levelTileTypes[xIndex, yIndex] == 3 || levelTileTypes[xIndex, yIndex] == 4 || levelTileTypes[xIndex, yIndex] == 5))
                                        || ((xChange == 1 && yChange == 0) && (levelTileTypes[xIndex, yIndex] == 1 || levelTileTypes[xIndex, yIndex] == 2 || levelTileTypes[xIndex, yIndex] == 4 || levelTileTypes[xIndex, yIndex] == 5))
                                        || ((xChange == 0 && yChange == 1) && (levelTileTypes[xIndex, yIndex] == 1 || levelTileTypes[xIndex, yIndex] == 2 || levelTileTypes[xIndex, yIndex] == 3 || levelTileTypes[xIndex, yIndex] == 5))
                                        || ((xChange == 1 && yChange == 1) && (levelTileTypes[xIndex, yIndex] == 1 || levelTileTypes[xIndex, yIndex] == 2 || levelTileTypes[xIndex, yIndex] == 3 || levelTileTypes[xIndex, yIndex] == 4)))
                                    {
                                        Vector2 corner = new Vector2((xIndex + xChange) * tileSize, (yIndex + yChange) * tileSize);
                                        Vector2 closestPointToCorner;
                                        closestPointToCorner = GetIntersection(velocity, position, new Vector2(-velocity.Y, velocity.X), new Vector2((xIndex + xChange) * tileSize, (yIndex + yChange) * tileSize));
                                        float distanceFromCornerToClosest = Vector2.Distance(closestPointToCorner, corner);
                                        if (distanceFromCornerToClosest <= size)
                                        {
                                            float distanceToCheck = Vector2.Distance(position, closestPointToCorner) - (float)Math.Sqrt(size * size - distanceFromCornerToClosest * distanceFromCornerToClosest);
                                            Vector2 newPosition = position + (smallestDistance / distanceRemaining) * velocity;
                                            Tuple<float, Vector2, Vector2> newComponents = FinalizeHit(velocity, new Vector2(corner.Y - newPosition.Y, newPosition.X - corner.X), special, distanceToCheck, smallestDistance, newVelocity, lastPlane);
                                            smallestDistance = newComponents.Item1;
                                            newVelocity = newComponents.Item2;
                                            lastPlane = newComponents.Item3;
                                        }
                                    }
                                }
                            }
                            #endregion


                        }
                    }
                }
                position += (smallestDistance / velocity.Length()) * velocity;
                distanceRemaining -= smallestDistance;
                distanceRemaining *= newVelocity.Length() / velocity.Length();

                velocity = newVelocity;
            }
            return new Tuple<Vector2, Vector2>(position, velocity);
        }

        protected Tuple<float, Vector2, Vector2> FinalizeHit(Vector2 velocity, Vector2 plane, bool special, float thisDistance, float smallestDistance, Vector2 oldVelocity, Vector2 lastPlane)
        {
            float floatToReturn = smallestDistance;
            Vector2 vectorToReturn = oldVelocity;
            Vector2 planeToReturn = lastPlane;
            if (thisDistance > 0 && thisDistance <= smallestDistance + .00001f)
            {
                plane = Vector2.Normalize(plane);
                if (thisDistance >= smallestDistance - .00001f)
                    plane = (lastPlane + plane) / 2f;
                floatToReturn = thisDistance;
                Vector2 rotatedVelocity = RotateVelocity(velocity, plane, special);
                if (rotatedVelocity.Y < 0)
                {
                    floatToReturn = smallestDistance;
                    vectorToReturn = oldVelocity;
                }
                else
                {
                    if (special == true)
                    {
                        if (plane.X > .9f)
                            //myPlayer.onGround = true;
                        if (plane.Y > .9f || plane.Y < -.9f)
                            //myPlayer.onWall = true;
                        if (rotatedVelocity.Y < .5f)
                        {
                            rotatedVelocity = new Vector2(rotatedVelocity.X, -.0001f);
                        }
                        else
                        {
                            rotatedVelocity = new Vector2(rotatedVelocity.X, -.2f * rotatedVelocity.Y);
                        }
                    }
                    else
                    {
                        rotatedVelocity = new Vector2(rotatedVelocity.X, rotatedVelocity.Y);
                    }
                }
                rotatedVelocity = RotateVelocity(rotatedVelocity, plane, false);
                vectorToReturn = rotatedVelocity;
                planeToReturn = plane;
            }
            return new Tuple<float, Vector2, Vector2>(floatToReturn, vectorToReturn, plane);
        }

        protected Vector2 RotateVelocity(Vector2 velocity, Vector2 plane, bool toAxis)
        {
            plane = Vector2.Normalize(plane);
            if (toAxis == true)
                return new Vector2(plane.X * velocity.X + plane.Y * velocity.Y, -plane.Y * velocity.X + plane.X * velocity.Y);
            else
                return new Vector2(velocity.X * plane.X - velocity.Y * plane.Y, velocity.X * plane.Y + velocity.Y * plane.X);
        }

        protected Vector2 GetIntersection(Vector2 slope1, Vector2 p1, Vector2 slope2, Vector2 p2)
        {
            Vector2 point;
            if (slope1.X == 0 || slope2.X == 0)
            {
                float m1 = (slope1.X / slope1.Y);
                float m2 = (slope2.X / slope2.Y);
                if (slope1.X == 0f)
                {
                    point.Y = p1.Y;
                    point.X = m2 * (point.Y - p2.Y) + p2.X;
                }
                else
                {
                    point.Y = p2.Y;
                    point.X = m1 * (point.Y - p1.Y) + p1.X;
                }
            }
            else
            {
                float m1 = (slope1.Y / slope1.X);
                float m2 = (slope2.Y / slope2.X);
                if (slope1.X == 0f)
                {
                    point.X = p1.X;
                    point.Y = m2 * (point.X - p2.X) + p2.Y;
                }
                else if (slope2.X == 0f)
                {
                    point.X = p2.X;
                    point.Y = m1 * (point.X - p1.X) + p1.Y;
                }
                else
                {
                    point.X = (m1 * p1.X - p1.Y - m2 * p2.X + p2.Y) / (m1 - m2);
                    point.Y = m1 * (point.X - p1.X) + p1.Y;
                }
            }
            return point;
        }

        #region Game Screen / Editor
        Texture2D levelEditHud;
        Texture2D levelEditArrow;
        Texture2D levelEditIconBorders;
        Texture2D levelEditIcons;
        Texture2D tileTypeTexture;
        bool editTileType;
        int tileTypeChange;
        int tileTextureChange;
        #endregion

        spriteBatch.Draw(levelEditHud, new Vector2(409, 504), currentColor);
                spriteBatch.Draw(levelEditArrow, new Vector2(592, 510), currentColor);
                spriteBatch.Draw(levelEditIcons, new Vector2(410, 504), Color.White);
                spriteBatch.Draw(levelEditIconBorders, new Vector2(417, 510), currentColor);
                spriteBatch.Draw(levelEditIconBorders, new Vector2(417, 528), currentColor);
                for (int index = 0; index < 4; index += 1)
                {
                    if (editTileType == true)
                    {
                        if (tileTypeChange + index > tileTypeTexture.Width / tileSize)
                            spriteBatch.Draw(tileTypeTexture, new Vector2(445 + index * 34, 513), new Rectangle((tileTypeChange + index - tileTypeTexture.Width / (int)tileSize) * (int)tileSize, 0, (int)tileSize, (int)tileSize), Color.White);
                        else
                            spriteBatch.Draw(tileTypeTexture, new Vector2(445 + index * 34, 513), new Rectangle((tileTypeChange + index) * (int)tileSize, 0, (int)tileSize, (int)tileSize), Color.White);
                    }
                    else
                    {
                        if (tileTextureChange + index > levelTileTexture.Width / tileSize)
                            spriteBatch.Draw(levelTileTexture, new Vector2(445 + index * 34, 513), new Rectangle((tileTextureChange + index - levelTileTexture.Width / (int)tileSize) * (int)tileSize, 0, (int)tileSize, (int)tileSize), Color.White);
                        else
                            spriteBatch.Draw(levelTileTexture, new Vector2(445 + index * 34, 513), new Rectangle((tileTextureChange + index) * (int)tileSize, 0, (int)tileSize, (int)tileSize), Color.White);
                    }
                }
        #region Map Editor
            else if (currentScreen == screen.MapEditor)
            {
                if (currentKeyboardState.IsKeyDown(Keys.W) && screenPosition.Y > 0)
                    screenPosition.Y -= 4;
                if (currentKeyboardState.IsKeyDown(Keys.A) && screenPosition.X > 0)
                    screenPosition.X -= 4;
                if (currentKeyboardState.IsKeyDown(Keys.S) && screenPosition.Y + screenDimensions.Y < ySize * tileSize)
                    screenPosition.Y += 4;
                if (currentKeyboardState.IsKeyDown(Keys.D) && screenPosition.X + screenDimensions.X < xSize * tileSize)
                    screenPosition.X += 4;
                if (currentKeyboardState.IsKeyDown(Keys.E) && !lastKeyboardState.IsKeyDown(Keys.E))
                {
                    editTileType = !editTileType;
                }
                if (currentMouseRectangle.Intersects(new Rectangle(409, 504, 206, 48)))
                {
                    if ((currentMouseState.LeftButton == ButtonState.Pressed && lastMouseState.LeftButton != ButtonState.Pressed))
                    {
                        if (currentMouseRectangle.Intersects(new Rectangle(592, 510, 19, 38)))
                        {
                            if (editTileType == true)
                            {
                                tileTypeChange += 1;
                                if (tileTypeChange > tileTypeTexture.Width / tileSize - 1)
                                    tileTypeChange = 0;
                            }
                            else
                            {
                                tileTextureChange += 1;
                                if (tileTextureChange > levelTileTexture.Width / tileSize - 1)
                                    tileTextureChange = 0;
                            }

                        }
                        else if (currentMouseRectangle.Intersects(new Rectangle(417, 510, 18, 18)))
                        {
                            editTileType = true;
                        }
                        else if (currentMouseRectangle.Intersects(new Rectangle(417, 528, 18, 18)))
                        {
                            editTileType = false;
                        }
                    }
                }
                else
                {
                    if ((currentMouseState.LeftButton == ButtonState.Pressed) && currentMouseRectangle.X + (int)screenPosition.X >= 0 && currentMouseRectangle.Y + (int)screenPosition.Y >= 0 && currentMouseRectangle.X + (int)screenPosition.X < xSize * 32f && currentMouseRectangle.Y + (int)screenPosition.Y < ySize * 32f)
                    {
                        if (editTileType == true)
                        {
                            if (tileTypeChange > tileTypeTexture.Width / tileSize)
                                levelTileTypes[(currentMouseRectangle.X + (int)screenPosition.X) / (int)tileSize, (currentMouseRectangle.Y + (int)screenPosition.Y) / (int)tileSize] = tileTextureChange - tileTypeTexture.Width / (int)tileSize;
                            else
                                levelTileTypes[(currentMouseRectangle.X + (int)screenPosition.X) / (int)tileSize, (currentMouseRectangle.Y + (int)screenPosition.Y) / (int)tileSize] = tileTypeChange;
                        }
                        else
                        {
                            if (tileTextureChange > levelTileTexture.Width / tileSize)
                                levelTileTextures[(currentMouseRectangle.X + (int)screenPosition.X) / (int)tileSize, (currentMouseRectangle.Y + (int)screenPosition.Y) / (int)tileSize] = tileTextureChange - levelTileTexture.Width / (int)tileSize;
                            else
                                levelTileTextures[(currentMouseRectangle.X + (int)screenPosition.X) / (int)tileSize, (currentMouseRectangle.Y + (int)screenPosition.Y) / (int)tileSize] = tileTextureChange;
                        }
                    }
                }
            }
            #endregion
    levelEditHud = Content.Load<Texture2D>(@"Game/Game_Objects/levelEditHud");//, PositionScaled(512, 528), true, currentColor, 1f, false);
            levelEditArrow = Content.Load<Texture2D>(@"Game/Game_Objects/levelEditArrow");//, levelEditHud.position, true, currentColor, 1f, false);
            levelEditIconBorders = Content.Load<Texture2D>(@"Game/Game_Objects/levelEditIconBorders");//, levelEditHud.position, true, currentColor, 1f, false);
            levelEditIcons = Content.Load<Texture2D>(@"Game/Game_Objects/levelEditIcons");//, levelEditHud.position, true, Color.White, 1f, false);
            tileTypeTexture = Content.Load<Texture2D>(@"Game/Game_Objects/tileTypes");
         * */
        /*
           spriteBatch.End();
           spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, alphaShader);
           alphaShader.Parameters["TextureTexture"].SetValue(tileTexture);
           alphaShader.Parameters["textureSize"].SetValue(tileTexture.Width);
           for (int xIndex = 0; xIndex < xSize; xIndex += 1)
           {
               for (int yIndex = 0; yIndex < ySize; yIndex += 1)
               {
                   //Not a blank texture.
                   if (tiles[xIndex, yIndex].texturePosition != 0)
                   {
                       alphaShader.Parameters["textureStart"].SetValue((tiles[xIndex, yIndex].texturePosition - 1)*tileSize);
                       alphaShader.Parameters["typeStart"].SetValue((tiles[xIndex, yIndex].type - 1)*tileSize);
                       alphaShader.Parameters["rotation"].SetValue(tiles[xIndex, yIndex].rotation);
                       spriteBatch.Draw(tileTexture, new Vector2((xIndex * tileSize - (tiles[xIndex, yIndex].texturePosition - 1) * tileSize), (yIndex * tileSize)) - new Vector2((int)screenPosition.X,(int)screenPosition.Y), Color.White);
                   }
               }
           }
           spriteBatch.End();
           spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null);
           */
        /*
         *  int playerOwner;
        int mainType;
        int subType;
        int modifier;

        public float maxCooldown;
        public float cooldown;
        List<Entity> entities;
        List<float> life;
        float animation;

        static int sheetBoxSize = 64;
        
        public Ability(int mainType, int subType, int modifier, int playerOwner)
        {
            this.mainType = mainType;
            this.subType = subType;
            this.modifier = modifier;
            this.playerOwner = playerOwner;
            entities = new List<Entity>();
            life = new List<float>();
            this.maxCooldown = 0;
            this.animation = 0;
            if (mainType == 0)
            {
                if (subType == 0)
                {
                    maxCooldown = 1.0f;
                    if (modifier == 2)
                        maxCooldown *= .6f;
                }
                else if (subType == 1)
                {
                    maxCooldown = 1.0f;
                    if (modifier == 1)
                        maxCooldown *= 2.0f;
                }
                else if (subType == 2)
                {
                    maxCooldown = 4.0f;
                }
            }
            else if (mainType == 1)
            {
                if (subType == 0)
                {
                    maxCooldown = 10.0f;
                    if (modifier == 2)
                        maxCooldown = 4.0f;
                }
                else if (subType == 1)
                {
                    maxCooldown = 10.0f;
                    if (modifier == 2)
                        maxCooldown = 4.0f;
                }
            }
            else if (mainType == 2)
            {
                if (subType == 0)
                {
                    maxCooldown = 10.0f;
                }
                else if (subType == 1)
                {
                    maxCooldown = 10.0f;
                }
            }
            else if (mainType == 3)
            {
                if (subType == 0)
                {
                    maxCooldown = 15.0f;
                }
                else if (subType == 2)
                {
                    maxCooldown = 20.0f;
                }
            }
            this.cooldown = 0;
        }

        public void Try(GameTime gameTime)
        {
            if (cooldown <= 0.0f)
            {
                cooldown = maxCooldown;
                if (mainType == 0)
                {
                    #region Shot
                    if (subType == 0)
                    {
                        entities.Add(new Entity(16.0f, 1, Main.players[playerOwner].position, 10 * Vector2.Normalize(-Main.players[0].position + Main.mouseLocation + Main.screenPosition)));
                        if (modifier == 1)
                            entities[entities.Count - 1].velocity *= .65f;
                        life.Add(4.0f);
                    }  
                    #endregion
                    #region Blast
                    else if (subType == 1)
                    {
                        entities.Add(new Entity(16.0f, 1, Main.players[playerOwner].position, Main.RotateVelocity(10 * Vector2.Normalize(-Main.players[0].position + Main.mouseLocation + Main.screenPosition), Vector2.Normalize(new Vector2(1, 1)), true)));
                        entities.Add(new Entity(16.0f, 1, Main.players[playerOwner].position, 10 * Vector2.Normalize(-Main.players[0].position + Main.mouseLocation + Main.screenPosition)));
                        entities.Add(new Entity(16.0f, 1, Main.players[playerOwner].position, Main.RotateVelocity(10 * Vector2.Normalize(-Main.players[0].position + Main.mouseLocation + Main.screenPosition), Vector2.Normalize(new Vector2(1, -1)), true)));
                        for (int index = 0; index < 3; index++)
                        {
                            life.Add(.5f);
                        }
                    } 
                    #endregion
                    #region Bolt
                    else if (subType == 2)
                    {
                        entities.Add(new Entity(16.0f, 2, Main.players[playerOwner].position, Vector2.Zero));
                        if (modifier == 2)
                            life.Add(3.90f);
                        else
                            life.Add(1f);
                        for (int index = 0; index < 6; index += 1)
                        {
                            entities.Add(new Entity(16.0f, 2 + index, Main.players[playerOwner].position, 10000 * Vector2.Normalize(-Main.players[0].position + Main.mouseLocation + Main.screenPosition)));
                            entities[index + 1].CollideWithEnvironment(gameTime);
                            if (modifier == 2)
                                life.Add(3.90f);
                            else
                                life.Add(1f);
                        }
                    } 
                    #endregion
                }
                else if (mainType == 1)
                {
                    #region Charge
                    if (subType == 0)
                    {
                        if (modifier == 0)
                        {   
                            Main.players[playerOwner].special = 1;
                            Main.players[playerOwner].velocity = 6 * Vector2.Normalize(-Main.players[0].position + Main.mouseLocation + Main.screenPosition);

                        }
                        else if (modifier == 1)
                        {
                            Main.players[playerOwner].special = 1;
                            Main.players[playerOwner].velocity = 6 * Vector2.Normalize(-Main.players[0].position + Main.mouseLocation + Main.screenPosition);
                        }
                        else if (modifier == 2)
                        {
                            entities.Add(new Entity(0f, 0, Main.players[playerOwner].position, Vector2.Zero));
                            Main.players[playerOwner].position = Main.mouseLocation + Main.screenPosition;
                            entities.Add(new Entity(0f, 0, Main.players[playerOwner].position, Vector2.Zero));
                            life.Add(2.0f);
                        }
                    } 
                    #endregion
                    #region Missile
                    if (subType == 1)
                    {
                        entities.Add(new Entity(24.0f, 1, Main.players[playerOwner].position, 5.0f * Vector2.Normalize(-Main.players[0].position + Main.mouseLocation + Main.screenPosition)));
                        if (modifier == 2)
                            entities[entities.Count - 1].velocity *= 2;
                        life.Add(1.6f);
                    } 
                    #endregion
                }
                else if (mainType == 2)
                {
                    #region Circle
                    if (subType == 0)
                    {
                        entities.Add(new Entity(100.0f, 0, Main.mouseLocation + Main.screenPosition, Vector2.Zero));
                        life.Add(5.0f);
                    } 
                    #endregion
                    #region Mine
                    else if (subType == 1)
                    {
                        if (modifier == 2)
                        {
                            entities.Add(new Entity(32f, 2, Main.mouseLocation + Main.screenPosition, Vector2.Zero));
                        }
                        else
                        {
                            entities.Add(new Entity(8f, 2, Main.players[playerOwner].position, 10000 * Vector2.Normalize(-Main.players[0].position + Main.mouseLocation + Main.screenPosition)));
                            entities[entities.Count - 1].CollideWithEnvironment(gameTime);
                        }
                        life.Add(25.0f);
                    } 
                    #endregion
                }
                else if (mainType == 3)
                {
                    #region MyRegion
                    
                    #endregion
                }
            }
        }

        public void Update(GameTime gameTime)
        {
            if (mainType == 0)
            {
                #region Shot
                if (subType == 0)
                {
                    int index = 0;
                    while (index < entities.Count)
                    {
                        entities[index].CollideWithEnvironment(gameTime);
                        life[index] -= .001f * gameTime.ElapsedGameTime.Milliseconds;
                        index += 1;
                    }
                    if (entities.Count > 0)
                    {
                        if (life[0] <= 0.0f)
                        {
                            life.RemoveAt(0);
                            entities.RemoveAt(0);
                        }
                    }
                } 
                #endregion
                #region Blast
                else if (subType == 1)
                {
                    int index = 0;
                    while (index < entities.Count)
                    {
                        entities[index].CollideWithEnvironment(gameTime);
                        life[index] -= .001f * gameTime.ElapsedGameTime.Milliseconds;
                        index += 1;
                    }
                    if (entities.Count > 0)
                    {
                        if (life[0] <= 0.0f)
                        {
                            life.RemoveAt(0);
                            entities.RemoveAt(0);
                        }
                    }
                }  
                #endregion
                #region Bolt
                else if (subType == 2)
                {
                    int index = 0;
                    while (index < entities.Count)
                    {
                        life[index] -= .001f * gameTime.ElapsedGameTime.Milliseconds;
                        index += 1;
                    }
                    if (entities.Count > 0)
                    {
                        if (life[0] <= 0.0f)
                        {
                            life.RemoveAt(0);
                            entities.RemoveAt(0);
                        }
                    }
                } 
                #endregion
            }
            else if (mainType == 1)
            {
                #region Charge
                if (subType == 0)
                {
                    if (modifier == 2)
                    {
                        if (life.Count > 0)
                        {
                            life[0] -= .001f * gameTime.ElapsedGameTime.Milliseconds;
                            if (life[0] <= 0.0f)
                            {
                                life.RemoveAt(0);
                                entities.RemoveAt(0);
                                entities.RemoveAt(0);
                            }
                        }
                    }
                } 
                #endregion
                #region Missile
                else if (subType == 1)
                {
                    int index = 0;
                    while (index < entities.Count)
                    {
                        if (entities[index].velocity != Vector2.Zero)
                        {
                            if (modifier == 2)
                                entities[index].velocity -= .00610f * gameTime.ElapsedGameTime.Milliseconds * Vector2.Normalize(entities[index].velocity);
                            else
                                entities[index].velocity += .01f * gameTime.ElapsedGameTime.Milliseconds * Vector2.Normalize(entities[index].velocity);
                            entities[index].CollideWithEnvironment(gameTime);
                        }
                        else
                        {
                        }
                        life[index] -= .001f * gameTime.ElapsedGameTime.Milliseconds;
                        index += 1;
                    }
                    if (entities.Count > 0)
                    {
                        if (life[0] <= 0.0f)
                        {
                            if (entities[0].velocity == Vector2.Zero)
                            {
                                life.RemoveAt(0);
                                entities.RemoveAt(0);
                            }
                            else
                            {
                                animation = 0;
                                entities[0].velocity = Vector2.Zero;
                                life[0] = 1f;
                            }
                        }
                    }
                } 
                #endregion
            }
            
            else if (mainType == 2)
            {
                #region Circle
                if (subType == 0)
                {
                    int index = 0;
                    while (index < entities.Count)
                    {
                        life[index] -= .001f * gameTime.ElapsedGameTime.Milliseconds;
                        index += 1;
                    }
                    if (entities.Count > 0)
                    {
                        if (life[0] <= 0.0f)
                        {
                            life.RemoveAt(0);
                            entities.RemoveAt(0);
                        }
                    }
                } 
                #endregion
                #region Mine
                else if (subType == 1)
                {
                    int index = 0;
                    while (index < entities.Count)
                    {
                        life[index] -= .001f * gameTime.ElapsedGameTime.Milliseconds;
                        index += 1;
                    }
                    if (entities.Count > 0)
                    {
                        if (life[0] <= 0.0f)
                        {
                            life.RemoveAt(0);
                            entities.RemoveAt(0);
                        }
                    }
                } 
                #endregion 
            }
            cooldown = Math.Max(0, cooldown - .001f * gameTime.ElapsedGameTime.Milliseconds);
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, bool gameNotEditor)
        {
            if (mainType == 0)
            {
                #region Shot
                if (subType == 0)
                {
                    animation += .01f * gameTime.ElapsedGameTime.Milliseconds;
                    if (animation > 4)
                        animation = 0;
                    foreach (Entity e in entities)
                    {
                        spriteBatch.Draw(Main.shotNoColor, e.position - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, ((int)animation) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getTransparentColor(), (float)Math.Atan2(e.velocity.Y, e.velocity.X), new Vector2(44, Main.tileSize), 1f, SpriteEffects.None, 1f);
                        spriteBatch.Draw(Main.shotColor, e.position - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, ((int)animation) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getColor(), (float)Math.Atan2(e.velocity.Y, e.velocity.X), new Vector2(44, Main.tileSize), 1f, SpriteEffects.None, 1f);
                    }
                } 
                #endregion
                #region Blast
                else if (subType == 1)
                {
                    animation += .01f * gameTime.ElapsedGameTime.Milliseconds;
                    if (animation > 4)
                        animation = 0;
                    foreach (Entity e in entities)
                    {
                        spriteBatch.Draw(Main.blastNoColor, e.position - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, ((int)animation) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getTransparentColor(), (float)Math.Atan2(e.velocity.Y, e.velocity.X), new Vector2(Main.tileSize), 1f, SpriteEffects.None, 1f);
                        spriteBatch.Draw(Main.blastColor, e.position - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, ((int)animation) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getColor(), (float)Math.Atan2(e.velocity.Y, e.velocity.X), new Vector2(Main.tileSize), 1f, SpriteEffects.None, 1f);
                    }
                } 
                #endregion
                #region Bolt
                else if (subType == 2)
                {
                    animation += .01f * gameTime.ElapsedGameTime.Milliseconds;
                    if (animation > 2)
                        animation = 0;

                    int index = 0;
                    while (index < entities.Count - 1)
                    {
                        float distanceLeft = Vector2.Distance(entities[index].position, entities[index + 1].position);
                        Vector2 direction = Vector2.Normalize(entities[index + 1].position - entities[index].position);
                        while (distanceLeft > 0)
                        {
                            float thisDistance = Math.Min(4.0f, distanceLeft);
                            spriteBatch.Draw(Main.boltNoColor, entities[index].position + direction * distanceLeft - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, ((int)animation) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getTransparentColor(), (float)Math.Atan2(direction.Y, direction.X), new Vector2(Main.tileSize), 1f, SpriteEffects.None, 1f);
                            spriteBatch.Draw(Main.boltColor, entities[index].position + direction * distanceLeft - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, ((int)animation) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getColor(), (float)Math.Atan2(direction.Y, direction.X), new Vector2(Main.tileSize), 1f, SpriteEffects.None, 1f);
                            distanceLeft -= thisDistance;
                        }
                        index += 1;
                    }

                    index = 0;
                    while (index < entities.Count)
                    {
                        spriteBatch.Draw(Main.boltNoColor, entities[index].position - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, 128 + (int)(animation) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getTransparentColor(), (float)Math.Atan2(entities[index].velocity.Y, entities[index].velocity.X), new Vector2(Main.tileSize), 1f, SpriteEffects.None, 1f);
                        spriteBatch.Draw(Main.boltColor, entities[index].position - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, 128 + (int)(animation) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getColor(), (float)Math.Atan2(entities[index].velocity.Y, entities[index].velocity.X), new Vector2(Main.tileSize), 1f, SpriteEffects.None, 1f);
                        index += 1;
                    }
                } 
                #endregion
            }
            else if (mainType == 1)
            {
                #region Charge
                if (subType == 0)
                {
                    animation += .01f * gameTime.ElapsedGameTime.Milliseconds;
                    if (animation > 4)
                        animation = 0;
                    if (Main.players[playerOwner].charging == true && modifier != 2)
                    {
                        spriteBatch.Draw(Main.chargeNoColor, Main.players[playerOwner].position - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, ((int)animation) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getTransparentColor(), (float)Math.Atan2(Main.players[playerOwner].velocity.Y, Main.players[playerOwner].velocity.X), new Vector2(Main.tileSize), 1f, SpriteEffects.None, 1f);
                        spriteBatch.Draw(Main.chargeColor, Main.players[playerOwner].position - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, ((int)animation) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getColor(), (float)Math.Atan2(Main.players[playerOwner].velocity.Y, Main.players[playerOwner].velocity.X), new Vector2(Main.tileSize), 1f, SpriteEffects.None, 1f);
                    }
                    else if (modifier == 2)
                    {
                        if (entities.Count > 0)
                        {
                            spriteBatch.Draw(Main.chargeNoColor, entities[0].position - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, ((int)animation) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getTransparentColor(), 0f, new Vector2(Main.tileSize), 1f, SpriteEffects.None, 1f);
                            spriteBatch.Draw(Main.chargeColor, entities[0].position - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, ((int)animation) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getColor(), 0f, new Vector2(Main.tileSize), 1f, SpriteEffects.None, 1f);
                            spriteBatch.Draw(Main.chargeNoColor, entities[1].position - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, ((int)animation) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getTransparentColor(), 0f, new Vector2(Main.tileSize), 1f, SpriteEffects.None, 1f);
                            spriteBatch.Draw(Main.chargeColor, entities[1].position - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, ((int)animation) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getColor(), 0f, new Vector2(Main.tileSize), 1f, SpriteEffects.None, 1f);
                        }
                    }
                } 
                #endregion
                #region Missile
                else if (subType == 1)
                {

                    animation += .01f * gameTime.ElapsedGameTime.Milliseconds;
                    if (modifier == 2)
                    {
                        if (animation > 6)
                            animation = 0;
                        foreach (Entity e in entities)
                        {
                            if (e.velocity == Vector2.Zero)
                            {
                                spriteBatch.Draw(Main.missileNoColor, e.position - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, 128 + ((int)animation % 2) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getTransparentColor(), (float)Math.Atan2(e.velocity.Y, e.velocity.X), new Vector2(Main.tileSize), 2f, SpriteEffects.None, 1f);
                                spriteBatch.Draw(Main.missileColor, e.position - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, 128 + ((int)animation % 2) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getColor(), (float)Math.Atan2(e.velocity.Y, e.velocity.X), new Vector2(Main.tileSize), 2f, SpriteEffects.None, 1f);
                            }
                            else
                            {
                                spriteBatch.Draw(Main.missileNoColor, e.position - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, ((int)animation) / 3 * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getTransparentColor(), (float)Math.PI * animation / 3f, new Vector2(Main.tileSize), 1f, SpriteEffects.None, 1f);
                                spriteBatch.Draw(Main.missileColor, e.position - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, ((int)animation) / 3 * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getColor(), (float)Math.PI * animation / 3f, new Vector2(Main.tileSize), 1f, SpriteEffects.None, 1f);
                            }
                        }
                    }
                    else
                    {
                        if (animation > 2)
                            animation = 0;
                        foreach (Entity e in entities)
                        {
                            if (e.velocity == Vector2.Zero)
                            {
                                spriteBatch.Draw(Main.missileNoColor, e.position - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, 128 + ((int)animation) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getTransparentColor(), (float)Math.Atan2(e.velocity.Y, e.velocity.X), new Vector2(Main.tileSize), 2f, SpriteEffects.None, 1f);
                                spriteBatch.Draw(Main.missileColor, e.position - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, 128 + ((int)animation) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getColor(), (float)Math.Atan2(e.velocity.Y, e.velocity.X), new Vector2(Main.tileSize), 2f, SpriteEffects.None, 1f);
                            }
                            else
                            {
                                spriteBatch.Draw(Main.missileNoColor, e.position - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, ((int)animation) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getTransparentColor(), (float)Math.Atan2(e.velocity.Y, e.velocity.X), new Vector2(Main.tileSize), 1f, SpriteEffects.None, 1f);
                                spriteBatch.Draw(Main.missileColor, e.position - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, ((int)animation) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getColor(), (float)Math.Atan2(e.velocity.Y, e.velocity.X), new Vector2(Main.tileSize), 1f, SpriteEffects.None, 1f);
                            }
                        }
                    }
                } 
                #endregion
            }
            else if (mainType == 2)
            {
                #region Circle
                if (subType == 0)
                {
                    foreach (Entity e in entities)
                    {
                        for (int xIndex = -1; xIndex < 1; xIndex += 1)
                        {
                            for (int yIndex = -1; yIndex < 1; yIndex += 1)
                            {
                                spriteBatch.Draw(Main.circleNoColor, e.position - Main.screenPosition + new Vector2(xIndex, yIndex) * 50f, new Rectangle(modifier * sheetBoxSize, (3 + 2 * yIndex + xIndex) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getTransparentColor(), 0f, new Vector2(7f), 1f, SpriteEffects.None, 1f);
                                spriteBatch.Draw(Main.circleColor, e.position - Main.screenPosition + new Vector2(xIndex, yIndex) * 50f, new Rectangle(modifier * sheetBoxSize, (3 + 2 * yIndex + xIndex) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getColor(), 0f, new Vector2(7f), 1f, SpriteEffects.None, 1f);
                            }
                        }
                    }
                } 
                #endregion
                #region Mine
                else if (subType == 1)
                {
                    animation += .01f * gameTime.ElapsedGameTime.Milliseconds;
                    if (animation > 4)
                        animation = 0;
                    foreach (Entity e in entities)
                    {
                        spriteBatch.Draw(Main.mineNoColor, e.position - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, ((int)animation / 2) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getTransparentColor(), (float)Math.Atan2(e.velocity.Y, e.velocity.X), new Vector2(Main.tileSize), 1f, SpriteEffects.None, 1f);
                        spriteBatch.Draw(Main.mineColor, e.position - Main.screenPosition, new Rectangle(modifier * sheetBoxSize, ((int)animation / 2) * sheetBoxSize, sheetBoxSize, sheetBoxSize), Main.players[playerOwner].getColor(), (float)Math.Atan2(e.velocity.Y, e.velocity.X), new Vector2(Main.tileSize), 1f, SpriteEffects.None, 1f);
                    }
                } 
                #endregion
            }
         */
    }
}
