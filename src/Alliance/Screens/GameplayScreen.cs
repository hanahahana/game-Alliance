using System;

namespace Alliance
{
  /// <summary>
  /// This screen implements the actual game logic. It is just a
  /// placeholder to get the idea across: you'll probably want to
  /// put some more interesting gameplay in here!
  /// </summary>
  public class GameplayScreen : GameScreen
  {
    DrawableComponent[] components;

    MessageComponent messages;
    GridComponent grid;
    HudComponent player;

    /// <summary>
    /// Constructor.
    /// </summary>
    public GameplayScreen()
    {
      TransitionOnTime = TimeSpan.FromSeconds(0.5);
      TransitionOffTime = TimeSpan.FromSeconds(0.5);
    }

    /// <summary>
    /// Load graphics content for the game.
    /// </summary>
    public override void LoadContent()
    {
      grid = new GridComponent(ScreenManager.Game);
      messages = MessageComponent.CreateInstance(ScreenManager.Game);
      player = HudComponent.CreateInstance(ScreenManager.Game);

      components = new DrawableComponent[] { grid, messages, player, };
      Array.ForEach(components, component => component.Initialize());
      Array.ForEach(components, component => component.LoadContent());

      // once the load has finished, we use ResetElapsedTime to tell the game's
      // timing mechanism that we have just finished a very long frame, and that
      // it should not try to catch up.
      ScreenManager.Game.ResetElapsedTime();

      // set the current grid so we can save/load
      AllianceGame.CurrentGrid = grid;
    }

    /// <summary>
    /// Updates the state of the game. This method checks the GameScreen.IsActive
    /// property, so the game will stop updating when the pause menu is active,
    /// or if you tab away to a different application.
    /// </summary>
    public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
    {
      base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
      if (IsActive || IsCurrentInactiveScreen)
      {
        // update the components
        Array.ForEach(components, component => component.Update(gameTime));
        
        // assert that the game is still going
        AssertGameStillGoing();
      }
    }

    private void AssertGameStillGoing()
    {
      if (grid.GameOver)
      {
        // this happens when the player loses all of their life
        const string message = "Game Over! Do you want to load a game?";
        MessageBoxScreen confirmLoadSelectedMessageBox = new MessageBoxScreen(message, "Yes", "No");
        confirmLoadSelectedMessageBox.Accepted += ConfirmLoadGameMessageBoxAccepted;
        confirmLoadSelectedMessageBox.Cancelled += ConfirmExitGameMessageBoxAccepted;
        ScreenManager.AddScreen(confirmLoadSelectedMessageBox);
      }

      if (grid.GameWon)
      {
        // this happens when the player defeats the maximum level invader
        const string message = "You won! Nothing here yet but GOOD JOB! This is the best I got.";
        MessageBoxScreen youwon = new MessageBoxScreen(message, "YOU", "WON");
        youwon.Accepted += ConfirmExitGameMessageBoxAccepted;
        youwon.Cancelled += ConfirmExitGameMessageBoxAccepted;
        ScreenManager.AddScreen(youwon);
      }
    }

    private void ConfirmLoadGameMessageBoxAccepted(object sender, EventArgs e)
    {
      LoadingScreen.Load(ScreenManager, false,
        new GameplayScreen(),
        new SaveDataScreen(SaveDataScreenMode.Load));
    }

    private void ConfirmExitGameMessageBoxAccepted(object sender, EventArgs e)
    {
      LoadingScreen.Load(ScreenManager, false, new MainMenuScreen());
    }

    /// <summary>
    /// Lets the game respond to player input. Unlike the Update method,
    /// this will only be called when the gameplay screen is active.
    /// </summary>
    public override void HandleInput(InputState input)
    {
      if (input == null)
        throw new ArgumentNullException("input");

      if (input.PauseGame)
      {
        // If they pressed pause, bring up the pause menu screen.
        ScreenManager.AddScreen(new PauseMenuScreen());
      }
      else
      {
        // set the fill mode on the grid
        grid.FillMode = input.IsKeyDown(Keys.P) ? GridFillMode.Polygons : GridFillMode.Solid;
      }
    }

    /// <summary>
    /// Draws the gameplay screen.
    /// </summary>
    public override void Draw(GameTime gameTime)
    {
      // if we're transitioning on, then just don't draw
      if (ScreenState == ScreenState.TransitionOn)
        return;

      // draw everything
      ScreenManager.GraphicsDevice.Clear(ClearOptions.Target, Color.CornflowerBlue, 0, 0);
      Array.ForEach(components, component => component.Draw(gameTime));
    }
  }
}
