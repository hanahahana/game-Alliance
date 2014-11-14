using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Alliance
{
  public static class TextureExtensions
  {
    public static Vector2[] CreateConvexHull(this Texture2D texture)
    {
      Color[] colorData = new Color[texture.Width * texture.Height];
      texture.GetData<Color>(colorData);

      List<Vector2> pixels = new List<Vector2>(colorData.Length);
      for (int x = 0; x < texture.Width; ++x)
      {
        for (int y = 0; y < texture.Height; ++y)
        {
          Color color = colorData[x + (y * texture.Width)];
          if (color.A > 250)
          {
            pixels.Add(new Vector2(x, y));
          }
        }
      }

      Vector2[] polygon = pixels.Select(v => new Vector2(v.X, v.Y)).ToArray();
      Vector2[] H = new Vector2[polygon.Length];
      int n = ChainConvexHull.ComputeHull(polygon, polygon.Length, ref H);
      Vector2[] values = new Vector2[n];
      Array.Copy(H, values, n);
      return values;
    }
  }
}
