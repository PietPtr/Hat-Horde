using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace HatHorde
{
    public class Bullet
    {
        public Vector2 position;
        public Vector2 velocity;

        public float speed;
        public float angle;

        public int index;

        public Bullet(Vector2 position, Vector2 velocity, float speed, float angle, int index)
        {
            this.position = position;
            this.velocity = velocity;
            this.speed = speed;
            this.angle = angle;
            this.index = index;
        }

        public int Update()
        {
            this.position = this.position += this.velocity * speed;

            if (this.position.X > Main.screenWidth || this.position.X < 0 || this.position.Y > Main.screenHeight || this.position.Y < 0)
            {
                return 1;
            }

            //Collisions
            foreach (Zombie Zombie in Main.ZombieList)
            {
                if (Zombie.dead == false && (this.position.X > Zombie.position.X && this.position.X < Zombie.position.X + 16 && this.position.Y > Zombie.position.Y && this.position.Y < Zombie.position.Y + 40))
                {
                    Zombie.dead = true;
                    Zombie.timeOfDeath = Main.stopwatch.ElapsedMilliseconds;

                    //Add a flesh-explosion
                    for (int i = 0; i < 5; i++ )
                    {
                        Main.fleshList.Add(new Flesh(this.position));
                    }

                    return 1;
                }
            }

            return 0;
        }
    }
}
