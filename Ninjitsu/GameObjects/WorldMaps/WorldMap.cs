using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TiledLib;
using G.Globals;
using System.IO;
using Microsoft.Xna.Framework.Storage;
using G.GameObjects.Characters;
using G.GameObjects.Levels;
using G.GameObjects.Streams;
using Microsoft.Xna.Framework.Media;

namespace G.GameObjects.WorldMaps
{
    public class WorldMap
    {
        MapHud mapHud;

       

        ContentManager content;

        #region Fields

        Texture2D whitePixel;

        SpriteFont gameFont;

        Map map;

        public WorldMapCamera camera;

        #endregion

        public WorldMap()
        {
            Statics.LevelPlayer = new LevelPlayer(new Vector2(0, 0));
            mapHud = new MapHud();
        }

        public void LoadContent()
        {
            if (content == null)
                content = new ContentManager(Statics.Game.Services, "Content");

            MediaPlayer.Play(content.Load<Song>("Sounds/WorldMap"));

            Statics.LoadContent(Statics.Game.GraphicsDevice);

            gameFont = content.Load<SpriteFont>("Fonts\\Hud");

            LoadMap("Maps\\WorldMap");

            whitePixel = new Texture2D(Statics.Game.GraphicsDevice, 1, 1);
            Color[] pixels = new Color[1 * 1];
            pixels[0] = Color.White;
            whitePixel.SetData<Color>(pixels);
        }

        private void LoadMap(string assetName)
        {
            //List of Portals declared for traveling between World map and scrolling levels
            Statics.Portals = new List<Portal>();
            
            Statics.ClipMap = new Dictionary<Vector2, Rectangle>();

            map = content.Load<Map>(assetName);

            camera = new WorldMapCamera(new Vector2(map.Width, map.Height), new Vector2(map.TileWidth, map.TileHeight), new Vector2(Statics.Game.GraphicsDevice.Viewport.Width, Statics.Game.GraphicsDevice.Viewport.Height));

            MapObjectLayer objects = map.GetLayer("Objects") as MapObjectLayer;
            
            //Read in information about portals from tmx file
            foreach (MapObject obj in objects.Objects)
            {
                switch (obj.Name)
                {
                    case "PlayerStart":
                        if (Statics.WorldPlayer == null)
                        {
                            Statics.WorldPlayer = new WorldPlayer(new Vector2(obj.Location.X + (obj.Location.Width / 2), obj.Location.Y), new Vector2(91f, 91f));
                            Statics.WorldPlayer.LoadContent(content, "Sprites\\Characters\\Warrior1WalkLeft");
                            Statics.WorldPlayer.Map = map;
                        }
                        else
                        {
                            Statics.WorldPlayer.Position = new Vector2(obj.Location.X + (obj.Location.Width / 2), obj.Location.Y);
                        }
                        break;
                    case "Portal1":
                    case "Portal2":
                    case "Portal3":
                    case "Portal4":
                        Portal portal = new Portal();
                        portal.Position = new Vector2(obj.Location.X, obj.Location.Y);
                        portal.Size = new Vector2(obj.Location.Width, obj.Location.Height);
                        portal.LevelName = obj.Properties["LevelName"].Value;
                        string[] tileLoc = obj.Properties["DestinationTileLocation"].Value.Split(',');
                        portal.DestinationTileLocation = new Vector2(Convert.ToInt32(tileLoc[0]), Convert.ToInt32(tileLoc[1]));
                        Statics.Portals.Add(portal);
                        break;
                }
            }

            Statics.WorldPlayer.UpdateBounds(map.TileWidth, map.TileHeight);
    //        camera.FocusOn(Statics.WorldPlayer);

            LoadClipMap();
        }

        private void LoadClipMap()
        {
            Statics.ClipMap = new Dictionary<Vector2, Rectangle>();
            TileLayer clipLayer = map.GetLayer("Clip") as TileLayer;
            if (clipLayer.Tiles.Length > 0)
            {
                for (int y = 0; y < clipLayer.Width; y++)
                {
                    for (int x = 0; x < clipLayer.Height; x++)
                    {
                        Tile tile = clipLayer.Tiles[x, y];
                        if (tile != null)
                        {
                            Statics.ClipMap.Add(new Vector2(x, y), new Rectangle(x * tile.Source.Width, y * tile.Source.Height, tile.Source.Width, tile.Source.Height));
                        }
                    }
                }
            }
        }

        public void UnloadContent()
        {
            content.Unload();
        }

        public void Update(GameTime gameTime)
        {
            Statics.WorldPlayer.Update(gameTime, camera);
            mapHud.Update(gameTime);
        }

        public void CheckCollisions()
        {
            Statics.WorldPlayer.UpdateBounds(map.TileWidth, map.TileHeight);

            foreach (Rectangle clip in Statics.ClipMap.Values)
            {
                if (Statics.WorldPlayer.Bounds.Intersects(clip))
                {
                    Statics.WorldPlayer.Collision();
                    break;
                }
            }

            foreach (Portal portal in Statics.Portals)
            {
                if (Statics.WorldPlayer.Bounds.Intersects(portal.Bounds))
                {
                        Statics.Realm = Enums.Realm.Level;
                        String path = String.Format("Levels/{0}.txt", portal.LevelName);
                        path = Path.Combine(StorageContainer.TitleLocation, "Content/" + path);
                        Statics.Level = new Level(path, portal.LevelName);
                        Statics.Level.Player = Statics.LevelPlayer;
                    if(portal.LevelName.Equals("Boss"))
                        MediaPlayer.Play(content.Load<Song>("Sounds/" + portal.LevelName));
                    else
                        MediaPlayer.Play(content.Load<Song>("Sounds/0"));
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.None, camera.Matrix);
            
            SetVisible(true, true, false);
            map.Draw(spriteBatch, camera.VisibleArea);

            Statics.WorldPlayer.Draw(spriteBatch);

            SetVisible(false, false, true);
            map.Draw(spriteBatch, camera.VisibleArea);
            spriteBatch.End();

            mapHud.Draw(spriteBatch);
        }

        public void SetVisible(bool ground, bool details, bool overlay)
        {
            map.GetLayer("Ground").Visible = ground;
            map.GetLayer("Detail").Visible = details;
            map.GetLayer("Overlay").Visible = overlay;
        }
    }
}