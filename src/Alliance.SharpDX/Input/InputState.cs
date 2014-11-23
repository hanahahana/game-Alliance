using System;
using System.Collections.Generic;
using System.Linq;
using GraphicsSystem;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;

namespace Alliance
{
  public sealed class InputState : IAllianceInputState, IInputProvider
  {
    private readonly KeyboardManager keyboard;
    private readonly MouseManager mouse;

    public KeyboardState CurrentKeyboardState;
    public KeyboardState LastKeyboardState;

    public MouseState CurrentMouseState;
    public MouseState LastMouseState;

    public GsVector CursorPosition { get; private set; }
    public bool SelectPressed { get; private set; }
    public bool SelectReleased { get { return !SelectPressed; } }
    public Game Game { get; private set; }

    public bool MenuUp
    {
      get { return IsNewKeyPress(Keys.Up); }
    }

    public bool MenuDown
    {
      get { return IsNewKeyPress(Keys.Down); }
    }

    public bool MenuSelect
    {
      get { return IsNewKeyPress(Keys.Enter); }
    }

    public bool MenuCancel
    {
      get { return IsNewKeyPress(Keys.Escape); }
    }

    public bool PauseGame
    {
      get { return IsNewKeyPress(Keys.Space); }
    }

    public bool QuitStory
    {
      get { return IsNewKeyPress(Keys.Escape) || IsNewKeyPress(Keys.Enter); }
    }

    public bool Any
    {
      get { return IsAnyKeyPress(); }
    }

    public bool SelectObject
    {
      get { return IsNewMouseClick(x => x.LeftButton); }
    }

    public bool SellRequested
    {
      get { return IsNewKeyPress(Keys.S); }
    }

    public bool UpgradeRequested
    {
      get { return IsNewKeyPress(Keys.U); }
    }

    public bool ClearSelections
    {
      get { return MenuCancel; }
    }

    public bool EctoplasTransmaterPort
    {
      get { return IsKeyDown(Keys.N); }
    }

    IAllianceInputState IInputProvider.GetState()
    {
      return this;
    }

    public InputState(Game game)
    {
      Game = game;
      keyboard = new KeyboardManager(game);
      mouse = new MouseManager(game);
      InputProvider.Register(this);
    }

    public void Update(GameTime gameTime)
    {
      LastKeyboardState = CurrentKeyboardState;
      LastMouseState = CurrentMouseState;

      CurrentKeyboardState = keyboard.GetState();
      CurrentMouseState = mouse.GetState();

      var device = Game.GraphicsDevice;
      var pt = new GsVector(CurrentMouseState.X, CurrentMouseState.Y);
      pt *= new GsVector(device.Viewport.Width, device.Viewport.Height);

      CursorPosition = pt;
      SelectPressed = (CurrentMouseState.LeftButton.Down);
    }

    public bool IsAnyKeyPress()
    {
      List<Keys> keys = new List<Keys>();
      CurrentKeyboardState.GetDownKeys(keys);
      return keys.Count > 0;
    }

    public bool IsNewMouseClick(Func<MouseState, ButtonState> getState)
    {
      var current = getState(CurrentMouseState);
      if (!current.Down)
        return false;

      var previous = getState(LastMouseState);
      return !previous.Down;
    }

    public bool IsNewKeyPress(Keys key)
    {
      return CurrentKeyboardState.IsKeyDown(key) && !LastKeyboardState.IsKeyDown(key);
    }

    public bool IsKeyDown(Keys key)
    {
      return CurrentKeyboardState.IsKeyDown(key);
    }
  }
}
