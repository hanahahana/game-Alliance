using System;
using System.Collections.Generic;
using System.Text;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace Alliance
{
  public class AllianceGame : Game
  {
    private GraphicsDeviceManager graphics;
    private ComponentManager components;
    private ScreenComponent screens;

    public AllianceGame()
    {
      graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";
      components = new ComponentManager(this);

      screens = new ScreenComponent(components);
      components.Add(screens);

      screens.AddScreen(new MainMenuScreen());
    }

    protected override void Initialize()
    {
      IsMouseVisible = true;
      Window.Title = "Alliance";

      graphics.PreferredBackBufferWidth = 800;
      graphics.PreferredBackBufferHeight = 600;
      graphics.ApplyChanges();

      components.Initialize();
      base.Initialize();
    }

    protected override void LoadContent()
    {
      base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
      components.Update(gameTime);
      base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);
      components.Draw(gameTime);
      base.Draw(gameTime);
    }
  }
}
