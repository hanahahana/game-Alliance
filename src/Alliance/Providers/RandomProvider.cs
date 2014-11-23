using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alliance
{
  public static class RandomProvider
  {
    private static Random random = new Random();

    public static int Next() { return random.Next(); }
    public static int Next(int maxValue) { return random.Next(maxValue); }
    public static int Next(int minValue, int maxValue) { return random.Next(minValue, maxValue); }
    public static float NextSingle() { return (float)random.NextDouble(); }
    public static bool NextBool() { return random.NextDouble() > 0.5; }
  }
}
