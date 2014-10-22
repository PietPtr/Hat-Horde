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
    public class Zombie
    {
        //movement
        public Vector2 position;
        public Vector2 velocity;
        public Vector2 goal;

        //appearence
        public Hat hat;

        public float speed;

        public long timeOfBirth;
        public long timeOfDeath;

        public bool dead = false;

        public Zombie(Vector2 position, Vector2 velocity, float speed, Hat hat)
        {
            this.position = position;
            this.velocity = velocity;
            this.hat = hat;
            this.speed = speed;

            this.timeOfBirth = Main.stopwatch.ElapsedMilliseconds;
        }

        public int Update()
        {
            //It takes 1 second to spawn
            if (Main.stopwatch.ElapsedMilliseconds - timeOfBirth > 1000 && this.dead == false)
            {
                this.goal = new Vector2(Main.playerPosition.X + 141, Main.playerPosition.Y + 14);

                float distanceFromPlayer = (float)(Math.Sqrt(Math.Pow(Math.Abs(this.position.X - this.goal.X), 2) + Math.Pow(Math.Abs(this.position.Y - this.goal.Y), 2)));
                if (distanceFromPlayer < 540)
                {
                    float verticalSide = this.goal.Y - this.position.Y;
                    float horizontalSide = this.goal.X - this.position.X;
                    float diagonalSide = (float)Math.Sqrt(Math.Pow(verticalSide, 2) + Math.Pow(horizontalSide, 2));

                    double angle = Math.Asin(verticalSide / diagonalSide);

                    if (this.goal.X < this.position.X)
                    {
                        angle = Math.PI - angle;
                    }

                    this.velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                }
                else
                {
                    this.velocity = new Vector2(0, 0);
                }

                this.position += this.velocity * this.speed;

                
            }
            //Stuff to do while spawning
            else if (Main.stopwatch.ElapsedMilliseconds - timeOfBirth < 1000)
            {
                //Cant die while spawning
                this.dead = false;
            }

            //Stuff that happens when dead
            if (this.dead == true)
            {
                if (Main.stopwatch.ElapsedMilliseconds - this.timeOfDeath > 1000)
                {
                    return 1;
                }
            }

            if (this.position.X > Main.playerRect.X && this.position.X < Main.playerRect.X + Main.playerRect.Width && this.position.Y > Main.playerRect.Y && this.position.Y < Main.playerRect.Y + Main.playerRect.Height)
            {
                Console.WriteLine(Main.stopwatch.ElapsedMilliseconds - Main.lastHit);
                if (Main.stopwatch.ElapsedMilliseconds - Main.lastHit > 500)
                {
                    Main.playerhealth -= Main.randint(1, 10);
                    Main.lastHit = Main.stopwatch.ElapsedMilliseconds;
                }
                

                //Add a flesh-explosion
                Main.fleshList.Add(new Flesh(this.position));

                this.timeOfDeath = Main.stopwatch.ElapsedMilliseconds;
                this.dead = true;

                
            }

            //Stuff that always happens
            this.position.X -= Main.speed;

            this.speed = -0.1f + Main.speed;

            return 0;
        }
    }
}
