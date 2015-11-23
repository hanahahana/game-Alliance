using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework;
using MLA.Utilities;
using Alliance.Data;
using Microsoft.Xna.Framework.Graphics;

namespace Alliance.Utilities
{
  public static class Utils
  {
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
  }
}
