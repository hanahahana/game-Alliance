using System;
using System.Collections.Generic;
using System.Text;

namespace Alliance
{
  /// <summary>
  /// Provides a help wrapper around the System.Random class for generating random objects.
  /// </summary>
  public static class RandomHelper
  {
    private const int AChar = (int)'A';
    private const int ZChar = (int)'Z';
    private static Random Random = new Random();

    /// <summary>
    /// Returns a nonnegative random number.
    /// </summary>
    /// <returns>
    /// A 32-bit signed integer greater than or equal to zero and 
    /// less than Int32.MaxValue.
    /// </returns>
    public static int Next()
    {
      return Random.Next();
    }

    /// <summary>
    /// Returns a nonnegative random number less than the specified maximum.
    /// </summary>
    /// <param name="maxValue">
    /// The exclusive upper bound of the random number to be generated. maxValue 
    /// must be greater than or equal to zero.
    /// </param>
    /// <returns>
    /// A 32-bit signed integer greater than or equal to zero, and less than maxValue; 
    /// that is, the range of return values includes zero but not maxValue.
    /// </returns>
    public static int Next(int maxValue)
    {
      return Random.Next(maxValue);
    }

    /// <summary>
    /// Returns a random number within a specified range.
    /// </summary>
    /// <param name="range">The bounds of the random number returned.</param>
    /// <returns>
    /// A 32-bit signed integer greater than or equal to minValue and less than maxValue; 
    /// that is, the range of return values includes minValue but not maxValue. 
    /// If minValue equals maxValue, minValue is returned.
    /// </returns>
    public static int Next(Range range)
    {
      return Random.Next(range.Min, range.Max);
    }

    /// <summary>
    /// Returns a random number within a specified range.
    /// </summary>
    /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
    /// <param name="maxValue">
    /// The exclusive upper bound of the random number returned. maxValue must be 
    /// greater than or equal to minValue.
    /// </param>
    /// <returns>
    /// A 32-bit signed integer greater than or equal to minValue and less than maxValue; 
    /// that is, the range of return values includes minValue but not maxValue. 
    /// If minValue equals maxValue, minValue is returned.
    /// </returns>
    public static int Next(int minValue, int maxValue)
    {
      return Random.Next(minValue, maxValue);
    }

    /// <summary>
    /// Fills the elements of a specified array of bytes with random numbers.
    /// </summary>
    /// <param name="buffer">An array of bytes to contain random numbers.</param>
    public static void NextBytes(byte[] buffer)
    {
      Random.NextBytes(buffer);
    }

    /// <summary>
    /// Returns a random number between 0.0 and 1.0.
    /// </summary>
    /// <returns>
    /// A double-precision floating point number greater than or 
    /// equal to 0.0, and less than 1.0.
    /// </returns>
    public static double NextDouble()
    {
      return Random.NextDouble();
    }

    /// <summary>
    /// Returns a random number between 0.0f and 1.0f.
    /// </summary>
    /// <returns>
    /// A single-precision floating point number greater than or 
    /// equal to 0.0f, and less than 1.0f.
    /// </returns>
    public static float NextSingle()
    {
      return (float)Random.NextDouble();
    }

    /// <summary>
    /// Returns a random boolean value. This is usually "true".
    /// </summary>
    /// <returns>A boolean value.</returns>
    public static bool NextBool()
    {
      return Random.Next() % 2 == 0;
    }

    /// <summary>
    /// Returns a random boolean value. This is usually "false".
    /// </summary>
    /// <returns>A boolean value.</returns>
    public static bool NextRareBool()
    {
      return Random.Next() % 7 == 0 && Random.Next() % 17 == 0;
    }

    /// <summary>
    /// Returns a random lowercase or uppercase letter.
    /// </summary>
    /// <returns>A letter of the alphabet.</returns>
    public static char NextLetter()
    {
      char c = Convert.ToChar(Random.Next(AChar, ZChar + 1));
      if (NextBool())
        c = char.ToLower(c);
      return c;
    }

    /// <summary>
    /// Returns a random string of length [3,30). The letters in the string can be
    /// lowercase and/or uppercase.
    /// </summary>
    /// <returns>A random string with no spaces.</returns>
    public static string NextString()
    {
      return NextString(3, 30);
    }

    /// <summary>
    /// Returns a random string. The letters in the string can be
    /// lowercase and/or uppercase.
    /// </summary>
    /// <param name="minLength">The inclusive minimum length of the string.</param>
    /// <param name="maxLength">The exclusive maximum length of the string.</param>
    /// <returns>A random string with no spaces.</returns>
    public static string NextString(int minLength, int maxLength)
    {
      int count = Random.Next(minLength, maxLength);
      StringBuilder sb = new StringBuilder(count);
      for (; count > 0; --count)
      {
        sb.Append(NextLetter());
      }
      return sb.ToString();
    }
  }
}
