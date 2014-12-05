using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsSystem;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Alliance
{
  public class KeyMouseProvider : IInputProvider, IAllianceInputState
  {
    private readonly GameWindow mWindow;

    public bool SelectObject
    {
      get { return mWindow.Mouse[MouseButton.Left]; }
    }

    public bool SellRequested
    {
      get { return mWindow.Keyboard[Key.S]; }
    }

    public bool UpgradeRequested
    {
      get { return mWindow.Keyboard[Key.U]; }
    }

    public bool ClearSelections
    {
      get { return mWindow.Keyboard[Key.Escape]; }
    }

    public bool SelectPressed
    {
      get { return mWindow.Mouse[MouseButton.Left]; }
    }

    public bool EctoplasTransmaterPort
    {
      get { return mWindow.Keyboard[Key.N]; }
    }

    public GsVector CursorPosition
    {
      get { return new GsVector(mWindow.Mouse.X, mWindow.Mouse.Y); }
    }

    public KeyMouseProvider(GameWindow window)
    {
      InputProvider.Register(this);
      mWindow = window;
    }

    public void Update()
    {

    }

    public IAllianceInputState GetState()
    {
      return this;
    }
  }
}
