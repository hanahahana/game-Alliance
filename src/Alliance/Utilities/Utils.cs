using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework;
using MLA.Utilities;
using Alliance.Data;
using Microsoft.Xna.Framework.Graphics;
using MLA.Utilities.Helpers;

namespace Alliance.Utilities
{
  public static class Utils
  {
    private enum MajorAxis { X, Y };
    private static Dictionary<MajorAxis, float> CosValue = new Dictionary<MajorAxis, float>(2);
    private static Dictionary<MajorAxis, float> SinValue = new Dictionary<MajorAxis, float>(2);

    static Utils()
    {
      double Pi = Math.PI;
      double PiOver2 = Pi / 2.0;

      CosValue[MajorAxis.X] = (float)Math.Cos(Pi);
      CosValue[MajorAxis.Y] = (float)Math.Cos(PiOver2);

      SinValue[MajorAxis.X] = (float)Math.Sin(Pi);
      SinValue[MajorAxis.Y] = (float)Math.Sin(PiOver2);
    }

    public static bool InRange(int value, int min, int max)
    {
      return min <= value && value <= max;
    }

    public static float WrapAngle(float radians)
    {
      while (radians < -MathHelper.Pi)
      {
        radians += MathHelper.TwoPi;
      }
      while (radians > MathHelper.Pi)
      {
        radians -= MathHelper.TwoPi;
      }
      return radians;
    }

    public static List<T> RetrieveAllSublcassesOf<T>()
    {
      List<T> retval = new List<T>(100);
      Type tType = typeof(T);
      if (tType.IsAbstract)
      {
        Assembly assembly = Assembly.GetAssembly(tType);
        Type[] types = assembly.GetTypes();
        foreach (Type type in types)
        {
          if (type.IsSubclassOf(tType))
          {
            ConstructorInfo ctor = type.GetConstructor(Type.EmptyTypes);
            if (ctor != null)
            {
              T value = (T)ctor.Invoke(null);
              retval.Add(value);
            }
          }
        }
      }
      return retval;
    }

    public static Index GetIndexCorrespondingTo(Vector2 position, int cellWidth, int cellHeight)
    {
      return GetIndexCorrespondingTo(position, cellWidth, cellHeight, null);
    }

    public static Index GetIndexCorrespondingTo(Vector2 position, int cellWidth, int cellHeight, Vector2? offset)
    {
      Vector2 pos = position - ((offset != null && offset.HasValue) ? offset.Value : Vector2.Zero);
      int c = (int)(pos.X / cellWidth);
      int r = (int)(pos.Y / cellHeight);
      return new Index(c, r);
    }

    public static bool IndexValid(Index index, int columnCount, int rowCount)
    {
      return (-1 < index.C && index.C < columnCount) && (-1 < index.R && index.R < rowCount);
    }

    public static BoxF CreateBox(int columnRank, int rowRank, float width, float height, Vector2 center)
    {
      return CreateBox(columnRank, rowRank, width, height, center, null);
    }

    public static BoxF CreateBox(int columnRank, int rowRank, float width, float height, Vector2 center, Vector2? offset)
    {
      float halfX = (columnRank / 2) * width;
      float halfY = (rowRank / 2) * height;
      Vector2 pos = center - ((offset != null && offset.HasValue) ? (offset.Value) : Vector2.Zero);

      BoxF retval = new BoxF();
      retval.X = pos.X - halfX;
      retval.Y = pos.Y - halfY;
      retval.Width = width;
      retval.Height = height;

      return retval;
    }

    public static Color NewAlpha(Color color, byte alpha)
    {
      return NewAlpha(color, alpha / 255f);
    }

    public static Color NewAlpha(Color color, float alpha)
    {
      return new Color(new Vector4(color.ToVector3(), alpha));
    }

    public static float CalculatePercent(float value, float min, float max)
    {
      return (value - min) / (max - min);
    }

    public static Color GetIntermediateColor(Color startColor, Color endColor, float value, float min, float max)
    {
      Color c = startColor;
      Color c2 = endColor;

      float pc = value * 1.0F / (max - min);

      int ca = c.A, cr = c.R, cg = c.G, cb = c.B;
      int c2a = c2.A, c2r = c2.R, c2g = c2.G, c2b = c2.B;

      int a = (int)Math.Abs(ca + (ca - c2a) * pc);
      int r = (int)Math.Abs(cr - ((cr - c2r) * pc));
      int g = (int)Math.Abs(cg - ((cg - c2g) * pc));
      int b = (int)Math.Abs(cb - ((cb - c2b) * pc));

      if (a > 255) { a = 255; }
      if (r > 255) { r = 255; }
      if (g > 255) { g = 255; }
      if (b > 255) { b = 255; }

      return FromArgb(a, r, g, b);
    }

    public static Color FromArgb(int a, int r, int g, int b)
    {
      return new Color((byte)r, (byte)g, (byte)b, (byte)a);
    }

    public static Vector2 ComputeScale(SizeF originalSize, SizeF desiredSize)
    {
      return new Vector2(desiredSize.Width / originalSize.Width, desiredSize.Height / originalSize.Height);
    }

    public static bool EllipseContains(BoxF ellipse, Vector2 pt)
    {
      /*
       * X = (x-x0)*cos(t)+(y-y0)*sin(t); % Translate and rotate coords. 
       * Y = -(x-x0)*sin(t)+(y-y0)*cos(t); % to align with ellipse 
       * X^2/a^2 + Y^2/b^2 < 1
       */

      float dx = pt.X - ellipse.X;
      float dy = pt.Y - ellipse.Y;

      float a = Math.Max(ellipse.Width, ellipse.Height);
      float b = Math.Min(ellipse.Width, ellipse.Height);

      MajorAxis axis = (ellipse.Width < ellipse.Height) ? MajorAxis.Y : MajorAxis.X;
      float cosT = CosValue[axis];
      float sinT = SinValue[axis];

      float X = (dx * cosT) + (dy * sinT);
      float Y = -((dx * sinT) + (dy * cosT));

      float X2 = X * X;
      float Y2 = Y * Y;

      float a2 = a * a;
      float b2 = b * b;

      return ((X2 / a2) + (Y2 / b2)) < 1;
    }

    public static Color RandomColor()
    {
      byte[] rgb = new byte[3];
      RandomHelper.NextBytes(rgb);
      return new Color(rgb[0], rgb[1], rgb[2]);
    }

    public static Vector2 ComputeProjectileDirection(float angle)
    {
      Vector2 v = new Vector2(1, 0);
      Matrix rotMatrix = Matrix.CreateRotationZ(angle);
      return Vector2.Transform(v, rotMatrix);
    }

    public static Color[,] TextureTo2DArray(Texture2D texture)
    {
      Color[] colors1D = new Color[texture.Width * texture.Height];
      texture.GetData(colors1D);

      Color[,] colors2D = new Color[texture.Width, texture.Height];
      for (int x = 0; x < texture.Width; x++)
        for (int y = 0; y < texture.Height; y++)
          colors2D[x, y] = colors1D[x + y * texture.Width];

      return colors2D;
    }

    public static Vector2 TexturesCollide(Color[,] tex1, Matrix mat1, Color[,] tex2, Matrix mat2)
    {
      Matrix mat1to2 = mat1 * Matrix.Invert(mat2);
      int width1 = tex1.GetLength(0);
      int height1 = tex1.GetLength(1);
      int width2 = tex2.GetLength(0);
      int height2 = tex2.GetLength(1);

      for (int x1 = 0; x1 < width1; x1++)
      {
        for (int y1 = 0; y1 < height1; y1++)
        {
          Vector2 pos1 = new Vector2(x1, y1);
          Vector2 pos2 = Vector2.Transform(pos1, mat1to2);

          int x2 = (int)pos2.X;
          int y2 = (int)pos2.Y;
          if ((x2 >= 0) && (x2 < width2))
          {
            if ((y2 >= 0) && (y2 < height2))
            {
              if (tex1[x1, y1].A > 0)
              {
                if (tex2[x2, y2].A > 0)
                {
                  Vector2 screenPos = Vector2.Transform(pos1, mat1);
                  return screenPos;
                }
              }
            }
          }
        }
      }

      return new Vector2(-1, -1);
    }

    public static double FastDist(Vector2 a, Vector2 b)
    {
      return FastDist(a.X, a.Y, b.X, b.Y);
    }

    public static double FastDist(double x1, double y1, double x2, double y2)
    {
      double dx = x1 - x2;
      double dy = y1 - y2;

      return (dx * dx) + (dy * dy);
    }
  }
}
