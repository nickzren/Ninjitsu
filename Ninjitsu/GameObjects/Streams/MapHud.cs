using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using G.Globals;

namespace G.GameObjects.Streams
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class MapHud 
    {

        // to draw the player health, exper and power
        SpriteFont hudFont;
        Texture2D comboStar;
        Texture2D background;
        string level;
        string lives; 
        float experiance = Statics.LevelPlayer.Experience / Statics.LevelPlayer.maxExper * 100;
        float health = Statics.LevelPlayer.Health / Statics.LevelPlayer.fullHealth * 100;
        float power = Statics.LevelPlayer.CountActions;
        string experString = "";
        string healthString = "";
        string comboCount = "";
        ContentManager contentManager; 
        public MapHud()
            
        {
            //create contentManger
            contentManager = new ContentManager(Statics.Game.Services, "Content");

            if (contentManager == null)
                contentManager = new ContentManager(Statics.Game.Services, "Content");

            hudFont = contentManager.Load<SpriteFont>("Fonts/Hud");
            comboStar = contentManager.Load<Texture2D>("Hud/star");
            background = contentManager.Load<Texture2D>("Hud/MapHudBackground");
            level = "Lv. " + Statics.LevelPlayer.Lv.ToString();
            lives = "Lives: " + Statics.LevelPlayer.Lives.ToString();
            experString = "Exper: " + experiance + " % ";
            comboCount = "X " + Statics.LevelPlayer.ComboCount.ToString();
            healthString = "Health: " + health+ " % ";
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
     

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime)
        {
            experiance = Statics.LevelPlayer.Experience / Statics.LevelPlayer.maxExper * 100;
            health = Statics.LevelPlayer.Health / Statics.LevelPlayer.fullHealth * 100;
            power = Statics.LevelPlayer.CountActions;
            //for to displayed , persantage
            level = "Lv. " + Statics.LevelPlayer.Lv.ToString();
            lives = "Lives: " + Statics.LevelPlayer.Lives.ToString();
            experString = "Exper: " + experiance + " % ";
            comboCount = " X " + Statics.LevelPlayer.ComboCount.ToString(); 
            healthString = "Health: " + health + " % ";
            
        }

        public void Draw( SpriteBatch mBatch)
        {
            mBatch.Begin();
            mBatch.Draw(background, new Rectangle(1130, -90, 150, 250), Color.White);
            mBatch.DrawString(hudFont, experString, new Vector2(1140, 90), Color.White);
            mBatch.DrawString(hudFont, comboCount, new Vector2(1160, 130), Color.White);
            mBatch.Draw(comboStar, new Rectangle(1140,130,20,20), Color.White);
            mBatch.DrawString(hudFont, healthString, new Vector2(1140,50), Color.White);
            mBatch.DrawString(hudFont, level, new Vector2(1140, 5), Color.White);
            mBatch.DrawString(hudFont, lives, new Vector2(1140, 25), Color.White);

            mBatch.End();

        }
    }
}