using System;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using G.Globals;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

namespace G.UI.Screens
{
	class ContinueGameScreen : MenuScreen
    {
        #region Fields


		ContentManager content;
		Texture2D background;

		MenuEntry slot1;
        MenuEntry slot2;
        MenuEntry slot3;
        MenuEntry slot4;
        MenuEntry slot5;

        #endregion
				
		#region Initialization


		/// <summary>
        /// Constructor.
        /// </summary>
		public ContinueGameScreen() : base("Continue Game")
        {
			MenuEntry backMenuEntry = new MenuEntry("Back");
            backMenuEntry.Selected += OnCancel;            
            
			slot1 = new MenuEntry("Slot 1");
			slot2 = new MenuEntry("Slot 2");
			slot3 = new MenuEntry("Slot 3");
			slot4 = new MenuEntry("Slot 4");
			slot5 = new MenuEntry("Slot 5");

			SetMenuText();

			slot1.Selected += Slot1Selected;
			slot2.Selected += Slot2Selected;
			slot3.Selected += Slot3Selected;
			slot4.Selected += Slot4Selected;
			slot5.Selected += Slot5Selected;

			MenuEntries.Add(slot1);
			MenuEntries.Add(slot2);
			MenuEntries.Add(slot3);
			MenuEntries.Add(slot4);
			MenuEntries.Add(slot5);
			MenuEntries.Add(backMenuEntry);
        }
		
        #endregion

		#region Load Content

		public override void LoadContent()
		{
			if (content == null) content = new ContentManager(ScreenManager.Game.Services, "Content");
	    		background = content.Load<Texture2D>("Backgrounds\\Background001");
			CenterTitle();
			LineSpacing = ScreenManager.Font.LineSpacing * 1.25f;
		}

		#endregion

        #region Handle Input

		void Slot1Selected(object sender, PlayerIndexEventArgs e)
		{
			SetMenuText();
		}

		void Slot2Selected(object sender, PlayerIndexEventArgs e)
		{
			SetMenuText();
		}

		void Slot3Selected(object sender, PlayerIndexEventArgs e)
		{
			SetMenuText();
		}

		void Slot4Selected(object sender, PlayerIndexEventArgs e)
		{
			SetMenuText();
		}

		void Slot5Selected(object sender, PlayerIndexEventArgs e)
		{
			SetMenuText();
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

		void SetMenuText()
		{
		/*	if (Statics.SaveGameDescriptions.Count > 1)
			{
				SaveGameDescription save = Statics.SaveGameDescriptions[0];
				slot1.Text = string.Format("Slot 1 | Name: {0} Class: {1} Level: {2} Region: {3}\r\n       | {4}", save.PlayerName, save.PlayerClass, save.PlayerLevel, save.Region, save.Description);
			}
			if (Statics.SaveGameDescriptions.Count > 2)
			{
				SaveGameDescription save = Statics.SaveGameDescriptions[1];
				slot2.Text = string.Format("Slot 2 | Name: {0} Class: {1} Level: {2} Region: {3}\r\n       | {4}", save.PlayerName, save.PlayerClass, save.PlayerLevel, save.Region, save.Description);
			}
			if (Statics.SaveGameDescriptions.Count > 3)
			{
				SaveGameDescription save = Statics.SaveGameDescriptions[2];
				slot3.Text = string.Format("Slot 3 | Name: {0} Class: {1} Level: {2} Region: {3}\r\n       | {4}", save.PlayerName, save.PlayerClass, save.PlayerLevel, save.Region, save.Description);
			}
			if (Statics.SaveGameDescriptions.Count > 4)
			{
				SaveGameDescription save = Statics.SaveGameDescriptions[3];
				slot4.Text = string.Format("Slot 4 | Name: {0} Class: {1} Level: {2} Region: {3}\r\n       | {4}", save.PlayerName, save.PlayerClass, save.PlayerLevel, save.Region, save.Description);
			}
			if (Statics.SaveGameDescriptions.Count > 1)
			{
				SaveGameDescription save = Statics.SaveGameDescriptions[4];
				slot5.Text = string.Format("Slot 5 | Name: {0} Class: {1} Level: {2} Region: {3}\r\n       | {4}", save.PlayerName, save.PlayerClass, save.PlayerLevel, save.Region, save.Description);
			}*/
		}


		private void ReadSaves()
		{
		}

    }
}
