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
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Main : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Textures
        Texture2D[] jeepPics = new Texture2D[3];
        Texture2D[] ZombiePics = new Texture2D[2];
        Texture2D[] hats = new Texture2D[9];
        Texture2D[] smallhats = new Texture2D[9];
        Texture2D[] ZombieSpawn = new Texture2D[5];
        Texture2D background, bulletPic, fleshPic;
        Texture2D hatOutline, deadZombiePic;
        Texture2D speedometer, speedoneedle;
        Texture2D gun, overheatgun;
        Texture2D gameoverPic, introPic;
        Texture2D startButton, tryagainButton;
        Texture2D health;

        SpriteFont font;

        SoundEffect shotsSound;
        SoundEffect windSound;
        SoundEffectInstance shotsInstance;
        SoundEffectInstance windInstance;

        int[] hatList = new int[9];

        public static int screenWidth = 1200;
        public static int screenHeight = 720;

        KeyboardState keyboardState;
        KeyboardState oldKeyboardState;
        MouseState mouseState;
        MouseState oldMouseState;

        public static float speed = 1;

        double gunAngle = Math.PI;

        public static Vector2 playerPosition = new Vector2(0, 1080 / 2 - 40);
        public static Rectangle playerRect = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, 169, 73);
        public static int playerhealth = 100;

        public static Stopwatch stopwatch = new Stopwatch();
        public static Random random = new Random();

        public static List<Bullet> bulletList = new List<Bullet>();
        public static List<Flesh> fleshList = new List<Flesh>();
        public static List<Zombie> ZombieList = new List<Zombie>();

        public static GameState gameState = GameState.Start;

        bool isWindowActive;

        bool isGunBroken = false;

        float heat = 0.0f; // 0 out of 1.

        //For short invulnerability
        public static long lastHit = stopwatch.ElapsedMilliseconds;

        //The background will be displayed somewhere else depending on this value
        float[] bgPosition = new float[] { 0, 1920 };

        //buttons (not a class because its 4:04 AM)
        Vector2 startButtonPosition;
        Vector2 tryagainButtonPosition;

        public Main()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            
            graphics.PreferredBackBufferWidth = screenWidth;
            graphics.PreferredBackBufferHeight = screenHeight;

            this.Window.AllowUserResizing = true;
            this.IsMouseVisible = true;
        }

        public static int randint(int low, int high)
        {
            return random.Next(low, high + 1);
        }
        
        public void SpawnZombie(Rectangle bounds, int amount)
        {
            for (int i = 0; i < amount; i++ )
            {
                Vector2 randomPosition = new Vector2(randint(bounds.X, bounds.X + bounds.Width), randint(bounds.Y, bounds.Y + bounds.Width));
                ZombieList.Add(new Zombie(randomPosition, new Vector2(0, 0), 0.5f, (Hat)(randint(0, 8))));
            }
        }

        public void ResetAll()
        {
            speed = 1;

            gunAngle = Math.PI;

            playerPosition = new Vector2(0, 1080 / 2 - 40);
            playerRect = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, 169, 73);
            playerhealth = 100;

            bulletList = new List<Bullet>();
            fleshList = new List<Flesh>();
            ZombieList = new List<Zombie>();

            isGunBroken = false;

            hatList = new int[9];
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            stopwatch.Start();

            startButtonPosition = new Vector2(screenWidth / 2, screenHeight - 100);
            tryagainButtonPosition = new Vector2(screenWidth / 2, screenHeight - 100);

            

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            jeepPics[0] = Content.Load<Texture2D>("jeep1");
            jeepPics[1] = Content.Load<Texture2D>("jeep2");
            jeepPics[2] = Content.Load<Texture2D>("jeep3");

            ZombiePics[0] = Content.Load<Texture2D>("zombie1");
            ZombiePics[1] = Content.Load<Texture2D>("zombie2");

            background = Content.Load<Texture2D>("bg");
            gun = Content.Load<Texture2D>("gun");
            overheatgun = Content.Load<Texture2D>("overheatgun");
            bulletPic = Content.Load<Texture2D>("bullet");
            fleshPic = Content.Load<Texture2D>("flesh");
            hatOutline = Content.Load<Texture2D>("hatoutline");
            deadZombiePic = Content.Load<Texture2D>("deadZombie");

            speedometer = Content.Load<Texture2D>("speedometer");
            speedoneedle = Content.Load<Texture2D>("speedometer needle");

            gameoverPic = Content.Load<Texture2D>("GAME OVER");
            tryagainButton = Content.Load<Texture2D>("tryagainutton");
            startButton = Content.Load<Texture2D>("startbutton");
            introPic = Content.Load<Texture2D>("intro");

            health = Content.Load<Texture2D>("health");

            font = Content.Load<SpriteFont>("font");

            shotsSound = Content.Load<SoundEffect>("shotsfirededit");
            windSound = Content.Load<SoundEffect>("wind");

            string path = Directory.GetCurrentDirectory();

            string[] filesInDir = Directory.GetFiles(path + "\\content\\hats");

            for (int i = 0; i < filesInDir.Length; i++)
            {
                hats[i] = Content.Load<Texture2D>("hats\\" + filesInDir[i].Split('\\')[filesInDir[i].Split('\\').Length - 1].Split('.')[0]);
            }

            filesInDir = Directory.GetFiles(path + "\\content\\smallhats");

            for (int i = 0; i < filesInDir.Length; i++)
            {
                smallhats[i] = Content.Load<Texture2D>("smallhats\\" + filesInDir[i].Split('\\')[filesInDir[i].Split('\\').Length - 1].Split('.')[0]);
            }

            filesInDir = Directory.GetFiles(path + "\\content\\zombiespawn");

            for (int i = 0; i < filesInDir.Length; i++)
            {
                ZombieSpawn[i] = Content.Load<Texture2D>("zombiespawn\\" + filesInDir[i].Split('\\')[filesInDir[i].Split('\\').Length - 1].Split('.')[0]);
            }

            shotsInstance = shotsSound.CreateInstance();
            windInstance = windSound.CreateInstance();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                this.Exit();

            //debugging
            if (Keyboard.GetState().IsKeyDown(Keys.F3))
            {
                try
                {
                    SpawnZombie(new Rectangle(600, 40, 100, 100), 1);
                }
                catch
                {

                }
            }
            
            screenWidth = GraphicsDevice.Viewport.Width;
            screenHeight = GraphicsDevice.Viewport.Height;

            keyboardState = Keyboard.GetState();
            mouseState = Mouse.GetState();

            //Update the active window check
            isWindowActive = this.IsActive;

            if (isWindowActive)
            {
                if (randint(0, 600) == 55 && windInstance.State == SoundState.Stopped)
                {
                    windInstance.Play();
                }

                if (gameState == GameState.Game)
                {
                    playerPosition = new Vector2(180 * speed, screenHeight / 2 - 40);
                    playerRect = new Rectangle((int)playerPosition.X, (int)playerPosition.Y, 169, 73);

                    if (keyboardState.IsKeyDown(Keys.Up))
                    {
                        speed *= 1.01f;
                    }
                    if (keyboardState.IsKeyDown(Keys.Down))
                    {
                        speed /= 1.01f;
                    }

                    //Speed limit
                    speed = speed <= 0.1f ? 0.1f : speed;
                    speed = speed >= 3f ? 3f : speed;

                    bgPosition[0] -= speed;
                    bgPosition[1] -= speed;

                    if (bgPosition[0] < -1920)
                    {
                        bgPosition[0] = 0;
                        bgPosition[1] = 1920;
                    }

                    //Calculate the sides of the triangle
                    Vector2 gunPosition = playerPosition + new Vector2(107, 27);

                    float verticalSide = mouseState.Y - gunPosition.Y;
                    float horizontalSide = mouseState.X - gunPosition.X;
                    float diagonalSide = (float)Math.Sqrt(Math.Pow(verticalSide, 2) + Math.Pow(horizontalSide, 2));

                    gunAngle = Math.Asin(verticalSide / diagonalSide);

                    if (mouseState.X < gunPosition.X)
                    {
                        gunAngle = Math.PI - gunAngle;
                    }

                    //Shoot at click
                    if (mouseState.LeftButton == ButtonState.Pressed && isGunBroken == false)
                    {
                        float spread = 65f;
                        Vector2 bulletVelocity = new Vector2((float)Math.Cos(gunAngle) + randint(-5, 5) / spread, (float)Math.Sin(gunAngle) + randint(-5, 5) / spread);
                        Vector2 bulletPosition = new Vector2((float)Math.Cos(gunAngle) * 25, (float)Math.Sin(gunAngle) * 25) + gunPosition;
                        bulletList.Add(new Bullet(bulletPosition, bulletVelocity, 16, (float)gunAngle, bulletList.Count));

                        //sound
                        if (shotsInstance.State == SoundState.Stopped)
                        {
                            shotsInstance.Play();
                        }

                        //heat up
                        heat += 0.002f;
                    }
                    else if (heat >= 0)
                    {
                        heat -= 0.008f;
                    }

                    if (heat >= 1)
                    {
                        isGunBroken = true;
                    }

                    else if (heat <= 0)
                    {
                        isGunBroken = false;
                    }

                    //Update Zombies
                    for (int i = ZombieList.Count - 1; i >= 0; i--)
                    {
                        if (ZombieList[i].Update() == 1)
                        {
                            try
                            {
                                hatList[(int)ZombieList[i].hat]++;
                            }
                            catch (IndexOutOfRangeException)
                            {
                                Console.WriteLine("ERROR INFO: " + (int)ZombieList[i].hat);
                            }

                            ZombieList.RemoveAt(i);
                        }
                    }

                    for (int i = bulletList.Count - 1; i >= 0; i--)
                    {
                        if (bulletList[i].Update() == 1)
                        {
                            bulletList.RemoveAt(i);
                        }
                    }

                    for (int i = fleshList.Count - 1; i >= 0; i--)
                    {
                        if (fleshList[i].Update() == 1)
                        {
                            fleshList.RemoveAt(i);
                        }
                    }

                    if (randint(0, 20) == 20)
                    {
                        if (randint(0, 1) == 0)
                        {
                            SpawnZombie(new Rectangle(randint(550, screenWidth < 720 ? 720 : screenWidth), randint(0, 200), 120, 120), randint(1, 4));
                        }
                        else
                        {
                            SpawnZombie(new Rectangle(randint(550, screenWidth < 720 ? 720 : screenWidth), randint(screenHeight - 200, screenHeight), 120, 120), randint(1, 4));
                        }
                    }

                    if (playerhealth < 0)
                    {
                        gameState = GameState.End;
                    }
                }
            }

            if (gameState == GameState.Start)
            {
                startButtonPosition = new Vector2(screenWidth / 2 - startButton.Width / 2, screenHeight - 200);

                if (mouseState.LeftButton == ButtonState.Pressed && (mouseState.X > startButtonPosition.X && mouseState.X < startButtonPosition.X + startButton.Width && mouseState.Y > startButtonPosition.Y && mouseState.Y < startButtonPosition.Y + startButton.Width))
                {
                    gameState = GameState.Game;
                }
            }

            if (gameState == GameState.End)
            {
                tryagainButtonPosition = new Vector2(screenWidth / 2, screenHeight - 300);

                if (mouseState.LeftButton == ButtonState.Pressed && (mouseState.X > tryagainButtonPosition.X && mouseState.X < tryagainButtonPosition.X + tryagainButton.Width && mouseState.Y > tryagainButtonPosition.Y && mouseState.Y < tryagainButtonPosition.Y + tryagainButton.Width))
                {
                    ResetAll();
                    gameState = GameState.Game;
                }
            }

            oldKeyboardState = Keyboard.GetState();
            oldMouseState = Mouse.GetState();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkGray);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);

            if (gameState == GameState.Game)
            {
                spriteBatch.Draw(background, new Vector2(bgPosition[0], (screenHeight / 2) - 540), Color.White);
                spriteBatch.Draw(background, new Vector2(bgPosition[1], (screenHeight / 2) - 540), Color.White);

                spriteBatch.Draw(jeepPics[stopwatch.Elapsed.Seconds % 2], new Vector2(180 * speed, screenHeight / 2 - 40), Color.White);

                spriteBatch.Draw(gun, new Vector2(180 * speed + 107, screenHeight / 2 - 15 + stopwatch.Elapsed.Seconds % 2), new Rectangle(0, 0, 56, 11), Color.White, (float)gunAngle, new Vector2(56 / 2, 11 / 2), 1.0f, SpriteEffects.None, 0);
                spriteBatch.Draw(overheatgun, new Vector2(180 * speed + 107, screenHeight / 2 - 15 + stopwatch.Elapsed.Seconds % 2), new Rectangle(0, 0, 56, 11), Color.White * heat, (float)gunAngle, new Vector2(56 / 2, 11 / 2), 1 + (heat - 0.5f), SpriteEffects.None, 0);


                //draw bullets
                foreach (Bullet bullet in bulletList)
                {
                    spriteBatch.Draw(bulletPic, bullet.position, new Rectangle(0, 0, 2, 6), Color.White, (float)(bullet.angle + 0.5 * Math.PI), new Vector2(2 / 2, 6 / 2), 1.0f, SpriteEffects.None, 0);
                }

                //Draw Zombies & hats
                foreach (Zombie Zombie in ZombieList)
                {
                    if (stopwatch.ElapsedMilliseconds - Zombie.timeOfBirth >= 1000 && Zombie.dead == false)
                    {
                        spriteBatch.Draw(ZombiePics[stopwatch.Elapsed.Seconds % 2], Zombie.position, new Rectangle(0, 0, 16, 40), Color.White, 0.0f, new Vector2(0, 0), 1.0f, Zombie.velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                        spriteBatch.Draw(hats[(int)Zombie.hat], Zombie.position, new Rectangle(0, 0, 16, 40), Color.White, 0.0f, new Vector2(0, 0), 1.0f, Zombie.velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                    }
                    else if (stopwatch.ElapsedMilliseconds - Zombie.timeOfBirth <= 1000)
                    {
                        //draw Zombies
                        long spawnSprite = ((stopwatch.ElapsedMilliseconds - Zombie.timeOfBirth) - (stopwatch.ElapsedMilliseconds - Zombie.timeOfBirth) % 200) / 200;

                        if (spawnSprite >= 0 && spawnSprite <= 4)
                        {
                            spriteBatch.Draw(ZombieSpawn[spawnSprite], Zombie.position, Color.White);

                            //draw hats
                            int[] hatPosList = new int[] { 24, 17, 12, 7, 1 };
                            spriteBatch.Draw(hats[(int)Zombie.hat], new Vector2(Zombie.position.X, Zombie.position.Y + hatPosList[spawnSprite]), Color.White);
                        }
                    }
                    else
                    {
                        spriteBatch.Draw(hats[(int)Zombie.hat], Zombie.position, new Rectangle(0, 0, 16, 40), Color.White, 0.0f, new Vector2(0, 0), 1.0f, Zombie.velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                    }
                }

                foreach (Flesh flesh in fleshList)
                {
                    spriteBatch.Draw(fleshPic, flesh.position, new Rectangle(0, 0, 5, 3), Color.White, flesh.angle, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0);
                }

                //draw hats in the cart
                int totalHats = 0;
                foreach (int hatnum in hatList)
                {
                    totalHats += hatnum;
                }

                int stackHeight = (int)(totalHats / 100.0f) + 1;
                int stackWidth = 10;

                for (int y = 1; y <= stackHeight; y++)
                {
                    for (int x = 1; x <= stackWidth; x++)
                    {
                        Vector2 hatPos = new Vector2(x * 7 - 5, (-y * 4) + 41 - (stopwatch.Elapsed.Seconds % 2));
                        spriteBatch.Draw(smallhats[(x * y % 8)], playerPosition + hatPos, Color.White);
                    }
                }

                //draw HUD
                spriteBatch.Draw(deadZombiePic, new Vector2(4, 4), Color.White);
                spriteBatch.DrawString(font, "x" + totalHats, new Vector2(52, 16), Color.DarkGreen);

                for (int i = 1; i < 10; i++)
                {
                    spriteBatch.Draw(hatOutline, new Vector2(4, 4 + i * 48), Color.White);
                    spriteBatch.Draw(hats[i - 1], new Vector2(14, 10 + i * 48), new Rectangle(0, 0, 16, 40), Color.White, 0.0f, new Vector2(0, 0), 1.8f, SpriteEffects.None, 0);

                    spriteBatch.DrawString(font, "x" + hatList[i - 1], new Vector2(52, 16 + i * 48), Color.LightGray);
                }

                spriteBatch.Draw(health, new Vector2(0, screenHeight - health.Height), new Rectangle(0, 0, health.Width, health.Height), Color.White, 0.0f, new Vector2(0, 0), new Vector2((playerhealth / 100.0f), 1), SpriteEffects.None, 0);
                spriteBatch.Draw(speedometer, new Vector2(0, screenHeight - speedometer.Height), Color.White);
                spriteBatch.Draw(speedoneedle, new Vector2(76, screenHeight - 45), new Rectangle(0, 0, 152, 10), Color.White, (float)((speed / 3.0f) * Math.PI), new Vector2(76, 7), 1.0f, SpriteEffects.None, 0);
            }

            if (gameState == GameState.End)
            {
                spriteBatch.Draw(jeepPics[2], new Vector2(180 * speed, screenHeight / 2 - 40), Color.White);

                //draw hats in the cart
                int totalHats = 0;
                foreach (int hatnum in hatList)
                {
                    totalHats += hatnum;
                }

                int stackHeight = (int)(totalHats / 100.0f) + 1;
                int stackWidth = 10;

                for (int y = 1; y <= stackHeight; y++)
                {
                    for (int x = 1; x <= stackWidth; x++)
                    {
                        Vector2 hatPos = new Vector2(x * 7 - 5, (-y * 4) + 41);
                        spriteBatch.Draw(smallhats[(x * y % 8)], playerPosition + hatPos, Color.White);
                    }
                }

                spriteBatch.Draw(deadZombiePic, new Vector2(4, 4), Color.White);
                spriteBatch.DrawString(font, "x" + totalHats, new Vector2(52, 16), Color.DarkGreen);

                for (int i = 1; i < 10; i++)
                {
                    spriteBatch.Draw(hatOutline, new Vector2(4, 4 + i * 48), Color.White);
                    spriteBatch.Draw(hats[i - 1], new Vector2(14, 10 + i * 48), new Rectangle(0, 0, 16, 40), Color.White, 0.0f, new Vector2(0, 0), 1.8f, SpriteEffects.None, 0);

                    spriteBatch.DrawString(font, "x" + hatList[i - 1], new Vector2(52, 16 + i * 48), Color.LightGray);
                }

                //Draw Zombies & hats
                foreach (Zombie Zombie in ZombieList)
                {
                    if (stopwatch.ElapsedMilliseconds - Zombie.timeOfBirth >= 1000 && Zombie.dead == false)
                    {
                        spriteBatch.Draw(ZombiePics[1], Zombie.position, new Rectangle(0, 0, 16, 40), Color.Black, 0.0f, new Vector2(0, 0), 1.0f, Zombie.velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                        spriteBatch.Draw(hats[(int)Zombie.hat], Zombie.position, new Rectangle(0, 0, 16, 40), Color.Black, 0.0f, new Vector2(0, 0), 1.0f, Zombie.velocity.X < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
                    }
                }

                spriteBatch.Draw(tryagainButton, tryagainButtonPosition, Color.White);

                spriteBatch.Draw(gameoverPic, new Vector2(75, 0), Color.White);
            }

            if (gameState == GameState.Start)
            {
                spriteBatch.Draw(startButton, startButtonPosition, Color.White);
                spriteBatch.Draw(introPic, new Vector2(screenWidth / 2 - 350, 0), Color.White);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
