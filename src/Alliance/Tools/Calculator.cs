using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsSystem;

namespace Alliance
{
  public static class Calculator
  {
    public static float StepToNext(float a, float dA, float tA)
    {
      return ((dA > 0 && a >= tA) || (dA < 0 && a <= tA)) ? tA : a;
    }

    public static float CalculatePercent(float value, float min, float max)
    {
      return (value - min) / (max - min);
    }

    public static GsVector ComputeScale(GsSize originalSize, GsSize desiredSize)
    {
      return ComputeScale(originalSize.Width, originalSize.Height, desiredSize.Width, desiredSize.Height);
    }

    public static GsVector ComputeScale(float orgWidth, float orgHeight, float desWidth, float desHeight)
    {
      return new GsVector(desWidth / orgWidth, desHeight / orgHeight);
    }

    public static GsVector ComputeProjectileDirection(float angle)
    {
      GsVector v = new GsVector(1, 0);
      GsMatrix rotMatrix = GsMatrix.CreateRotationZ(angle);
      return GsVector.Transform(v, rotMatrix);
    }

    public static GsVector ComputeDirection(Piece piece, Invader invader)
    {
      float speed = piece.ProjectileSpeed;
      GsVector position = invader.Bounds.Center - piece.Bounds.Center;
      GsVector velocity = invader.Velocity * invader.VelocityFactor;

      float a = speed * speed - velocity.LengthSquared();
      float b = GsVector.Dot(position, velocity);
      float c = position.LengthSquared();
      // Cope with rare special case where bullet and target have same speed, to avoid dividing by zero  
      if (a == 0)
      {
        if (b < 0)
        {
          // Meet halfway...  
          float time = -0.5f * c / b;
          return (position + velocity * time) / (speed * time);
        }
        else
        {
          // Can't hit target
        }
      }
      else
      {
        float bSqPlusAC = b * b + a * c;
        // Can't take square root of negative number  
        if (bSqPlusAC >= 0)
        {
          float solution = (b + (float)Math.Sqrt(bSqPlusAC)) / a;
          if (solution >= 0)
          {
            float time = solution;
            return (position + velocity * time) / (speed * time);
          }
          else
          {
            // Can't hit target  
          }
        }
        else
        {
          // Can't hit target  
        }
      }

      return GsVector.Zero;
    }

    public static float ComputeOrientation(float orientation, GsVector direction, float turnSpeed)
    {
      float desiredAngle = (float)Math.Atan2(direction.Y, direction.X);
      float difference = GsMath.WrapAngle(desiredAngle - orientation);
      return GsMath.WrapAngle(orientation + difference);
    }

    public static GsRectangle CreateBoxAroundPoint(GsVector pt, GsVector offset, float width, float height)
    {
      GsRectangle box = new GsRectangle(pt.X - (width / 2), pt.Y - (height / 2), width, height);
      box.Offset(offset);
      return box;
    }

    public static int CombineHashCodes<T>(params T[] values)
    {
      return CombineHashCodes(values.Select(v => v.GetHashCode()).ToArray());
    }

    public static int CombineHashCodes(params int[] hashCodes)
    {
      if (hashCodes.Length == 1)
      {
        return hashCodes[0];
      }

      var result = hashCodes[0];

      for (var i = 1; i < hashCodes.Length; i++)
      {
        result = CombineHashCodes(result, hashCodes[i]);
      }

      return result;
    }

    private static int CombineHashCodes(int h1, int h2)
    {
      return (h1 << 5) + h1 ^ h2;
    }
  }
}
