﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Win8ShooterGame
{
    class Animation
    {
        // The image representing the collection of images used for animation
        Texture2D spriteStrip;

        // The scale used to display the sprite strip
        float scale;

        //The time we last updated the frame
        int elapsedTime;

        // The time we display a frame until the next one
        int frameTime;

        // The number of frames that the animation contains
        int frameCount;

        // The index of the current frame we are displaying
        int currentFrame;

        //The color of the frame we will are displaying
        Color color;

        //The area of the image strip we want to display
        Rectangle sourceRect = new Rectangle();

        //The area where we want to display the image strip in the game
        Rectangle destinationRect = new Rectangle();

        //Width of a given frame
        public int FrameWidth;

        //Height of a given Frame
        public int FrameHeight;

        // The state of the Animation
        public bool Active;

        //Determine if the animation will keep playing or deactivate after one run
        public bool Looping;

        //Width of a given frame
        public Vector2 Position;
        
        public void  Initialize(Texture2D texture, Vector2 position, int frameWidth, int frameHeight, int frameCount, int frameTime, Color color, float scale, bool looping)
        {
            // keep a local copy of the values passed to 
            this.color = color;
            this.FrameWidth = frameWidth;
            this.FrameHeight = frameHeight;
            this.frameCount = frameCount;
            this.frameTime = frameTime;
            this.scale = scale;
            Looping = looping;
            Position = position;
            spriteStrip = texture;

            // Set the time to zero
            elapsedTime = 0;
            currentFrame = 0;

            // Set the Animation to active by default
            Active = true;

        }

        public void Update(GameTime gameTime)
        {
            // Do not update the game if we are not active
            if (Active == false)
                return;

            // Update the elapsed time 
            elapsedTime += (int)gameTime.ElapsedGameTime.TotalMilliseconds;

            // If the elapsed time is larger than the frame time
            // we need to switch frames
            if(elapsedTime > frameTime)
            {
                // Move to the next frame
                currentFrame++;

                // If the current frame is equal to frameCount reset currentFrame to zero
                if(currentFrame == frameCount)
                {
                    currentFrame = 0;
                    //If we are not looping deactivate the animation
                    if (Looping == false)
                         Active = false;
                }

                // Reset the elapsed time to zero
                elapsedTime = 0;
                               
            }
            // Grab the correct frame in the image strip by multiplying the currentFrame index by the frame width

            sourceRect = new Rectangle(currentFrame * FrameWidth, 0, FrameWidth, FrameHeight);
            destinationRect = new Rectangle((int)Position.X - (int)(FrameWidth * scale) / 2,
                (int)Position.Y - (int)(FrameHeight * scale) / 2,
                (int)(FrameWidth * scale),
                (int)(FrameHeight * scale));

        }
        public void Draw(SpriteBatch spriteBatch)
        {
            // only draw the animation when we are active
            if(Active)
            {
                spriteBatch.Draw(spriteStrip, destinationRect, sourceRect, color);
            }
        }
    }
}
