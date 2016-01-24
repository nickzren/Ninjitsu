using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace G.GameObjects.Characters.Sprites
{
    public class Sprite
    {
        #region Fields

        #endregion

        #region Properties

        public virtual string AssetName { get; set; }
        public virtual Texture2D Texture { get; set; }
        public virtual Vector2 Position { get; set; }
        public virtual Rectangle? SourceRectangle { get; set; }
        public virtual Color Tint { get; set; }
        public virtual float RotationAngle { get; set; }
        public virtual Vector2 Origin { get; set; }
        public virtual Vector2 Scale { get; set; }
        public virtual SpriteEffects Orientation { get; set; }
        public virtual bool IsVisible { get; set; }
        public virtual float Opacity { get; set; }
        public virtual Rectangle Bounds { get; set; }
        public virtual float LayerDepth { get; set; }


        #endregion

        #region Constructor

        public Sprite()
		{
			AssetName = "";
			Texture = null;
			Position = Vector2.Zero;
			SourceRectangle = null;
			Tint = Color.White;
			RotationAngle = 0f;
			Origin = Vector2.Zero;
			Scale = Vector2.One;
			Orientation = SpriteEffects.None;
			IsVisible = true;
			Opacity = 1f;			
			LayerDepth = 0f;
		}

        #endregion

        #region Methods

        public virtual void LoadContent(ContentManager content, string assetName)
        {
            AssetName = assetName;
            Texture = content.Load<Texture2D>(AssetName);            
        }


        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (IsVisible && Texture != null)
            {
                Color tint = new Color(Tint.R, Tint.G, Tint.B, Convert.ToByte(Opacity * Tint.A));
                spriteBatch.Draw(Texture, Position, SourceRectangle, tint, MathHelper.ToRadians(RotationAngle), Origin, Scale, Orientation, LayerDepth);                
            }
        }     

        #endregion
    }
}
