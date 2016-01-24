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
using G.UI.Lib;
using System.Diagnostics;
using G.Input;
using G.Globals;


namespace G.UI.Screens
{   

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    class InstructionMenuScreen : MenuScreen
    {
        ContentManager content;
        Texture2D background;
    
        public InstructionMenuScreen() : base("")
        {       
         MenuEntry backMenuEntry = new MenuEntry("Back");
         backMenuEntry.Selected += OnCancel;
         MenuEntries.Add(backMenuEntry);

         if (content == null) 
             content = new ContentManager(Statics.Game.Services, "Content");
              
            background = content.Load<Texture2D>("Backgrounds\\Background001");
                 
    }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
              /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void LoadContent()
        {
            if (content == null) content = new ContentManager(ScreenManager.Game.Services, "Content");
            background = content.Load<Texture2D>("Backgrounds\\Instruction");

        }
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(50, 50, viewport.Width, viewport.Height);
            byte fade = TransitionAlpha;

            Color transitionColor = new Color(fade, fade, fade);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            spriteBatch.Draw(background, fullscreen, transitionColor);


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}