using System;
using Alliance.Enums;
using Alliance.Input;
using Microsoft.Xna.Framework;

namespace Alliance.Screens
{
  /// <summary>
  /// The main menu screen is the first thing displayed when the game starts up.
  /// </summary>
  public class MainMenuScreen : MenuScreen
  {
    const double SecondsToWaitToShowStory = 30.0;
    double totalElapsedTime = 0.0;

    /// <summary>
    /// Constructor fills in the menu contents.
    /// </summary>
    public MainMenuScreen()
      : base("Alliance Main Menu")
    {
      
    }

    public override void LoadContent()
    {
      // load the GUI
      base.LoadContent();

      // add the entries AFTER the base had loaded
      Add("New Game", NewGameMenuEntrySelected);
      Add("Load Game", LoadGameMenuEntrySelected);
      Add("Options", OptionsMenuEntrySelected);
      Add("Exit", OnCancel);
    }

    void NewGameMenuEntrySelected(object sender, EventArgs e)
    {
      LoadingScreen.Load(ScreenManager, true, 
        new GameplayScreen(),
        new WizardScreen(),
        new StoryScreen());
    }

    void LoadGameMenuEntrySelected(object sender, EventArgs e)
    {
      LoadingScreen.Load(ScreenManager, false,
        new GameplayScreen(),
        new SaveDataScreen(SaveDataScreenMode.Load));
    }

    void OptionsMenuEntrySelected(object sender, EventArgs e)
    {
      ScreenManager.AddScreen(new OptionsMenuScreen());
    }

    /// <summary>
    /// When the user cancels the main menu, ask if they want to exit the sample.
    /// </summary>
    protected override void OnCancel()
    {
      const string message = "Are you sure you want to exit the game?";
      MessageBoxScreen confirmExitMessageBox = new MessageBoxScreen(message);
      confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;
      ScreenManager.AddScreen(confirmExitMessageBox);
    }

    void ConfirmExitMessageBoxAccepted(object sender, EventArgs e)
    {
      ScreenManager.Game.Exit();
    }

    public override void HandleInput(InputState input)
    {
      if (input == null)
        throw new ArgumentNullException("input");

      if (input.Any)
        totalElapsedTime = 0;

      base.HandleInput(input);
    }

    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
      base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
      if (!coveredByOtherScreen)
      {
        totalElapsedTime += gameTime.ElapsedGameTime.TotalSeconds;
        if (totalElapsedTime >= SecondsToWaitToShowStory)
        {
          ScreenManager.AddScreen(new StoryScreen());
          totalElapsedTime -= SecondsToWaitToShowStory;
        }
      }
    }
  }
}
