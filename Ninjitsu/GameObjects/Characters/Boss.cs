#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using G.Globals;
using G.GameObjects.Characters.Sprites;
using G.GameObjects.Levels.Objects;
using G.GameObjects.Characters.Moves;
using G.GameObjects.Streams;
using G.GameObjects.Collisions;
using G.UI.Screens;
using Microsoft.Xna.Framework.Media;
#endregion

namespace G.GameObjects.Characters
{
    /// <summary>
    /// Basic structure for all enemies
    /// </summary>
    public class Boss : Character
    {
        #region Fields

        MovePlayer movePlayer;

        //The recover time is the time between the past hit time and next move time
        public readonly TimeSpan RecoverTime = TimeSpan.FromMilliseconds(300);

        public Animation lightingAnimation;

        //Rectangle
        public Rectangle extendedBounds;
        public Rectangle minimizedBounds;

        #endregion

        #region Properties

        //Used to give vision to bosses
        public Rectangle BacwardViewRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                if (direction == Enums.FaceDirection.Left)
                    return new Rectangle(left + localBounds.Width, top, 1000, 200);
                else
                    return new Rectangle(left - 1000, top, 1000, 200);
            }
        }

        #endregion

        #region Initialization
        /// <summary>
        /// Constructs a new 
        /// </summary>
        public Boss(Vector2 position, string spriteSet, int health, int attack)
        {
            this.Position = position;
            this.Alive = true;
            this.SpecialAnimationDone = true;
            this.AttackOnce = true;
            this.Health = health;
            this.AttackStrength = attack;
            this.movement = 200;
            direction = Enums.FaceDirection.Left;
            movePlayer = new MovePlayer();
            LoadContent(spriteSet);
        }

        /// <summary>
        /// Loads a particular enemy sprite sheet and sounds.
        /// </summary>
        public void LoadContent(string spriteSet)
        {
            #region Load animated textures and sounds.

            if (content == null)
                content = new ContentManager(Statics.Game.Services, "Content");

            // Load animations.
            spriteSet = "Sprites/" + spriteSet + "/";
            idleAnimation = new Animation(content.Load<Texture2D>(spriteSet + "Idle"), 0.1f, true);
            fallAnimation = new Animation(content.Load<Texture2D>(spriteSet + "Idle"), 0.02f, false);
            punchAnimation = new Animation(content.Load<Texture2D>(spriteSet + "Punch"), 0.075f, false);
            lightingAnimation = new Animation(content.Load<Texture2D>(spriteSet + "Lighting"), 0.1f, false);
    
            dieAnimation = new Animation(content.Load<Texture2D>(spriteSet + "Die"), 0.15f, false);

            killedSound = content.Load<SoundEffect>("Sounds/PlayerKilled");
            hurtSound = content.Load<SoundEffect>("Sounds/EnemyHurt");

            sprite.PlayAnimation(idleAnimation);

            #endregion

            // Calculate bounds within texture size.
            int width = (int)(idleAnimation.FrameWidth * 0.35);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.7);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);
            oneFrame = new Rectangle(left, top, FrameWidth, FrameHeight);
            extendedBounds = new Rectangle(BoundingRectangle.Left, BoundingRectangle.Top, BoundingRectangle.Width + 25, BoundingRectangle.Height);
            minimizedBounds = new Rectangle(left, top, width, height);

        }
        #endregion

        #region Methods

        public void UnLoadContent()
        {
            content.Unload();
        }

        /// <summary>
        /// Paces back and forth along a platform, waiting at either end.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            #region Update following variables for per pixel collision

            FrameHeight = sprite.animation.texture.Height;
            FrameWidth = sprite.animation.texture.Height;

            ColorData = new Color[FrameWidth * FrameHeight];
            Rectangle source = new Rectangle(sprite.FrameIndex * FrameWidth, 0, FrameWidth, FrameHeight);
            sprite.animation.texture.GetData(0, source, ColorData, 0, FrameWidth * FrameHeight);

            oneFrame = new Rectangle((int)(Position.X - sprite.Origin.X), (int)(Position.Y - sprite.Origin.Y), FrameWidth, FrameHeight);

            #endregion

            // Calculate tile position based on the side we are walking towards.
            float posX = Position.X + localBounds.Width / 2 * (int)direction;
            int tileX = (int)Math.Floor(posX / LevelTile.Width) - (int)direction;
            int tileY = (int)Math.Floor(Position.Y / LevelTile.Height);

            // If we are about to run into a wall or off a cliff, start waiting or the player is in back side.
            if ((BacwardViewRectangle.Intersects(Statics.LevelPlayer.BoundingRectangle) &&
                SpecialAnimationDone) ||
                (Statics.Level.TileEngine.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.Impassable ||
                 Statics.Level.TileEngine.GetCollision(tileX + (int)direction, tileY) == TileCollision.Passable))
            {
                direction = (Enums.FaceDirection)(-(int)direction);
            }

            if (SpecialAnimationDone)
            {
                if (MoveState == Enums.MoveState.Reaction ||
                    MoveState == Enums.MoveState.Fall)
                {
                    MoveState = Enums.MoveState.Idle;
                }
                else
                {
                    if (movement == 0 && gameTime.TotalRealTime - Detection.HitTime > RecoverTime)
                    {
                        if (hitCount == 3 || hurtCount >=5)
                        {
                            MoveState = Enums.MoveState.FinalCombo2;
                            if(hitCount == 3)
                                hitCount = 0;
                            if (hurtCount >= 5)
                                hurtCount = 0;
                        }
                        else
                        {
                            sprite.animation = idleAnimation;
                            MoveState = Enums.MoveState.Punch;
                        }

                        SpecialAnimationDone = false;
                        AttackOnce = false;
                    }
                }
            }

            movePlayer.UpdateBossMove(gameTime, this);
        }

        /// <summary>
        /// Draws the animated 
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (Alive)
            {
                if (Health <= 0)
                {
                    sprite.PlayAnimation(dieAnimation);
                    killedSound.Play();
                    Alive = false;
                    Statics.Gameover = true;
                    MediaPlayer.Stop();
                }
            }

            // Draw facing the way the enemy is moving.
            flip = direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            sprite.Draw(gameTime, spriteBatch, Position, flip);
        }

        #endregion
    }
}
