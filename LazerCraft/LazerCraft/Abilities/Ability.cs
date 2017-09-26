using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace LazerCraft
{
    public class Ability
    {
        public int playerOwner;
        public int subType;
        public int modifier;

        public float cooldown;

        public Ability()
        {
        }

        public void Try()
        {

        }

        public void Update()
        {
        }

        public void Draw()
        {
        }

        static public void DrawLoadout()
        {
        }

        static public string GetDescription()
        {
            return "";
        }

        public float GetMaxCooldown()
        {
            return 0;
        }
    }

    //projectiles.Add(new Projectile(13f, players[0].position, Vector2.Normalize(-players[0].position + mouseLocation + screenPosition), 0));

    /* Abilities
     * =====================================================================
     * 
     * Normal
     * ======
     * *Assault Shot- Fires a single laser that travels a medium distance to deal 2 damage. Cooldown: 2 seconds.
     * 1)Slow Shot- Decreases the speed by 35% but reduces the enemies speed by 70% for 3 seconds.
     * 2)Frenzy Shot- Reduces the cooldown by 40% seconds but cuts damage in half but 
     * 
     * *Shotgun Blast - Fires 3 bullets in a spread that deal 1 damage at a short range. Cooldown: 2 seconds.
     * 1)Tank Blast- Increases cooldown to 1% but gain health equal to the amount of damage dealt.
     * 2)Assasin Blast- Hitting the target in the back causes damage to increase to 2, but cooldown is increased to x%.
     * 
     * *Ranger Bolt - Charges a high damage (3) laser that travels the furthest distance instantly, but has the longest cooldown(4 second).
     * 1)Poison Bolt - Upon hitting the target, shields for them will begin regenerating twice as slow but damage is now dealt over 3 seconds
     * 2)Extended Bolt - Leaves a trail where the shot was fired, dealing damage every second for 5 seconds, but reduces damage to 1.
     * 
     * Special
     * =======
     * *Crushing Charge - Charge towards the cursor turning your player into a laser that deals 2 damage and slows the player.
     * 1)Enduring Charge - The charge now lasts for x seconds, but does not deal damage on impact
     * 2)Tactical Charge - The charge becomes a teleport but deals no damage, increases the damge of your next attack by x.
     * 
     * * Devastating Missile- Fires a rapidly accelerating missile that deals high damage on impact in an area.
     * 1) Expiremetal Missile- The missile no longer knocks back you or your enemies, but creates a vortex sucking in players and certain lasers.
     * 2) Primed Missile - Cooldown reduced by x seconds, but instead of accelerating the mwissile decelerates. 
     * 
     * Tactical
     * ========
     * Stopping Circle-Creates a large circle that players will be not be able to pass through.
     * 1) Sticky Circle-The Circle now slows enemies by x% and allows you and your allies to pass through it.
     * 2) Stopping Square-The circle becomes a square.
     * 
     * Trip Mine- Places a mine at the player position that will detonate when an enemy walks near it dealing x damage.
     * 1) The mine is now invisible, but requires manual detonation.
     * 2) The mine becomes a block and primes after 2 seconds.
     * 
     * Boost
     * ======
     * Durable Shield - The player does not take damage for x seconds.
     * 1) Instead of blocking damage, the player absorbs it as health but the time active is reduced to x seconds.
     * 2) The shield now returns damage, but the cooldown is increased to x seconds.
     * 
     * Invisible Cloaker - The player becomes invisible for x seconds.
     * 1) Dummy Cloaker- Leaves behind a dummy that explodes when the duration ends.
     * 2) Charged Cloaker - The first attack coming out of invisibility now deals 1 more damage.
     */

}
