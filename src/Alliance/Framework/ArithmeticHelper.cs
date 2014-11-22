using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alliance
{
  public static class ArithmeticHelper
  {
    private static void SolveTridiag(double[] sub, double[] diag, double[] sup, ref double[] b, int n)
    {
      for (int i = 2; i <= n; i++)
      {
        sub[i] /= diag[i - 1];
        diag[i] -= sub[i] * sup[i - 1];
        b[i] -= sub[i] * b[i - 1];
      }
      b[n] /= diag[n];
      for (int i = n - 1; i >= 1; i--)
      {
        b[i] = (b[i] - sup[i] * b[i + 1]) / diag[i];
      }
    }
    public static sbyte Clamp(sbyte value, sbyte min, sbyte max)
    {
      return ArithmeticHelper.Clamp<sbyte>(value, min, max);
    }
    public static byte Clamp(byte value, byte min, byte max)
    {
      return ArithmeticHelper.Clamp<byte>(value, min, max);
    }
    public static short Clamp(short value, short min, short max)
    {
      return ArithmeticHelper.Clamp<short>(value, min, max);
    }
    public static ushort Clamp(ushort value, ushort min, ushort max)
    {
      return ArithmeticHelper.Clamp<ushort>(value, min, max);
    }
    public static int Clamp(int value, int min, int max)
    {
      return ArithmeticHelper.Clamp<int>(value, min, max);
    }
    public static uint Clamp(uint value, uint min, uint max)
    {
      return ArithmeticHelper.Clamp<uint>(value, min, max);
    }
    public static long Clamp(long value, long min, long max)
    {
      return ArithmeticHelper.Clamp<long>(value, min, max);
    }
    public static ulong Clamp(ulong value, ulong min, ulong max)
    {
      return ArithmeticHelper.Clamp<ulong>(value, min, max);
    }
    public static decimal Clamp(decimal value, decimal min, decimal max)
    {
      return ArithmeticHelper.Clamp<decimal>(value, min, max);
    }
    public static float Clamp(float value, float min, float max)
    {
      return ArithmeticHelper.Clamp<float>(value, min, max);
    }
    public static double Clamp(double value, double min, double max)
    {
      return ArithmeticHelper.Clamp<double>(value, min, max);
    }
    public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
    {
      value = (ArithmeticHelper.GreaterThan<T>(value, max) ? max : value);
      value = (ArithmeticHelper.LessThan<T>(value, min) ? min : value);
      return value;
    }
    public static long GCD(long a, long b)
    {
      if (a < 0L)
      {
        a = -a;
      }
      if (b < 0L)
      {
        b = -b;
      }
      do
      {
        if (a < b)
        {
          long tmp = a;
          a = b;
          b = tmp;
        }
        a %= b;
      }
      while (a != 0L);
      return b;
    }
    public static long LCM(long a, long b)
    {
      return a * b / ArithmeticHelper.GCD(a, b);
    }
    public static bool GreaterThan<T>(T left, T right) where T : IComparable<T>
    {
      return left.CompareTo(right) > 0;
    }
    public static bool LessThan<T>(T left, T right) where T : IComparable<T>
    {
      return left.CompareTo(right) < 0;
    }
    public static bool InRange(int value, int min, int max)
    {
      return min <= value && value <= max;
    }
    public static T Max<T>(params T[] values) where T : IComparable<T>
    {
      if (values.Length <= 0)
      {
        return default(T);
      }
      T max = values[0];
      for (int i = 1; i < values.Length; i++)
      {
        if (ArithmeticHelper.GreaterThan<T>(values[i], max))
        {
          max = values[i];
        }
      }
      return max;
    }
    public static T Min<T>(params T[] values) where T : IComparable<T>
    {
      if (values.Length <= 0)
      {
        return default(T);
      }
      T min = values[0];
      for (int i = 1; i < values.Length; i++)
      {
        if (ArithmeticHelper.LessThan<T>(values[i], min))
        {
          min = values[i];
        }
      }
      return min;
    }
    public static double LinearInterpolate(double x, double y, double mu)
    {
      return x * (1.0 - mu) + y * mu;
    }
    public static double CosineInterpolate(double x, double y, double mu)
    {
      double mu2 = (1.0 - Math.Cos(mu * 3.1415926535897931)) / 2.0;
      return x * (1.0 - mu2) + y * mu2;
    }
    public static bool IsPrime(long n)
    {
      if (n == 2L || n == 3L)
      {
        return true;
      }
      if (n % 2L == 0L || n % 3L == 0L || n < 2L)
      {
        return false;
      }
      long root = (long)Math.Sqrt((double)n) + 1L;
      for (long i = 6L; i <= root; i += 6L)
      {
        if (n % (i - 1L) == 0L)
        {
          return false;
        }
        if (n % (i + 1L) == 0L)
        {
          return false;
        }
      }
      return true;
    }
    public static double Power(double n, int power)
    {
      if (power == 0)
      {
        return 1.0;
      }
      if (power == 1)
      {
        return n;
      }
      double result = ArithmeticHelper.Power(n, power / 2);
      double x = result * result;
      if (power % 2 == 1)
      {
        return x * n;
      }
      return x;
    }
    public static double HermiteInterpolate(double y0, double y1, double yp0, double yp1, double mu, double tension, double bias)
    {
      double mu2 = mu * mu;
      double mu3 = mu2 * mu;
      double m0 = (y1 - y0) * (1.0 + bias) * (1.0 - tension) / 2.0;
      m0 += (yp0 - y1) * (1.0 - bias) * (1.0 - tension) / 2.0;
      double m = (yp0 - y1) * (1.0 + bias) * (1.0 - tension) / 2.0;
      m += (yp1 - yp0) * (1.0 - bias) * (1.0 - tension) / 2.0;
      double a0 = 2.0 * mu3 - 3.0 * mu2 + 1.0;
      double a = mu3 - 2.0 * mu2 + mu;
      double a2 = mu3 - mu2;
      double a3 = -2.0 * mu3 + 3.0 * mu2;
      return a0 * y1 + a * m0 + a2 * m + a3 * yp0;
    }
    public static double SplineInterpolate(List<Sample> knownSamples, double knownX)
    {
      knownSamples.Sort();
      Sample t = new Sample(knownX, -1.7976931348623157E+308);
      int idx = knownSamples.BinarySearch(t);
      if (idx > -1)
      {
        return knownSamples[idx].Y;
      }
      int np = knownSamples.Count;
      if (np > 1)
      {
        double[] a = new double[np];
        double[] h = new double[np];
        for (int i = 1; i <= np - 1; i++)
        {
          h[i] = knownSamples[i].X - knownSamples[i - 1].X;
        }
        if (np > 2)
        {
          double[] sub = new double[np - 1];
          double[] diag = new double[np - 1];
          double[] sup = new double[np - 1];
          for (int j = 1; j <= np - 2; j++)
          {
            diag[j] = (h[j] + h[j + 1]) / 3.0;
            sup[j] = h[j + 1] / 6.0;
            sub[j] = h[j] / 6.0;
            a[j] = (knownSamples[j + 1].Y - knownSamples[j].Y) / h[j + 1] - (knownSamples[j].Y - knownSamples[j - 1].Y) / h[j];
          }
          ArithmeticHelper.SolveTridiag(sub, diag, sup, ref a, np - 2);
        }
        int gap = 0;
        double previous = -1.7976931348623157E+308;
        for (int k = 0; k < knownSamples.Count; k++)
        {
          if (knownSamples[k].X < knownX && knownSamples[k].X > previous)
          {
            previous = knownSamples[k].X;
            gap = k + 1;
          }
        }
        double x = knownX - previous;
        double x2 = h[gap] - x;
        return ((-a[gap - 1] / 6.0 * (x2 + h[gap]) * x + knownSamples[gap - 1].Y) * x2 + (-a[gap] / 6.0 * (x + h[gap]) * x2 + knownSamples[gap].Y) * x) / h[gap];
      }
      return 0.0;
    }
    public static float CalculatePercent(float value, float min, float max)
    {
      return (value - min) / (max - min);
    }
  }
}
