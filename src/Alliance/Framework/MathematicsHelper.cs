using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Alliance
{
  public static class MathHelper
  {
    public const float E = (float)Math.E;
    public const float Log10E = 0.4342945f;
    public const float Log2E = 1.442695f;
    public const float Pi = (float)Math.PI;
    public const float PiOver2 = (float)(Math.PI / 2.0);
    public const float PiOver4 = (float)(Math.PI / 4.0);
    public const float TwoPi = (float)(Math.PI * 2.0);

    public static float Barycentric(float value1, float value2, float value3, float amount1, float amount2)
    {
      return value1 + (value2 - value1) * amount1 + (value3 - value1) * amount2;
    }

    public static float CatmullRom(float value1, float value2, float value3, float value4, float amount)
    {
      // Using formula from http://www.mvps.org/directx/articles/catmull/
      // Internally using doubles not to lose precission
      double amountSquared = amount * amount;
      double amountCubed = amountSquared * amount;
      return (float)(0.5f * (2.0f * value2 +
          (value3 - value1) * amount +
          (2.0f * value1 - 5.0f * value2 + 4.0f * value3 - value4) * amountSquared +
          (3.0f * value2 - value1 - 3.0f * value3 + value4) * amountCubed));
    }

    public static float Clamp(float value, float min, float max)
    {
      return value > max ? max : (value < min ? min : value);
    }

    public static float Distance(float value1, float value2)
    {
      return Math.Abs(value1 - value2);
    }

    public static float Hermite(float value1, float tangent1, float value2, float tangent2, float amount)
    {
      // All transformed to double not to lose precission
      // Otherwise, for high numbers of param:amount the result is NaN instead of Infinity
      double v1 = value1, v2 = value2, t1 = tangent1, t2 = tangent2, s = amount, result;
      double sCubed = s * s * s;
      double sSquared = s * s;

      if (amount == 0f)
        result = value1;
      else if (amount == 1f)
        result = value2;
      else
        result = (2.0f * v1 - 2.0f * v2 + t2 + t1) * sCubed +
            (3.0f * v2 - 3.0f * v1 - 2.0f * t1 - t2) * sSquared +
            t1 * s +
            v1;
      return (float)result;
    }


    public static float Lerp(float value1, float value2, float amount)
    {
      return value1 + (value2 - value1) * amount;
    }

    public static float Max(float value1, float value2)
    {
      return Math.Max(value1, value2);
    }

    public static float Min(float value1, float value2)
    {
      return Math.Min(value1, value2);
    }

    public static float SmoothStep(float value1, float value2, float amount)
    {
      // It is expected that 0 < amount < 1
      // If amount < 0, return value1
      // If amount > 1, return value2
      float result = MathHelper.Clamp(amount, 0f, 1f);
      result = MathHelper.Hermite(value1, 0f, value2, 0f, result);
      return result;
    }

    public static float ToDegrees(float radians)
    {
      // This method uses double precission internally,
      // though it returns single float
      // Factor = 180 / pi
      return (float)(radians * 57.295779513082320876798154814105);
    }

    public static float ToRadians(float degrees)
    {
      // This method uses double precission internally,
      // though it returns single float
      // Factor = pi / 180
      return (float)(degrees * 0.017453292519943295769236907684886);
    }

    private enum MajorAxis
    {
      X,
      Y
    }
    private static Dictionary<MathHelper.MajorAxis, float> CosValue;
    private static Dictionary<MathHelper.MajorAxis, float> SinValue;
    private static Dictionary<double, double> CosTable;
    private static Dictionary<double, double> SineTable;
    static MathHelper()
    {
      MathHelper.CosValue = new Dictionary<MathHelper.MajorAxis, float>(2);
      MathHelper.SinValue = new Dictionary<MathHelper.MajorAxis, float>(2);
      MathHelper.CosTable = new Dictionary<double, double>(1000);
      MathHelper.SineTable = new Dictionary<double, double>(1000);
      MathHelper.CosValue[MathHelper.MajorAxis.X] = (float)Math.Cos(3.1415927410125732);
      MathHelper.CosValue[MathHelper.MajorAxis.Y] = (float)Math.Cos(1.5707963705062866);
      MathHelper.SinValue[MathHelper.MajorAxis.X] = (float)Math.Sin(3.1415927410125732);
      MathHelper.SinValue[MathHelper.MajorAxis.Y] = (float)Math.Sin(1.5707963705062866);
    }
    public static float WrapAngle(float angle)
    {
      while (angle < -3.14159274f)
      {
        angle += 6.28318548f;
      }
      while (angle > 3.14159274f)
      {
        angle -= 6.28318548f;
      }
      return angle;
    }
    public static Vector2 ComputeScale(SizeF originalSize, SizeF desiredSize)
    {
      return MathHelper.ComputeScale(originalSize.Width, originalSize.Height, desiredSize.Width, desiredSize.Height);
    }
    public static Vector2 ComputeScale(float orgWidth, float orgHeight, float desWidth, float desHeight)
    {
      return new Vector2(desWidth / orgWidth, desHeight / orgHeight);
    }
    public static bool EllipseContains(BoxF ellipse, Vector2 pt)
    {
      float dx = pt.X - ellipse.X;
      float dy = pt.Y - ellipse.Y;
      float a = Math.Max(ellipse.Width, ellipse.Height);
      float b = Math.Min(ellipse.Width, ellipse.Height);
      MathHelper.MajorAxis axis = (ellipse.Width < ellipse.Height) ? MathHelper.MajorAxis.Y : MathHelper.MajorAxis.X;
      float cosT = MathHelper.CosValue[axis];
      float sinT = MathHelper.SinValue[axis];
      float X = dx * cosT + dy * sinT;
      float Y = -(dx * sinT + dy * cosT);
      float X2 = X * X;
      float Y2 = Y * Y;
      float a2 = a * a;
      float b2 = b * b;
      return X2 / a2 + Y2 / b2 < 1f;
    }
    public static bool CircleContains(Vector2 center, float radius, Vector2 point)
    {
      float rad2 = radius * radius;
      return Vector2.DistanceSquared(point, center) <= rad2;
    }
    public static bool CircleContains(Vector2 center, float radius, BoxF box)
    {
      Vector2[] points = new Vector2[]
			{
				new Vector2(box.Left, box.Top),
				new Vector2(box.Right, box.Top),
				new Vector2(box.Right, box.Bottom),
				new Vector2(box.Left, box.Bottom),
				box.Center
			};
      bool anyTrue = false;
      int i = 0;
      while (!anyTrue && i < points.Length)
      {
        Vector2 pt = points[i];
        anyTrue = MathHelper.CircleContains(center, radius, pt);
        i++;
      }
      return anyTrue;
    }
    public static T Max<T>(params T[] values) where T : IComparable<T>
    {
      T max = values[0];
      for (int i = 1; i < values.Length; i++)
      {
        if (values[i].CompareTo(max) > 0)
        {
          max = values[i];
        }
      }
      return max;
    }
    public static T Min<T>(params T[] values) where T : IComparable<T>
    {
      T min = values[0];
      for (int i = 1; i < values.Length; i++)
      {
        if (values[i].CompareTo(min) < 0)
        {
          min = values[i];
        }
      }
      return min;
    }
    public static bool PointInsideBoundingBox(Vector3 point, BoundingBox box)
    {
      return point.X >= box.Minimum.X && point.Y >= box.Minimum.Y && point.Z >= box.Minimum.Z && point.X <= box.Maximum.X && point.Y <= box.Maximum.Y && point.Z <= box.Maximum.Z;
    }
    public static bool PointInsideCone(Vector3 point, Vector3 coneOrigin, Vector3 coneDirection, double coneAngle)
    {
      Vector3 tempVect = Vector3.Normalize(point - coneOrigin);
      return (double)Vector3.Dot(coneDirection, tempVect) >= Math.Cos(coneAngle);
    }
    public static bool PointInsideBoundingSphere(Vector3 point, BoundingSphere sphere)
    {
      return (point - sphere.Center).LengthSquared() < Math.Abs(sphere.Radius * sphere.Radius);
    }
    public static bool CircleInCircle(Vector2 centerA, double radiusA, Vector2 centerB, double radiusB)
    {
      double dX = (double)(centerB.X - centerA.X);
      double dY = (double)(centerB.Y - centerA.Y);
      double d = Math.Sqrt(dX * dX + dY * dY);
      return d <= radiusA + radiusB;
    }
    public static bool PointInPolygon(Vector2[] polygonVertex, Vector2 testVertex)
    {
      bool c = false;
      int nvert = polygonVertex.Length;
      if (nvert > 2)
      {
        int i = 0;
        int j = nvert - 1;
        while (i < nvert)
        {
          if (polygonVertex[i].Y > testVertex.Y != polygonVertex[j].Y > testVertex.Y && testVertex.X < (polygonVertex[j].X - polygonVertex[i].X) * (testVertex.Y - polygonVertex[i].Y) / (polygonVertex[j].Y - polygonVertex[i].Y) + polygonVertex[i].X)
          {
            c = !c;
          }
          j = i++;
        }
      }
      return c;
    }
    public static float CalculatePercent(float value, float minimum, float maximum)
    {
      return (value - minimum) / (maximum - minimum);
    }
    public static double Cos(double t)
    {
      if (MathHelper.CosTable.ContainsKey(t))
      {
        return MathHelper.CosTable[t];
      }
      double result = Math.Cos(t);
      MathHelper.CosTable[t] = result;
      return result;
    }
    public static double Sin(double t)
    {
      if (MathHelper.SineTable.ContainsKey(t))
      {
        return MathHelper.SineTable[t];
      }
      double result = Math.Sin(t);
      MathHelper.SineTable[t] = result;
      return result;
    }
  }
}
