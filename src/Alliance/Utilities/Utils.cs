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

    public static Color BlendColors(Color[] colors, float[] factors)
    {
      Color retval = colors[0];
      for (int i = 1; i < colors.Length; ++i)
      {
        retval = BlendColors(retval, colors[i], factors[i - 1]);
      }
      return retval;
    }

    public static Color BlendColors(Color startColor, Color endColor, float factor)
    {
      Color c = startColor;
      Color c2 = endColor;

      float pc = factor;

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

    /// <summary>
    /// tests if a point is Left|On|Right of an infinite line.
    /// </summary>
    /// <param name="P0">Point 1.</param>
    /// <param name="P1">Point 2.</param>
    /// <param name="P2">Point 3.</param>
    /// <returns>
    /// &gt;0 for P2 left of the line through P0 and P1
    /// =0 for P2 on the line
    /// &lt;0 for P2 right of the line
    /// </returns>
    private static float isLeft(Vector2 P0, Vector2 P1, Vector2 P2)
    {
      return (P1.X - P0.X) * (P2.Y - P0.Y) - (P2.X - P0.X) * (P1.Y - P0.Y);
    }

    /// <summary>
    /// Andrew's monotone chain 2D convex hull algorithm
    /// </summary>
    /// <param name="P">an array of 2D points presorted by increasing x- and y-coordinates</param>
    /// <param name="n">the number of points in P[]</param>
    /// <param name="H">an array of the convex hull vertices (max is n)</param>
    /// <returns>the number of points in H[]</returns>
    public static int Chain2DConvexHull(Vector2[] P, int n, ref Vector2[] H)
    {
      // the output array H[] will be used as the stack
      int bot = 0, top = (-1);  // indices for bottom and top of the stack
      int i;                // array scan index

      // Get the indices of points with min x-coord and min|max y-coord
      int minmin = 0, minmax;
      float xmin = P[0].X;
      for (i = 1; i < n; i++)
        if (P[i].X != xmin) break;
      minmax = i - 1;
      if (minmax == n - 1)
      {       // degenerate case: all x-coords == xmin
        H[++top] = P[minmin];
        if (P[minmax].Y != P[minmin].Y) // a nontrivial segment
          H[++top] = P[minmax];
        H[++top] = P[minmin];           // add polygon endpoint
        return top + 1;
      }

      // Get the indices of points with max x-coord and min|max y-coord
      int maxmin, maxmax = n - 1;
      float xmax = P[n - 1].X;
      for (i = n - 2; i >= 0; i--)
        if (P[i].X != xmax) break;
      maxmin = i + 1;

      // Compute the lower hull on the stack H
      H[++top] = P[minmin];      // push minmin point onto stack
      i = minmax;
      while (++i <= maxmin)
      {
        // the lower line joins P[minmin] with P[maxmin]
        if (isLeft(P[minmin], P[maxmin], P[i]) >= 0 && i < maxmin)
          continue;          // ignore P[i] above or on the lower line

        while (top > 0)        // there are at least 2 points on the stack
        {
          // test if P[i] is left of the line at the stack top
          if (isLeft(H[top - 1], H[top], P[i]) > 0)
            break;         // P[i] is a new hull vertex
          else
            top--;         // pop top point off stack
        }
        H[++top] = P[i];       // push P[i] onto stack
      }

      // Next, compute the upper hull on the stack H above the bottom hull
      if (maxmax != maxmin)      // if distinct xmax points
        H[++top] = P[maxmax];  // push maxmax point onto stack
      bot = top;                 // the bottom point of the upper hull stack
      i = maxmin;
      while (--i >= minmax)
      {
        // the upper line joins P[maxmax] with P[minmax]
        if (isLeft(P[maxmax], P[minmax], P[i]) >= 0 && i > minmax)
          continue;          // ignore P[i] below or on the upper line

        while (top > bot)    // at least 2 points on the upper stack
        {
          // test if P[i] is left of the line at the stack top
          if (isLeft(H[top - 1], H[top], P[i]) > 0)
            break;         // P[i] is a new hull vertex
          else
            top--;         // pop top point off stack
        }
        H[++top] = P[i];       // push P[i] onto stack
      }
      if (minmax != minmin)
        H[++top] = P[minmin];  // push joining endpoint onto stack

      return top + 1;
    }

    public static Vector2[] CreateConvexHull(Texture2D texture)
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

      Vector2[] polygon = pixels.ToArray();
      Vector2[] H = new Vector2[polygon.Length];
      int n = Utils.Chain2DConvexHull(polygon, polygon.Length, ref H);

      Vector2[] values = new Vector2[n];
      Array.Copy(H, values, n);

      return values;
    }
  }
}
