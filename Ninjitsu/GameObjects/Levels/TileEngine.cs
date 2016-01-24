#region Using Statements
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using G.Globals;
using G.GameObjects.Characters;
using G.GameObjects.Levels.Objects;
using G.GameObjects.Characters.Moves;
#endregion

namespace G.GameObjects.Levels
{
    public class TileEngine : IDisposable
    {
        public ContentManager content;

        public List<HealthPotion> healthPotions = new List<HealthPotion>();

        public Level Level
        {
            get { return level; }
            set { level = value; }
        }
        Level level;

        // Physical structure of the level.
        private LevelTile[,] tiles;
        private List<Spider> spiders = new List<Spider>();
        
        
        private Layer[] layers;

        // Arbitrary, but constant seed
        private Random random = new Random(354668);

        public List<MovableTile> movableTiles = new List<MovableTile>();
        

        // Key locations in the level.        
        public Vector2 start;
        public Point exit = InvalidPosition;
        public Point exit2 = InvalidPosition;
        public static readonly Point InvalidPosition = new Point(-1, -1);

        ////Overlays
        //private Texture2D winOverlay;
        //private Texture2D loseOverlay;
        //private Texture2D diedOverlay;

        // The layer which entities are drawn on top of.
        public const int EntityLayer = 2;

        public TileEngine(Level level)
        {
            this.Level = level;

            // Create a new content manager to load content used just by this level.
            if (content == null)
                content = new ContentManager(Statics.Game.Services, "Content");

            // Load background layer textures. For now, all levels must
            // use the same backgrounds and only use the left-most part of them.
            layers = new Layer[3];
            if (!Level.Name.Equals("Boss"))
            {
                layers[0] = new Layer("Backgrounds/Layer0", 0.2f);
                layers[1] = new Layer("Backgrounds/Layer1", 0.5f);
                layers[2] = new Layer("Backgrounds/Layer2", 0.8f);
            }
            else
            {
                layers[0] = new Layer("Backgrounds/BossLayer", 0.2f);
                layers[1] = new Layer("Backgrounds/BossLayer", 0.5f);
                layers[2] = new Layer("Backgrounds/BossLayer", 0.8f);
            }
        }
        
        /// <summary>
        /// Iterates over every tile in the structure file and loads its
        /// appearance and behavior. This method also validates that the
        /// file is well-formed with a player start point, exit, etc.
        /// </summary>
        /// <param name="path">
        /// The absolute path to the level file to be loaded.
        /// </param>
        public void LoadTiles(string path)
        {
            // Load the level and ensure all of the lines are the same length.
            int width;
            List<string> lines = new List<string>();
            using (StreamReader reader = new StreamReader(path))
            {
                string line = reader.ReadLine();
                width = line.Length;
                while (line != null)
                {
                    lines.Add(line);
                    if (line.Length != width)
                        throw new Exception(String.Format("The length of line {0} is different from all preceeding lines.", lines.Count));
                    line = reader.ReadLine();
                }
            }

            // Allocate the tile grid.
            tiles = new LevelTile[width, lines.Count];

            // Loop over every tile position,
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    // to load each tile.
                    char tileType = lines[y][x];
                    tiles[x, y] = LoadTile(tileType, x, y);
                }
            }
        }

        /// <summary>
        /// Loads an individual tile's appearance and behavior.
        /// </summary>
        /// <param name="tileType">
        /// The character loaded from the structure file which
        /// indicates what should be loaded.
        /// </param>
        /// <param name="x">
        /// The X location of this tile in tile space.
        /// </param>
        /// <param name="y">
        /// The Y location of this tile in tile space.
        /// </param>
        /// <returns>The loaded tile.</returns>
        private LevelTile LoadTile(char tileType, int x, int y)
        {
            switch (tileType)
            {
                // Blank space
                case '.':
                    return new LevelTile(null, TileCollision.Passable);

                // Exit
                case 'X':
                    return LoadExitTile(x, y);

                case 'R':
                    return LoadSpiderTile(x, y);

                // Spike Tiles
                case '-':
                    return LoadTile("Spike", TileCollision.Spike);

                // Various enemies
                case 'A':
                    return LoadEnemyTile(x, y, "MonsterA", 25, 3, 3);
                case 'B':
                    return LoadEnemyTile(x, y, "MonsterB", 40, 5, 5);
                case 'C':
                    return LoadEnemyTile(x, y, "MonsterC", 60, 8, 8);
                case 'D':
                    return LoadEnemyTile(x, y, "MonsterD", 80, 10, 10);

                case 'S':
                    return LoadBossTile(x, y, "Boss", 500, 20);

                // HealthPotion
                case 'H':
                    return LoadHealthPotionTile(x, y);

                // Moving platform - Horizontal
                case 'M':
                    return LoadMovableTile(x, y, TileCollision.Platform);
                
                // Moving platform - Verticle
                case 'V':
                    return LoadMovableTile2(x, y, TileCollision.Platform);

                // Player 1 start point
                case '1':
                    return LoadStartTile(x, y);

                // Impassable block

                case '#':
                    return LoadTile("Platform", TileCollision.Platform);

                case '*':
                    return LoadVarietyTile("BlockA", 6, TileCollision.Impassable);

                // Unknown tile type character
                default:
                    throw new NotSupportedException(String.Format("Unsupported tile type character '{0}' at position {1}, {2}.", tileType, x, y));
            }
        }

        private LevelTile LoadMovableTile(int x, int y, TileCollision collision)
        {
            Point position = GetBounds(x, y).Center;
            movableTiles.Add(new MovableTile(level, new Vector2(position.X, position.Y), collision, false, "Tiles/BlockB0"));
            return new LevelTile(null, TileCollision.Passable);
        }
        private LevelTile LoadMovableTile2(int x, int y, TileCollision collision)
        {
            Point position = GetBounds(x, y).Center;
            movableTiles.Add(new MovableTile(level, new Vector2(position.X, position.Y), collision, false, "Tiles/BlockB1"));
            return new LevelTile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Creates a new tile. The other tile loading methods typically chain to this
        /// method after performing their special logic.
        /// </summary>
        /// <param name="name">
        /// Path to a tile texture relative to the Content/Tiles directory.
        /// </param>
        /// <param name="collision">
        /// The tile collision type for the new tile.
        /// </param>
        /// <returns>The new tile.</returns>
        private LevelTile LoadTile(string name, TileCollision collision)
        {
            return new LevelTile(content.Load<Texture2D>("Tiles/" + name), collision);
        }

        /// <summary>
        /// Loads a tile with a random appearance.
        /// </summary>
        /// <param name="baseName">
        /// The content name prefix for this group of tile variations. Tile groups are
        /// name LikeThis0.png and LikeThis1.png and LikeThis2.png.
        /// </param>
        /// <param name="variationCount">
        /// The number of variations in this group.
        /// </param>
        private LevelTile LoadVarietyTile(string baseName, int variationCount, TileCollision collision)
        {
            int index = random.Next(variationCount);
            return LoadTile(baseName + index, collision);
        }

        /// <summary>
        /// Instantiates a player, puts him in the level, and remembers where to put him when he is resurrected.
        /// </summary>
        private LevelTile LoadStartTile(int x, int y)
        {
            start = RectangleExtensions.GetBottomCenter(GetBounds(x, y));

            if (Statics.Gameover)
            {
                Statics.Gameover = false;
            }
            else
                Statics.LevelPlayer.Reset(start);

            return new LevelTile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Remembers the location of the level's exit.
        /// </summary>
        private LevelTile LoadExitTile(int x, int y)
        {
            if (exit != InvalidPosition)
                throw new NotSupportedException("A level may only have one exit.");

            exit = GetBounds(x, y).Center;

            return LoadTile("Exit", TileCollision.Passable);
        }

        /// <summary>
        /// Loads the falling Knife Tiles
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private LevelTile LoadSpiderTile(int x, int y)
        {
            Point position = GetBounds(x, y).Center;
            spiders.Add(new Spider(this.Level, new Vector2(position.X, position.Y)));
            return new LevelTile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Instantiates an enemy and puts him in the level.
        /// </summary>
        private LevelTile LoadBossTile(int x, int y, string spriteSet, int health, int attack)
        {
            Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            Level.Boss = new Boss(position, spriteSet, health, attack);

            return new LevelTile(null, TileCollision.Passable);
        }

        /// <summary>
        /// Instantiates an enemy and puts him in the level.
        /// </summary>
        private LevelTile LoadEnemyTile(int x, int y, string spriteSet, int health, int attack, int exp)
        {
            Vector2 position = RectangleExtensions.GetBottomCenter(GetBounds(x, y));
            Enemy e = new Enemy(position, spriteSet, health, attack, exp);
            Level.enemies.Add(e);

            return new LevelTile(null, TileCollision.Passable);
        }

        public void DrawHealthPotions(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (HealthPotion healthPotion in healthPotions)
                healthPotion.Draw(gameTime, spriteBatch);
            foreach (Spider knife in spiders)
                knife.Draw(gameTime, spriteBatch);
        }

        /// <summary>
        /// Animates each HealthPotion and checks to allows the player to collect them.
        /// </summary>
        public void UpdateHealthPotions(GameTime gameTime, LevelPlayer player)
        {
            UpdateKnives(gameTime);
            for (int i = 0; i < healthPotions.Count; ++i)
            {
                HealthPotion healthPotion = healthPotions[i];

                healthPotion.Update(gameTime);

                if (healthPotion.BoundingCircle.Intersects(player.BoundingRectangle))
                {
                    healthPotions.RemoveAt(i--);
                    OnHealthPotionCollected(healthPotion, player);
                }
            }
        }
        /// <summary>
        /// Update Knives
        /// </summary>
        /// <param name="gameTime"></param>
        private void UpdateKnives(GameTime gameTime)
        {
            for (int i = 0; i < spiders.Count; ++i)
            {
                Spider knife = spiders[i];
                knife.Update(gameTime);

                if (knife.BoundingCircle.Intersects(Statics.LevelPlayer.BoundingRectangle) && knife.attackComplete == false)
                {
                    Statics.LevelPlayer.Health -= knife.AttackStrength;
                    knife.attackComplete = true;
                }

                else if (knife.TriggerRectangle.Intersects(Statics.LevelPlayer.BoundingRectangle))
                {
                    knife.isFalling = true;
                }
              
            }
     }

        

        /// <summary>
        /// Called when a HealthPotion is collected.
        /// </summary>
        /// <param name="healthPotion">The healthPotion that was collected.</param>
        /// <param name="collectedBy">The player who collected this healthPotion.</param>
        private void OnHealthPotionCollected(HealthPotion healthPotion, LevelPlayer collectedBy)
        {
            collectedBy.Health += (2 * collectedBy.fullHealth/ 3);
            if (collectedBy.Health >= collectedBy.fullHealth)
                collectedBy.Health = collectedBy.fullHealth;

            healthPotion.OnCollected(collectedBy);
        }

        /// <summary>
        /// Instantiates a healthPotion and puts it in the level.
        /// </summary>
        private LevelTile LoadHealthPotionTile(int x, int y)
        {
            Point position = GetBounds(x, y).Center;
            healthPotions.Add(new HealthPotion(Statics.Level, new Vector2(position.X, position.Y)));

            return new LevelTile(null, TileCollision.Passable);
        }

        #region Bounds and collision

        /// <summary>
        /// Gets the collision mode of the tile at a particular location.
        /// This method handles tiles outside of the levels boundries by making it
        /// impossible to escape past the left or right edges, but allowing things
        /// to jump beyond the top of the level and fall off the bottom.
        /// </summary>
        public TileCollision GetCollision(int x, int y)
        {
            // Prevent escaping past the level ends.
            if (x < 0 || x >= Width)
                return TileCollision.Impassable;
            // Allow jumping past the level top and falling through the bottom.
            if (y < 0 || y >= Height)
                return TileCollision.Passable;

            return tiles[x, y].Collision;
        }

        /// <summary>
        /// Gets the bounding rectangle of a tile in world space.
        /// </summary>        
        public Rectangle GetBounds(int x, int y)
        {
            return new Rectangle(x * LevelTile.Width, y * LevelTile.Height, LevelTile.Width, LevelTile.Height);
        }

        /// <summary>
        /// Width of level measured in tiles.
        /// </summary>
        public int Width
        {
            get { return tiles.GetLength(0); }
        }

        /// <summary>
        /// Height of the level measured in tiles.
        /// </summary>
        public int Height
        {
            get { return tiles.GetLength(1); }
        }

        #endregion

        public void DrawLayersStart(float cameraPosition, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            for (int i = 0; i <= EntityLayer; ++i)
                layers[i].Draw(spriteBatch, cameraPosition);
            spriteBatch.End();
        }

        public void DrawLayersEnd(float cameraPosition, SpriteBatch spriteBatch)
        {
            spriteBatch.Begin();
            for (int i = EntityLayer + 1; i < layers.Length; ++i)
                layers[i].Draw(spriteBatch, cameraPosition);
            spriteBatch.End();
        }

        /// <summary>
        /// Draws each tile in the level.
        /// </summary>
        public void DrawTiles(float cameraPosition, SpriteBatch spriteBatch, GameTime gameTime)
        {
            // Calculate the visible range of tiles.
            int left = (int)Math.Floor(cameraPosition / LevelTile.Width);
            int right = left + spriteBatch.GraphicsDevice.Viewport.Width / LevelTile.Width;
            right = Math.Min(right, Width - 1);

            foreach (MovableTile tile in movableTiles)
                tile.Draw(gameTime, spriteBatch);

            // For each tile position
            for (int y = 0; y < Height; ++y)
            {
                for (int x = left; x <= right; ++x)
                {
                    // If there is a visible tile in that position
                    Texture2D texture = tiles[x, y].Texture;
                    if (texture != null)
                    {
                        // Draw it in screen space.
                        Vector2 position = new Vector2(x, y) * LevelTile.Size;
                        spriteBatch.Draw(texture, position, Color.White);
                    }
                }
            }
            

        }

       

        /// <summary>
        /// Unloads the tile content.
        /// </summary>
        public void Dispose()
        {
            content.Unload();
        }
    }
}
