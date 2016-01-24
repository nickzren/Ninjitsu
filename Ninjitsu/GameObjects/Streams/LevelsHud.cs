using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using G.Globals;

namespace G.GameObjects.Streams
{
    class LevelsHud
    {

        // declare veriables 
        Texture2D mHealthBar;
        int mCurrentHealth = 100;

        Texture2D PowerBar;
        int mCurrentPower= 0;

        Texture2D experBar;
        int mCurrentExper = 0;
        
        // combo star texture
        Texture2D comboStar;
        Rectangle starRec;

        Texture2D Background;
        SpriteFont hudFont;

        ContentManager contentManager;

        public LevelsHud()
        {
            //create contentManger
            contentManager = new ContentManager(Statics.Game.Services, "Content");
            
            if (contentManager == null)
                contentManager = new ContentManager(Statics.Game.Services, "Content");
                     

            //Load the HealthBar , exp and powr  images from the disk into the Texture2D object      
             mHealthBar = contentManager.Load<Texture2D>("Hud/HealthBar2");
             PowerBar = contentManager.Load<Texture2D>("Hud/PowBar");
             experBar = contentManager.Load<Texture2D>("Hud/ExperBar");
            
             // load the background , stars ,font 
             Background = contentManager.Load<Texture2D>("Hud/Background");
             starRec = new Rectangle(490, 61, 20, 20);
             comboStar = contentManager.Load<Texture2D>("Hud/star");
             hudFont = contentManager.Load<SpriteFont>("Fonts/Hud");            
        }

        public void LoadContent()
        {



        }

        public void Update(GameTime gameTime)
        {
            // Allows the default game to exit on Xbox 360 and Windows           
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                Statics.Game.Exit();        
          
            //update the health , expericane and power bars 
            mCurrentHealth = Statics.LevelPlayer.Health;
                      
            mCurrentHealth = (int)MathHelper.Clamp(mCurrentHealth, 0, Statics.LevelPlayer.fullHealth);

            mCurrentPower = Statics.LevelPlayer.CountActions;

            mCurrentExper = Statics.LevelPlayer.Experience;

            



            


           


        }

        public void Draw(GameTime gameTime, SpriteBatch mBatch)
        {

            string experianceLevel = Statics.LevelPlayer.Lv.ToString();
            
                    
            mBatch.Begin();
            //draw background and fonts
            mBatch.Draw(Background, new Rectangle(0, 0, Background.Width, Background.Height), Color.White);
            mBatch.DrawString(hudFont, experianceLevel, new Vector2(175, 76), Color.White);
            
            //draw red bar when the prayer health is less than 20% of the full health
            if (mCurrentHealth <= 0.2 * Statics.LevelPlayer.fullHealth)
            {
                mBatch.Draw(mHealthBar, new Rectangle(300, 13, (int)(mHealthBar.Width * ((double)mCurrentHealth / Statics.LevelPlayer.fullHealth)), 30), new Rectangle(0, 45, mHealthBar.Width, 30), Color.Red);
                //Draw the box around the health bar  
                mBatch.Draw(mHealthBar, new Rectangle(300, 13, 250, 30), new Rectangle(0, 0, mHealthBar.Width, 30), Color.White);
            }
            else
            {
                mBatch.Draw(mHealthBar, new Rectangle(300, 13, (int)(mHealthBar.Width * ((double)mCurrentHealth / Statics.LevelPlayer.fullHealth)), 30), new Rectangle(0, 45, mHealthBar.Width, 30), Color.LawnGreen);
                //Draw the box around the health bar  
                mBatch.Draw(mHealthBar, new Rectangle(300, 13, 250, 30), new Rectangle(0, 0, mHealthBar.Width, 30), Color.White);
            }


            //Draw the power for negative  health bar 
            mBatch.Draw(PowerBar, new Rectangle(300, 61, PowerBar.Width, 20), new Rectangle(0, 45, PowerBar.Width,20), Color.Gray);
            // current power bar
            mBatch.Draw(PowerBar, new Rectangle(300, 61, (int)(PowerBar.Width * ((double)mCurrentPower / 100)), 20), new Rectangle(0, 45, PowerBar.Width, 20), Color.Red);
            //Draw the box around the power bar  
            mBatch.Draw(PowerBar, new Rectangle(300, 61 , PowerBar.Width, 20), new Rectangle(0, 0, PowerBar.Width, 20), Color.White);

            //Draw the power for negative  health bar 
            mBatch.Draw(experBar, new Rectangle(980, 41, experBar.Width, 20), new Rectangle(0, 45, experBar.Width, 20), Color.Gray);
            // current power bar
            mBatch.Draw(experBar, new Rectangle(980, 41, (int)(experBar.Width * ((double)mCurrentExper / Statics.LevelPlayer.maxExper)), 20), new Rectangle(0, 45, experBar.Width, 20), Color.Blue);
            //Draw the box around the power bar  
            mBatch.Draw(experBar, new Rectangle(980, 41, experBar.Width, 20), new Rectangle(0, 0, experBar.Width, 20), Color.White);
            

            //draw the stars 
            if (Statics.LevelPlayer.ComboCount > 0)
            {
                for (int i = 1; i <= Statics.LevelPlayer.ComboCount; i++)
                {
                    mBatch.Draw(comboStar, starRec, Color.White);

                    starRec = new Rectangle(490+ i*30, 61, 20, 20);
                }


            }
            mBatch.End();  
            // base.Draw(gameTime);      
        }

       
    }
}
