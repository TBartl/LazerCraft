using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Text;
using System.Diagnostics;

namespace LazerCraft
{
    public class Player : Entity
    {
        int indexInList;

        public float respawn;
        public int team;
        public float animationBreathing;
        public float animationRunning;
        public bool facingForward;
        public bool charging;
        public float shieldRotation;

        public static float GRAVITY = .0078125f;
        public int jumpStrength;
        public int glideControl;
        public int turnStrength;
        public int acceleration;
        public int maxHealth;
        public int maxSpeed;

        public Loadout loadout;

        public Player():base(24.0f,0,Vector2.Zero, Vector2.Zero)
        {
            this.indexInList = 0;
            this.team = -1;
            this.animationRunning = 0f;
            this.animationBreathing = 0f;
            this.respawn = .05f;
            this.facingForward = true;
            this.onGround = false;
            this.onWall = false;
            this.charging = false;
            loadout = new Loadout(indexInList);
            setStats();

            loadout.setAbility(0, 2, 1, 0);
            loadout.setAbility(1, 2, 1, 1);
            loadout.setAbility(2, 2, 1, 2);
            loadout.setAbility(3, 1, 0, 2);
        }

        public void UpdateSelf(GameTime gameTime)
        {
            if (respawn>0)
            {
                respawn -= .001f * gameTime.ElapsedGameTime.Milliseconds;
                if (respawn <= 0)
                {
                    respawn = 0;
                }
            }
            else
            {
                #region Orientation
                if (velocity.X < 0)
                    facingForward = false;
                else if (velocity.X > 0)
                    facingForward = true;
                #endregion

                if (charging == true)
                {
                    special = 0;
                }
                else
                {
                    #region Moving
                    if (Main.keyUpPressed == true)
                    {
                        if (onWall && onGround)
                        {
                            velocity.Y -= getJumpStrength() * (float)Math.Sqrt(3) / 2.0f;
                            velocity.X += Math.Sign(velocity.X) * getJumpStrength() / 2.0f;
                            onGround = false;
                            onWall = false;
                        }
                        else if (onWall)
                        {
                            velocity.Y -= getJumpStrength() / (float)Math.Sqrt(2);
                            velocity.X += Math.Sign(velocity.X) * getJumpStrength() / (float)Math.Sqrt(2);
                            onWall = false;
                        }
                        else if (onGround)
                        {
                            velocity.Y -= getJumpStrength();
                            onGround = false;
                        }
                    }
                    if (Main.keyLeftPressed == true)
                    {
                        if (onGround == true)
                        {
                            if (velocity.X >= 0.0f)
                                velocity.X -= getTurnStrength(gameTime);
                            else
                                velocity.X -= getAcceleration(gameTime);
                        }
                        else
                            velocity.X -= getGlideControl(gameTime);
                    }
                    else if (Main.keyRightPressed == true)
                    {
                        if (onGround == true)
                        {
                            if (velocity.X <= 0.0f)
                                velocity.X += getTurnStrength(gameTime);
                            else
                                velocity.X += getAcceleration(gameTime);
                        }
                        else
                            velocity.X += getGlideControl(gameTime);
                    }
                    else if (onGround == true)
                    {
                        if (velocity.X > 0)
                            velocity.X -= Math.Min(velocity.X, velocity.X * getTurnStrength(gameTime) / 8f);
                        else if (velocity.X < 0)
                            velocity.X -= Math.Max(velocity.X, velocity.X * getTurnStrength(gameTime) / 8f);
                    }
                    if (onWall == false)
                    {
                        if (Main.keyUpPressed == true && velocity.Y > 0)
                        {
                            velocity.Y -= getGlideControl(gameTime);
                        }
                        else if (Main.keyDownPressed == true)
                        {
                            velocity.Y += getGlideControl(gameTime);
                        }
                    }
                    velocity.Y += getGravity(gameTime);
                    if (velocity.X >= getMaxSpeed())
                        velocity.X = getMaxSpeed();
                    else if (velocity.X <= -getMaxSpeed())
                        velocity.X = -getMaxSpeed();
                    #endregion
                }

                if (Main.keySpecial1Pressed == true)
                {
                    //loadout.abilities[0].Try(gameTime);
                }
                if (Main.keySpecial2Pressed == true)
                {
                    //loadout.abilities[1].Try(gameTime);
                }
                if (Main.keySpecial3Pressed == true)
                {
                    //loadout.abilities[2].Try(gameTime);
                }
                if (Main.keySpecial4Pressed == true)
                {
                    //loadout.abilities[3].Try(gameTime);
                }
                loadout.Update(gameTime);
                onGround = false;
                onWall = false;
                CollideWithEnvironment(gameTime);
                Main.screenPosition = position - Main.screenDimensions / 2f;
                if (Main.screenPosition.X < 0)
                    Main.screenPosition.X = 0;
                else if (Main.screenPosition.X > Main.level.xSize * Tile.tileSize - Main.screenDimensions.X)
                    Main.screenPosition.X = Main.level.xSize * Tile.tileSize - Main.screenDimensions.X;
                if (Main.screenPosition.Y < 0)
                    Main.screenPosition.Y = 0;
                else if (Main.screenPosition.Y > Main.level.ySize * Tile.tileSize - Main.screenDimensions.Y)
                    Main.screenPosition.Y = Main.level.ySize * Tile.tileSize - Main.screenDimensions.Y;
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (respawn <= 0)
            {
                if (charging == false)
                {
                    #region Chicken
                    if (onGround == true)
                    {
                        if (Math.Abs(velocity.X) >= .1f)
                        {
                            animationBreathing = 0f;
                            if (facingForward == true)
                                spriteBatch.Draw(Main.chicken, position - Main.screenPosition - new Vector2(20f), new Rectangle(40 * ((int)(animationRunning) - 2 * ((int)(animationRunning) / 3)), 40, 40, 40), Color.White);
                            else
                                spriteBatch.Draw(Main.chicken, position - Main.screenPosition - new Vector2(20f), new Rectangle(40 * ((int)(animationRunning) - 2 * ((int)(animationRunning) / 3)), 40, 40, 40), Color.White, 0f, Vector2.Zero, 1, SpriteEffects.FlipHorizontally, 1f);
                            animationRunning += Math.Abs(velocity.X / 65);

                            if (animationRunning >= 4)
                                animationRunning = 0;
                        }
                        else
                        {
                            animationRunning = 0f;
                            if (facingForward == true)
                                spriteBatch.Draw(Main.chicken, position - Main.screenPosition - new Vector2(20), new Rectangle(40 * (int)animationBreathing, 0, 40, 40), Color.White);
                            else
                                spriteBatch.Draw(Main.chicken, position - Main.screenPosition - new Vector2(20), new Rectangle(40 * (int)animationBreathing, 0, 40, 40), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 1f);
                            animationBreathing += .003f * gameTime.ElapsedGameTime.Milliseconds;
                            if (animationBreathing >= 2)
                                animationBreathing = 0;
                        }
                    }
                    else if (onWall == false)
                    {
                        if (velocity.Y > .5f)
                        {
                            if (facingForward == true)
                                spriteBatch.Draw(Main.chicken, position - Main.screenPosition - new Vector2(20f), new Rectangle(80, 40, 40, 40), Color.White);
                            else
                                spriteBatch.Draw(Main.chicken, position - Main.screenPosition - new Vector2(20f), new Rectangle(80, 40, 40, 40), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 1f);
                        }
                        else if (velocity.Y < -.5f)
                        {
                            if (facingForward == true)
                                spriteBatch.Draw(Main.chicken, position - Main.screenPosition - new Vector2(20f), new Rectangle(0, 40, 40, 40), Color.White);
                            else
                                spriteBatch.Draw(Main.chicken, position - Main.screenPosition - new Vector2(20f), new Rectangle(0, 40, 40, 40), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 1f);
                        }
                        else
                        {
                            if (facingForward == true)
                                spriteBatch.Draw(Main.chicken, position - Main.screenPosition - new Vector2(20f), new Rectangle(40, 40, 40, 40), Color.White);
                            else
                                spriteBatch.Draw(Main.chicken, position - Main.screenPosition - new Vector2(20f), new Rectangle(40, 40, 40, 40), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 1f);
                        }
                    }
                    else
                    {
                        if (facingForward == true)
                            spriteBatch.Draw(Main.chicken, position - Main.screenPosition - new Vector2(20f), new Rectangle(0, 40, 40, 40), Color.White);
                        else
                            spriteBatch.Draw(Main.chicken, position - Main.screenPosition - new Vector2(20f), new Rectangle(0, 40, 40, 40), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.FlipHorizontally, 1f);
                    }

                    spriteBatch.Draw(Main.shield, position - Main.screenPosition, null, getColor(), shieldRotation, new Vector2(13f, 13f), 1f, SpriteEffects.None, 1f);
                    
                    #endregion
                }
                loadout.Draw(spriteBatch, gameTime, true);
            }
        }

        public float getJumpStrength()
        {
            return getCurved(jumpStrength)/2 + 3;
        }
        public float getGravity(GameTime gameTime)
        {
            return GRAVITY * gameTime.ElapsedGameTime.Milliseconds;
        }
        public float getGlideControl(GameTime gameTime)
        {
            return Math.Min(GRAVITY, GRAVITY/10 *getCurved(glideControl)) * gameTime.ElapsedGameTime.Milliseconds;
        }
        public float getMaxSpeed()
        {
            return getCurved(maxSpeed) * 2 + 2.5f;
        }
        public float getTurnStrength(GameTime gameTime)
        {
            return Math.Min(getMaxSpeed(), getCurved(turnStrength) * .01f * gameTime.ElapsedGameTime.Milliseconds);
        }
        public float getAcceleration(GameTime gameTime)
        {
            return Math.Min(getMaxSpeed(),getCurved(acceleration) * .0025f * gameTime.ElapsedGameTime.Milliseconds);
        }
        public float getCurved(int x)
        {
            return 10 * (float)Math.Sqrt(.1 * x);
        }

        public void setStats()
        {
            this.jumpStrength = loadout.jumpStrength;
            this.glideControl = loadout.glideControl;
            this.turnStrength = loadout.turnStrength;
            this.acceleration = loadout.acceleration;
            this.maxHealth = loadout.maxHealth;
            this.maxSpeed = loadout.maxSpeed;
        }

        public Color getColor()
        {
            if (team == -1)
                return Main.currentColor;
            else if (team == 0)
                return loadout.prefferedColor;
            else if (team == 1)
                return Color.Red;
            else if (team == 2)
                return Color.Blue;
            else if (team == 3)
                return Color.Green;
            return Color.Yellow;
        }

        /* STATS PLANNING
         * ===============
         * Ground Control
         * 3) Wall Jumping
         * 5) Wall Running
         * ^) +20% Friction (Max 100%)
         * ^) +15% Turn Speed (Max 75%)
         * ^) +15% Acceleration (Max 75%)
         * ^) (3) +1 Max Speed (Max +8)
         * 
         * ================
         * Air Control
         * 3) Double Jump
         * 5) Flight
         * ^) +.5s Glide Duration (Max 2.5s)
         * ^) (2) +1 Jump Height (Max 7)
         * ^) +20% Jump Control (Max 100%)
         * ^) + 1 Max Speed in air (Max 5)
         * ^) (50%) + 10% Acceleration In air.
         * ^) 20% Greater Quick Fall.
         * 
         * ================
         * Ability Increase
         * 3) Refreshed Cooldowns on Kill/Assist
         * 5) Normal Abilities can be used for movement.
         * ^) (108%) - 8% Cooldowns (Max 68%)
         * 
         * =================
         * Durability Increase
         * 3) Explode upon death, dealing 4 damage.
         * 5) The Player now bounces off normal abilities;
         * ^) (10) +1 Increased Health
         * ^) (4s) - .5s Health Regen Speed(Max 1.5)
         */
    }
}
