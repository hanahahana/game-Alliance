using System;
using System.Threading;
using Alliance.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLA.Utilities.Xna.Drawing;
using MLA.Xna.Gui;
using MLA.Xna.Gui.Controls;
using Alliance.Input;
using System.Collections.Generic;

namespace Alliance.Components
{
  /// <summary>
  /// 
  /// </summary>
  public class HudComponent : DrawableComponent
  {
    private static readonly TimeSpan TenSeconds = TimeSpan.FromSeconds(10.0);
    private static int creationRequests;
    private static HudComponent mComponentInstance;

    public static bool AllowSkipping { get; set; }
    static HudComponent()
    {
      AllowSkipping = true;
    }

    public static HudComponent CreateInstance(Game game)
    {
      if (Interlocked.Increment(ref creationRequests) == 1)
      {
        mComponentInstance = new HudComponent(game);
      }
      return mComponentInstance;
    }

    SpriteBatch spriteBatch;
    SpriteFont spriteFont;
    GuiComponent mGui;
    Button btnSkip;
    InputState input;

    private HudComponent(Game game)
      : base(game)
    {
      input = new InputState();
    }

    public override void LoadContent()
    {      
      spriteBatch = new SpriteBatch(GraphicsDevice);
      spriteFont = AllianceGame.Fonts["Georgia"];

      mGui = new GuiComponent(Game);
      mGui.Initialize();
      mGui.LoadContent();

      btnSkip = new Button();
      btnSkip.Text = "Skip";
      btnSkip.BackColor = Color.Silver;
      btnSkip.ForeColor = Color.Black;
      btnSkip.Width = 90;
      btnSkip.Height = 25;
      btnSkip.Font = AllianceGame.Fonts["BookmanOldStyle"];
      btnSkip.TextAlignment = DataAlignment.MiddleCenter;
      btnSkip.Click += new EventHandler(btnSkip_Click);

      mGui.Controls.Add(btnSkip);
    }

    private void btnSkip_Click(object sender, EventArgs e)
    {
      Ectoplastransmaterport();
    }

    private void Ectoplastransmaterport()
    {
      Player.TimeUntilInvadersArrive = TimeSpan.Zero;
    }

    public override void Update(GameTime gameTime)
    {
      btnSkip.Visible = AllowSkipping;
      if (AllowSkipping)
      {
        TimeSpan amount = TimeSpan.FromSeconds(gameTime.ElapsedGameTime.TotalSeconds);
        Player.TimeUntilInvadersArrive -= amount;

        if (Player.TimeUntilInvadersArrive < TimeSpan.Zero)
          Player.TimeUntilInvadersArrive = TimeSpan.Zero;

        if (input.EctoplasTransmaterPort)
          Ectoplastransmaterport();
      }

      input.Update(gameTime);
      mGui.Update(gameTime);
    }

    public override void Draw(GameTime gameTime)
    {
      const float Offset = 50f;
      TimeSpan span = Player.TimeUntilInvadersArrive;
      bool almostOutOfTime = span <= TenSeconds;
      double time = gameTime.TotalGameTime.TotalSeconds;
      float pulsate = (float)Math.Sin(time * 6) + 1;

      // determine the text to display
      List<string> text = new List<string>()
      {
        string.Format("Cash: {0:c2}", Player.Cash),
        string.Format("Civilians: {0}", Player.Civilians),
        string.Format("Level: {0}", Player.Level),
        string.Format("Experience: {0}", Player.Experience),
        string.Format("Time: {0}", new DateTime(span.Ticks).ToString("mm:ss")),
      };

      // if we don't allow skipping, then don't display the time!
      if (!AllowSkipping)
      {
        int count = text.RemoveAll(str => str.StartsWith("Time: "));
        if (count != 1)
          throw new Exception("The time HUD wasn't removed!");
      }

      // determine the position to place the text
      List<Vector2> pos = new List<Vector2> { new Vector2(5, 5) };
      for (int i = 0; i < text.Count; ++i)
      {
        // get the text that's being used for measurement
        string str = text[i];

        // get the last position
        Vector2 vec = pos[i];

        // measure the text
        Vector2 size = spriteFont.MeasureString(str);

        // add the size, plus offset, to the current position to determine the next position
        pos.Add(new Vector2(vec.X + size.X + Offset, vec.Y));
      }

      // determine the scale of the text
      List<float> scale = new List<float>()
      {
        1f, 1f, 1f, 1f, almostOutOfTime ? 1f + pulsate * 0.05f : 1f
      };

      // determine the color of the text
      List<Color> color = new List<Color>()
      {
        Color.White, Color.White, Color.White, Color.White, almostOutOfTime ? Color.Red : Color.White,
      };

      // draw the text
      spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
      for (int i = 0; i < text.Count; ++i)
      {
        spriteBatch.DrawString(spriteFont, text[i], pos[i] + Vector2.One, Color.Black, 0, Vector2.Zero, scale[i], SpriteEffects.None, 0);
        spriteBatch.DrawString(spriteFont, text[i], pos[i], color[i], 0, Vector2.Zero, scale[i], SpriteEffects.None, 0);
      }
      spriteBatch.End();

      // position the button
      btnSkip.X = (int)(GraphicsDevice.Viewport.Width - (btnSkip.Width + Offset * .1f));
      btnSkip.Y = (int)pos[0].Y;

      // draw the GUI
      mGui.Draw(gameTime);
    }
  }
}
