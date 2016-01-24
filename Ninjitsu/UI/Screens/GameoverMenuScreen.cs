#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using G.Globals;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
#endregion

namespace G.UI.Screens
{
    /// <summary>
    /// The gameover screen comes up over the top of the game,
    /// giving the player options to replay or quit.
    /// </summary>
    class GameoverMenuScreen : MenuScreen
    {
        //fields
        public ContentManager content;
        public Texture2D winOverlay;
        public Texture2D loseOverlay;
        
        
        //Determine the status overlay message to show.
        public Texture2D status = null;
        #region Initialization


       

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameoverMenuScreen()
            : base("Gameover")
        {
            // Create a new content manager to load content used just by this level.
            if (content == null)
                content = new ContentManager(Statics.Game.Services, "Content");

            //Load Overlays
            winOverlay = content.Load<Texture2D>("Overlays/you_win");
            loseOverlay = content.Load<Texture2D>("Overlays/you_lose");
           

            // Flag that there is no need for the game to transition
            // off when the Gameover screen is on top of it.
            IsPopup = true;

            // Create our menu entries.
            MenuEntry quitGameMenuEntry = new MenuEntry("Return Main Menu");

            // Hook up menu event handlers.
            quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(quitGameMenuEntry);
        }


        #endregion

        #region Handle Input


        /// <summary>
        /// Event handler for when the Replay Game menu entry is selected.
        /// </summary>
        void ReplayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Are you sure you want to replay this game?";

            MessageBoxScreen confirmReplayMenuMessageBox = new MessageBoxScreen(message);

            ScreenManager.AddScreen(confirmReplayMenuMessageBox, ControllingPlayer);
        }

        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            const string message = "Are you sure you want to quit this game?";

            MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox, ControllingPlayer);
        }

        void ConfirmQuitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, null, new BackgroundScreen(),
                                                           new MainMenuScreen());

            Statics.Gameover = true;
            MediaPlayer.Stop();
        }


        #endregion

        #region Draw


        /// <summary>
        /// Draws the pause menu screen. This darkens down the gameplay screen
        /// that is underneath us, and then chains to the base MenuScreen.Draw.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            //ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);
            Rectangle titleSafeArea = ScreenManager.GraphicsDevice.Viewport.TitleSafeArea;
            Vector2 center = new Vector2(titleSafeArea.X + titleSafeArea.Width / 2.0f, titleSafeArea.Y + titleSafeArea.Height / 2.0f);
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            if (Statics.LevelPlayer.Alive)
                status = winOverlay;
            else
                status = loseOverlay;
            spriteBatch.Begin();
            if (status != null)
            {
                // Draw status message.
                Vector2 statusSize = new Vector2(status.Width, status.Height);
                spriteBatch.Draw(status, center - statusSize / 2, Color.White);
            }
            spriteBatch.End();
            base.Draw(gameTime);
        }


        #endregion
    }
}
