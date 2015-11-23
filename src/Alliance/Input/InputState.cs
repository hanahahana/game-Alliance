using System;
using System.Linq;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;


using MLA.Xna.Gui.Input;

namespace Alliance.Input
{
  /// <summary>
  /// 
  /// </summary>
  public sealed class InputState
  {
    public const int MaxInputs = 1;

    public readonly KeyboardState[] CurrentKeyboardStates;
    public readonly KeyboardState[] LastKeyboardStates;

    public readonly GamePadState[] CurrentGamePadStates;
    public readonly GamePadState[] LastGamePadStates;

    public readonly MouseState[] CurrentMouseStates;
    public readonly MouseState[] LastMouseStates;

    private static readonly Buttons[] AllButtons;

    static InputState()
    {
      AllButtons = Enum.GetValues(typeof(Buttons)).Cast<Buttons>().ToArray();
    }

    public Vector2 CursorPosition { get; private set; }
    public bool SelectPressed { get; private set; }
    public bool SelectReleased { get; private set; }

    public bool MenuUp
    {
      get { return IsNewKeyPress(Keys.Up) || IsNewButtonPress(Buttons.DPadUp) || IsNewButtonPress(Buttons.LeftThumbstickUp); }
    }

    public bool MenuDown
    {
      get { return IsNewKeyPress(Keys.Down) || IsNewButtonPress(Buttons.DPadDown) || IsNewButtonPress(Buttons.LeftThumbstickDown); }
    }

    public bool MenuSelect
    {
      get { return IsNewKeyPress(Keys.Enter) || IsNewButtonPress(Buttons.A) || IsNewButtonPress(Buttons.Start); }
    }

    public bool MenuCancel
    {
      get { return IsNewKeyPress(Keys.Escape) || IsNewButtonPress(Buttons.B) || IsNewButtonPress(Buttons.Back); }
    }

    public bool PauseGame
    {
      get { return IsNewKeyPress(Keys.Space) || IsNewButtonPress(Buttons.Back) || IsNewButtonPress(Buttons.Start); }
    }

    public bool QuitStory
    {
      get { return IsNewKeyPress(Keys.Escape) || IsNewKeyPress(Keys.Enter) || IsNewButtonPress(Buttons.Back) || IsNewButtonPress(Buttons.Start); }
    }

    public bool Any
    {
      get { return IsAnyKeyPress() || IsAnyButtonPress(); }
    }

    public bool SelectObject
    {
      get { return IsNewMouseClick(MouseButton.Left) || IsNewButtonPress(Buttons.A); }
    }

    public bool SellRequested
    {
      get { return IsNewKeyPress(Keys.S) || IsNewButtonPress(Buttons.B); }
    }

    public bool UpgradeRequested
    {
      get { return IsNewKeyPress(Keys.U) || IsNewButtonPress(Buttons.Y); }
    }

    public bool ClearSelections
    {
      get { return MenuCancel; }
    }

    public bool EctoplasTransmaterPort
    {
      get { return IsKeyDown(Keys.N) || IsNewButtonPress(Buttons.RightTrigger); }
    }

    /// <summary>
    /// 
    /// </summary>
    public InputState()
    {
      CurrentKeyboardStates = new KeyboardState[MaxInputs];
      LastKeyboardStates = new KeyboardState[MaxInputs];

      CurrentGamePadStates = new GamePadState[MaxInputs];
      LastGamePadStates = new GamePadState[MaxInputs];

      CurrentMouseStates = new MouseState[MaxInputs];
      LastMouseStates = new MouseState[MaxInputs];
    }

    public void Update(GameTime gameTime)
    {
      for (int i = 0; i < MaxInputs; i++)
      {
        LastKeyboardStates[i] = CurrentKeyboardStates[i];
        LastGamePadStates[i] = CurrentGamePadStates[i];
        LastMouseStates[i] = CurrentMouseStates[i];

        CurrentKeyboardStates[i] = Keyboard.GetState((PlayerIndex)i);
        CurrentGamePadStates[i] = GamePad.GetState((PlayerIndex)i);
        CurrentMouseStates[i] = Mouse.GetState();
      }

      CursorPosition = new Vector2(CurrentMouseStates[0].X, CurrentMouseStates[0].Y);
      SelectPressed = (CurrentMouseStates[0].LeftButton == ButtonState.Pressed);
      SelectReleased = (CurrentMouseStates[0].LeftButton == ButtonState.Released);
    }

    public bool IsAnyButtonPress(PlayerIndex playerIndex)
    {
      return Array.FindAll<Buttons>(AllButtons, bt => CurrentGamePadStates[(int)playerIndex].IsButtonDown(bt)).Length > 0;
    }

    public bool IsAnyButtonPress()
    {
      for (int i = 0; i < MaxInputs; i++)
      {
        if (IsAnyButtonPress((PlayerIndex)i))
          return true;
      }

      return false;
    }

    public bool IsAnyKeyPress(PlayerIndex playerIndex)
    {
      return (CurrentKeyboardStates[(int)playerIndex].GetPressedKeys().Length > 0);
    }

    public bool IsAnyKeyPress()
    {
      for (int i = 0; i < MaxInputs; i++)
      {
        if (IsAnyKeyPress((PlayerIndex)i))
          return true;
      }

      return false;
    }

    public bool IsNewMouseClick(MouseButton mouseButton)
    {
      for (int i = 0; i < MaxInputs; i++)
      {
        if (IsNewMouseClick(mouseButton, (PlayerIndex)i))
          return true;
      }
      return false;
    }

    public bool IsNewMouseClick(MouseButton mouseButton, PlayerIndex playerIndex)
    {
      ButtonState current, previous;
      current = previous = ButtonState.Pressed;

      switch (mouseButton)
      {
        case MouseButton.Left:
          {
            current = CurrentMouseStates[(int)playerIndex].LeftButton;
            previous = LastMouseStates[(int)playerIndex].LeftButton;
            break;
          }
        case MouseButton.Right:
          {
            current = CurrentMouseStates[(int)playerIndex].RightButton;
            previous = LastMouseStates[(int)playerIndex].RightButton;
            break;
          }
        case MouseButton.Middle:
          {
            current = CurrentMouseStates[(int)playerIndex].MiddleButton;
            previous = LastMouseStates[(int)playerIndex].MiddleButton;
            break;
          }
      }

      return current == ButtonState.Pressed && previous == ButtonState.Released;
    }

    public bool IsNewKeyPress(Keys key)
    {
      for (int i = 0; i < MaxInputs; i++)
      {
        if (IsNewKeyPress(key, (PlayerIndex)i))
          return true;
      }
      return false;
    }

    public bool IsNewKeyPress(Keys key, PlayerIndex playerIndex)
    {
      return (CurrentKeyboardStates[(int)playerIndex].IsKeyDown(key) &&
              LastKeyboardStates[(int)playerIndex].IsKeyUp(key));
    }

    public bool IsNewButtonPress(Buttons button)
    {
      for (int i = 0; i < MaxInputs; i++)
      {
        if (IsNewButtonPress(button, (PlayerIndex)i))
          return true;
      }

      return false;
    }

    public bool IsNewButtonPress(Buttons button, PlayerIndex playerIndex)
    {
      return (CurrentGamePadStates[(int)playerIndex].IsButtonDown(button) &&
              LastGamePadStates[(int)playerIndex].IsButtonUp(button));
    }

    public bool IsKeyDown(Keys key)
    {
      for (int i = 0; i < MaxInputs; i++)
      {
        if (IsKeyDown(key, (PlayerIndex)i))
          return true;
      }

      return false;
    }

    public bool IsKeyDown(Keys key, PlayerIndex playerIndex)
    {
      return CurrentKeyboardStates[(int)playerIndex].IsKeyDown(key);
    }

    public bool IsButtonDown(Buttons button)
    {
      for (int i = 0; i < MaxInputs; i++)
      {
        if (IsButtonDown(button, (PlayerIndex)i))
          return true;
      }

      return false;
    }

    public bool IsButtonDown(Buttons button, PlayerIndex playerIndex)
    {
      return CurrentGamePadStates[(int)playerIndex].IsButtonDown(button);
    }
  }
}
