using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsSystem;

namespace Alliance
{
  public class ImageParams
  {
    public GsImage Image { get; private set; }
    public GsSize ImageSize { get; private set; }
    public GsVector Position { get; private set; }
    public GsVector Origin { get; private set; }
    public GsVector Scale { get; private set; }

    public ImageParams(GsImage image, GsSize imageSize, GsVector position, GsVector origin, GsVector scale)
    {
      Image = image;
      ImageSize = imageSize;
      Position = position;
      Origin = origin;
      Scale = scale;
    }
  }
}
