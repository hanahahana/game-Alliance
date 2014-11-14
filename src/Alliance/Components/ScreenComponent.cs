using System.Collections.Generic;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Alliance
{
  /// <summary>
  /// The screen manager is a component which manages one or more GameScreen
  /// instances. It maintains a stack of screens, calls their Update and Draw
  /// methods at the appropriate times, and automatically routes input to the
  /// topmost active screen.
  /// </summary>
  public class ScreenComponent : GameSystem
  {
    List<GameScreen> screens = new List<GameScreen>();
    List<GameScreen> screensToUpdate = new List<GameScreen>();

    InputState input;
    SpriteBatch spriteBatch;
    SpriteFont font;
    Texture2D blankTexture;
    bool isInitialized;

    /// <summary>
    /// A default SpriteBatch shared by all the screens. This saves
    /// each screen having to bother creating their own local instance.
    /// </summary>
    public SpriteBatch SpriteBatch
    {
      get { return spriteBatch; }
    }

    /// <summary>
    /// A default font shared by all the screens. This saves
    /// each screen having to bother loading their own local copy.
    /// </summary>
    public SpriteFont Font
    {
      get { return font; }
    }

    /// <summary>
    /// The screen that is currently being shown. This ignores the screen's
    /// state and just returns the screen on top of all the screens, or null
    /// if there are no screens.
    /// </summary>
    public GameScreen CurrentScreen
    {
      get { return screens.Count > 0 ? screens[screens.Count - 1] : null; }
    }

    /// <summary>
    /// Constructs a new screen manager component.
    /// </summary>
    public ScreenComponent(Game game)
      : base(game)
    {
      input = new InputState(game);
    }

    /// <summary>
    /// Initializes the screen manager component.
    /// </summary>
    public override void Initialize()
    {
      base.Initialize();
      isInitialized = true;
    }

    /// <summary>
    /// Load your graphics content.
    /// </summary>
    protected override void LoadContent()
    {
      spriteBatch = ToDisposeContent(new SpriteBatch(GraphicsDevice));
      font = Program.Resources.Fonts["ComicSans"];
      blankTexture = Program.Resources.Textures["blank"];

      // Tell each of the screens to load their content.
      foreach (GameScreen screen in screens)
      {
        screen.LoadContent();
      }
    }

    /// <summary>
    /// Allows each screen to run logic.
    /// </summary>
    public override void Update(GameTime gameTime)
    {
      // Read the keyboard and gamepad.
      input.Update(gameTime);

      // Make a copy of the master screen list, to avoid confusion if
      // the process of updating one screen adds or removes others.
      screensToUpdate.Clear();

      foreach (GameScreen screen in screens)
        screensToUpdate.Add(screen);

      bool otherScreenHasFocus = !Game.IsActive;
      bool coveredByOtherScreen = false;

      // Loop as long as there are screens waiting to be updated.
      while (screensToUpdate.Count > 0)
      {
        // Pop the topmost screen off the waiting list.
        GameScreen screen = screensToUpdate[screensToUpdate.Count - 1];

        screensToUpdate.RemoveAt(screensToUpdate.Count - 1);

        // Update the screen.
        screen.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

        if (screen.ScreenState == ScreenState.TransitionOn || screen.ScreenState == ScreenState.Active)
        {
          // If this is the first active screen we came across,
          // give it a chance to handle input.
          if (!otherScreenHasFocus)
          {
            screen.HandleInput(input);
            otherScreenHasFocus = true;
          }

          // If this is an active non-popup, inform any subsequent
          // screens that they are covered by it.
          if (!screen.IsPopup)
            coveredByOtherScreen = true;
        }
      }
    }

    /// <summary>
    /// Tells each screen to draw itself.
    /// </summary>
    public override void Draw(GameTime gameTime)
    {
      foreach (GameScreen screen in screens)
      {
        if (screen.ScreenState == ScreenState.Hidden)
          continue;

        screen.Draw(gameTime);
      }
    }

    /// <summary>
    /// Adds a new screen to the screen manager.
    /// </summary>
    public void AddScreen(GameScreen screen)
    {
      screen.ScreenManager = this;
      screen.IsExiting = false;

      // If we have a graphics device, tell the screen to load content.
      if (isInitialized)
      {
        screen.LoadContent();
      }

      screens.Add(screen);
    }


    /// <summary>
    /// Removes a screen from the screen manager. You should normally
    /// use GameScreen.ExitScreen instead of calling this directly, so
    /// the screen can gradually transition off rather than just being
    /// instantly removed.
    /// </summary>
    public void RemoveScreen(GameScreen screen)
    {
      // If we have a graphics device, tell the screen to unload content.
      screens.Remove(screen);
      screensToUpdate.Remove(screen);
    }

    /// <summary>
    /// Expose an array holding all the screens. We return a copy rather
    /// than the real master list, because screens should only ever be added
    /// or removed using the AddScreen and RemoveScreen methods.
    /// </summary>
    public GameScreen[] GetScreens()
    {
      return screens.ToArray();
    }

    /// <summary>
    /// Helper draws a translucent black fullscreen sprite, used for fading
    /// screens in and out, and for darkening the background behind popups.
    /// </summary>
    public void FadeBackBufferToBlack(int alpha)
    {
      var viewport = GraphicsDevice.Viewport;
      spriteBatch.Begin();
      spriteBatch.Draw(blankTexture,
        new Rectangle(0, 0, (int)viewport.Width, (int)viewport.Height),
        new Color(0, 0, 0, alpha));
      spriteBatch.End();
    }
  }
}
