using System;
using System.Collections.Generic;
using System.Text;
using GraphicsSystem;
using GuiSystem;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace Alliance
{
  public class AllianceGame : Game, IGuiSurface
  {
    private GraphicsDeviceManager graphicsDeviceManager;
    private GridComponent grid;
    private InputState input;
    private ResourceCache resources;
    private SharpDXGraphics graphics;

    int IGuiSurface.Height { get { return graphicsDeviceManager.PreferredBackBufferHeight; } }
    int IGuiSurface.Width { get { return graphicsDeviceManager.PreferredBackBufferWidth; } }

    public AllianceGame()
    {
      graphicsDeviceManager = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
      input = new InputState(this);
    }

    protected override void Initialize()
    {
      IsMouseVisible = true;
      Window.Title = "Alliance";

      graphicsDeviceManager.PreferredBackBufferWidth = 800;
      graphicsDeviceManager.PreferredBackBufferHeight = 600;
      graphicsDeviceManager.ApplyChanges();

      base.Initialize();
    }

    protected override void LoadContent()
    {
      graphics = new SharpDXGraphics(GraphicsDevice);
      resources = new ResourceCache(this);

      grid = new GridComponent(this);
      grid.Initialize();

      base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
      input.Update(gameTime);

      var state = new GuiInputState
      {
        Point = input.CursorPosition,
        Pressed = input.SelectPressed,
        Released = input.SelectReleased,
      };

      grid.Update(state, gameTime.ElapsedGameTime);
      base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);

      graphics.Begin();
      grid.Draw(graphics);
      graphics.End();

      base.Draw(gameTime);
    }
  }
}
