#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using G.Globals;
using Microsoft.Xna.Framework.Media;

#endregion

namespace G.UI.Screens
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        #region Fields

        ContentManager content;
        Texture2D pictureBackground;
        Texture2D gameTitle;
        Rectangle gameTitlePosition;
        Video DemoVideo;
        #endregion

        #region Initialization

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen() : base("")
        {
            // Create our menu entries.
            MenuEntry playGameMenuEntry = new MenuEntry("New Game");
            //MenuEntry continueGameMenuEntry = new MenuEntry("Continue Story");
            MenuEntry HelpMenuEntry = new MenuEntry("Intstructions");
            MenuEntry exitMenuEntry = new MenuEntry("Exit");

            // Hook up menu event handlers.
            playGameMenuEntry.Selected += PlayGameMenuEntrySelected;
            //continueGameMenuEntry.Selected += ContinueGameMenuEntrySelected;
            HelpMenuEntry.Selected += HelpMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;
            // Add entries to the menu.
            MenuEntries.Add(playGameMenuEntry);
            MenuEntries.Add(HelpMenuEntry);
            MenuEntries.Add(exitMenuEntry);
            MenuPosition = new Vector2(60f, 570f);
        }


        #endregion

        #region Load Content

        public override void LoadContent()
        {
            if (content == null) content = new ContentManager(ScreenManager.Game.Services, "Content");

            pictureBackground = content.Load<Texture2D>("Backgrounds\\Ninjitus_title");

            gameTitle = content.Load<Texture2D>("Backgrounds\\Ninjitsu");
            gameTitlePosition = new Rectangle(80, 150, gameTitle.Width, gameTitle.Height);

            DemoVideo = content.Load<Video>("Backgrounds\\Demo");
            Statics.VideoPlayer = new VideoPlayer();
            Statics.VideoPlayer.IsLooped = true;
            Statics.VideoPlayer.Play(DemoVideo);
        }

        #endregion

        #region Handle Input

        /// <summary>
        /// Event handler for when the Play Game menu entry is selected.
        /// </summary>
        void PlayGameMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            LoadingScreen.Load(ScreenManager, true, e.PlayerIndex, new GameplayScreen());
        }

        /// <summary>
        /// Event handler for when the Options menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new OptionsMenuScreen(), e.PlayerIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        void HelpMenuEntrySelected(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.AddScreen(new ControlMenuScreen(), e.PlayerIndex);
        }
        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel(PlayerIndex playerIndex)
        {
            const string message = "Are you sure you want to exit this sample?";

            MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox, playerIndex);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, PlayerIndexEventArgs e)
        {
            ScreenManager.Game.Exit();
        }

        #endregion

        #region Drawing

        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            Vector2 position = new Vector2(400, 160);
            byte fade = TransitionAlpha;
            Color transitionColor = new Color(fade, fade, fade);

            spriteBatch.Begin();

            if (Statics.VideoPlayer.State == MediaState.Playing)
            {
                spriteBatch.Draw(Statics.VideoPlayer.GetTexture(), new Rectangle(0, 0, DemoVideo.Width, DemoVideo.Height), Color.White);
                spriteBatch.Draw(gameTitle, gameTitlePosition, Color.White);
                spriteBatch.DrawString(ScreenManager.Font, "-  David Norris, Zhong Ren, Afnan Alawani.", position, Color.White);

            }
            else
                spriteBatch.Draw(pictureBackground, fullscreen, transitionColor);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #endregion
    }
}
