using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using G.GameObjects.Levels.Objects;
using TiledLib;
using G.GameObjects.Levels;
using Microsoft.Xna.Framework.Content;
using G.Globals;

namespace G.GameObjects.Levels.Objects
{
    public class MovableTile
    {
        private Texture2D texture;
        private Vector2 origin;
        public ContentManager content;
        String File;

        public Level Level
        {
            get { return level; }
        }
        Level level;

        public Vector2 Position
        {
            get { return position; }
        }
        Vector2 position;

        public Boolean IsSmashBlock
        {
            get { return IsSmashBlock; }
            set { isSmashBlock = value; }
        }
        Boolean isSmashBlock;

        public Vector2 Velocity
        {
            get { return velocity; }
        }
        Vector2 velocity;

        /// <summary>  
        /// Gets whether or not the player's feet are on the MovableTile.  
        /// </summary>  
        public bool PlayerIsOn { get; set; }

        public Rectangle BoundingRectangle
        {
            get
            {
                int left = (int)Math.Round(Position.X - origin.X) + localBounds.X;
                int top = (int)Math.Round(Position.Y - origin.Y) + localBounds.Y;

                return new Rectangle(left, top, localBounds.Width, localBounds.Height);
            }
        }

        public G.Globals.Enums.FaceDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        G.Globals.Enums.FaceDirection direction = G.Globals.Enums.FaceDirection.Left;

        public TileCollision Collision
        {
            get { return collision; }
            set { collision = value; }
        }
        public TileCollision collision;

        private Rectangle localBounds;
        private float waitTime;
        private const float MaxWaitTime = 0.1f;
        private const float MoveSpeed = 120.0f;

        public MovableTile(Level level, Vector2 position, TileCollision collision , Boolean SmashBlock, String file)
        {
            this.level = level;
            this.position = position;
            this.collision = collision;
            this.isSmashBlock = SmashBlock;
            this.File = file;
            LoadContent();
        }

        public void LoadContent()
        {
            if (content == null)
            {content = new ContentManager(Statics.Game.Services, "Content");}
            texture = content.Load<Texture2D>(File);
            origin = new Vector2(texture.Width / 2.0f, texture.Height / 2.0f);
            // Calculate bounds within texture size.  
            localBounds = new Rectangle(0, 0, texture.Width, texture.Height);
        }

        public void Update(GameTime gameTime)
        {
            float elapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float posX = 0;
            float posY = 0;
            int tileX = 0;
            int tileY = 0;  
                
            
            // Calculate tile position based on the side we are moving towards.  
            posX = Position.X + localBounds.Width / 2 * (int)direction;
            tileX = (int)Math.Floor(posX / 64) - (int)direction;
            tileY = (int)Math.Floor(Position.Y / 48);


            if (isSmashBlock)
            {
                // Calculate tile position based on the side we are moving towards.  
                posY = Position.Y + localBounds.Height / 2 * (int)direction;
                tileX = (int)Math.Floor(Position.X / 64);
                tileY = (int)Math.Floor(posY / 48) - (int)direction;
            }

            if (waitTime > 0)
            {
                // Wait for some amount of time.  
                waitTime = Math.Max(0.0f, waitTime - (float)gameTime.ElapsedGameTime.TotalSeconds);
                if (waitTime <= 0.0f)
                {
                    // Then turn around.  
                    direction = (G.Globals.Enums.FaceDirection)(-(int)direction);
                }
            }
            else
            {
                if (isSmashBlock)
                {
                    if (Statics.Level.TileEngine.GetCollision(tileX, tileY + (int)direction) == TileCollision.Impassable ||
                    Statics.Level.TileEngine.GetCollision(tileX, tileY + (int)direction) == TileCollision.Platform)
                    {
                        velocity = new Vector2(0.0f, 0.0f);
                        waitTime = MaxWaitTime;
                    }
                }
                //If we're about to run into a wall that isn't a MovableTile move in other direction.  
                if (!isSmashBlock && Statics.Level.TileEngine.GetCollision(tileX + (int)direction, tileY) == TileCollision.Impassable ||
                    Statics.Level.TileEngine.GetCollision(tileX + (int)direction, tileY) == TileCollision.Platform)
                {
                    velocity = new Vector2(0.0f, 0.0f);
                    waitTime = MaxWaitTime;
                }
                else
                {
                    if (isSmashBlock)
                    {
                        // Move in the current direction.  
                        velocity = new Vector2(0.0f, (int)direction * (MoveSpeed/2) * elapsed);
                        //velocity = new Vector2((int)direction * MoveSpeed * elapsed, 0.0f);
                    }
                    else
                    {
                        velocity = new Vector2((int)direction * MoveSpeed * elapsed, 0.0f);
                    }
                    
                    position = position + velocity;
                }
            }

            if (Statics.Level.TileEngine.movableTiles.Count > 0)
            {
                //If we're about to run into a MovableTile move in other direction.  
                foreach (var movableTile in Statics.Level.TileEngine.movableTiles)
                {
                    if (BoundingRectangle != movableTile.BoundingRectangle)
                    {
                        if (BoundingRectangle.Intersects(movableTile.BoundingRectangle))
                        {
                            direction = (G.Globals.Enums.FaceDirection)(-(int)direction);
                            velocity = new Vector2((int)direction * MoveSpeed * elapsed, 0.0f);
                        }
                    }
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture,
                Position,
                null,
                Color.White,
                0.0f,
                origin,
                1.0f,
                SpriteEffects.None,
                0.0f);
        }
    }
}