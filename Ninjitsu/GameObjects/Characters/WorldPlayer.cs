using Microsoft.Xna.Framework;
using G.Input;
using Microsoft.Xna.Framework.Input;
using G.Globals;
using Microsoft.Xna.Framework.Graphics;
using System;
using G.GameObjects.Characters.Sprites;
using TiledLib;
using Microsoft.Xna.Framework.Content;
using G.GameObjects.Streams;

namespace G.GameObjects.Characters
{
    public class WorldPlayer : AnimatedSprite
    {
        #region Fields
        private Texture2D whitePixel;

        #endregion

        #region Properties

        public int MaxFramesPerDirection { get; set; }
        public Vector2 TilePosition { get; set; }
        public Enums.Direction Direction { get; set; }
        public float Speed { get; set; }
        public Vector2 LastMovement { get; set; }
        public Map Map { get; set; }

        #endregion

        #region Constructor

        public WorldPlayer()
        {
        }
        //Constructor for setting up a player object based on a custom sprite sheet
        public WorldPlayer(Vector2 startLocation, Vector2 frameSize)
        {
            Position = startLocation;
            FrameSize = frameSize;
            Origin = frameSize / 2;
            
            MaxFramesPerDirection = 1;
            Direction = Enums.Direction.South;
            Speed = 5f;
        }

        #endregion

        #region Methods

        public override void LoadContent(ContentManager content, string assetName)
        {
            AssetName = assetName;
            Texture = content.Load<Texture2D>(AssetName);

        }

        public void Update(GameTime gameTime, WorldMapCamera camera)
        {
            if (!IsAnimating) return;

            if (totalFrames == -1) totalFrames = 1 * 1;

            timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            
        }

        public void Collision()
        {
            Position -= LastMovement;
        }

        public void UpdateBounds(int tileWidth, int tileHeight)
        {
            Bounds = new Rectangle((int)(Position.X - (tileWidth / 2)), (int)(Position.Y), tileWidth - (tileWidth / 16), tileHeight - (tileHeight / 16)); ;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (whitePixel == null)
            {
                whitePixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                Color[] pixels = new Color[1 * 1];
                pixels[0] = Color.White;
                whitePixel.SetData<Color>(pixels);
            }
            if (IsVisible && Texture != null)
            {
                Color tint = new Color(Tint.R, Tint.G, Tint.B, Convert.ToByte(Opacity * Tint.A));
                Rectangle destRect = new Rectangle((int)Position.X, (int)Position.Y, (int)FrameSize.X, (int)FrameSize.Y);
                spriteBatch.Draw(Texture, destRect, SourceRectangle, tint, MathHelper.ToRadians(RotationAngle), Origin, Orientation, 1.0f - (Position.Y / (Map.Height * Map.TileHeight)));

            }
        }

        public void HandleInput(MenuInput input, PlayerIndex? ControllingPlayer)
        {
            Vector2 playerMovement = Vector2.Zero;
            int playerIndex = (int)ControllingPlayer;
            //Xbox controls
            if (input.GamePadWasConnected[playerIndex])
            {
                playerMovement.X = input.CurrentGamePadStates[playerIndex].ThumbSticks.Left.X;
                playerMovement.Y = -input.CurrentGamePadStates[playerIndex].ThumbSticks.Left.Y;
            }
            //Keyboard Controls WASD or Arrow Keys  
            //Also Prevents Diagonal Movement
            else
            {
                if ((input.CurrentKeyboardStates[playerIndex].GetPressedKeys().Length == 1)&&((input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.A)))|| input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.Left))
                {
                    playerMovement.X = -1;
                }
                else if ((input.CurrentKeyboardStates[playerIndex].GetPressedKeys().Length == 1) && ((input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.D))) || input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.Right))
                {
                    playerMovement.X = 1;
                }
                if ((input.CurrentKeyboardStates[playerIndex].GetPressedKeys().Length == 1) && ((input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.W))) || input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.Up))
                {
                    playerMovement.Y = -1;
                }
                else if ((input.CurrentKeyboardStates[playerIndex].GetPressedKeys().Length == 1) && ((input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.S)))|| input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.Down))
                {
                    playerMovement.Y = 1;
                }

               
            }
            //Control Animation 
			if (playerMovement == Vector2.Zero)
				IsAnimating = false;
			
            
            //Adjust Player direction
            if (playerMovement.X > 0 && playerMovement.Y == 0) Direction = Enums.Direction.East;
            if (playerMovement.X < 0 && playerMovement.Y == 0) Direction = Enums.Direction.West;
            if (playerMovement.X == 0 && playerMovement.Y > 0) Direction = Enums.Direction.South;
            if (playerMovement.X == 0 && playerMovement.Y < 0) Direction = Enums.Direction.North;
			
            Position += Speed * playerMovement;

			LastMovement = Speed * playerMovement;
        }

        #endregion
    }
}
