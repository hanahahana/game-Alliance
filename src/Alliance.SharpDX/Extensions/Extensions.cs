using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsSystem;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Alliance
{
  public static class SharpDXExtensions
  {
    public static SpriteEffects ToSpriteEffects(this GsImageFlip flip)
    {
      switch (flip)
      {
        case GsImageFlip.Both:
          return SpriteEffects.FlipBoth;
        case GsImageFlip.Horizontal:
          return SpriteEffects.FlipHorizontally;
        case GsImageFlip.Vertical:
          return SpriteEffects.FlipVertically;
      }
      return SpriteEffects.None;
    }

    public static Color ToColor(this GsColor color)
    {
      return new Color(color.R, color.G, color.B, color.A);
    }

    public static GsColor ToGsColor(this Color color)
    {
      return new GsColor(color.A, color.R, color.G, color.B);
    }

    public static Color[] GetFrame(this Color[,] data, Rectangle src, int length)
    {
      Color[] arr = new Color[length];
      int x, y, i = 0;
      for (y = src.Y; y < src.Bottom; ++y)
      {
        for (x = src.X; x < src.Right; ++x)
        {
          arr[i++] = data[x, y];
        }
      }
      return arr;
    }

    public static int ForEachIf<T>(this IList<T> list, Func<T, bool> predicate, Action<T> action)
    {
      int count = 0;
      foreach (T item in list)
      {
        if (!predicate(item))
          continue;
        ++count;
        action(item);
      }
      return count;
    }

    public static T[,] To2D<T>(this T[] arr, int columns, int rows)
    {
      T[,] a = new T[columns, rows];
      int c = 0, r = 0;
      for (int i = 0; i < arr.Length; ++i)
      {
        a[c, r] = arr[i];
        ++c;
        if (c == columns)
        {
          c = 0;
          r++;
        }
      }
      return a;
    }

    public static GsVector[] CreateConvexHull(this Texture2D texture)
    {
      Color[] colorData = new Color[texture.Width * texture.Height];
      texture.GetData<Color>(colorData);

      List<GsVector> pixels = new List<GsVector>(colorData.Length);
      int x, y;

      for (x = 0; x < texture.Width; ++x)
      {
        for (y = 0; y < texture.Height; ++y)
        {
          Color color = colorData[x + (y * texture.Width)];
          if (color.A > 250)
          {
            pixels.Add(new GsVector(x, y));
          }
        }
      }

      GsVector[] polygon = pixels.ToArray();
      GsVector[] H = new GsVector[polygon.Length];

      ChainConvexHull.ComputeHull(polygon, polygon.Length, ref H);
      return H;
    }
  }
}
