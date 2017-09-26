using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.IO;
using System.Text;
namespace LazerCraft
{
    public class Loadout
    {
        int playerOwner;

        public Ability[] abilities;

        public int jumpStrength;
        public int glideControl;
        public int maxSpeed;
        public int turnStrength;
        public int acceleration;
        public int maxHealth;

        public Color prefferedColor;

        public string name;

        public Loadout(int playerOwner)
        {
            this.playerOwner = playerOwner;

            jumpStrength=1;
            glideControl=1;
            maxSpeed=1;
            turnStrength=1;
            acceleration=1;
            maxHealth=1;

            abilities = new Ability[4];
        }

        public void Draw(SpriteBatch spriteBatch, GameTime gameTime, bool gameNotEditor)
        {
            for (int index = 0; index < 4; index += 1)
            {
                //abilities[index].Draw(spriteBatch, gameTime, gameNotEditor);
            }
        }
        public void Update(GameTime gameTime)
        {
            for (int index = 0; index < 4; index += 1)
            {
                //abilities[index].Update(gameTime);
            }
        }

        public void setAbility(int position, int mainType, int subType, int modifier)
        {
            //abilities[position] = new Ability(mainType, subType, modifier, playerOwner);
        }
    }
}