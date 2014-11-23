using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
namespace Alliance
{
  /// <summary>
  /// Represents parameters passed to methods to perform drawing.
  /// </summary>
  public class DrawParams
  {
    public GameTime GameTime { get; private set; }
    public Vector2 Offset { get; private set; }
    public GridFillMode FillMode { get; private set; }
    public SpriteBatch SpriteBatch { get; private set; }
    public GraphicsBase Graphics { get; private set; }

    public DrawParams(GameTime gameTime, Vector2 offset, GridFillMode gridFillMode, SpriteBatch spriteBatch, GraphicsBase graphics)
    {
      GameTime = gameTime;
      Offset = offset;
      FillMode = gridFillMode;
      SpriteBatch = spriteBatch;
      Graphics = graphics;
    }
  }
}
