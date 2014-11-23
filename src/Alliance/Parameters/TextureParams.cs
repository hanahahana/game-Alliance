using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsSystem;

namespace Alliance
{
  public class TextureParams
  {
    public GsImage Texture { get; private set; }
    public GsSize TextureSize { get; private set; }
    public GsVector Position { get; private set; }
    public GsVector Origin { get; private set; }
    public GsVector Scale { get; private set; }

    public TextureParams(GsImage texture, GsSize textureSize, GsVector position, GsVector origin, GsVector scale)
    {
      Texture = texture;
      TextureSize = textureSize;
      Position = position;
      Origin = origin;
      Scale = scale;
    }
  }
}
