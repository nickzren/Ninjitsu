using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using G.Globals;

namespace G.UI.Screens
{
    class ControlMenuScreen : MenuScreen
    {
        ContentManager content;
        Texture2D background;
        public ControlMenuScreen():base("")
        {
            MenuEntry backMenuEntry = new MenuEntry("");
            backMenuEntry.Selected += OnCancel;
            MenuEntries.Add(backMenuEntry);
        }
        public override void LoadContent()
        {
            if (content == null) content = new ContentManager(ScreenManager.Game.Services, "Content");
               background = content.Load<Texture2D>("Backgrounds\\controls");

        }
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
    }
}
