using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using MLA.Utilities;
using MLA.Utilities.Helpers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alliance.Data;

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

    public static float SpLine(List<Vector2> knownSamples, float unknownY)
    {
      int np = knownSamples.Count;
      if (np > 1)
      {
        float[] a = new float[np];
        float x1;
        float x2;
        float y;
        float[] h = new float[np];

        for (int i = 1; i <= np - 1; i++)
        {
          h[i] = knownSamples[i].X - knownSamples[i - 1].X;
        }

        if (np > 2)
        {
          float[] sub = new float[np - 1];
          float[] diag = new float[np - 1];
          float[] sup = new float[np - 1];

          for (int i = 1; i <= np - 2; i++)
          {
            diag[i] = (h[i] + h[i + 1]) / 3;
            sup[i] = h[i + 1] / 6;
            sub[i] = h[i] / 6;

            a[i] = (knownSamples[i + 1].Y - knownSamples[i].Y) / h[i + 1] -
                   (knownSamples[i].Y - knownSamples[i - 1].Y) / h[i];
          }

          // SolveTridiag is a support function, see Marco Roello's original code
          // for more information at
          // http://www.codeproject.com/useritems/SplineInterpolation.asp
          SolveTridiag(sub, diag, sup, ref a, np - 2);
        }

        int gap = 0;
        float previous = float.MinValue;

        // At the end of this iteration, "gap" will contain the index of the interval
        // between two known values, which contains the unknown z, and "previous" will
        // contain the biggest z value among the known samples, left of the unknown z
        for (int i = 0; i < knownSamples.Count; i++)
        {
          if (knownSamples[i].X < unknownY && knownSamples[i].X > previous)
          {
            previous = knownSamples[i].X;
            gap = i + 1;
          }
        }

        x1 = unknownY - previous;
        x2 = h[gap] - x1;
        y = ((-a[gap - 1] / 6 * (x2 + h[gap]) * x1 + knownSamples[gap - 1].Y) * x2 +
            (-a[gap] / 6 * (x1 + h[gap]) * x2 + knownSamples[gap].Y) * x1) / h[gap];

        return y;
      }

      return 0;
    }

    private static void SolveTridiag(float[] sub, float[] diag, float[] sup, ref float[] b, int n)
    {
      /*                  solve linear system with tridiagonal n by n matrix a
								using Gaussian elimination *without* pivoting
								where   a(i,i-1) = sub[i]  for 2<=i<=n
										a(i,i)   = diag[i] for 1<=i<=n
										a(i,i+1) = sup[i]  for 1<=i<=n-1
								(the values sub[1], sup[n] are ignored)
								right hand side vector b[1:n] is overwritten with solution 
								NOTE: 1...n is used in all arrays, 0 is unused */
      int i;
      /*                  factorization and forward substitution */
      for (i = 2; i <= n; i++)
      {
        sub[i] = sub[i] / diag[i - 1];
        diag[i] = diag[i] - sub[i] * sup[i - 1];
        b[i] = b[i] - sub[i] * b[i - 1];
      }
      b[n] = b[n] / diag[n];
      for (i = n - 1; i >= 1; i--)
      {
        b[i] = (b[i] - sup[i] * b[i + 1]) / diag[i];
      }
    }

    public static Vector2 RandomVector2(float minXY, float maxXY)
    {
      float x = MathHelper.Lerp(minXY, maxXY, RandomHelper.NextSingle());
      float y = MathHelper.Lerp(minXY, maxXY, RandomHelper.NextSingle());
      return new Vector2(x, y);
    }

    public static bool CircleContains(Vector2 center, float radius, BoxF box)
    {
      /*
           * if sqrt( (rectangleRight.x - circleCenter.x)^2 + (rectangleBottom.y - circleCenter.y)^2) < radius then they intersect
           * if sqrt( (rectangleRight.x - circleCenter.x)^2 + (rectangleTop.y - circleCenter.y)^2) < radius then they intersect
           * if sqrt( (rectangleLeft.x - circleCenter.x)^2 + (rectangleTop.y - circleCenter.y)^2) < radius then they intersect
           * if sqrt( (rectangleLeft.x - circleCenter.x)^2 + (rectangleBottom.y - circleCenter.y)^2) < radius then they intersect
           */

      // get the radius squared
      float rad2 = radius * radius;

      float dR = box.Right - center.X;
      float dL = box.Left - center.X;
      float dB = box.Bottom - center.Y;
      float dT = box.Top - center.Y;

      float dist1 = (dR * dR) + (dB * dB);
      float dist2 = (dR * dR) + (dT * dT);
      float dist3 = (dL * dL) + (dT * dT);
      float dist4 = (dL * dL) + (dB * dB);

      return dist1 < rad2 ||
        dist2 < rad2 ||
        dist3 < rad2 ||
        dist4 < rad2;
    }

    public static float ClampSpecial(float sign, float value, float minMax)
    {
      // if the sign is zero, then this isn't a special clamp
      if(sign == 0) 
        return value;

      // if the sign is less than 0, that means that the value COULD fall below the minMax, so the minimum
      // becomes the minMax and the maximum becomes the value.
      return MathHelper.Clamp(value,
        (sign < 0) ? minMax : value,
        (sign < 0) ? value : minMax);
    }
  }
}
