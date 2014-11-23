using System;
using GraphicsSystem;

namespace Alliance
{
  /// <summary>
  /// Represents parameters passed to methods to perform drawing.
  /// </summary>
  public class DrawParams
  {
    public GsVector Offset { get; private set; }
    public GridFillMode FillMode { get; private set; }
    public IGsGraphics Graphics { get; private set; }

    public DrawParams(GsVector offset, GridFillMode gridFillMode, IGsGraphics graphics)
    {
      Offset = offset;
      FillMode = gridFillMode;
      Graphics = graphics;
    }
  }
}
