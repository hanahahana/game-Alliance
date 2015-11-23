using System;
using Alliance.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alliance.Screens
{
  public class StoryScreen : GameScreen
  {
    const double LettersPerSecond = 25.0;
    const double SecondsPerLetter = 1.0 / LettersPerSecond;
    const float WidthDelta = 10f;
    const float HeightDelta = 2f;

    SpriteFont storyFont;
    string storyText;
    int letterCount = 1;
    double totalElapsedSeconds = 0;

    /// <summary>
    /// Constructor.
    /// </summary>
    public StoryScreen()
    {
      TransitionOnTime = TimeSpan.FromSeconds(0.5);
      TransitionOffTime = TimeSpan.FromSeconds(0.5);
    }

    /// <summary>
    /// Load graphics content for the game.
    /// </summary>
    public override void LoadContent()
    {
      storyFont = AllianceGame.Fonts["Verdana"];
      storyText = AllianceGame.Strings["story"][0].Replace("\r", string.Empty).Replace("\n", string.Empty);
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
      base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
      if (IsActive)
      {
        totalElapsedSeconds += gameTime.ElapsedGameTime.TotalSeconds;
        if (totalElapsedSeconds >= SecondsPerLetter)
        {
          totalElapsedSeconds -= SecondsPerLetter;

          // we want to get the index of the next character
          int idx = letterCount;

          // increment the letter count
          letterCount = Math.Min(letterCount + 1, storyText.Length);

          // if we have reached the end of the story
          if (letterCount.Equals(storyText.Length))
          {
            ExitScreen();
          }
          // if the index is valid AND the character is a period, then pause
          else if (idx < storyText.Length && (storyText[idx].Equals('.') || storyText[idx].Equals('!')))
          {
            // wait for a while
            totalElapsedSeconds = -(SecondsPerLetter * 120.0);
          }
        }
      }
    }

    public override void HandleInput(InputState input)
    {
      if (input == null)
        throw new ArgumentNullException("input");

      if (input.QuitStory)
        ExitScreen();
    }

    public override void Draw(GameTime gameTime)
    {
      // draw something
      SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
      spriteBatch.Begin();

      float spaceWidth = storyFont.MeasureString(" ").X;
      float lineWidth = ScreenManager.GraphicsDevice.Viewport.Width - (WidthDelta * 2);

      string text = AllianceUtilities.Wrap(storyText.Substring(0, letterCount), spaceWidth, lineWidth, storyFont);
      Vector2 position = new Vector2(WidthDelta, 1);

      // Make the menu slide into place during transitions, using a
      // power curve to make things look more interesting (this makes
      // the movement slow down as it nears the end).
      float transitionOffset = (float)Math.Pow(TransitionPosition, 2);
      if (ScreenState == ScreenState.TransitionOn)
        position.X -= transitionOffset * 256;
      else
        position.X += transitionOffset * 512;

      spriteBatch.DrawString(ScreenManager.Font, "Alliance", position, Color.Yellow);
      position.Y = 40;

      string[] lines = text.Split(new string[] { "\r\n" }, StringSplitOptions.None);
      foreach (string line in lines)
      {
        spriteBatch.DrawString(storyFont, line, position, Color.White);
        position.Y += (storyFont.MeasureString(line)).Y + HeightDelta;
      }

      spriteBatch.End();
    }
  }
}
