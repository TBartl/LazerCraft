using System;
using Microsoft.Xna.Framework;
using System.IO;
using System.Text;

namespace LazerCraft
{
    // Entities are things with a position, a velocity, and a size.
    // This includes players, bullets, items, ect...
    // This also means it includes everything that requires a hit detection algorithm
    public class Entity
    {
        // Vectors represent two floats in the x and y direction
        // Outside of this class, things such as gravity, the nature of the object, ect will determine the initial position.
        // For example, a players position will be determined by a spawn point, and things like teleportation abilities (as while as movement).
        // A lazer's position will be determined by the player that fires it, and the velocity by the direction it was shot.
        // Rather than accounting for all this within this class, I let the rest of the program handle this, which is why it is public.
        public Vector2 position;
        public Vector2 velocity;
        //Size is determined by these factors too.
        public float size;
        public float halfSize;
        // The player's ability to move and jump is determined by if it is on a wall or the ground, so we need to get this in the collision.
        // Although not all entities will use it, it doesn't take much extra effort and wont sacrifice any efficiency.
        public bool onWall;
        public bool onGround;
        // Special (recently changed from a boolean to an integer) changes how the object reacts after hitting an object.
        // 0 means stop the velocity in the direction of the seperating axis of the tile, sort like how a car works (not bouncy)
        // 1 means reverse the velocity in the direction of the seperating axis of the tile, sort of like how a bouncy ball works.
        // 2+ is lazer specific stuff that is odd and not yet working, so don't worry about it.
        public int special;


        // Having a default constructor is always nice, expecially since some classes extend off of this one and I don't want to always have to super() it (or base() in C#).
        public Entity()
        {
            this.position = Vector2.Zero;
            this.velocity = Vector2.Zero;
            this.size = 0.0f;
            this.halfSize = 0.0f;
            onWall = false;
            onGround = false;
            special = 0;
        }
        // The usual constructor, sets a lot of initial conditions.
        public Entity(float size,int special, Vector2 position, Vector2 velocity)
        {
            this.position = position;
            this.velocity = velocity;
            this.size = size;
            this.halfSize = size / 2.0f;
            onWall = false;
            onGround = false;
            this.special = special;
        }

        public void CollideWithEnvironment(GameTime gameTime)
        {
            // Because an object can be traveling very fast, I want to make sure that it is not going so fast that it travels through one of the shapes over the course of a frame.
            // To fix this, I'm going to multiphase the collision detection when it travels very fast.
            // What this means is that I will check the collision in segments by moving the objects position only up to half of its size at a time.
            float velocityLength = velocity.Length();
            float distanceToTravel = velocityLength;
            while (distanceToTravel > 0)
            {
                // Here I take the smallest of the two possible distances: either half its size or the remaining distance. 
                // If the remaining distance is smaller than the half size, than this will be    the last time the loop executes otherwise it will continue to execute at each step
                float distance = Math.Min(distanceToTravel, halfSize);
                position += Vector2.Normalize(velocity) * distance;
                distanceToTravel -= distance;

                // At any given position, the circle may be colliding with 1 to 4 tiles depending on if it is between 0, 2, or 4 tiles.
                // In addition to this, I want to make sure it collides with the closest tiles first.
                // This means that I want to check the tile it is in first, followed by the closest one inn the x or y direction, then finnaly the corner tiles.
                // Because the player can fall out of the stage, conditions are used to make sure an index out of range error doesn't occur.
                // This means using a method shown later called CheckTile(x,y).

                // to convert the world position, which is in a float, to an integer scaled down to the tilesize, I use integer division.
                int x = (int)(position.X) / Tile.tileSize;
                int y = (int)(position.Y) / Tile.tileSize;

                //First I check the tile that it is directly on.
                if (Main.level.CheckTile(x,y))
                {
                    //This method, also the biggest part of the program, will be shown later after I show the order of tiles I check.
                    CollideWithTile(x, y, gameTime);
                }

                // For offset tiles, we do some other calculations.
                // So we check the difference between the center and the 
                Vector2 tileCenter = new Vector2((x + .5f) * Tile.tileSize, (y + .5f) * Tile.tileSize);
                Vector2 delta = position - tileCenter; 
                int xShifted = x + Math.Sign(delta.X);
                int yShifted = y + Math.Sign(delta.Y);

                //Into another X
                if (Math.Abs(delta.X) + halfSize > Tile.halfTileSize)
                {
                    //Into another Y and X
                    if (Math.Abs(delta.Y) + halfSize > Tile.halfTileSize)
                    {
                        // If the x tile is closer, check it first.
                        if (Math.Abs(delta.X) <= Math.Abs(delta.Y))
                        {
                            if (Main.level.CheckTile(xShifted, y))
                            {
                                CollideWithTile(xShifted, y, gameTime);
                            }
                            if (Main.level.CheckTile(x, yShifted))
                            {
                                CollideWithTile(x, yShifted, gameTime);
                            }
                        }
                        // Otherwise the y tile is closer, check it first.
                        else
                        {
                            if (Main.level.CheckTile(x, yShifted))
                            {
                                CollideWithTile(x, yShifted, gameTime);
                            }
                            if (Main.level.CheckTile(xShifted, y))
                            {
                                CollideWithTile(xShifted, y, gameTime);
                            }
                        } 
                        // If it is in both, we need to check the corner too.
                        if (Main.level.CheckTile(xShifted, yShifted))
                        {
                            CollideWithTile(xShifted, yShifted, gameTime);
                        }
                    }
                    // Into another X, but not Y
                    else
                    {
                        if (Main.level.CheckTile(xShifted, y))
                        {
                            CollideWithTile(xShifted, y, gameTime);
                        }
                    }
                }
                //Into another Y, but notX
                else if (Math.Abs(delta.Y) + halfSize > Tile.halfTileSize)
                {
                    if (Main.level.CheckTile(x, yShifted))
                    {
                        CollideWithTile(x, yShifted, gameTime);
                    }
                }

                float newVelocityLength = velocity.Length();
                distanceToTravel *= newVelocityLength / velocityLength;
                velocityLength = newVelocityLength;
            }
        }

        public void CollideWithTile(int xIndex, int yIndex, GameTime gameTime)
        {

            //I now get the type of the tile, this will be used later to handle collisions for each shape.
            int type = Main.level.tiles[xIndex, yIndex].type;
            // I also need the rotation. 
            //0 means it is rotated 0 degreees, 1 means 90 degrees, 2 means 180, 3 is 270.
            int tileRotation = Main.level.tiles[xIndex, yIndex].typeRotation;
            // If the type is 0, the tile is empty so I don't need to go any further.
            if (type != 0)
            {
                // Many calculations are taken in regard to the center of the tile, so finding that will help.
                Vector2 tileCenter = new Vector2((xIndex + .5f) * Tile.tileSize, (yIndex + .5f) * Tile.tileSize);
                // I need another vector telling me the difference between the positions center and the tiles center.
                Vector2 delta = position - tileCenter;

                // Here is where I start to use voronoi reigons.
                // Since every type has a straight axis on atleast two of its sides, finding the horizontal and vertical won't hurt
                // If it is negative, that means that it is to the left or the top of the tile.
                // If it is positive, that means that it is to the right or the bottom of the tile.
                // Otherwise, the circle is in the center area of the tile.
                int horizontal = 0;
                int vertical = 0;
                if (delta.X < -Tile.halfTileSize)
                    horizontal = -1;
                else if (delta.X > Tile.halfTileSize)
                    horizontal = 1;
                if (delta.Y < -Tile.halfTileSize)
                    vertical = -1;
                else if (delta.Y > Tile.halfTileSize)
                    vertical = 1;
                // I now get the projection for the x and y, since every shape will use this in detirmining its collision relative to the axis.
                float projectionX = Tile.halfTileSize + halfSize - Math.Abs(delta.X);
                float projectionY = Tile.halfTileSize + halfSize - Math.Abs(delta.Y);

                // The pure square. 
                if (type == 1)
                {
                    //Luckily for me it doesnt matter if this shape is rotated, since it will always just be a square.

                    //First we check along the center parts
                    if (horizontal == 0)
                    {
                        //If both the horizontal and the vertical are zero it is in the center
                        if (vertical == 0)
                        {
                            // I now need to see which axis has the smallest distance. The smaller one is where the circle will be pushed out.
                            if (projectionX < projectionY)
                            {
                                CollideWithAxis(projectionX, new Vector2(Math.Sign(delta.X), 0), gameTime);
                            }
                            else
                            {
                                CollideWithAxis(projectionY, new Vector2(0, Math.Sign(delta.Y)), gameTime);
                            }
                        }
                        //If its not hitting the vertical but it is the horizontal, I collide it along the y axis.
                        else
                        {
                            CollideWithAxis(projectionY, new Vector2(0, Math.Sign(delta.Y)), gameTime);
                        }
                    }
                    // If its not hitting the horizontal but it is the vertical, I collide it along the x axis.
                    else if (vertical == 0)
                    {
                        CollideWithAxis(projectionX, new Vector2(Math.Sign(delta.X), 0), gameTime);
                    }
                    else
                    {
                        // Its colliding with the axis. So lets check if it is colliding with one of the corners.
                        Vector2 corner = tileCenter + new Vector2(Math.Sign(delta.X), Math.Sign(delta.Y)) * Tile.halfTileSize;
                        // I get the distance between the center and the corner, and subtract it from the size to check the projection
                        float projectionCorner = halfSize - Vector2.Distance(position, corner);
                        //In this region the circle may not collide with the square, so I need to check to make sure the projection is negative.
                        if (projectionCorner > 0)
                        {
                            CollideWithAxis(projectionCorner, Vector2.Normalize(position - corner), gameTime);
                        }
                    }
                }
                // The half square/rectangle
                else if (type == 2)
                {
                    // Rather than adjuting for the 3 "special sides", I want to just redo the tile properties so it is shrunk in one direction.
                    // After this I will be able to check it like a normal square.
                    if (tileRotation == 0 || tileRotation == 2)
                    {
                        if (tileRotation == 0)
                            tileCenter.X -= Tile.halfTileSize / 2;
                        else
                            tileCenter.X += Tile.halfTileSize / 2;
                        delta.X = position.X - tileCenter.X;
                        horizontal = 0;
                        if (delta.X < -Tile.halfTileSize / 2)
                            horizontal = -1;
                        else if (delta.X > Tile.halfTileSize / 2)
                            horizontal = 1;
                        projectionX = Tile.halfTileSize / 2 + halfSize - Math.Abs(delta.X);
                    }
                    else
                    {
                        if (tileRotation == 1)
                            tileCenter.Y -= Tile.halfTileSize / 2;
                        else
                            tileCenter.Y += Tile.halfTileSize / 2;
                        delta.Y = position.Y - tileCenter.Y;
                        vertical = 0;
                        if (delta.Y < -Tile.halfTileSize / 2)
                            vertical = -1;
                        else if (delta.Y > Tile.halfTileSize / 2)
                            vertical = 1;
                        projectionY = Tile.halfTileSize / 2 + halfSize - Math.Abs(delta.Y);
                    }
                    //Have to check for collision since it is now possible that it isnt.
                    if (((tileRotation == 0 || tileRotation == 2) && Math.Abs(delta.X) <= halfSize + Tile.halfTileSize / 2.0f) ||
                            ((tileRotation == 1 || tileRotation == 3) && Math.Abs(delta.Y) <= halfSize + Tile.halfTileSize / 2.0f))
                    //Now we can handle this like a normal square
                    {
                        if (horizontal == 0)
                        {
                            if (vertical == 0)
                            {
                                if (projectionX < projectionY)
                                {
                                    CollideWithAxis(projectionX, new Vector2(Math.Sign(delta.X), 0), gameTime);
                                }
                                else
                                {
                                    CollideWithAxis(projectionY, new Vector2(0, Math.Sign(delta.Y)), gameTime);
                                }
                            }
                            else
                            {
                                CollideWithAxis(projectionY, new Vector2(0, Math.Sign(delta.Y)), gameTime);
                            }
                        }
                        else if (vertical == 0)
                        {
                            CollideWithAxis(projectionX, new Vector2(Math.Sign(delta.X), 0), gameTime);
                        }
                        else
                        {
                            //The only difference is the corners, we need to tell the program the corners are lower.
                            Vector2 corner;
                            if (tileRotation == 0 || tileRotation == 2)
                            {
                                corner = tileCenter + new Vector2(Math.Sign(delta.X) / 2.0f, Math.Sign(delta.Y)) * Tile.halfTileSize;
                            }
                            else
                            {
                                corner = tileCenter + new Vector2(Math.Sign(delta.X), Math.Sign(delta.Y) / 2.0f) * Tile.halfTileSize;
                            }
                            float projectionCorner = halfSize - Vector2.Distance(position, corner);
                            if (projectionCorner > 0)
                            {
                                CollideWithAxis(projectionCorner, Vector2.Normalize(position - corner), gameTime);
                            }
                        }
                    }
                }
                // The convex circle
                else if (type == 3)
                {
                    // When I have 2 normal flat sides, I want to handle them like I would a normal square.
                    // This means that 5 out of 9 voronoi reigons will be checkable like a square, so I'm going to check for those first.
                    if ((tileRotation == 0 && (horizontal == -1 || vertical == 1)) ||
                        (tileRotation == 1 && (horizontal == -1 || vertical == -1)) ||
                        (tileRotation == 2 && (horizontal == 1 || vertical == -1)) ||
                        (tileRotation == 3 && (horizontal == 1 || vertical == 1)))
                    {
                        if (horizontal == 0)
                        {
                            CollideWithAxis(projectionY, new Vector2(0, Math.Sign(delta.Y)), gameTime);
                        }
                        else if (vertical == 0)
                        {
                            CollideWithAxis(projectionX, new Vector2(Math.Sign(delta.X), 0), gameTime);
                        }
                        else
                        {
                            Vector2 corner = tileCenter + new Vector2(Math.Sign(delta.X), Math.Sign(delta.Y)) * Tile.halfTileSize;
                            float projectionCorner = halfSize - Vector2.Distance(position, corner);
                            if (projectionCorner > 0)
                            {
                                CollideWithAxis(projectionCorner, Vector2.Normalize(position - corner), gameTime);
                            }
                        }
                    }
                    else
                    {
                        //We now need to check for the curved part of the tile. Luckily for me this is incredibly easy.
                        //Because of the first if statement, we know that the player circle can only possibly be colliding with the curved side.
                        // First we need to pretend the quarter circle is a full circle to make this easier.
                        Vector2 tileCircleCenter = new Vector2(tileCenter.X, tileCenter.Y);
                        if (tileRotation == 0 || tileRotation == 1)
                            tileCircleCenter.X -= Tile.halfTileSize;
                        else
                            tileCircleCenter.X += Tile.halfTileSize;
                        if (tileRotation == 1 || tileRotation == 2)
                            tileCircleCenter.Y -= Tile.halfTileSize;
                        else
                            tileCircleCenter.Y += Tile.halfTileSize;
                        float projectionRadial = Tile.tileSize + halfSize - Vector2.Distance(position, tileCircleCenter);
                        // Circle on circle collision tests are really easy. We can just take the distance between the centers and check if it is smaller than the circle's radius.
                        // In other words if distance from A_center to B_center < A_size + B_Size, than the circles are colliding.
                        // Because the difference of these is the projection outward, we can just subtract them and check it to see if it is > 0 and we will have the projection outwards.
                        if (projectionRadial > 0)
                        {
                            //In the center there is 3 possible places to go.
                            //We want the circle to go out the closest one, so I check against the curve, x axis, and y axis to see which is smaller.
                            if (horizontal == 0 && vertical == 0)
                            {
                                if (projectionRadial < projectionX && projectionRadial < projectionY)
                                {
                                    CollideWithAxis(projectionRadial, Vector2.Normalize(position - tileCircleCenter), gameTime);
                                }
                                else
                                {
                                    if (projectionX < projectionY)
                                    {
                                        CollideWithAxis(projectionX, new Vector2(Math.Sign(delta.X), 0), gameTime);
                                    }
                                    else
                                    {
                                        CollideWithAxis(projectionY, new Vector2(0, Math.Sign(delta.Y)), gameTime);
                                    }
                                }
                            }
                            //If it is not in the center it can only possibly collide with the curve, so we can just check that. 
                            else
                            {
                                CollideWithAxis(projectionRadial, Vector2.Normalize(position - tileCircleCenter), gameTime);
                            }
                        }
                    }
                }
                // The concave circle
                else if (type == 4)
                {
                    // Uh oh, I used a concave shape. Time for some weird code.
                    // The only possible way for it to be colliding with the curve is it to be in the center region of the tile.
                    // If the circle colliding with the tile's inverse circle has a larger radius, this code won't work. This is the danger of concave shapes.
                    // At this stage in the game all the circles that could collide are smaller, so this won't be too big of a problem.
                    // If something bigger gets added I will have to change the code. 
                    if (horizontal == 0 && vertical == 0)
                    {
                        Vector2 tileCircleCenter = new Vector2(tileCenter.X, tileCenter.Y);
                        if (tileRotation == 0 || tileRotation == 1)
                            tileCircleCenter.X -= Tile.halfTileSize;
                        else
                            tileCircleCenter.X += Tile.halfTileSize;
                        if (tileRotation == 1 || tileRotation == 2)
                            tileCircleCenter.Y -= Tile.halfTileSize;
                        else
                            tileCircleCenter.Y += Tile.halfTileSize;
                        float projectionRadial = halfSize + Vector2.Distance(position, tileCircleCenter) - Tile.tileSize;
                        // This opertation is a little weird. If this distance(A_center, B_center) > B_radius - A radius it is colliding.
                        // This is similiar to the other code in type == 3, but the way the curve is curved in makes it a bit differend.
                        if (projectionRadial > 0)
                        {
                            // Like any other center, we have to find the smallest escape distance.
                            if (projectionRadial < projectionX && projectionRadial < projectionY)
                            {
                                CollideWithAxis(projectionRadial, Vector2.Normalize(tileCircleCenter - position), gameTime);
                            }
                            else
                            {
                                if (projectionX < projectionY)
                                {
                                    CollideWithAxis(projectionX, new Vector2(Math.Sign(delta.X), 0), gameTime);
                                }
                                else
                                {
                                    CollideWithAxis(projectionY, new Vector2(0, Math.Sign(delta.Y)), gameTime);
                                }
                            }
                        }
                    }
                    else
                    {
                        //Ah yes, the check the normal 5 voronoi reigons part. 
                        if ((tileRotation == 0 && (horizontal == 1 || vertical == -1)) ||
                            (tileRotation == 1 && (horizontal == 1 || vertical == 1)) ||
                            (tileRotation == 2 && (horizontal == -1 || vertical == 1)) ||
                            (tileRotation == 3 && (horizontal == -1 || vertical == -1)))
                        {
                            if (horizontal == 0)
                            {
                                CollideWithAxis(projectionY, new Vector2(0, Math.Sign(delta.Y)), gameTime);
                            }
                            else if (vertical == 0)
                            {
                                CollideWithAxis(projectionX, new Vector2(Math.Sign(delta.X), 0), gameTime);
                            }
                            else
                            {
                                Vector2 corner = tileCenter + new Vector2(Math.Sign(delta.X), Math.Sign(delta.Y)) * Tile.halfTileSize;
                                float projectionCorner = halfSize - Vector2.Distance(position, corner);
                                if (projectionCorner > 0)
                                {
                                    CollideWithAxis(projectionCorner, Vector2.Normalize(position - corner), gameTime);
                                }
                            }
                        }
                        else
                        {
                            // This is probably my worst written code in this entire segment.
                            // while I would like to blame it on the shape being convex, I really can't.
                            // If it is not in the center or the other 5 voronoi reigions, we need to check the corners as the only other option.
                            // I would check it like any other corner, but because one of the corners can't be checked against (the one where there is open space) I do... whatever this mess is.
                            // Basically I check it against each "real" corner (depending on the rotation) and see which one is closest.
                            float distanceToClosestCorner = float.MaxValue;
                            Vector2 closestCorner = Vector2.Zero;
                            if (tileRotation != 0)
                            {
                                Vector2 temporaryCorner = tileCenter + new Vector2(-Tile.halfTileSize, Tile.halfTileSize);
                                float temporaryDistance = Vector2.Distance(temporaryCorner, position);
                                if (temporaryDistance < distanceToClosestCorner)
                                {
                                    distanceToClosestCorner = temporaryDistance;
                                    closestCorner = temporaryCorner;
                                }
                            }
                            if (tileRotation != 1)
                            {
                                Vector2 temporaryCorner = tileCenter + new Vector2(-Tile.halfTileSize, -Tile.halfTileSize);
                                float temporaryDistance = Vector2.Distance(temporaryCorner, position);
                                if (temporaryDistance < distanceToClosestCorner)
                                {
                                    distanceToClosestCorner = temporaryDistance;
                                    closestCorner = temporaryCorner;
                                }
                            }
                            if (tileRotation != 2)
                            {
                                Vector2 temporaryCorner = tileCenter + new Vector2(Tile.halfTileSize, -Tile.halfTileSize);
                                float temporaryDistance = Vector2.Distance(temporaryCorner, position);
                                if (temporaryDistance < distanceToClosestCorner)
                                {
                                    distanceToClosestCorner = temporaryDistance;
                                    closestCorner = temporaryCorner;
                                }
                            }
                            if (tileRotation != 3)
                            {
                                Vector2 temporaryCorner = tileCenter + new Vector2(Tile.halfTileSize, Tile.halfTileSize);
                                float temporaryDistance = Vector2.Distance(temporaryCorner, position);
                                if (temporaryDistance < distanceToClosestCorner)
                                {
                                    distanceToClosestCorner = temporaryDistance;
                                    closestCorner = temporaryCorner;
                                }
                            }
                            float projection = halfSize - Vector2.Distance(position, closestCorner);
                            if (projection > 0)
                            {
                                CollideWithAxis(projection, Vector2.Normalize(position - closestCorner), gameTime);
                            }

                        }
                    }
                }
                // Triangle
                else if (type == 5)
                {
                    //The regular 5 voronoi regions, I should probably make a function by now but Im lazy.
                    if ((tileRotation == 0 && (horizontal == -1 || vertical == 1)) ||
                        (tileRotation == 1 && (horizontal == -1 || vertical == -1)) ||
                        (tileRotation == 2 && (horizontal == 1 || vertical == -1)) ||
                        (tileRotation == 3 && (horizontal == 1 || vertical == 1)))
                    {
                        if (horizontal == 0)
                        {
                            CollideWithAxis(projectionY, new Vector2(0, Math.Sign(delta.Y)), gameTime);
                        }
                        else if (vertical == 0)
                        {
                            CollideWithAxis(projectionX, new Vector2(Math.Sign(delta.X), 0), gameTime);
                        }
                        else
                        {
                            Vector2 corner = tileCenter + new Vector2(Math.Sign(delta.X), Math.Sign(delta.Y)) * Tile.halfTileSize;
                            float projection = halfSize - Vector2.Distance(position, corner);
                            if (projection > 0)
                            {
                                CollideWithAxis(projection, Vector2.Normalize(position - corner), gameTime);
                            }
                        }
                    }
                    else
                    {
                        //First I get the righthand slope normal of the slope depending on rotation.
                        Vector2 slope = Vector2.Zero;
                        if (tileRotation == 0)
                        {
                            slope = Vector2.Normalize(new Vector2(-1, 1));
                        }
                        else if (tileRotation == 1)
                        {
                            slope = Vector2.Normalize(new Vector2(-1, -1));
                        }
                        else if (tileRotation == 2)
                        {
                            slope = Vector2.Normalize(new Vector2(1, -1));
                        }
                        else
                        {
                            slope = Vector2.Normalize(new Vector2(1, 1));
                        }
                        float projectionSlope;
                        //For a straight line, there is only one possible point that the circle can collide on. Its also pretty easy to find and is useful in checking stuff.
                        Vector2 hitLocation = position + slope * halfSize;
                        //Here we find the vector that represents the center of the slope minus the center, this gives us a vector from the origin that represents the distance they are apart.
                        Vector2 originatedPosition = hitLocation - tileCenter;
                        //Here I use a dot product to find the projection the circle is into the curve.
                        //Ironically, this technique is also called "projection" in algebra, and is what you would find in your math textbook rather than what I have been calling projection for the past code.
                        projectionSlope = (originatedPosition.X * slope.X + originatedPosition.Y * slope.Y);
                        if (projectionSlope > 0)
                        {
                            //Hopefully if you have made it this far you know the procedure for this by now. If it is in the center check all the escape distanes, if not check whatever else.
                            if (horizontal == 0 && vertical == 0)
                            {
                                if (projectionSlope < projectionX && projectionSlope < projectionY)
                                {
                                    CollideWithAxis(projectionSlope, -slope, gameTime);
                                }
                                else
                                {
                                    if (projectionX < projectionY)
                                    {
                                        CollideWithAxis(projectionX, new Vector2(Math.Sign(delta.X), 0), gameTime);
                                    }
                                    else
                                    {
                                        CollideWithAxis(projectionY, new Vector2(0, Math.Sign(delta.Y)), gameTime);
                                    }
                                }
                            }
                            else
                            {
                                // The slope really likes continuing past where it should end, which can actually create some hilarious glitches where the player walks on these invisible walls.
                                // But like any other glitch, this needs to be fixed.
                                // One of the other advantages of taking the hit location is that it will tell us if the slope is actually in the boundaries.
                                // Now we can check it against one of the axis to see if it is in the tiles boundaries.
                                if (hitLocation.Y >= tileCenter.Y - Tile.halfTileSize && hitLocation.Y <= tileCenter.Y + Tile.halfTileSize)
                                {
                                    CollideWithAxis(projectionSlope, -slope, gameTime);
                                }
                                // The final possibility is the corner. The other conditionals make sure that this circle won't ever collide with the "fake" corner.
                                // Later though, this left out corner will come back.
                                else
                                {
                                    Vector2 corner = tileCenter + new Vector2(Math.Sign(delta.X), Math.Sign(delta.Y)) * Tile.halfTileSize;
                                    float projectionCorner = halfSize - Vector2.Distance(position, corner);
                                    if (projectionCorner > 0)
                                        CollideWithAxis(projectionCorner, Vector2.Normalize(position - corner), gameTime);
                                }
                            }
                        }
                    }
                }
                // Compressed Triangle 1
                else if (type == 6)
                {
                    // I want to deal with this like a half rectangle, so lets convert it to that (see type == 2 for more information)
                    if (tileRotation == 1 || tileRotation == 3)
                    {
                        if (tileRotation == 1)
                            tileCenter.X -= Tile.halfTileSize / 2;
                        else
                            tileCenter.X += Tile.halfTileSize / 2;
                        delta.X = position.X - tileCenter.X;
                        horizontal = 0;
                        if (delta.X < -Tile.halfTileSize / 2)
                            horizontal = -1;
                        else if (delta.X > Tile.halfTileSize / 2)
                            horizontal = 1;
                        projectionX = Tile.halfTileSize / 2 + halfSize - Math.Abs(delta.X);
                    }
                    else
                    {
                        if (tileRotation == 2)
                            tileCenter.Y -= Tile.halfTileSize / 2;
                        else
                            tileCenter.Y += Tile.halfTileSize / 2;
                        delta.Y = position.Y - tileCenter.Y;
                        vertical = 0;
                        if (delta.Y < -Tile.halfTileSize / 2)
                            vertical = -1;
                        else if (delta.Y > Tile.halfTileSize / 2)
                            vertical = 1;
                        projectionY = Tile.halfTileSize / 2 + halfSize - Math.Abs(delta.Y);
                    }
                    if (((tileRotation == 1 || tileRotation == 3) && Math.Abs(delta.X) <= halfSize + Tile.halfTileSize / 2.0f) ||
                            ((tileRotation == 2 || tileRotation == 0) && Math.Abs(delta.Y) <= halfSize + Tile.halfTileSize / 2.0f))
                    {
                        //Oh look, the 5 voronoi reigons came for a visit.
                        if ((tileRotation == 0 && (horizontal == 1 || vertical == 1)) ||
                            (tileRotation == 1 && (horizontal == -1 || vertical == 1)) ||
                            (tileRotation == 2 && (horizontal == -1 || vertical == -1)) ||
                            (tileRotation == 3 && (horizontal == 1 || vertical == -1)))
                        {
                            if (horizontal == 0)
                            {
                                CollideWithAxis(projectionY, new Vector2(0, Math.Sign(delta.Y)), gameTime);
                            }
                            else if (vertical == 0)
                            {
                                CollideWithAxis(projectionX, new Vector2(Math.Sign(delta.X), 0), gameTime);
                            }
                            else
                            {
                                Vector2 corner;
                                if (tileRotation == 1 || tileRotation == 3)
                                {
                                    corner = tileCenter + new Vector2(Math.Sign(delta.X) / 2.0f, Math.Sign(delta.Y)) * Tile.halfTileSize;
                                }
                                else
                                {
                                    corner = tileCenter + new Vector2(Math.Sign(delta.X), Math.Sign(delta.Y) / 2.0f) * Tile.halfTileSize;
                                }
                                float projection = halfSize - Vector2.Distance(position, corner);
                                if (projection > 0)
                                {
                                    CollideWithAxis(projection, Vector2.Normalize(position - corner), gameTime);
                                }
                            }
                        }
                        else
                        {
                            float projectionSlope;
                            Vector2 slope = Vector2.Zero;
                            if (tileRotation == 0)
                            {
                                slope = Vector2.Normalize(new Vector2(1, 2));
                            }
                            else if (tileRotation == 1)
                            {
                                slope = Vector2.Normalize(new Vector2(-2, 1));
                            }
                            else if (tileRotation == 2)
                            {
                                slope = Vector2.Normalize(new Vector2(-1, -2));
                            }
                            else
                            {
                                slope = Vector2.Normalize(new Vector2(2, -1));
                            }
                            Vector2 hitLocation = position + slope * halfSize;
                            Vector2 originatedPosition = hitLocation - tileCenter;
                            projectionSlope = (originatedPosition.X * slope.X + originatedPosition.Y * slope.Y);
                            if (projectionSlope > 0)
                            {
                                if (horizontal == 0 && vertical == 0)
                                {
                                    if (projectionSlope < projectionX && projectionSlope < projectionY)
                                    {
                                        CollideWithAxis(projectionSlope, -slope, gameTime);
                                    }
                                    else
                                    {
                                        if (projectionX < projectionY)
                                        {
                                            CollideWithAxis(projectionX, new Vector2(Math.Sign(delta.X), 0), gameTime);
                                        }
                                        else
                                        {
                                            CollideWithAxis(projectionY, new Vector2(0, Math.Sign(delta.Y)), gameTime);
                                        }
                                    }
                                }
                                else
                                {
                                    if (((tileRotation == 0 || tileRotation == 2) && hitLocation.X >= tileCenter.X - Tile.halfTileSize && hitLocation.X <= tileCenter.X + Tile.halfTileSize) || ((tileRotation == 1 || tileRotation == 3) && hitLocation.Y >= tileCenter.Y - Tile.halfTileSize && hitLocation.Y <= tileCenter.Y + Tile.halfTileSize))
                                    {
                                        CollideWithAxis(projectionSlope, -slope, gameTime);
                                    }
                                    else
                                    {

                                        Vector2 corner;
                                        if (tileRotation == 1 || tileRotation == 3)
                                        {
                                            if (tileRotation == 1 && delta.Y < 0)
                                                corner = tileCenter + new Vector2(-1 / 2.0f, Math.Sign(delta.Y)) * Tile.halfTileSize;
                                            else if (tileRotation == 3 && delta.Y > 0)
                                                corner = tileCenter + new Vector2(1 / 2.0f, Math.Sign(delta.Y)) * Tile.halfTileSize;
                                            else
                                                corner = tileCenter + new Vector2(Math.Sign(delta.X) / 2.0f, Math.Sign(delta.Y)) * Tile.halfTileSize;
                                        }
                                        else
                                        {
                                            if (tileRotation == 0 && delta.X < 0)
                                                corner = tileCenter + new Vector2(Math.Sign(delta.X), 1 / 2.0f) * Tile.halfTileSize;
                                            else if (tileRotation == 2 && delta.X > 0)
                                                corner = tileCenter + new Vector2(Math.Sign(delta.X), -1 / 2.0f) * Tile.halfTileSize;
                                            else
                                                corner = tileCenter + new Vector2(Math.Sign(delta.X), Math.Sign(delta.Y) / 2.0f) * Tile.halfTileSize;
                                        }
                                        float projectionCorner = halfSize - Vector2.Distance(position, corner);
                                        if (projectionCorner > 0)
                                            CollideWithAxis(projectionCorner, Vector2.Normalize(position - corner), gameTime);
                                    }
                                }
                            }
                        }
                    }
                }
                // Rectangle + Compressed Triangle 1
                else if (type == 7)
                {
                    //The regular 5 voronoi regions, I should probably make a function by now but Im lazy.
                    if ((tileRotation == 0 && (horizontal == 1 || vertical == 1)) ||
                        (tileRotation == 1 && (horizontal == -1 || vertical == 1)) ||
                        (tileRotation == 2 && (horizontal == -1 || vertical == -1)) ||
                        (tileRotation == 3 && (horizontal == 1 || vertical == -1)))
                    {
                        if (horizontal == 0)
                        {
                            CollideWithAxis(projectionY, new Vector2(0, Math.Sign(delta.Y)), gameTime);
                        }
                        else if (vertical == 0)
                        {
                            CollideWithAxis(projectionX, new Vector2(Math.Sign(delta.X), 0), gameTime);
                        }
                        else
                        {
                            Vector2 corner = tileCenter + new Vector2(Math.Sign(delta.X), Math.Sign(delta.Y)) * Tile.halfTileSize;
                            float projection = halfSize - Vector2.Distance(position, corner);
                            if (projection > 0)
                            {
                                CollideWithAxis(projection, Vector2.Normalize(position - corner), gameTime);
                            }
                        }
                    }
                    else
                    {
                        if (tileRotation == 0 && delta.Y > 0)
                        {
                            CollideWithAxis(projectionX, new Vector2(Math.Sign(delta.X), 0), gameTime);
                        }
                        else if (tileRotation == 1 && delta.X < 0)
                        {
                            CollideWithAxis(projectionY, new Vector2(0, Math.Sign(delta.Y)), gameTime);
                        }
                        else if (tileRotation == 2 && delta.Y < 0)
                        {
                            CollideWithAxis(projectionX, new Vector2(Math.Sign(delta.X), 0), gameTime);
                        }
                        else if (tileRotation == 3 && delta.X > 0)
                        {
                            CollideWithAxis(projectionY, new Vector2(0, Math.Sign(delta.Y)), gameTime);
                        }
                        else
                        {
                            if (tileRotation == 0)
                                tileCenter.Y -= Tile.halfTileSize / 2.0f;
                            else if (tileRotation == 1)
                                tileCenter.X += Tile.halfTileSize / 2.0f;
                            else if (tileRotation == 2)
                                tileCenter.Y += Tile.halfTileSize / 2.0f;
                            else
                                tileCenter.X -= Tile.halfTileSize / 2.0f;
                            float projectionSlope;
                            Vector2 slope = Vector2.Zero;
                            if (tileRotation == 0)
                            {
                                slope = Vector2.Normalize(new Vector2(1, 2));
                            }
                            else if (tileRotation == 1)
                            {
                                slope = Vector2.Normalize(new Vector2(-2, 1));
                            }
                            else if (tileRotation == 2)
                            {
                                slope = Vector2.Normalize(new Vector2(-1, -2));
                            }
                            else
                            {
                                slope = Vector2.Normalize(new Vector2(2, -1));
                            }
                            Vector2 hitLocation = position + slope * halfSize;
                            Vector2 originatedPosition = hitLocation - tileCenter;
                            projectionSlope = (originatedPosition.X * slope.X + originatedPosition.Y * slope.Y);
                            if (projectionSlope > 0)
                            {
                                if (horizontal == 0 && vertical == 0)
                                {
                                    if (projectionSlope < projectionX && projectionSlope < projectionY)
                                    {
                                        CollideWithAxis(projectionSlope, -slope, gameTime);
                                    }
                                    else
                                    {
                                        if (projectionX < projectionY)
                                        {
                                            CollideWithAxis(projectionX, new Vector2(Math.Sign(delta.X), 0), gameTime);
                                        }
                                        else
                                        {
                                            CollideWithAxis(projectionY, new Vector2(0, Math.Sign(delta.Y)), gameTime);
                                        }
                                    }
                                }
                                else
                                {
                                    if (((tileRotation == 0 || tileRotation == 2) && hitLocation.X >= tileCenter.X - Tile.halfTileSize && hitLocation.X <= tileCenter.X + Tile.halfTileSize) || ((tileRotation == 1 || tileRotation == 3) && hitLocation.Y >= tileCenter.Y - Tile.halfTileSize && hitLocation.Y <= tileCenter.Y + Tile.halfTileSize))
                                    {
                                        CollideWithAxis(projectionSlope, -slope, gameTime);
                                    }
                                    else
                                    {

                                        Vector2 corner;
                                        if (tileRotation == 1 || tileRotation == 3)
                                        {
                                            if (tileRotation == 1 && delta.Y < 0)
                                                corner = tileCenter + new Vector2(-1 / 2.0f, Math.Sign(delta.Y)) * Tile.halfTileSize;
                                            else if (tileRotation == 3 && delta.Y > 0)
                                                corner = tileCenter + new Vector2(1 / 2.0f, Math.Sign(delta.Y)) * Tile.halfTileSize;
                                            else
                                                corner = tileCenter + new Vector2(Math.Sign(delta.X) / 2.0f, Math.Sign(delta.Y)) * Tile.halfTileSize;
                                        }
                                        else
                                        {
                                            if (tileRotation == 0 && delta.X < 0)
                                                corner = tileCenter + new Vector2(Math.Sign(delta.X), 1 / 2.0f) * Tile.halfTileSize;
                                            else if (tileRotation == 2 && delta.X > 0)
                                                corner = tileCenter + new Vector2(Math.Sign(delta.X), -1 / 2.0f) * Tile.halfTileSize;
                                            else
                                                corner = tileCenter + new Vector2(Math.Sign(delta.X), Math.Sign(delta.Y) / 2.0f) * Tile.halfTileSize;
                                        }
                                        float projectionCorner = halfSize - Vector2.Distance(position, corner);
                                        if (projectionCorner > 0)
                                            CollideWithAxis(projectionCorner, Vector2.Normalize(position - corner), gameTime);
                                    }
                                }
                            }
                        }
                    }
                }
                // Rectangle + Compressed Triangle 2
                else if (type == 8)
                {
                    //The regular 5 voronoi regions, I should probably make a function by now but Im lazy.
                    if ((tileRotation == 0 && (horizontal == -1 || vertical == 1)) ||
                        (tileRotation == 1 && (horizontal == -1 || vertical == -1)) ||
                        (tileRotation == 2 && (horizontal == 1 || vertical == -1)) ||
                        (tileRotation == 3 && (horizontal == 1 || vertical == 1)))
                    {
                        if (horizontal == 0)
                        {
                            CollideWithAxis(projectionY, new Vector2(0, Math.Sign(delta.Y)), gameTime);
                        }
                        else if (vertical == 0)
                        {
                            CollideWithAxis(projectionX, new Vector2(Math.Sign(delta.X), 0), gameTime);
                        }
                        else
                        {
                            Vector2 corner = tileCenter + new Vector2(Math.Sign(delta.X), Math.Sign(delta.Y)) * Tile.halfTileSize;
                            float projection = halfSize - Vector2.Distance(position, corner);
                            if (projection > 0)
                            {
                                CollideWithAxis(projection, Vector2.Normalize(position - corner), gameTime);
                            }
                        }
                    }
                    else
                    {
                        if (tileRotation == 0 && delta.Y > 0)
                        {
                            CollideWithAxis(projectionX, new Vector2(Math.Sign(delta.X), 0), gameTime);
                        }
                        else if (tileRotation == 1 && delta.X < 0)
                        {
                            CollideWithAxis(projectionY, new Vector2(0, Math.Sign(delta.Y)), gameTime);
                        }
                        else if (tileRotation == 2 && delta.Y < 0)
                        {
                            CollideWithAxis(projectionX, new Vector2(Math.Sign(delta.X), 0), gameTime);
                        }
                        else if (tileRotation == 3 && delta.X > 0)
                        {
                            CollideWithAxis(projectionY, new Vector2(0, Math.Sign(delta.Y)), gameTime);
                        }
                        else
                        {
                            if (tileRotation == 0)
                                tileCenter.Y -= Tile.halfTileSize / 2.0f;
                            else if (tileRotation == 1)
                                tileCenter.X += Tile.halfTileSize / 2.0f;
                            else if (tileRotation == 2)
                                tileCenter.Y += Tile.halfTileSize / 2.0f;
                            else
                                tileCenter.X -= Tile.halfTileSize / 2.0f;
                            float projectionSlope;
                            Vector2 slope = Vector2.Zero;
                            if (tileRotation == 0)
                            {
                                slope = Vector2.Normalize(new Vector2(-1, 2));
                            }
                            else if (tileRotation == 1)
                            {
                                slope = Vector2.Normalize(new Vector2(-2, -1));
                            }
                            else if (tileRotation == 2)
                            {
                                slope = Vector2.Normalize(new Vector2(1, -2));
                            }
                            else
                            {
                                slope = Vector2.Normalize(new Vector2(2, 1));
                            }
                            Vector2 hitLocation = position + slope * halfSize;
                            Vector2 originatedPosition = hitLocation - tileCenter;
                            projectionSlope = (originatedPosition.X * slope.X + originatedPosition.Y * slope.Y);
                            if (projectionSlope > 0)
                            {
                                if (horizontal == 0 && vertical == 0)
                                {
                                    if (projectionSlope < projectionX && projectionSlope < projectionY)
                                    {
                                        CollideWithAxis(projectionSlope, -slope, gameTime);
                                    }
                                    else
                                    {
                                        if (projectionX < projectionY)
                                        {
                                            CollideWithAxis(projectionX, new Vector2(Math.Sign(delta.X), 0), gameTime);
                                        }
                                        else
                                        {
                                            CollideWithAxis(projectionY, new Vector2(0, Math.Sign(delta.Y)), gameTime);
                                        }
                                    }
                                }
                                else
                                {
                                    if (((tileRotation == 0 || tileRotation == 2) && hitLocation.X >= tileCenter.X - Tile.halfTileSize && hitLocation.X <= tileCenter.X + Tile.halfTileSize) || ((tileRotation == 1 || tileRotation == 3) && hitLocation.Y >= tileCenter.Y - Tile.halfTileSize && hitLocation.Y <= tileCenter.Y + Tile.halfTileSize))
                                    {
                                        CollideWithAxis(projectionSlope, -slope, gameTime);
                                    }
                                    else
                                    {

                                        Vector2 corner;
                                        if (tileRotation == 1 || tileRotation == 3)
                                        {
                                            if (tileRotation == 1 && delta.Y > 0)
                                                corner = tileCenter + new Vector2(-1 / 2.0f, Math.Sign(delta.Y)) * Tile.halfTileSize;
                                            else if (tileRotation == 3 && delta.Y < 0)
                                                corner = tileCenter + new Vector2(1 / 2.0f, Math.Sign(delta.Y)) * Tile.halfTileSize;
                                            else
                                                corner = tileCenter + new Vector2(Math.Sign(delta.X) / 2.0f, Math.Sign(delta.Y)) * Tile.halfTileSize;
                                        }
                                        else
                                        {
                                            if (tileRotation == 0 && delta.X > 0)
                                                corner = tileCenter + new Vector2(Math.Sign(delta.X), 1 / 2.0f) * Tile.halfTileSize;
                                            else if (tileRotation == 2 && delta.X < 0)
                                                corner = tileCenter + new Vector2(Math.Sign(delta.X), -1 / 2.0f) * Tile.halfTileSize;
                                            else
                                                corner = tileCenter + new Vector2(Math.Sign(delta.X), Math.Sign(delta.Y) / 2.0f) * Tile.halfTileSize;
                                        }
                                        float projectionCorner = halfSize - Vector2.Distance(position, corner);
                                        if (projectionCorner > 0)
                                            CollideWithAxis(projectionCorner, Vector2.Normalize(position - corner), gameTime);
                                    }
                                }
                            }
                        }
                    }
                }
                //Compressed Triangle 2
                else if (type == 9)
                {
                    // I want to deal with this like a half rectangle, so lets convert it to that (see type == 2 for more information)
                    if (tileRotation == 1 || tileRotation == 3)
                    {
                        if (tileRotation == 1)
                            tileCenter.X -= Tile.halfTileSize / 2;
                        else
                            tileCenter.X += Tile.halfTileSize / 2;
                        delta.X = position.X - tileCenter.X;
                        horizontal = 0;
                        if (delta.X < -Tile.halfTileSize / 2)
                            horizontal = -1;
                        else if (delta.X > Tile.halfTileSize / 2)
                            horizontal = 1;
                        projectionX = Tile.halfTileSize / 2 + halfSize - Math.Abs(delta.X);
                    }
                    else
                    {
                        if (tileRotation == 2)
                            tileCenter.Y -= Tile.halfTileSize / 2;
                        else
                            tileCenter.Y += Tile.halfTileSize / 2;
                        delta.Y = position.Y - tileCenter.Y;
                        vertical = 0;
                        if (delta.Y < -Tile.halfTileSize / 2)
                            vertical = -1;
                        else if (delta.Y > Tile.halfTileSize / 2)
                            vertical = 1;
                        projectionY = Tile.halfTileSize / 2 + halfSize - Math.Abs(delta.Y);
                    }
                    if (((tileRotation == 1 || tileRotation == 3) && Math.Abs(delta.X) <= halfSize + Tile.halfTileSize / 2.0f) ||
                            ((tileRotation == 2 || tileRotation == 0) && Math.Abs(delta.Y) <= halfSize + Tile.halfTileSize / 2.0f))
                    {
                        //Oh look, the 5 voronoi reigons came for a visit.
                        if ((tileRotation == 0 && (horizontal == -1 || vertical == 1)) ||
                            (tileRotation == 1 && (horizontal == -1 || vertical == -1)) ||
                            (tileRotation == 2 && (horizontal == 1 || vertical == -1)) ||
                            (tileRotation == 3 && (horizontal == 1 || vertical == 1)))
                        {
                            if (horizontal == 0)
                            {
                                CollideWithAxis(projectionY, new Vector2(0, Math.Sign(delta.Y)), gameTime);
                            }
                            else if (vertical == 0)
                            {
                                CollideWithAxis(projectionX, new Vector2(Math.Sign(delta.X), 0), gameTime);
                            }
                            else
                            {
                                Vector2 corner;
                                if (tileRotation == 1 || tileRotation == 3)
                                {
                                    corner = tileCenter + new Vector2(Math.Sign(delta.X) / 2.0f, Math.Sign(delta.Y)) * Tile.halfTileSize;
                                }
                                else
                                {
                                    corner = tileCenter + new Vector2(Math.Sign(delta.X), Math.Sign(delta.Y) / 2.0f) * Tile.halfTileSize;
                                }
                                float projection = halfSize - Vector2.Distance(position, corner);
                                if (projection > 0)
                                {
                                    CollideWithAxis(projection, Vector2.Normalize(position - corner), gameTime);
                                }
                            }
                        }
                        else
                        {
                            float projectionSlope;
                            Vector2 slope = Vector2.Zero;
                            if (tileRotation == 0)
                            {
                                slope = Vector2.Normalize(new Vector2(-1, 2));
                            }
                            else if (tileRotation == 1)
                            {
                                slope = Vector2.Normalize(new Vector2(-2, -1));
                            }
                            else if (tileRotation == 2)
                            {
                                slope = Vector2.Normalize(new Vector2(1, -2));
                            }
                            else
                            {
                                slope = Vector2.Normalize(new Vector2(2, 1));
                            }
                            Vector2 hitLocation = position + slope * halfSize;
                            Vector2 originatedPosition = hitLocation - tileCenter;
                            projectionSlope = (originatedPosition.X * slope.X + originatedPosition.Y * slope.Y);
                            if (projectionSlope > 0)
                            {
                                if (horizontal == 0 && vertical == 0)
                                {
                                    if (projectionSlope < projectionX && projectionSlope < projectionY)
                                    {
                                        CollideWithAxis(projectionSlope, -slope, gameTime);
                                    }
                                    else
                                    {
                                        if (projectionX < projectionY)
                                        {
                                            CollideWithAxis(projectionX, new Vector2(Math.Sign(delta.X), 0), gameTime);
                                        }
                                        else
                                        {
                                            CollideWithAxis(projectionY, new Vector2(0, Math.Sign(delta.Y)), gameTime);
                                        }
                                    }
                                }
                                else
                                {
                                    if (((tileRotation == 0 || tileRotation == 2) && hitLocation.X >= tileCenter.X - Tile.halfTileSize && hitLocation.X <= tileCenter.X + Tile.halfTileSize) || ((tileRotation == 1 || tileRotation == 3) && hitLocation.Y >= tileCenter.Y - Tile.halfTileSize && hitLocation.Y <= tileCenter.Y + Tile.halfTileSize))
                                    {
                                        CollideWithAxis(projectionSlope, -slope, gameTime);
                                    }
                                    else
                                    {

                                        Vector2 corner;
                                        if (tileRotation == 1 || tileRotation == 3)
                                        {
                                            if (tileRotation == 1 && delta.Y > 0)
                                                corner = tileCenter + new Vector2(-1 / 2.0f, Math.Sign(delta.Y)) * Tile.halfTileSize;
                                            else if (tileRotation == 3 && delta.Y < 0)
                                                corner = tileCenter + new Vector2(1 / 2.0f, Math.Sign(delta.Y)) * Tile.halfTileSize;
                                            else
                                                corner = tileCenter + new Vector2(Math.Sign(delta.X) / 2.0f, Math.Sign(delta.Y)) * Tile.halfTileSize;
                                        }
                                        else
                                        {
                                            if (tileRotation == 0 && delta.X > 0)
                                                corner = tileCenter + new Vector2(Math.Sign(delta.X), 1 / 2.0f) * Tile.halfTileSize;
                                            else if (tileRotation == 2 && delta.X < 0)
                                                corner = tileCenter + new Vector2(Math.Sign(delta.X), -1 / 2.0f) * Tile.halfTileSize;
                                            else
                                                corner = tileCenter + new Vector2(Math.Sign(delta.X), Math.Sign(delta.Y) / 2.0f) * Tile.halfTileSize;
                                        }
                                        float projectionCorner = halfSize - Vector2.Distance(position, corner);
                                        if (projectionCorner > 0)
                                            CollideWithAxis(projectionCorner, Vector2.Normalize(position - corner), gameTime);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public void CollideWithAxis(float scalarProjection, Vector2 normal, GameTime gameTime)
        {
            //This is where the bulk of the reaction code is done
            // First we push it out of the projected area.
            position += normal * (scalarProjection);
            // There is a possibility that it hit two corners, so we dont want to apply bouce or friction twice.
            // Therefore, we check the dotproduct to ensure that the velocity and the normal are pointing opposite directions.
            if (normal.X * velocity.X + velocity.Y * normal.Y < 0)
            {
                // To make calculations simpler, I rotate the normal.
                Vector2 newNormal = new Vector2(normal.Y, -normal.X);
                // Since the normal is normalized (hah that's kind of punny), we can determine wether the object is on walls or the ground.
                if (newNormal.X < -.7f)
                {
                    onGround = true;
                }
                if (Math.Abs(newNormal.Y) > .9f)
                {
                    onWall = true;
                }
                //We use a function I will show later that will rotate the velocity to the axis, so we can modify only the component of velocity perpendicular to the axis.
                velocity = Main.RotateVelocity(velocity, newNormal, true);

                // As described earlier, this is the non-bouncy collision.
                if (special == 0)
                    velocity.Y = .0005f;
                //Why not zero? This arbitrarily small number won't affect the code since it is instantly overpowered by things like gravity.
                //Also it helps point the character, so when I am checking the direction of the player, I can show it as pointing away from the wall.

                // Anything >= 2 is of a special laser type that will keep bouncing off walls until it reaches two
                // For example, an initial condition of 5 means it will bounce off of 5 walls and then stop at the last wall
                else if (special == 2)
                    velocity = -.01f * newNormal;

                // The other option is the bouncy one, which is used for things like the laser.
                else
                    // Since we rotated the velocity, all we have to do is reflect the y component.
                    velocity.Y = -velocity.Y;
                
                // Rotate it back with a function I will show later.
                if (special != 2)
                    velocity = Main.RotateVelocity(velocity, newNormal, false);

                // This is for the special bouncy laser that bounces off of a set number of walls
                // I determine the walls remaining by every number greater than 2, so if it hit a wall (which it did if it hit this code) I need to subtract from the remaining hits.
                if (special > 2)
                    special -= 1;
            }
            //Debug.WriteLine("Projection:" + scalarProjection);
            //Debug.WriteLine("Position: (" + position.X + "," + position.Y + ")");
            //Debug.WriteLine("Velocity: (" + velocity.X + "," + velocity.Y + ")");
            //Debug.WriteLine("Normal: (" + normal.X + "," + normal.Y + ")");
        }
    }
}
