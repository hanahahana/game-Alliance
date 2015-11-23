using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Alliance
{
  public sealed class InputProvider : GameComponent
  {
    private MouseState previousMouse;
    private KeyboardState previousKeys;

    private bool mAddClick;
    public bool AddClick
    {
      get { return mAddClick; }
    }

    private bool mSellRequested;
    public bool SellRequested
    {
      get { return mSellRequested; }
    }

    private bool mUpgradeRequested;
    public bool UpgradeRequested
    {
      get { return mUpgradeRequested; }
    }

    private Vector2 mCursorPosition;
    public Vector2 CursorPosition
    {
      get { return mCursorPosition; }
    }

    public InputProvider(Game game)
      : base(game)
    {

    }

    public override void Update(GameTime gameTime)
    {
      MouseState currentMouse = Mouse.GetState();
      KeyboardState currentKeys = Keyboard.GetState();

      mAddClick = (currentMouse.LeftButton == ButtonState.Released && previousMouse.LeftButton == ButtonState.Pressed);
      mCursorPosition = new Vector2(currentMouse.X, currentMouse.Y);
      mSellRequested = IsKeyPress(currentKeys, Keys.S);
      mUpgradeRequested = IsKeyPress(currentKeys, Keys.U);

      previousKeys = currentKeys;
      previousMouse = currentMouse;
      base.Update(gameTime);
    }

    private bool IsKeyPress(KeyboardState current, Keys key)
    {
      return current.IsKeyDown(key) && previousKeys.IsKeyUp(key);
    }
  }
}
