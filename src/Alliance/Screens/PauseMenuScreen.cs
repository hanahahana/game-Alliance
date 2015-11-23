using System;
using Alliance.Enums;
using Microsoft.Xna.Framework;

namespace Alliance.Screens
{
  /// <summary>
  /// The pause menu comes up over the top of the game,
  /// giving the player options to resume or quit.
  /// </summary>
  public class PauseMenuScreen : MenuScreen
  {
    /// <summary>
    /// Constructor.
    /// </summary>
    public PauseMenuScreen()
      : base("Paused")
    {
      // Flag that there is no need for the game to transition
      // off when the pause menu is on top of it.
      IsPopup = true;
    }

    public override void LoadContent()
    {
      // load the GUI
      base.LoadContent();

      // add the entries AFTER the base had loaded
      Add("Resume Game", OnCancel);
      Add("Save Game", SaveGameMenuEntrySelected);
      Add("Quit Game", QuitGameMenuEntrySelected);
    }

    void SaveGameMenuEntrySelected(object sender, EventArgs e)
    {
      // show the save game screen
      ScreenManager.AddScreen(new SaveDataScreen(SaveDataScreenMode.Save));
    }

    void QuitGameMenuEntrySelected(object sender, EventArgs e)
    {
      const string message = "Are you sure you want to quit this game and return to the main menu?";
      MessageBoxScreen confirmQuitMessageBox = new MessageBoxScreen(message);
      confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;
      ScreenManager.AddScreen(confirmQuitMessageBox);
    }

    void ConfirmQuitMessageBoxAccepted(object sender, EventArgs e)
    {
      LoadingScreen.Load(ScreenManager, false, new MainMenuScreen());
    }

    public override void Draw(GameTime gameTime)
    {
      ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 4 / 5);
      base.Draw(gameTime);
    }
  }
}
