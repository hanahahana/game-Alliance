using System;

namespace Alliance
{
  public abstract class MenuScreen : GameScreen
  {
    const int EntryOffset = 8;
    const int TwoEntryOffset = EntryOffset * 2;
    GuiComponent mGui;

    public Vector2 MenuTitlePosition { get; set; }
    public float MenuTitleScale { get; set; }
    public string MenuTitle { get; set; }
    public Vector2 MenuEntryStartPosition { get; set; }

    public MenuScreen(string menuTitle)
    {
      TransitionOnTime = TimeSpan.FromSeconds(0.5);
      TransitionOffTime = TimeSpan.FromSeconds(0.5);

      MenuTitle = menuTitle;
      MenuTitleScale = 1.45f;
      MenuTitlePosition = new Vector2(426, 80);
      MenuEntryStartPosition = new Vector2(100, 150);
    }

    public void Add(string text, EventHandler onEntrySelected)
    {
      MenuEntry entry = new MenuEntry(text, this);
      entry.BackColor = new Color(Color.Beige, 110);
      entry.DisabledBackColor = ColorHelper.Blend(entry.BackColor, Color.Gray, .5f);
      entry.Enabled = true;
      entry.Font = ScreenManager.Font;
      entry.ForeColor = Color.White;
      entry.TextAlignment = DataAlignment.MiddleCenter;
      entry.Visible = true;
      entry.Click += onEntrySelected;
      mGui.Controls.Add(entry);
    }

    public override void LoadContent()
    {
      mGui = new GuiComponent(ScreenManager.Game);
      mGui.Initialize();
      mGui.LoadContent();
      base.LoadContent();
    }

    public override void HandleInput(InputState input)
    {
      if (input.MenuCancel)
      {
        OnCancel();
      }
    }

    protected virtual void OnCancel()
    {
      ExitScreen();
    }

    protected void OnCancel(object sender, EventArgs e)
    {
      OnCancel();
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
      if (IsActive)
      {
        mGui.Update(gameTime);
      }
      base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
    }

    public override void Draw(GameTime gameTime)
    {
      // update the bounds
      SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
      SpriteFont font = ScreenManager.Font;
      Vector2 position = MenuEntryStartPosition;

      // Make the menu slide into place during transitions, using a
      // power curve to make things look more interesting (this makes
      // the movement slow down as it nears the end).
      float transitionOffset = (float)Math.Pow(TransitionPosition, 2);
      if (ScreenState == ScreenState.TransitionOn)
        position.X -= transitionOffset * 256;
      else
        position.X += transitionOffset * 512;

      spriteBatch.Begin();

      // Draw each menu entry in turn.
      for (int i = 0; i < mGui.Controls.Count; i++)
      {
        MenuEntry menuEntry = mGui.Controls[i] as MenuEntry;
        SizeF size = font.MeasureString(menuEntry.Text);

        menuEntry.X = (int)(position.X - EntryOffset);
        menuEntry.Y = (int)(position.Y - EntryOffset);
        menuEntry.Width = (int)(size.Width + TwoEntryOffset);
        menuEntry.Height = (int)(size.Height + TwoEntryOffset);

        position.Y += menuEntry.Height;
      }

      mGui.Draw(gameTime);

      // Draw the menu title.
      Vector2 titleOrigin = font.MeasureString(MenuTitle) / 2f;
      Color titleColor = new Color(Color.Yellow, TransitionAlpha);
      Vector2 titlePosition = MenuTitlePosition;

      titlePosition.Y -= transitionOffset * 100;
      spriteBatch.DrawString(font, MenuTitle, titlePosition, titleColor, 0,
                             titleOrigin, MenuTitleScale, SpriteEffects.None, 0);
      spriteBatch.End();
    }
  }
}
