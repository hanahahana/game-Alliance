using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsSystem;

namespace Alliance
{
  public static class ImageProvider
  {
    private static IImageProvider sProvider;

    public static void Register(IImageProvider provider)
    {
      sProvider = provider;
    }

    public static FramedImage GetFramedImage(string key)
    {
      return sProvider.GetFramedImage(key);
    }

    public static GsSize GetSize(GsImage image)
    {
      return sProvider.GetSize(image);
    }

    public static GsVector[] CreateConvexHull(GsImage image)
    {
      return sProvider.CreateConvexHull(image);
    }

    public static GsColor[,] ToColorData(GsImage image)
    {
      return sProvider.ToColorData(image);
    }

    public static GsImage FromColorData(GsColor[] data, int width, int height)
    {
      return sProvider.FromColorData(data, width, height);
    }
  }
}
