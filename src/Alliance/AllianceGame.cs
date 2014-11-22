using System;
using System.Collections.Generic;
using System.Text;
using GuiSystem;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;

namespace Alliance
{
  public class AllianceGame : Game
  {
    private GraphicsDeviceManager graphics;
    private List<BaseComponent> components;

    private GridComponent grid;
    private MessageComponent messages;


    public AllianceGame()
    {
      graphics = new GraphicsDeviceManager(this);
      Content.RootDirectory = "Content";

      components = new List<BaseComponent>();
      components.Add(new GridComponent(this));
      components.Add(MessageComponent.CreateInstance(this));
    }

    protected override void Initialize()
    {
      IsMouseVisible = true;
      Window.Title = "Alliance";

      graphics.PreferredBackBufferWidth = 800;
      graphics.PreferredBackBufferHeight = 600;
      graphics.ApplyChanges();

      base.Initialize();
      components.ForEach(c => c.Initialize());
    }

    protected override void LoadContent()
    {
      base.LoadContent();
      components.ForEach(c => c.LoadContent());
    }

    protected override void Update(GameTime gameTime)
    {
      base.Update(gameTime);
      components.ForEach(c => c.Update(gameTime));
    }

    protected override void Draw(GameTime gameTime)
    {
      GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.CornflowerBlue, 1f, 0);
      components.ForEach(c => c.Draw(gameTime));
      base.Draw(gameTime);
    }
  }
}
