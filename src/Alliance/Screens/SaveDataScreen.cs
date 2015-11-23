using System;
using System.Threading;
using Alliance.Data;
using Alliance.Enums;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MLA.Utilities.Helpers;
using MLA.Utilities.Xna.Helpers;
using MLA.Xna.Gui;
using MLA.Xna.Gui.Controls;
using Alliance.Input;

namespace Alliance.Screens
{
  public class SaveDataScreen : GameScreen
  {
    enum SaveDataScreenState { Fading, Retrieving, Idle, };

    const float SecondsUntilScreenDoneFading = 1f;
    const string EmptyFormatString = "-- {0} Empty --";

    static readonly Color StartColor = Color.TransparentBlack;
    static readonly Color EndColor = Color.CornflowerBlue;

    float totalElapsedSeconds = 0f;
    SaveDataScreenState state;
    Texture2D blankTexture;
    GuiComponent mGui;
    Thread loadingThread;
    ListBox lstCurrentSaveData;
    Button btnAccept;
    Button btnCancel;
    Button btnClear;

    public SaveDataScreenMode Mode { get; set; }

    private bool ShouldClear
    {
      get
      {
        return state != SaveDataScreenState.Fading;
      }
    }

    public SaveDataScreen(SaveDataScreenMode mode)
    {
      IsPopup = true;
      Mode = mode;
      state = (Mode == SaveDataScreenMode.Save ? SaveDataScreenState.Fading : SaveDataScreenState.Retrieving);
    }

    public override void LoadContent()
    {
      mGui = new GuiComponent(ScreenManager.Game);
      mGui.Initialize();

      lstCurrentSaveData = new ListBox();
      lstCurrentSaveData.BackColor = new Color(Color.Silver, 100);
      lstCurrentSaveData.BorderColor = Color.Black;
      lstCurrentSaveData.BorderThickness = 1;
      lstCurrentSaveData.DisplayStyle = ListBoxDisplayStyle.Text;
      lstCurrentSaveData.Font = AllianceGame.Fonts["ComicSans"];
      lstCurrentSaveData.ForeColor = Color.DarkBlue;
      lstCurrentSaveData.ItemHeight = 80;
      lstCurrentSaveData.SelectionColor = new Color(Color.SkyBlue, 120);
      lstCurrentSaveData.X = 20;
      lstCurrentSaveData.Y = 90;
      lstCurrentSaveData.Width = ScreenManager.GraphicsDevice.Viewport.Width - (lstCurrentSaveData.Y * 2);
      lstCurrentSaveData.Height = ScreenManager.GraphicsDevice.Viewport.Height - (lstCurrentSaveData.Y * 2);
      lstCurrentSaveData.ScrollAreaWidth = 50;

      const int btnOffset = 10;
      int btnX = lstCurrentSaveData.Bounds.Right + btnOffset;
      int btnWidth = ScreenManager.GraphicsDevice.Viewport.Width - (btnX + btnOffset);
      int btnHeight = 60;

      btnAccept = new Button();
      btnAccept.BackColor = Color.Silver;
      btnAccept.Font = AllianceGame.Fonts["ComicSans"];
      btnAccept.ForeColor = Color.Black;
      btnAccept.Text = (Mode == SaveDataScreenMode.Save) ? "Save" : "Load";
      btnAccept.TextAlignment = DataAlignment.MiddleCenter;
      btnAccept.X = btnX;
      btnAccept.Y = lstCurrentSaveData.Y;
      btnAccept.Width = btnWidth;
      btnAccept.Height = btnHeight;
      btnAccept.Click += new EventHandler(AcceptOnSelectedSlotRequest);

      btnClear = new Button();
      btnClear.BackColor = Color.Silver;
      btnClear.Font = AllianceGame.Fonts["ComicSans"];
      btnClear.ForeColor = Color.Black;
      btnClear.Text = "Clear";
      btnClear.TextAlignment = DataAlignment.MiddleCenter;
      btnClear.X = btnX;
      btnClear.Y = btnAccept.Bounds.Bottom + btnOffset;
      btnClear.Width = btnWidth;
      btnClear.Height = btnHeight;
      btnClear.Click += new EventHandler(ClearSelectedSave);

      btnCancel = new Button();
      btnCancel.BackColor = Color.Silver;
      btnCancel.Font = AllianceGame.Fonts["ComicSans"];
      btnCancel.ForeColor = Color.Black;
      btnCancel.Text = "Cancel";
      btnCancel.TextAlignment = DataAlignment.MiddleCenter;
      btnCancel.X = btnX;
      btnCancel.Y = btnClear.Bounds.Bottom + btnOffset;
      btnCancel.Width = btnWidth;
      btnCancel.Height = btnHeight;
      btnCancel.Click += new EventHandler(CancelOperationRequest);

      for (int i = 0; i < 10; ++i)
        lstCurrentSaveData.Items.Add(new ListBoxItem(string.Format(EmptyFormatString, i + 1)));

      mGui.Controls.Add(lstCurrentSaveData);
      mGui.Controls.Add(btnAccept);
      mGui.Controls.Add(btnClear);
      mGui.Controls.Add(btnCancel);

      blankTexture = AllianceGame.Textures["blank"];
      mGui.LoadContent();
      base.LoadContent();
    }

    private void ClearSelectedSave(object sender, EventArgs e)
    {
      const string message = "Are you sure you want to clear the selected save?";
      MessageBoxScreen confirmClearSelectedMessageBox = new MessageBoxScreen(message);
      confirmClearSelectedMessageBox.Accepted += ConfirmClearSelectedMessageBoxAccepted;
      ScreenManager.AddScreen(confirmClearSelectedMessageBox);
    }

    private void ConfirmClearSelectedMessageBoxAccepted(object sender, EventArgs e)
    {
      int idx = lstCurrentSaveData.SelectedIndex;
      SaveFile.ClearSaveData(idx);
      lstCurrentSaveData.Items[idx].Value = string.Format(EmptyFormatString, idx + 1);
    }

    private void AcceptOnSelectedSlotRequest(object sender, EventArgs e)
    {
      if (Mode == SaveDataScreenMode.Save)
      {
        if (lstCurrentSaveData.Items[lstCurrentSaveData.SelectedIndex].Value is SaveData)
        {
          const string message = "Do you want to overwrite the existing save?";
          MessageBoxScreen confirmOverwriteMessageBox = new MessageBoxScreen(message);
          confirmOverwriteMessageBox.Accepted += ConfirmSaveMessageBoxAccepted;
          ScreenManager.AddScreen(confirmOverwriteMessageBox);
        }
        else
        {
          ConfirmSaveMessageBoxAccepted(sender, e);
        }
      }
      else
      {
        SaveFile.LoadSaveData(lstCurrentSaveData.SelectedIndex);
        ExitScreen();
      }
    }

    private void ConfirmSaveMessageBoxAccepted(object sender, EventArgs e)
    {
      SaveFile.Save(lstCurrentSaveData.SelectedIndex);
      ExitScreen();
    }

    private void CancelOperationRequest(object sender, EventArgs e)
    {
      string message = string.Format("Are you sure you want to cancel {0}?", (Mode == SaveDataScreenMode.Save ? "saving" : "loading"));
      MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);
      confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;
      ScreenManager.AddScreen(confirmExitMessageBox);
    }

    private void ConfirmExitMessageBoxAccepted(object sender, EventArgs e)
    {
      if (Mode == SaveDataScreenMode.Save)
      {
        // go to the screen before save
        ExitScreen();
      }
      else
      {
        // return to the main menu
        LoadingScreen.Load(ScreenManager, false, new MainMenuScreen());
      }
    }

    public override void HandleInput(InputState input)
    {
      if (input.MenuCancel)
      {
        CancelOperationRequest(this, EventArgs.Empty);
      }
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
      switch (state)
      {
        case SaveDataScreenState.Fading:
          {
            totalElapsedSeconds += (float)(gameTime.ElapsedGameTime.TotalSeconds);
            totalElapsedSeconds = Math.Min(totalElapsedSeconds, SecondsUntilScreenDoneFading);
            break;
          }
        case SaveDataScreenState.Retrieving:
          {
            if (loadingThread == null)
            {
              loadingThread = new Thread(new ThreadStart(LoadCurrentSaves));
              loadingThread.IsBackground = true;
              loadingThread.Start();
            }
            else if (!loadingThread.IsAlive)
            {
              loadingThread = null;
              state = SaveDataScreenState.Idle;
            }
            break;
          }
        case SaveDataScreenState.Idle:
          {
            btnClear.Enabled = lstCurrentSaveData.SelectedIndex > -1;
            btnAccept.Enabled = lstCurrentSaveData.SelectedIndex > -1;

            if (Mode == SaveDataScreenMode.Load && lstCurrentSaveData.SelectedIndex > -1)
            {
              ListBoxItem item = lstCurrentSaveData.Items[lstCurrentSaveData.SelectedIndex];
              btnClear.Enabled = item.Value is SaveData;
              btnAccept.Enabled = item.Value is SaveData;
            }
            break;
          }
      }

      if (IsActive)
      {
        mGui.Update(gameTime);
      }
      base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
    }

    private void LoadCurrentSaves()
    {
      SaveFile.Load();
      foreach (SaveData data in SaveFile.Saves)
      {
        lstCurrentSaveData.Items[data.Index].Value = data;
      }
    }

    public override void Draw(GameTime gameTime)
    {
      // clear the screen
      if (ShouldClear)
        ScreenManager.GraphicsDevice.Clear(EndColor);

      // check the state and draw accordingly
      switch (state)
      {
        case SaveDataScreenState.Fading:
          {
            DrawFadingScreen(gameTime);
            break;
          }
        case SaveDataScreenState.Retrieving:
          {
            DrawLoadingCurrentSaves(gameTime);
            break;
          }
        case SaveDataScreenState.Idle:
          {
            DrawCurrentSaves(gameTime);
            break;
          }
      }


    }

    private void DrawCurrentSaves(GameTime gameTime)
    {
      mGui.Draw(gameTime);

      // determine the position 
      Vector2 pos = Vector2.One * 30;

      // get the sprite batch
      SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
      spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
      spriteBatch.DrawString(AllianceGame.Fonts["ComicSans"], "Current Save Files:", pos, Color.Yellow);
      spriteBatch.End();
    }

    private void DrawLoadingCurrentSaves(GameTime gameTime)
    {
      // get the sprite batch
      SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

      // determine the width (or count) for the dots
      int width = (int)((gameTime.TotalGameTime.TotalSeconds * 3.0) % 6);

      // determine the loading text
      string loadingText = string.Format("Loading Current Save Files{0}", string.Empty.PadLeft(width, '.'));

      // let the user know what's going on
      spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
      spriteBatch.DrawString(AllianceGame.Fonts["ComicSans"], loadingText, Vector2.One * 30, Color.Yellow);
      spriteBatch.End();
    }

    private void DrawFadingScreen(GameTime gameTime)
    {
      float mu = ArithmeticHelper.CalculatePercent(totalElapsedSeconds, 0, SecondsUntilScreenDoneFading);
      if (mu == 1.0f)
        state = SaveDataScreenState.Retrieving;

      SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
      Color color = ColorHelper.Blend(StartColor, EndColor, mu);

      Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
      Rectangle rect = new Rectangle(0, 0, viewport.Width, viewport.Height);

      spriteBatch.Begin();
      spriteBatch.Draw(blankTexture, rect, color);
      spriteBatch.End();
    }
  }
}
