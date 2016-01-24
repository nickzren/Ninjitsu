#region Using Statements
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using G.GameObjects.WorldMaps;
using G.GameObjects.Characters;
using G.GameObjects.Levels;
using G.GameObjects.Collisions;
using G.UI.Lib;
using System;
using Microsoft.Xna.Framework.Media;
#endregion

namespace G.Globals
{
	public static class Statics
    {
        #region Properties
        /// <summary>
        /// Global handle on the main game file, used for Controllers but can also be used
        /// to trace into the code if handles are not immediatly available
        /// </summary>
        public static Game Game { get; set; }

        /// <summary>
        /// Current state the game is in - either true or false
        /// </summary>
        public static bool Gameover { get; set; }

        /// <summary>
        /// Current realm the game is in - either WorldMap or Level
        /// </summary>
        public static Enums.Realm Realm { get; set; }

        /// <summary>
        /// Detect all the collision in the game 
        /// </summary>
        public static Detection Collision { get; set; }

        public static VideoPlayer VideoPlayer { get; set; }

        /// <summary>
        /// WorldMap, used to load, update, draw worldmap related content
        /// </summary>
        public static WorldMap WorldMap { get; set; }
        public static WorldPlayer WorldPlayer { get; set; }
        public static Dictionary<Vector2, Rectangle> ClipMap { get; set; }
        public static Texture2D WhitePixel { get; set; }
        public static List<Portal> Portals { get; set; }
        

        /// <summary>
        /// Level, used to load, update, draw level related content
        /// </summary>
        public static Level Level { get; set; }
        public static LevelPlayer LevelPlayer { get; set; }

        #endregion

        #region Methods

        public static void LoadContent(GraphicsDevice graphicsDevice)
        {
            WhitePixel = new Texture2D(graphicsDevice, 1, 1);
            Color[] pixels = new Color[1 * 1];
            pixels[0] = Color.White;
            WhitePixel.SetData<Color>(pixels);
        }

        #endregion
	}
}
