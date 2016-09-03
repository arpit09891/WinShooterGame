using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Win8ShooterGame;


namespace Shooter
{
    class Player
    {
        // public Texture 2D PlayerTexture;
        public Animation PlayerAnimation;

        // Animation representing the player
        public Texture2D PlayerTexture;

        // Position  of the Player relative to the upper left side of the screen
        public Vector2 Position;

        // State of the player
        public bool Activel;

        // Amount of hit points the player has
        public int Health;

        // Get the width of the player ship
        public int Width
        {
            get { return PlayerAnimation.FrameWidth; }
        }

        // Get the height of the player ship
        public int Height
        {
            get { return PlayerAnimation.FrameHeight; }
        }

        public void Initialize(Animation animation, Vector2 position)
        {
            PlayerAnimation = animation;
            
            // Set the starting position of the player around the middle of the screen and to the back
            Position = position;

            // Set the player to be active
            Activel = true;

            // Set the player health
            Health = 100;
        }
        public void Update(GameTime gameTime)
        {
            PlayerAnimation.Position = Position;
            PlayerAnimation.Update(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            PlayerAnimation.Draw(spriteBatch);
        }
    }
}
