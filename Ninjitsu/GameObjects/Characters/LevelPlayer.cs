#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using G.Globals;
using G.GameObjects.Characters.Sprites;
using G.GameObjects.Characters.Moves;
using G.Input;
using System.Collections.Generic;
using G.UI.Lib;
#endregion

namespace G.GameObjects.Characters
{
    public class LevelPlayer : Character
    {
        #region Fields

        #region Animations and Sounds Fields
        // Animations
        public Animation celebrateAnimation;
        public Animation comboKickAnimation;
        public Animation firePunch1Animation;
        public Animation firePunch2Animation;
        public Animation firePunch3Animation;
        public Animation punchUpAnimation;
        public Animation upgradeAnimation;
        public Animation specialAnimation;

        public Animation finalCombo2Animation;

        // Sounds
        public SoundEffect HitSound;
        public SoundEffect firePunchSound;
        public SoundEffect finalComboSound;

        SpriteFont font;

        #endregion

        MovePlayer movePlayer;

        // Stores player's most recent move and when they pressed it.
        public List<Move> chainMoves = new List<Move>();

        TimeSpan MoveTime;
        
        public readonly TimeSpan ChainComboTimer = TimeSpan.FromSeconds(1);

        // This is the master list of moves in logical order. 
        Move[] moves;

        // The move list used for move detection at runtime
        MoveList moveList;

        // The move list is used to match against an input manager for each player.
        public GamePlayInput inputManager;

        public int firePunchCount = 0;

        #endregion

        #region Properties

        //Current Level
        public int Lv { get; set; }

        // count killed enemy
        public int KilledEnemies { get; set; }

        //to count the combos that player  
        public int ComboCount { get; set; }
        // to count player actions 
        public int CountActions { get; set; }

        public int Lives { get; set; }

        // those values will be updated each level
        public int maxExper = 10;
        public int fullHealth = 100;

        #endregion

        #region Initialization
        /// <summary>
        /// Constructors a new player.
        /// </summary>
        public LevelPlayer(Vector2 position)
        {
            movePlayer = new MovePlayer();

            #region Moves List
            // Construct the master list of moves.
            moves = new Move[]
            {
                new Move(Enums.MoveState.Punch, Buttons.X),
                new Move(Enums.MoveState.Kick, Buttons.A),
                new Move(Enums.MoveState.SpecialCombo, Buttons.X | Buttons.A),
                new Move(Enums.MoveState.ComboKick, Direction.Down, Direction.DownRight, Direction.Right | Buttons.A),
                new Move(Enums.MoveState.ComboKick, Direction.Down, Direction.DownLeft, Direction.Left | Buttons.A),
                new Move(Enums.MoveState.FirePunch1, Direction.Down, Direction.DownRight, Direction.Right | Buttons.X),
                new Move(Enums.MoveState.FirePunch1, Direction.Down, Direction.DownLeft, Direction.Left | Buttons.X),
                new Move(Enums.MoveState.FinalCombo2,  Buttons.B),
            };
            #endregion

            // Construct a move list which will store its own copy of the moves array.
            moveList = new MoveList(moves);

            // Create an InputManager for player with a sufficiently large buffer.
            inputManager = new GamePlayInput((PlayerIndex)0, moveList.LongestMoveLength);

            Alive = true;
            Lives = 3;
            ComboCount = 3;
            Lv = 1;
            AttackStrength = 3;
            chainMoves.Capacity = 3;
            LoadContent();
            Reset(position);
        }

        /// <summary>
        /// Loads the player sprite sheet and sounds.
        /// </summary>
        public void LoadContent()
        {
            #region Load animated textures and sounds.

            if (content == null)
                content = new ContentManager(Statics.Game.Services, "Content");

            // Load animated textures.
            idleAnimation = new Animation(content.Load<Texture2D>("Sprites/Player/Idle"), 0.1f, true);  
            runAnimation = new Animation(content.Load<Texture2D>("Sprites/Player/Run"), 0.1f, true);
            jumpAnimation = new Animation(content.Load<Texture2D>("Sprites/Player/Jump"), 0.1f, true);
            dieAnimation = new Animation(content.Load<Texture2D>("Sprites/Player/Die"), 0.1f, false);
            kickAnimation = new Animation(content.Load<Texture2D>("Sprites/Player/Kick"), 0.02f, false);
            punchAnimation = new Animation(content.Load<Texture2D>("Sprites/Player/Punch"), 0.08f, false);
            comboKickAnimation = new Animation(content.Load<Texture2D>("Sprites/Player/ComboKick"), 0.055f, false);
            firePunch1Animation = new Animation(content.Load<Texture2D>("Sprites/Player/FirePunch1"), 0.06f, false);
            firePunch2Animation = new Animation(content.Load<Texture2D>("Sprites/Player/FirePunch2"), 0.06f, false);
            firePunch3Animation = new Animation(content.Load<Texture2D>("Sprites/Player/FirePunch3"), 0.06f, false);
            punchUpAnimation = new Animation(content.Load<Texture2D>("Sprites/Player/PunchUp"), 0.08f, false);
            finalCombo2Animation = new Animation(content.Load<Texture2D>("Sprites/Player/FinalCombo"), 0.08f, false);
            reactionAnimation = new Animation(content.Load<Texture2D>("Sprites/Player/Reaction"), 0.04f, false);
            fallAnimation = new Animation(content.Load<Texture2D>("Sprites/Player/Fall"), 0.08f, false);
            upgradeAnimation = new Animation(content.Load<Texture2D>("Sprites/Player/Upgrade"), 0.2f, false);
            specialAnimation = new Animation(content.Load<Texture2D>("Sprites/Player/SpecialCombo"), 0.05f, false);

            // Load sounds.            
            killedSound = content.Load<SoundEffect>("Sounds/PlayerKilled");
            jumpSound = content.Load<SoundEffect>("Sounds/PlayerJump");
            HitSound = content.Load<SoundEffect>("Sounds/Hit");
            firePunchSound = content.Load<SoundEffect>("Sounds/FirePunch");
            finalComboSound = content.Load<SoundEffect>("Sounds/finalComboSound");

            font = content.Load<SpriteFont>("Fonts/MenuFont");
            #endregion

            // Calculate bounds within texture size.            
            int width = (int)(idleAnimation.FrameWidth * 0.4);
            int left = (idleAnimation.FrameWidth - width) / 2;
            int height = (int)(idleAnimation.FrameWidth * 0.8);
            int top = idleAnimation.FrameHeight - height;
            localBounds = new Rectangle(left, top, width, height);
            oneFrame = new Rectangle(left, top, FrameWidth, FrameHeight);
        }

        /// <summary>
        /// Resets the player to life.
        /// </summary>
        /// <param name="position">The position to come to life at.</param>
        public void Reset(Vector2 position)
        {
            Health = fullHealth;
            SpecialAnimationDone = true;
            AttackOnce = true;
            Position = position;
            Velocity = Vector2.Zero;
            MoveState = Enums.MoveState.Idle;
            direction = Enums.FaceDirection.Left;
            sprite.PlayAnimation(idleAnimation);
        }

        #endregion

        #region Method

        public void UnLoadContent()
        {
            content.Unload();
        }

        /// <summary>
        /// Handles input, performs physics, and animates the player sprite.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            #region Update following variables for per pixel collision

            FrameHeight = sprite.animation.texture.Height;
            FrameWidth = sprite.animation.texture.Height;

            ColorData = new Color[sprite.animation.texture.Width * FrameHeight];
            Rectangle source = new Rectangle(sprite.FrameIndex * FrameWidth, 0, FrameWidth, FrameHeight);
            sprite.animation.texture.GetData(0, source, ColorData, 0, FrameWidth * FrameHeight);

            oneFrame = new Rectangle((int)(Position.X - sprite.Origin.X), (int)(Position.Y - sprite.Origin.Y), FrameWidth, FrameHeight);
            #endregion

            // for upgradeing the player 
            if(Experience >= maxExper && Lv < 10 )
            {
               Lv += 1 ;
               fullHealth += 20;
               Health = fullHealth;
               Experience = 0;
               AttackStrength += 1;
               maxExper += 8;
               MoveState = Enums.MoveState.Upgrade;
               SpecialAnimationDone = false;
            }

            //if the player make 100 MoveState , he will gain one combo move  
            if (ComboCount < 5)
            {
                if (CountActions >=100)
                {
                        ComboCount += 1;
                        CountActions = 0;
                }
            }
            else
                CountActions = 0;

            GetInput(gameTime);

            // Move update, to play new move
            movePlayer.UpdatePlayerMove(gameTime, this);
        }

        /// <summary>
        /// Gets player horizontal movement and jump commands from input.
        /// </summary>
        private void GetInput(GameTime gameTime)
        {
            // Get the updated input manager.
            inputManager.UpdatePlayer(gameTime, this);

            if (gameTime.TotalRealTime - MoveTime > ChainComboTimer)
            {
                firePunchCount = 0;
                chainMoves.Clear();
            }

            // Detection and record the current player's most recent move.
            Move newMove = moveList.DetectMove(inputManager);

            if (newMove != null)
            {
                if (newMove.State == Enums.MoveState.FirePunch1)
                    chainMoves.Add(newMove);
            }

            if (SpecialAnimationDone && 
               (newMove != null || chainMoves.Count != 0))
            {
                if (chainMoves.Count != 0)
                {
                    MoveState = chainMoves[0].State;
                    chainMoves.Remove(chainMoves[0]);
                    if (MoveState == Enums.MoveState.FirePunch1)
                    {
                        if (firePunchCount == 1)
                            MoveState = Enums.MoveState.FirePunch2;
                        else if (firePunchCount == 2)
                            MoveState = Enums.MoveState.FirePunch3;
                    }
                }
                else
                     MoveState = newMove.State;

                SpecialAnimationDone = false;
                AttackOnce = false;
                MoveTime = gameTime.TotalRealTime;
            }
        }

        /// <summary>
        /// Called when this player reaches the level's exit.
        /// </summary>
        public void Die()
        {
            sprite.PlayAnimation(dieAnimation);
        }

        /// <summary>
        /// Draws the animated player.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (MoveState == Enums.MoveState.Upgrade)
            {
                spriteBatch.DrawString(font, "Level Up", Position + new Vector2(0, -160), Color.Yellow);
            }
            // Draw facing the way the player is moving.
            flip = direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

            // Draw that sprite.
            sprite.Draw(gameTime, spriteBatch, Position, flip);
        }

        #endregion
    }
}
