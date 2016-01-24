using Microsoft.Xna.Framework;

namespace G.GameObjects.WorldMaps
{
	public class Portal
	{
		public Vector2 Position { get; set; }
		public Vector2 Size { get; set; }
        public string LevelName { get; set; }
		public string DestinationMap { get; set; }
		public Vector2 DestinationTileLocation { get; set; }

		public Rectangle Bounds
		{
			get
			{
				return new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y);
			}
		}
	}
}
