using Microsoft.Xna.Framework;
using G.UI.Lib;
using G.UI.Screens;
using G.Globals;
using Microsoft.Xna.Framework.Media;

namespace G
{
    public class NinjitsuGame : Game
    {
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;

        public NinjitsuGame()
        {
            Statics.Game = this;

            graphics = new GraphicsDeviceManager(this)
            {
                PreferredBackBufferWidth = 1280,
                PreferredBackBufferHeight = 720

            };
            
            Content.RootDirectory = "Content";
            this.graphics.IsFullScreen = true;
            screenManager = new ScreenManager(this);

            Components.Add(screenManager);

            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);
        }
    }
}
