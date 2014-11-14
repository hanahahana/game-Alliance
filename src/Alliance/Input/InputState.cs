using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Input;

namespace Alliance
{
  public sealed class InputState
  {
    private readonly KeyboardManager keyboard;
    private readonly MouseManager mouse;

    public KeyboardState CurrentKeyboardState;
    public KeyboardState LastKeyboardState;

    public MouseState CurrentMouseState;
    public MouseState LastMouseState;

    public Vector2 CursorPosition { get; private set; }
    public bool SelectPressed { get; private set; }
    public bool SelectReleased { get; private set; }

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

    public InputState(Game game)
    {
      keyboard = new KeyboardManager(game);
      mouse = new MouseManager(game);
    }

    public void Update(GameTime gameTime)
    {
      LastKeyboardState = CurrentKeyboardState;
      LastMouseState = CurrentMouseState;

      CurrentKeyboardState = keyboard.GetState();
      CurrentMouseState = mouse.GetState();

      CursorPosition = new Vector2(CurrentMouseState.X, CurrentMouseState.Y);
      SelectPressed = (CurrentMouseState.LeftButton.Pressed);
      SelectReleased = (CurrentMouseState.LeftButton.Released);
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
      if (!current.Pressed)
        return false;

      var previous = getState(LastMouseState);
      return previous.Released;
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
