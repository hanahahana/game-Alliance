using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alliance
{
  public struct APoint
  {
    public static APoint Zero { get { return new APoint(0); } }
    public static APoint One { get { return new APoint(1); } }
    
    public float X, Y;

    public APoint(float pt)
      : this(pt, pt)
    {

    }

    public APoint(float x, float y)
    {
      X = x;
      Y = y;
    }

    public static APoint Transform(APoint position, AMatrix matrix)
    {
      return new APoint((position.X * matrix.M11) + (position.Y * matrix.M21) + matrix.M41,
        (position.X * matrix.M12) + (position.Y * matrix.M22) + matrix.M42);
    }

    public static float Dot(APoint value1, APoint value2)
    {
      return value1.X * value2.X + value1.Y * value2.Y;
    }

    public static APoint operator -(APoint value)
    {
      value.X = -value.X;
      value.Y = -value.Y;
      return value;
    }

    public static bool operator ==(APoint value1, APoint value2)
    {
      return value1.X == value2.X && value1.Y == value2.Y;
    }

    public static bool operator !=(APoint value1, APoint value2)
    {
      return value1.X != value2.X || value1.Y != value2.Y;
    }

    public static APoint operator +(APoint value1, APoint value2)
    {
      value1.X += value2.X;
      value1.Y += value2.Y;
      return value1;
    }

    public static APoint operator -(APoint value1, APoint value2)
    {
      value1.X -= value2.X;
      value1.Y -= value2.Y;
      return value1;
    }

    public static APoint operator *(APoint value1, APoint value2)
    {
      value1.X *= value2.X;
      value1.Y *= value2.Y;
      return value1;
    }


    public static APoint operator *(APoint value, float scaleFactor)
    {
      value.X *= scaleFactor;
      value.Y *= scaleFactor;
      return value;
    }


    public static APoint operator *(float scaleFactor, APoint value)
    {
      value.X *= scaleFactor;
      value.Y *= scaleFactor;
      return value;
    }

    public static APoint operator /(APoint value1, APoint value2)
    {
      value1.X /= value2.X;
      value1.Y /= value2.Y;
      return value1;
    }

    public static APoint operator /(APoint value1, float divider)
    {
      float factor = 1 / divider;
      value1.X *= factor;
      value1.Y *= factor;
      return value1;
    }
  }
}
