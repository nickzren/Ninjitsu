using G.Input;
using Microsoft.Xna.Framework.Input;
using G.GameObjects.Characters;
using Microsoft.Xna.Framework;

namespace G.GameObjects.Streams
{
	public class WorldMapCamera
    {
        #region Fields

        #endregion

        #region Properties

        public Vector2 MapSize { get; set; }

		public Vector2 TileSize { get; set; }

		public Vector2 ViewSize { get; set; }

		public float Speed { get; set; }

        public WorldPlayer Focus { get; set; }

        public Vector2 ViewCenter { get; set; }

    	public Rectangle VisibleArea
		{
			get
			{
				return new Rectangle((int)Position.X, (int)Position.Y, (int)(ViewSize.X + Position.X), (int)(ViewSize.Y + Position.Y));
			}
		}

		public Matrix Matrix
		{
			get
			{
                return Matrix.CreateTranslation(-(int)Position.X, -(int)Position.Y, 0);
			}
		}

		public Vector2 Position { get; set; }

		#endregion

		#region Constructor

		public WorldMapCamera()
		{
			MapSize = Vector2.One;
			TileSize = Vector2.One;
			ViewSize = Vector2.One;
			Speed = 1f;
		}

		public WorldMapCamera(Vector2 mapSize, Vector2 tileSize, Vector2 viewSize)
		{
			MapSize = mapSize;
			TileSize = tileSize;
			ViewSize = viewSize;
			Speed = TileSize.X * 0.625f;
            ViewCenter = new Vector2(viewSize.X / 2, viewSize.Y / 2);
            Position = Vector2.Zero;
		}

		#endregion

		#region Methods

		public void Update(MenuInput input, PlayerIndex? controllingPlayer)
		{            
			Vector2 cameraMovement = Vector2.Zero;
			int playerIndex = (int)controllingPlayer;

            if (input.GamePadWasConnected[playerIndex])
            {
                cameraMovement.X = input.CurrentGamePadStates[playerIndex].ThumbSticks.Left.X;
                cameraMovement.Y = -input.CurrentGamePadStates[playerIndex].ThumbSticks.Left.Y;
            }
            else
            {
            /*    if (input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.Left))
                {
                    cameraMovement.X = -1;
                }
                else if (input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.Right))
                {
                    cameraMovement.X = 1;
                }
                if (input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.Up))
                {
                    cameraMovement.Y = -1;
                }
                else if (input.CurrentKeyboardStates[playerIndex].IsKeyDown(Keys.Down))
                {
                    cameraMovement.Y = 1;
                }

                if (cameraMovement != Vector2.Zero)
                {
                    cameraMovement.Normalize();
                }*/
            }            
            
			if (Focus == null)
			{
				Position += Speed * cameraMovement;
			}
			else
			{
				CenterOnFocus();
			}
            Vector2 cameraMax = new Vector2(MapSize.X * TileSize.X - ViewSize.X, MapSize.Y * TileSize.Y - ViewSize.Y);
            Position = Vector2.Clamp(Position, Vector2.Zero, cameraMax);            
		}

        public void CenterOnFocus()
        {
            Position = Focus.Position - ViewCenter;
        }

        public void FocusOn(WorldPlayer character)
        {
			Focus = character;
            CenterOnFocus();
        }

		#endregion
	}
}