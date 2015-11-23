using System;
using Alliance.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLA.Utilities.Xna.Graphics;
using MLA.Utilities.Xna.Helpers;
using MLA.Xna.Gui;
using MLA.Xna.Gui.Controls;
using MLA.Xna.Gui.Drawing;

namespace Alliance.Screens
{
  /// <summary>
  /// A popup message box screen, used to display "are you sure?"
  /// confirmation messages.
  /// </summary>
  public class MessageBoxScreen : GameScreen
  {
    const int EntryOffset = 6;
    const int TwoEntryOffset = EntryOffset * 2;

    string mMessageText;
    string mAcceptText;
    string mCancelText;

    GuiComponent mGui;
    Button acceptButton;
    Button cancelButton;

    public event EventHandler<EventArgs> Accepted;
    public event EventHandler<EventArgs> Cancelled;

    /// <summary>
    /// Constructor creates a message box screen
    /// </summary>
    public MessageBoxScreen(string message)
      : this(message, "OK", "Cancel")
    {

    }

    public MessageBoxScreen(string message, string acceptText, string cancelText)
    {
      IsPopup = true;
      TransitionOnTime = TimeSpan.FromSeconds(0.2);
      TransitionOffTime = TimeSpan.FromSeconds(0.2);

      mMessageText = message;
      mAcceptText = acceptText;
      mCancelText = cancelText;
    }

    /// <summary>
    /// Loads graphics content for this screen. This uses the shared ContentManager
    /// provided by the Game class, so the content will remain loaded forever.
    /// Whenever a subsequent MessageBoxScreen tries to load this same content,
    /// it will just get back another reference to the already loaded data.
    /// </summary>
    public override void LoadContent()
    {
      mGui = new GuiComponent(ScreenManager.Game);
      mGui.Initialize();
      mGui.LoadContent();

      acceptButton = new Button();
      acceptButton.BackColor = Color.Silver;
      acceptButton.ForeColor = Color.Black;
      acceptButton.Text = mAcceptText;
      acceptButton.Width = 90;
      acceptButton.Height = 30;
      acceptButton.Click += (sender, args) => { FireAccepted(); };

      cancelButton = new Button();
      cancelButton.BackColor = Color.Silver;
      cancelButton.ForeColor = Color.Black;
      cancelButton.Text = mCancelText;
      cancelButton.Width = 90;
      cancelButton.Height = 30;
      cancelButton.Click += (sender, args) => { FireCanceled(); };

      acceptButton.TextAlignment = cancelButton.TextAlignment = DataAlignment.MiddleCenter;
      acceptButton.Font = cancelButton.Font = AllianceGame.Fonts["Verdana"];

      mGui.Controls.Add(acceptButton);
      mGui.Controls.Add(cancelButton);
    }

    private void FireCanceled()
    {
      // Raise the cancelled event, then exit the message box.
      if (Cancelled != null)
        Cancelled(this, EventArgs.Empty);

      ExitScreen();
    }

    private void FireAccepted()
    {
      // Raise the accepted event, then exit the message box.
      if (Accepted != null)
        Accepted(this, EventArgs.Empty);

      ExitScreen();
    }

    /// <summary>
    /// Responds to user input, accepting or cancelling the message box.
    /// </summary>
    public override void HandleInput(InputState input)
    {
      if (input.MenuSelect)
      {
        FireAccepted();
      }
      else if (input.MenuCancel)
      {
        FireCanceled();
      }
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
      if (IsActive)
      {
        mGui.Update(gameTime);
      }
      base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
    }

    /// <summary>
    /// Draws the message box.
    /// </summary>
    public override void Draw(GameTime gameTime)
    {
      SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
      SpriteFont font = ScreenManager.Font;

      // Darken down any other screens that were drawn beneath the popup.
      ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

      // Center the message text in the viewport.
      Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
      Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
      Vector2 textSize = font.MeasureString(mMessageText);
      Vector2 textPosition = (viewportSize - textSize) / 2;

      // The background includes a border somewhat larger than the text itself.
      const int hPad = 32;
      const int vPad = 16;
      const int btnMargin = 6;

      Rectangle backgroundRectangle = new Rectangle((int)textPosition.X - hPad,
                                                    (int)textPosition.Y - vPad,
                                                    (int)textSize.X + hPad * 2,
                                                    (int)textSize.Y + vPad * 4);

      // Fade the popup alpha during transitions.
      Color bgColor = new Color(ColorHelper.Blend(Color.DarkGray, Color.CornflowerBlue, .25f), TransitionAlpha);
      Color fgColor = new Color(Color.Black, TransitionAlpha);

      spriteBatch.Begin();

      // Draw the background rectangle.
      PrimitiveGraphics graphics = new PrimitiveGraphics(spriteBatch);
      ControlDrawing.Draw3DRoundRectangle(graphics, backgroundRectangle, 4, 4, bgColor, true, true, true, false);

      // Draw the message box text.
      spriteBatch.DrawString(font, mMessageText, textPosition, fgColor);
      spriteBatch.End();

      // position the buttons
      cancelButton.X = backgroundRectangle.Right - (btnMargin + cancelButton.Width);
      cancelButton.Y = backgroundRectangle.Bottom - (btnMargin + cancelButton.Height);
      acceptButton.X = cancelButton.X - (btnMargin + acceptButton.Width);
      acceptButton.Y = cancelButton.Y;

      // update the back/fore colors
      acceptButton.BackColor = new Color(Color.Silver, TransitionAlpha);
      acceptButton.ForeColor = new Color(Color.Black, TransitionAlpha);
      cancelButton.BackColor = new Color(Color.Silver, TransitionAlpha);
      cancelButton.ForeColor = new Color(Color.Black, TransitionAlpha);

      // draw the GUI
      mGui.Draw(gameTime);
    }
  }
}
