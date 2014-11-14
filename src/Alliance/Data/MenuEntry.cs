using System;

namespace Alliance
{
  public class MenuEntry : Button
  {
    public MenuScreen MenuScreen { get; private set; }

    public MenuEntry(string text, MenuScreen menuScreen)
    {
      Text = text;
      TextAlignment = DataAlignment.MiddleCenter;
      MenuScreen = menuScreen;
    }

    protected override void Draw(GameTime gameTime)
    {
      // determine the colors
      Color back = (!Enabled ? DisabledBackColor : BackColor);
      Color fore = (!Enabled ? DisabledForeColor : ForeColor);

      // update the colors
      back = new Color(back, Math.Min(back.A, MenuScreen.TransitionAlpha));
      fore = new Color(fore, Math.Min(fore.A, MenuScreen.TransitionAlpha));

      // create a scale
      Vector2 textScale = new Vector2(Pressed ? .99f : 1f);

      // measure the text
      Vector2 textSize = fontEx.MeasureString(Text);

      // setup the text decoration
      TextDocoration decoration = TextDocoration.None;

      if (Hovered || Pressed)
      {
        // draw the button itself
        ControlDrawing.Draw3DRoundRectangle(Graphics, Bounds, 6, 6, back, !Pressed, true, true, Pressed);

        // set the decoration to "DropShadow"
        decoration = TextDocoration.DropShadow;
      }

      // draw the text of the button in the center
      ControlDrawing.DrawFormattedText(SpriteBatch, Font, Bounds, textSize, textScale, TextAlignment, Text, decoration, 6, fore);
    }
  }
}
