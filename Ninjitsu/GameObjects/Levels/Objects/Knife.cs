﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using G.GameObjects.Characters;
using Microsoft.Xna.Framework.Content;
using G.Globals;
using G.UI.Lib;

namespace G.GameObjects.Levels.Objects
{
    /// <summary>
    /// A valuable item the player can collect.
    /// </summary>
    class Spider
    {
        private Texture2D texture;
        private Vector2 origin;
        private SoundEffect hitSound;
        private SoundEffect killMonsterSound;

        public ContentManager content;

        public readonly int AttackStrength = 10;

        /*public const int PointValue = 30;
        public readonly Color Color = Color.Yellow;*/

        //public readonly int PointValue;
        //public bool IsPowerUp { get; private set; }
        public readonly Color Color;


        // The gem is animated from a base position along the Y axis.
        private Vector2 basePosition;
        private float bounce;

        //Attack Once
        public bool attackComplete = false;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Gets the current position of this gem in world space.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return basePosition + new Vector2(0.0f, bounce);
            }
            set
            {
                basePosition = value;
            }
        }

        private const float GravityAcceleration = 2000.0f;
        private const float MaxFallSpeed = 6000.0f;
        public bool isFalling;

        public Vector2 Velocity
        {
            get { return velocity; }
            set { velocity = value; }
        }
        Vector2 velocity;

        public Rectangle TriggerRectangle
        {
            get
            {
                int left = (int)basePosition.X;
                int width = 32;
                int top = (int)basePosition.Y;
                int height = 200;
                return new Rectangle(left, top, width, height);
            }
        }

        /// <summary>
        /// Gets a circle which bounds this gem in world space.
        /// </summary>
        public Circle BoundingCircle
        {
            get
            {
                return new Circle(Position, LevelTile.Width / 3.0f);
            }
        }

        
        public Spider(Level level, Vector2 position)
        {
            this.level = level;
            this.basePosition = position;

            LoadContent();

            Color = Color.White;

        }


        /// <summary>
        /// Loads the gem texture and collected sound.
        /// </summary>
        public void LoadContent()
        {
            if (content == null)
                content = new ContentManager(Statics.Game.Services, "Content");

            texture = content.Load<Texture2D>("Tiles/anvil");
            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            hitSound = content.Load<SoundEffect>("Sounds/HealthPotionCollected");
            killMonsterSound = content.Load<SoundEffect>("Sounds/PlayerKilled");
        }

        /// <summary>
        /// Bounces up and down in the air to entice players to collect them.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Bounce control constants
            const float BounceHeight = 0.18f;
            const float BounceRate = 3.0f;
            const float BounceSync = -0.75f;

            // Bounce along a sine curve over time.
            // Include the X coordinate so that neighboring gems bounce in a nice wave pattern.            
            double t = gameTime.TotalGameTime.TotalSeconds * BounceRate + Position.X * BounceSync;
            bounce = (float)Math.Sin(t) * BounceHeight * texture.Height * 0; //Set bounce
            ApplyPhysics(gameTime);

        }

        /// <summary>
        /// Called when this gem has been collected by a player and removed from the level.
        /// </summary>
        /// <param name="collectedBy">
        /// The player who collected this gem. Although currently not used, this parameter would be
        /// useful for creating special powerup gems. For example, a gem could make the player invincible.
        /// </param>
        public void OnCollected(LevelPlayer collectedBy)
        {
            hitSound.Play();
        }
        public void OnCollected(Enemy collectedBy)
        {

          //  killMonsterSound.Play(screenManager.Settings.SoundVolumeAmount);
            killMonsterSound.Play();

        }
        private void ApplyPhysics(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (isFalling)
            {
                velocity.Y = MathHelper.Clamp(velocity.Y + GravityAcceleration * elapsed, -MaxFallSpeed, MaxFallSpeed);
                Position += velocity * elapsed;
            }
        }

        public void OnKnifeFalling()
        {
            isFalling = true;
        }


        /// <summary>
        /// Draws a gem in the appropriate color.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
