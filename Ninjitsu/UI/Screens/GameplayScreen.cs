#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using G.Input;
using G.Globals;
using G.UI.Lib;
using G.GameObjects.WorldMaps;
using G.GameObjects.Collisions;
using Microsoft.Xna.Framework.Media;
using G.GameObjects.Characters;
#endregion

namespace G.UI.Screens
{
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Initialization
        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            Statics.WorldMap = new WorldMap();

            Statics.Collision = new Detection();

            Statics.Realm = Enums.Realm.WorldMap;

            Statics.VideoPlayer.Stop();
        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            switch (Statics.Realm)
            {
                case Enums.Realm.WorldMap:
                    Statics.WorldMap.LoadContent();
                    break;
                case Enums.Realm.Level:
                    Statics.Level.LoadContent();
                    break;
            }

            ScreenManager.Game.ResetElapsedTime();
        }

        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
        }

        #endregion

        #region Update and Draw

        /// <summary>
        /// Updates the state of the game. This method checks the GameScreen.IsActive
        /// property, so the game will stop updating when the pause menu is active,
        /// or if you tab away to a different application.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {
                    switch (Statics.Realm)
                    {
                        case Enums.Realm.WorldMap:
                            Statics.WorldMap.Update(gameTime);
                            break;
                        case Enums.Realm.Level:
                            if (!Statics.Gameover)
                                Statics.Level.Update(gameTime);
                            else
                                ScreenManager.AddScreen(new GameoverMenuScreen(), null);
                            break;
                    }

            }
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(MenuInput input)
        {
            if (input == null) throw new ArgumentNullException("input");

            int playerIndex = (int)ControllingPlayer.Value;

            KeyboardState keyboardState = input.CurrentKeyboardStates[playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[playerIndex];

            bool gamePadDisconnected = !gamePadState.IsConnected && input.GamePadWasConnected[playerIndex];

            if (input.IsPauseGame(ControllingPlayer) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(), ControllingPlayer);
            }
            else
            {
                switch (Statics.Realm)
                {
                    case Enums.Realm.WorldMap:
                        Statics.WorldPlayer.HandleInput(input, ControllingPlayer);
                        Statics.WorldMap.CheckCollisions();
                        Statics.WorldMap.camera.Update(input, ControllingPlayer);
                        break;
                    case Enums.Realm.Level:
                        break;
                }
            }
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            switch (Statics.Realm)
            {
                case Enums.Realm.WorldMap:
                    Statics.WorldMap.Draw(ScreenManager.SpriteBatch);
                    break;
                case Enums.Realm.Level:
                    Statics.Level.Draw(gameTime, ScreenManager.SpriteBatch);
                    break;
            }

            if (TransitionPosition > 0)
            {
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
            }
        }

        #endregion
    }
}
