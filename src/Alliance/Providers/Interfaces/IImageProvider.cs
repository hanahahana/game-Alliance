using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsSystem;

namespace Alliance
{
  public interface IImageProvider
  {
    FramedImage GetFramedImage(string key);
    GsSize GetSize(GsImage image);
    GsVector[] CreateConvexHull(GsImage image);
    GsColor[,] ToColorData(GsImage image);
    GsImage FromColorData(GsColor[] data, int width, int height);
  }
}
