using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using G.GameObjects.Characters;
using Microsoft.Xna.Framework.Content;
using G.Globals;

namespace G.GameObjects.Levels.Objects
{
    /// <summary>
    /// A valuable item the player can collect.
    /// </summary>
    public class HealthPotion
    {
        public ContentManager content;

        private Texture2D texture;
        private Vector2 origin;
        private SoundEffect collectedSound;

        public const int PointValue = 30;
        public readonly Color Color = Color.White;

        // The healthPotion is animated from a base position along the Y axis.
        private Vector2 basePosition;
        private float bounce;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        /// <summary>
        /// Gets the current position of this healthPotion in world space.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return basePosition + new Vector2(0.0f, bounce);
            }
        }

        /// <summary>
        /// Gets a circle which bounds this healthPotion in world space.
        /// </summary>
        public Circle BoundingCircle
        {
            get
            {
                return new Circle(Position, LevelTile.Width / 3.0f);
            }
        }

        /// <summary>
        /// Constructs a new healthPotion.
        /// </summary>
        public HealthPotion(Level level, Vector2 position)
        {
            this.level = level;
            this.basePosition = position;

            LoadContent();
        }

        /// <summary>
        /// Loads the healthPotion texture and collected sound.
        /// </summary>
        public void LoadContent()
        {
            if (content == null)
                content = new ContentManager(Statics.Game.Services, "Content");

            texture = content.Load<Texture2D>("Sprites/HealthPotion");
            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            collectedSound = content.Load<SoundEffect>("Sounds/HealthPotionCollected");
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
            // Include the X coordinate so that neighboring healthPotions bounce in a nice wave pattern.            
            double t = gameTime.TotalGameTime.TotalSeconds * BounceRate + Position.X * BounceSync;
            bounce = (float)Math.Sin(t) * BounceHeight * texture.Height;
        }

        public void OnCollected(LevelPlayer collectedBy)
        {
            collectedSound.Play();
        }

        /// <summary>
        /// Draws a healthPotion in the appropriate color.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, Position, null, Color, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
        }
    }
}
