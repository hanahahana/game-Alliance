using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;

namespace Alliance
{
  public abstract class BaseComponent
  {
    public Game Game { get; private set; }
    public GraphicsDevice GraphicsDevice { get { return Game.GraphicsDevice; } }

    public BaseComponent(Game game)
    {
      Game = game;
    }

    public abstract void Draw(GameTime gameTime);
    public virtual void Initialize() { }
    public virtual void LoadContent() { }
    public abstract void Update(GameTime gameTime);
  }
}
