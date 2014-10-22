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
    public class Flesh
    {
        public Vector2 position;
        Vector2 velocity;
        float speed;
        public float angle;

        public Flesh(Vector2 position)
        {
            this.position = position;
            this.velocity = new Vector2(Main.randint(-10, 10) / 10.0f, Main.randint(-20, 0) / 10.0f);
            this.speed = 0.8f;
            this.angle = (float)((Main.randint(0, 20) / 10) * Math.PI);
        }

        public int Update()
        {
            this.velocity.Y += 0.05f;
            this.position += this.velocity * this.speed;

            this.angle += 0.05f;

            if (this.velocity.Y > 3)
            {
                return 1;
            }

            return 0;
        }
    }
}
