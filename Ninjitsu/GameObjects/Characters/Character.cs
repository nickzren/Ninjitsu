#region Using Statements
using Microsoft.Xna.Framework;
using System;
using G.GameObjects.Characters.Sprites;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using G.Globals;
using Microsoft.Xna.Framework.Audio;
#endregion

namespace G.GameObjects.Characters
{
    public abstract class Character
    {
        #region Fields

        public ContentManager content;

        /// <summary>
        /// Basic Animations
        /// </summary>
        public Animation walkAnimation;
        public Animation runAnimation;
        public Animation idleAnimation;
        public Animation jumpAnimation;
        public Animation dieAnimation;
        public Animation punchAnimation;
        public Animation kickAnimation;
        public Animation reactionAnimation;
        public Animation fireReactionAnimation;
        public Animation fallAnimation;
        public Animation fireFallAnimation;

        public AnimationPlayer sprite;
        
        //Sounds
        public SoundEffect killedSound;
        public SoundEffect jumpSound;
        public SoundEffect hurtSound;

        public Vector2 Velocity;

        public float previousBottom;

        public Rectangle localBounds;

        public SpriteEffects flip = SpriteEffects.None;

        /// <summary>
        /// The direction is facing and moving along the X axis.
        /// </summary>
        public Enums.FaceDirection direction;

        /// <summary>
        /// Current character movement.
        /// </summary>
        public float movement;

        // Jumping state
        public bool isJumping;
        public bool wasJumping;
        public float jumpTime;

        public int punchHurtCount = 0;
        public int kickHurtCount = 0;
        public int hitCount = 0;
        public int hurtCount = 0;

        #endregion

        #region Properties

        /// <summary>
        /// Per Pixel Color Data.
        /// </summary>
        public Color[] ColorData
        {
            get { return colorData; }
            set { colorData = value; }
        }
        Color[] colorData;

        /// <summary>
        /// The character's move state.
        /// </summary>
        public Enums.MoveState MoveState
        {
            get { return moveState; }
            set { moveState = value; }
        }
        Enums.MoveState moveState;

        /// <summary>
        /// Position in world space of the bottom center of this enemy.
        /// </summary>
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }
        Vector2 position;

        public bool Alive
        {
            get { return alive; }
            set { alive = value; }
        }
        bool alive;

        /// <summary>
        /// Character's health, if 0 died
        /// </summary>
        public int Health
        {
            get { return health; }
            set { health = value; }
        }
        int health;

        public int Experience { get; set; }

        /// <summary>
        /// Character's attack
        /// </summary>
        public int AttackStrength;

        /// <summary>
        /// Gets whether or not the character's feet are on the ground.
        /// </summary>
        public bool IsOnGround
        {
            get { return isOnGround; }
            set { isOnGround = value; }
        }
        bool isOnGround;

        /// <summary>
        /// It could represent all the animations except walk and idle
        /// Its job is to detecte whether the animation has finished.
        /// </summary>
        public bool SpecialAnimationDone
        {
            get { return specialAnimationDone; }
            set { specialAnimationDone = value; }
        }
        bool specialAnimationDone;

        /// <summary>
        /// Its job is to reduce the enemy' health only once
        /// </summary>
        public bool AttackOnce
        {
            get { return attackOnce; }
            set { attackOnce = value; }
        }
        bool attackOnce;

        /// <summary>
        /// One frame rectangle in sprite sheet
        /// </summary>
        public Rectangle oneFrame;

        public int FrameWidth
        {
            get { return frameWidth; }
            set { frameWidth = value; }
        }
        int frameWidth;

        public int FrameHeight
        {
            get { return frameHeight; }
            set { frameHeight = value; }
        }
        int frameHeight;

        /// <summary>
        /// Gets a rectangle which bounds this character in world space.
        /// </summary>
        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        //Used to give vision to characters
        public Rectangle BackwardViewRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - sprite.Origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - sprite.Origin.Y) + localBounds.Y;

                if (direction == Enums.FaceDirection.Left)
                    return new Rectangle(left + localBounds.Width, top, 100, 1);
                else
                    return new Rectangle(left - 100, top, 100, 1);
            }
        }
        #endregion

        #region Methods

        #endregion
    }
}
