using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Microsoft.Xna.Framework;

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
  }
}
