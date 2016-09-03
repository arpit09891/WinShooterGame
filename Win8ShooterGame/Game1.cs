using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Shooter;
using Microsoft.Xna.Framework.Input;
using System.Collections;
using System.Collections.Generic;
using System;
// using Microsoft.Xna.Framework.Input.Touch;

namespace Win8ShooterGame
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        Animation playerAnimation = new Animation();
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // Represents the player
        Player player;

        // Keyboard states used to determine key presses
        KeyboardState currentKeyboardState;
        KeyboardState previousKeyboardState;

        // Mouse states used to track Mouse button press
        MouseState currentMouseState;
        MouseState previousMouseState;

        // A movement speed for the player
        float playerMoveSpeed;

        Texture2D playerTexture;
        Texture2D enemyTexture;
        List<Enemy> enemies;
        // The rate at which the enemies appear
        TimeSpan enemySpawnTime;
        TimeSpan previousSpawnTime;
        // A random number generator
        Random random;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            // Initialize the player class
            player = new Shooter.Player();

            // Set a constant move speed
            playerMoveSpeed = 8.0f;

            //Initialize the enemies List
            enemies = new List<Enemy> { };
            // Set the time keepers to zero 
            previousSpawnTime = TimeSpan.Zero;

            // Used to determine how fast enemy respawns
            enemySpawnTime = TimeSpan.FromSeconds(0.5f);

            //Initialize our number generator
            random = new Random();

            this.IsMouseVisible = true;
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
 
            // TODO: use this.Content to load your game content here
            // Load the player resources
            //Animation playerAnimation = new Animation();
            playerTexture = Content.Load<Texture2D>("shipAnimation");
           
            playerAnimation.Initialize(playerTexture, Vector2.Zero, 111, 92, 2, 30, Color.White, 1f, true);

            Vector2 playerPostion = new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + GraphicsDevice.Viewport.TitleSafeArea.Height/2);

            player.Initialize(playerAnimation, playerPostion);

            // Enemy Loading
            enemyTexture = Content.Load<Texture2D>("mario");

        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // Save the previous state of the keyboard so we can determine single key/button presses
            previousKeyboardState = currentKeyboardState;

            // Read the current state of the keyboard and mouse and store it
            currentKeyboardState = Keyboard.GetState();

            currentMouseState = Mouse.GetState();
            
            //update the player
            UpdatePlayer(gameTime);

            // Update the enemies
            UpdateEnemies(gameTime);

            UpdateCollision();


            base.Update(gameTime);
        }

        private void AddEnemy()
        {
            // Create the animation object
            Animation enemyAnimation = new Animation();
           

            //Initialize the animation with the correct animation information
            enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 150, 201, 1, 30, Color.White, 0.25f, true);

            //Randomly generate the position of the enemy
            Vector2 position = new Vector2( GraphicsDevice.Viewport.Width + enemyTexture.Width, (random.Next(100,GraphicsDevice.Viewport.Height - 100)));

            // Create an enemy
            Enemy enemy = new Enemy();

            // Initialize the enemy
            enemy.Initialize(enemyAnimation, position);

            // Add the enemy to active enemies list
            enemies.Add(enemy);
        }

        private void UpdateEnemies(GameTime gameTime)
        {
            // Spawn a new enemy every 1.5 seconds
            if(gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime)
            {
               previousSpawnTime = gameTime.TotalGameTime;

               //Add an enemy
               AddEnemy();
            }
            //Update the enemies
            for(int i = enemies.Count - 1; i >= 0; i-- )
            {
                enemies[i].Update(gameTime);
                if(enemies[i].Active == false)
                {
                    enemies.RemoveAt(i);
                }
            }
            
        }
        private void UpdatePlayer(GameTime gameTime)
        {
            player.Update(gameTime);
            
            // Use the Keyboard
            if(currentKeyboardState.IsKeyDown(Keys.Left))
            {
                player.Position.X -= playerMoveSpeed;
            }

            if (currentKeyboardState.IsKeyDown(Keys.Right))
            {
                player.Position.X += playerMoveSpeed;
            }
            if(currentKeyboardState.IsKeyDown(Keys.Up))
            {
                player.Position.Y -= playerMoveSpeed;
            }
            if(currentKeyboardState.IsKeyDown(Keys.Down))
            {
                player.Position.Y += playerMoveSpeed;
            }

            // Make sure that the player doesnot go out of bounds.
            player.Position.X = MathHelper.Clamp(player.Position.X, playerAnimation.FrameWidth/2, GraphicsDevice.Viewport.Width - playerAnimation.FrameWidth / 2);
            player.Position.Y = MathHelper.Clamp(player.Position.Y, playerAnimation.FrameHeight/2, GraphicsDevice.Viewport.Height - playerAnimation.FrameHeight / 2);

            //Get Mouse State then Capture the button type and respond Button press
            Vector2 mousePosition = new Vector2(currentMouseState.X, currentMouseState.Y);

            if(currentMouseState.LeftButton == ButtonState.Pressed)
            {
                Vector2 posDelta = mousePosition - player.Position;
                posDelta.Normalize();
                posDelta = posDelta * playerMoveSpeed;
                player.Position = player.Position + posDelta;
            }
        }

        private void UpdateCollision()
        {
            //Use the Rectangle's built-in intersect function to determine if two objects are overlapping
            Rectangle rectangle1;
            Rectangle rectangle2;

            //only create the rectangle once for the player
            rectangle1 = new Rectangle((int)player.Position.X, (int)player.Position.Y, player.Width, player.Height);
            // Do the collision between the player and the enemies

            for(int i =0; i<enemies.Count;i++)
            {
                rectangle2 = new Rectangle((int)enemies[i].Position.X, (int)enemies[i].Position.Y, enemies[i].Width/4, enemies[i].Height/4);
               // Determine if the two objects collided with each other

            if(rectangle1.Intersects(rectangle2))
                {
                    //Substract the health from player based on the enemy dmage
                    player.Health -= enemies[i].Damage;
                    //Since the enemy collided with the player destoy it
                    enemies[i].Health = 0;
                    //If the player health is less than zero we died
                    if(player.Health <=0)
                    {
                        player.Activel = false;
                    }
                }

            }
        }

         /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // Start drawing
            spriteBatch.Begin();

            //spriteBatch.Draw(playerTexture, Vector2.Zero, source, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);

            // Draw the Player
            player.Draw(spriteBatch);


            //Draw the enemies
            for (int i = 0; i < enemies.Count; i++)
            {
                enemies[i].Draw(spriteBatch);
            }

            // Stop drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
