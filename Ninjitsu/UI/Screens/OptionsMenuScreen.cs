#region File Description
//-----------------------------------------------------------------------------
// OptionsMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using System;
#endregion

namespace G.UI.Screens
{
    /// <summary>
    /// The options screen is brought up over the top of the main menu
    /// screen, and gives the user a chance to configure the game
    /// in various hopefully useful ways.
    /// </summary>
    class OptionsMenuScreen : MenuScreen
    {
        #region Fields

		ContentManager content;
		Texture2D background;

		MenuEntry mainVolume;
		MenuEntry sfxVolume;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsMenuScreen() : base("Options")
        {
			// Create our menu entries.
			mainVolume = new MenuEntry(string.Empty);
			sfxVolume = new MenuEntry(string.Empty);
           
            MenuEntry backMenuEntry = new MenuEntry("Back");

            // Hook up menu event handlers.
            backMenuEntry.Selected += OnCancel;
            
            // Add entries to the menu.
			MenuEntries.Add(mainVolume);
			MenuEntries.Add(sfxVolume);
            MenuEntries.Add(backMenuEntry);
        }

        #endregion

		#region Load Content

		public override void LoadContent()
		{
			if (content == null) content = new ContentManager(ScreenManager.Game.Services, "Content");
			background = content.Load<Texture2D>("Backgrounds\\Background001");
			CenterTitle();
		}

		#endregion

		#region Drawing

		public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            byte fade = TransitionAlpha;

            Color transitionColor = new Color(fade, fade, fade);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

                spriteBatch.Draw(background, fullscreen, transitionColor);                

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}
