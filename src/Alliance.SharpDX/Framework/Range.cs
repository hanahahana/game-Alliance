using System;
using System.Collections.Generic;
using System.Text;

namespace Alliance
{
  /// <summary>
  /// A class to describe a range. (e.g. A lower bounds and an upper bounds).
  /// </summary>
  public struct Range
  {
    private int mMax;

    /// <summary>
    /// Gets or sets the maximum (the upper bounds)
    /// </summary>
    public int Max
    {
      get { return mMax; }
      set { mMax = value; }
    }

    private int mMin;

    /// <summary>
    /// Gets or sets the minimum (the lower bounds)
    /// </summary>
    public int Min
    {
      get { return mMin; }
      set { mMin = value; }
    }

    /// <summary>
    /// Creates a new range
    /// </summary>
    /// <param name="min">The minimum</param>
    /// <param name="max">The maximum</param>
    public Range(int min, int max)
    {
      mMin = min;
      mMax = max;
    }

    /// <summary>
    /// Checks to see if a value is within the range
    /// </summary>
    /// <param name="value">The value to check</param>
    /// <param name="range">The allowed range</param>
    /// <returns>A boolean indicating if the value is within the range</returns>
    public static bool IsInRange(int value, Range range)
    {
      return IsInRange(value, range.Min, range.Max);
    }

    /// <summary>
    /// Checks to see if a value is within the range [min, max]
    /// </summary>
    /// <param name="value">The value to check</param>
    /// <param name="min">The inclusive minimum value allowed</param>
    /// <param name="max">The inclusive maximum value allowed</param>
    /// <returns>A boolean indicating if the value is within the range</returns>
    public static bool IsInRange(int value, int min, int max)
    {
      return ArithmeticHelper.InRange(value, min, max);
    }
  }
}
