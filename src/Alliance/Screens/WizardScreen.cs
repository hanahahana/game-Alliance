using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLA.Utilities.Xna.Graphics;
using MLA.Utilities.Xna.Helpers;
using MLA.Xna.Gui;
using MLA.Xna.Gui.Controls;
using MLA.Xna.Gui.Drawing;
using Alliance.Input;

namespace Alliance.Screens
{
  public class WizardScreen : GameScreen
  {
    const float WidthDelta = 10f;
    const int BtnWidth = 90;
    const int BtnHeight = 30;
    const int BtnMargin = 10;

    readonly static Color BtnBackColor = Color.Silver;
    readonly static Color BtnForeColor = Color.Black;

    SpriteFont wizardFont;
    List<string> wizardPages = new List<string>();
    int currentIndex = 0;

    GuiComponent mGui;
    Button btnExit;
    Button btnNext;

    public WizardScreen()
    {
      IsPopup = true;
    }

    public override void LoadContent()
    {
      wizardFont = AllianceGame.Fonts["Verdana"];
      wizardPages = AllianceGame.Strings["wizard"];

      mGui = new GuiComponent(ScreenManager.Game);
      mGui.Initialize();
      mGui.LoadContent();

      btnExit = new Button();
      btnExit.Text = "Exit";
      btnExit.Click += new EventHandler(btnExit_Click);
      
      btnNext = new Button();
      btnNext.Text = "Next";
      btnNext.Click += new EventHandler(btnNext_Click);

      // set the common properties
      btnExit.Width = btnNext.Width = BtnWidth;
      btnExit.Height = btnNext.Height = BtnHeight;
      btnExit.TextAlignment = btnNext.TextAlignment = DataAlignment.MiddleCenter;
      btnExit.BackColor = btnNext.BackColor = BtnBackColor;
      btnExit.ForeColor = btnNext.ForeColor = BtnForeColor;
      btnExit.Font = btnNext.Font = AllianceGame.Fonts["Verdana"];

      // add the buttons
      mGui.Controls.Add(btnExit);
      mGui.Controls.Add(btnNext);
    }

    private void btnNext_Click(object sender, EventArgs e)
    {
      ++currentIndex;
      btnNext.Enabled = ((currentIndex + 1) < wizardPages.Count);
    }

    private void btnExit_Click(object sender, EventArgs e)
    {
      ExitScreen();
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
      if (IsActive)
      {
        mGui.Update(gameTime);
      }
      base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
    }

    public override void HandleInput(InputState input)
    {
      if (input.MenuCancel)
        ExitScreen();
    }

    public override void Draw(GameTime gameTime)
    {
      // if we're transitioning on, then just don't draw
      if (ScreenState == ScreenState.TransitionOn)
        return;

      SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
      SpriteFont font = ScreenManager.Font;

      // Darken down any other screens that were drawn beneath the popup.
      ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

      // Center the message text in the viewport.
      Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
      Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
      Vector2 pageSize = new Vector2(viewportSize.X / 2f, viewportSize.Y / 2f);
      Vector2 pagePosition = (viewportSize - pageSize) / 2;

      // The background includes a border somewhat larger than the text itself.
      const int hPad = 10;
      const int vPad = 10;

      Rectangle backgroundRectangle = new Rectangle((int)pagePosition.X - hPad,
                                                    (int)pagePosition.Y - vPad,
                                                    (int)pageSize.X + hPad * 2,
                                                    (int)pageSize.Y + vPad * 2);

      // fade the popup alpha during transitions.
      Color bgColor = new Color(ColorHelper.Blend(Color.DarkGray, Color.CornflowerBlue, .25f), TransitionAlpha);
      spriteBatch.Begin();

      // draw the background rectangle.
      PrimitiveGraphics graphics = new PrimitiveGraphics(spriteBatch);
      ControlDrawing.Draw3DRoundRectangle(graphics, backgroundRectangle, 4, 4, bgColor, false, true, true, false);

      // okay, we need to draw the current wizard page. It has to be stationed in the middle, and we'll need room for the
      // "next" and "exit" buttons. So, subtract the height of the buttons, plus the buffer in between the buttons from the
      // height of the backgroundRectangle. Then, we'll have the middle position of the text

      float spaceWidth = wizardFont.MeasureString(" ").X;
      float lineWidth = backgroundRectangle.Width - (WidthDelta * 2);
      string text = AllianceUtilities.Wrap(wizardPages[currentIndex], spaceWidth, lineWidth, wizardFont);
      float lineCount = text.Split(new string[] { "\r\n" }, StringSplitOptions.None).Length;

      float x = backgroundRectangle.X + WidthDelta;
      float y = backgroundRectangle.Y + ((backgroundRectangle.Height / 2f) - (lineCount * wizardFont.LineSpacing * .5f));

      Vector2 titlePosition = new Vector2(x, backgroundRectangle.Y + WidthDelta);
      spriteBatch.DrawString(ScreenManager.Font, "Welcome to Alliance", titlePosition, Color.Yellow);
      spriteBatch.DrawString(wizardFont, text, new Vector2(x, y), Color.White);
      spriteBatch.End();

      // position the buttons
      btnExit.X = backgroundRectangle.X + BtnMargin;
      btnExit.Y = backgroundRectangle.Bottom - (BtnMargin + btnExit.Height);
      btnNext.X = backgroundRectangle.Right - (BtnMargin + btnNext.Width);
      btnNext.Y = btnExit.Y;

      // update the back/fore colors
      btnExit.BackColor = btnNext.BackColor = new Color(BtnBackColor, TransitionAlpha);
      btnExit.ForeColor = btnNext.ForeColor = new Color(BtnForeColor, TransitionAlpha);

      // draw the gui
      mGui.Draw(gameTime);
    }
  }
}
