#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using G.Globals;
using G.GameObjects.Characters;
using G.GameObjects.Levels.Objects;
using G.GameObjects.Streams;
using G.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
#endregion

namespace G.GameObjects.Levels
{
    /// <summary>
    /// A uniform grid of tiles with collections of enemies.
    /// The level owns the player and controls the game's win and lose
    /// conditions as well as scoring.
    /// </summary>
    public class Level
    {
        #region Fields

        ContentManager content;

        public List<Enemy> enemies = new List<Enemy>();

        private const int PointsPerSecond = 5;

        #endregion

        #region Properties

        public String Name { get; set; }
        public String FilePath { get; set; }

        public Boss Boss { get; set; }
            
        // The heads-up display in Level
        public LevelPlayer Player { get; set; }

        // The heads-up display in Level
        public TileEngine TileEngine { get; set; }

        // The heads-up display in Level
        private Hud Hud { get; set; }

        // The heads-up display in Level
        public LevelCamera Camera { get; set; }

        #endregion

        #region Initialization

        /// <summary>
        /// Constructs a new level.
        /// </summary>
        /// <param name="serviceProvider">
        /// The service provider that will be used to construct a ContentManager.
        /// </param>
        /// <param name="path">
        /// The absolute path to the level file to be loaded.
        /// </param>
        public Level(string path, string name)
        {
            this.FilePath = path;
            this.Name = name;

            TileEngine = new TileEngine(this);

            TileEngine.LoadTiles(path);

            Hud = new Hud();

            Camera = new LevelCamera();

        //    textBox = new Rectangle(10, 10, 300, 300);           
        }

        public void LoadContent()
        {
        }

        #endregion

        #region Method

        /// <summary>
        /// Updates all objects in the world, performs collision between them,
        /// and handles the time limit with scoring.
        /// </summary>
        public void Update(GameTime gameTime)
        {
            // Pause while the player is dead
            if (Player.Health <= 0)
            {
                // Still want to perform physics on the player.
                Hud.Update(gameTime);
                PlayerDied();
                
                Player.Lives--;
                if (Player.Lives <= 0)
                {
                    Player.Alive = false;
                    Statics.Gameover = true;
                    MediaPlayer.Stop();
                }
                else
                {
                    Statics.Realm = Enums.Realm.WorldMap;
                    int temp = 0;
                    if (Statics.WorldPlayer.Direction == Enums.Direction.East)
                        temp = -100;
                    else
                        temp = 100;

                    Statics.WorldPlayer.Position = new Vector2(Statics.WorldPlayer.Position.X + temp, Statics.WorldPlayer.Position.Y);
                }
            }
            else
            {
                Player.Update(gameTime);

                TileEngine.UpdateHealthPotions(gameTime, Player);

                UpdateMovableTiles(gameTime);

                Hud.Update(gameTime);

                if (Boss != null)
                {
                    Boss.Update(gameTime);
                    Statics.Collision.PlayerAndEnemy(Player, Boss, gameTime);
                }

                UpdateEnemies(gameTime);

                // The player has reached the exit if they are standing on the ground and
                // his bounding rectangle contains the center of the exit tile.
                if (Player.Health > 0 &&
                    Player.IsOnGround &&
                    Player.BoundingRectangle.Contains(TileEngine.exit) || Player.BoundingRectangle.Contains(TileEngine.exit2))
                {
                    OnExitReached();
                }
            }
        }
        /// <summary>
        /// Updates the position of movable tiles.
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateMovableTiles(GameTime gameTime)
        {
            for (int i = 0; i < TileEngine.movableTiles.Count; ++i)
            {
                MovableTile movableTile = TileEngine.movableTiles[i];
                movableTile.Update(gameTime);

                if (movableTile.PlayerIsOn)
                {
                    //Make player move with tile if the player is on top of tile  
                    Player.Position += movableTile.Velocity;
                }
            }
        }

        /// <summary>
        /// Animates each enemy and allow them to kill the player.
        /// </summary>
        private void UpdateEnemies(GameTime gameTime)
        {
            foreach (Enemy enemy in Statics.Level.enemies)
            {
                if (enemy.Alive)
                {
                    enemy.Update(gameTime);

                    Statics.Collision.PlayerAndEnemy(Player ,enemy, gameTime);
                }
            }
        }

        /// <summary>
        /// Called when the player reaches the level's exit.
        /// </summary>
        private void OnExitReached()
        {
            if (content == null)
                content = new ContentManager(Statics.Game.Services, "Content");

            Statics.Realm = Enums.Realm.WorldMap;
            MediaPlayer.Play(content.Load<Song>("Sounds/WorldMap"));
            int temp = 0;
            if (Statics.Level.Name.Equals("0") || Statics.Level.Name.Equals("2"))
                temp = 100;
            else
                temp = -200;

            Statics.WorldPlayer.Position = new Vector2(Statics.WorldPlayer.Position.X + temp, Statics.WorldPlayer.Position.Y);
        }

        private void PlayerDied()
        {
            Player.Die();
        }

        /// <summary>
        /// Draw everything in the level from background to foreground.
        /// </summary>
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            TileEngine.DrawLayersStart(Camera.CameraPositionXAxis, spriteBatch);
            
            Camera.ScrollCamera(spriteBatch.GraphicsDevice.Viewport, Player, TileEngine);

            Matrix cameraTransform = Matrix.CreateTranslation(-Camera.CameraPositionXAxis, -Camera.CameraPositionYAxis, 0.0f);  

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate, SaveStateMode.None, cameraTransform);

            TileEngine.DrawTiles(Camera.CameraPositionXAxis, spriteBatch, gameTime);

            TileEngine.DrawHealthPotions(gameTime, spriteBatch);

            Player.Draw(gameTime, spriteBatch);

            if(Boss != null)
                Boss.Draw(gameTime, spriteBatch);

            foreach (Enemy enemy in enemies)
                enemy.Draw(gameTime, spriteBatch);

            spriteBatch.End();

            TileEngine.DrawLayersEnd(Camera.CameraPositionXAxis, spriteBatch);

            Hud.Draw(gameTime, spriteBatch);
        }

        #endregion
    }
}

