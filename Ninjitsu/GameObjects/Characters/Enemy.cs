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
using G.GameObjects.Collisions;
#endregion

namespace G.GameObjects.Characters
{
    /// <summary>
    /// Basic structure for all enemies
    /// </summary>
    public class Enemy : Character
    {
        #region Fields

        public Animation finalCombo1Animation;
        public Animation finalCombo2Animation;

        MovePlayer movePlayer;

        //The recover time is the time between the past hit time and next move time
        public readonly TimeSpan RecoverTime = TimeSpan.FromMilliseconds(300);

        //Rectangle
        public Rectangle extendedBounds;
        public Rectangle minimizedBounds;

        #endregion

        #region Initialization
        /// <summary>
        /// Constructs a new 
        /// </summary>
        public Enemy(Vector2 position, string spriteSet, int health, int attack, int exp)
        {
            this.Position = position;
            this.Alive = true;
            this.SpecialAnimationDone = true;
            this.AttackOnce = true;
            this.Health = health;
            this.AttackStrength = attack;
            this.movement = 65;
            this.Experience = exp;
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
            walkAnimation = new Animation(content.Load<Texture2D>(spriteSet + "Walk"), 0.1f, true);
            dieAnimation = new Animation(content.Load<Texture2D>(spriteSet + "Die"), 0.15f, false);
            finalCombo1Animation = new Animation(content.Load<Texture2D>(spriteSet + "Combo1"), 0.1f, false);
            finalCombo2Animation = new Animation(content.Load<Texture2D>(spriteSet + "Combo2"), 0.1f, false);
            reactionAnimation = new Animation(content.Load<Texture2D>(spriteSet + "Reaction"), 0.1f, false);
            fireReactionAnimation = new Animation(content.Load<Texture2D>(spriteSet + "FireReaction"), 0.1f, false);
            fallAnimation = new Animation(content.Load<Texture2D>(spriteSet + "Fall"), 0.13f, false);
            fireFallAnimation = new Animation(content.Load<Texture2D>(spriteSet + "FireFall"), 0.2f, false);
            idleAnimation = new Animation(content.Load<Texture2D>(spriteSet + "idle"), 0.1f, false);

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
            FrameWidth = sprite.animation.FrameWidth;

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
            if ((BackwardViewRectangle.Intersects(Statics.LevelPlayer.BoundingRectangle) &&
                SpecialAnimationDone) ||
                (Statics.Level.TileEngine.GetCollision(tileX + (int)direction, tileY - 1) == TileCollision.Impassable ||
                 Statics.Level.TileEngine.GetCollision(tileX + (int)direction, tileY) == TileCollision.Passable))
            {
                direction = (Enums.FaceDirection)(-(int)direction);
            }

            if (SpecialAnimationDone)
            {
                if (MoveState == Enums.MoveState.Reaction ||
                    MoveState == Enums.MoveState.FireReaction ||
                    MoveState == Enums.MoveState.Fall ||
                    MoveState == Enums.MoveState.FireFall)
                {
                    MoveState = Enums.MoveState.Idle;
                }
                else
                {
                    if (movement == 0 && gameTime.TotalRealTime - Detection.HitTime > RecoverTime)
                    {
                        if (hitCount == 2)
                        {
                            MoveState = Enums.MoveState.FinalCombo2;
                            hitCount = 0;
                        }
                        else
                        {
                            sprite.animation = idleAnimation;
                            MoveState = Enums.MoveState.FinalCombo1;
                        }

                        AttackOnce = false;
                    }

                    SpecialAnimationDone = false;
                }
            }

            movePlayer.UpdateEnemyMove(gameTime, this);
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
                    Statics.LevelPlayer.Experience += Experience;
                    sprite.PlayAnimation(dieAnimation);
                    killedSound.Play();
                    Alive = false;
                }
            }

            // Draw facing the way the enemy is moving.
            flip = direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            sprite.Draw(gameTime, spriteBatch, Position, flip);
        }

        #endregion
    }
}
