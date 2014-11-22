using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Linq;

namespace Alliance
{
  /// <summary>
  /// Provides helper methods to easily manipulate collections (i.e. Arrays, ArrayLists, Lists, etc.)
  /// </summary>
  public static class CollectionHelper
  {
    /// <summary>
    /// Prints an array to a string. 
    /// </summary>
    /// <example>The array {1,2,3,4} is printed as [1,2,3,4]</example>
    /// <param name="array">The array to print. The contents of the array don't matter.</param>
    /// <returns>A string representation of the array.</returns>
    public static string ArrayToString(Array array)
    {
      StringBuilder sb = new StringBuilder();
      sb.Append("[");
      for (int i = 0; i < array.Length; ++i)
      {
        sb.AppendFormat("{0}{1}", array.GetValue(i), (i == array.Length - 1 ? string.Empty : ","));
      }
      sb.Append("]");
      return sb.ToString();
    }

    /// <summary>
    /// Converts an ICollection to an array of strongly-typed values.
    /// </summary>
    /// <typeparam name="T">The type to convert to.</typeparam>
    /// <param name="collection">The collection to convert.</param>
    /// <returns>An array of strongly-typed values.</returns>
    public static T[] CollectionToArray<T>(ICollection collection)
    {
      return (T[])new ArrayList(collection).ToArray(typeof(T));
    }

    /// <summary>
    /// Converts an ICollection to a list of strongly-typed values.
    /// </summary>
    /// <typeparam name="T">The type to convert to.</typeparam>
    /// <param name="collection">The collection to convert.</param>
    /// <returns>A list of strongly-typed values.</returns>
    public static List<T> CollectionToList<T>(ICollection collection)
    {
      return new List<T>(CollectionToArray<T>(collection));
    }

    /// <summary>
    /// Removes all duplicates from a list.
    /// </summary>
    /// <typeparam name="T">
    /// The type contained within the list. T must have a valid
    /// GetHashCode method or this will remove all (or no) items.
    /// </typeparam>
    /// <param name="lst">The list to adjust.</param>
    /// <returns>A list with unique values.</returns>
    public static List<T> RemoveDuplicates<T>(List<T> lst)
    {
      Dictionary<T, byte> tmp = new Dictionary<T, byte>(lst.Count);
      foreach (T item in lst)
        tmp[item] = 0x00;
      return new List<T>(tmp.Keys);
    }

    /// <summary>
    /// Retrieves the minimum value found inside a list.
    /// </summary>
    /// <typeparam name="T">The type contained within the list.</typeparam>
    /// <param name="lst">The list search for the minimum value.</param>
    /// <returns>The minimum value found.</returns>
    public static T FindMinimumValue<T>(List<T> lst) where T : IComparable<T>
    {
      return FindMinimum<T>(lst).First;
    }

    /// <summary>
    /// Retrieves the minimum value's index found inside a list.
    /// </summary>
    /// <typeparam name="T">The type contained within the list.</typeparam>
    /// <param name="lst">The list search for the minimum value.</param>
    /// <returns>The minimum value found.</returns>
    public static int FindMinimumValueIndex<T>(List<T> lst) where T : IComparable<T>
    {
      return FindMinimum<T>(lst).Second;
    }

    /// <summary>
    /// Retrieves the minimum value and the index of the minimum value found inside a list.
    /// </summary>
    /// <typeparam name="T">The type contained within the list.</typeparam>
    /// <param name="lst">The list search for the minimum value.</param>
    /// <returns>A tuple containing the minimum value and it's index.</returns>
    public static Tuple<T, int> FindMinimum<T>(List<T> lst) where T : IComparable<T>
    {
      if (lst.Count == 0) 
        return new Tuple<T, int>(default(T), -1);

      Tuple<T, int> retval = new Tuple<T, int>(lst[0], 0);
      for (int i = 1; i < lst.Count; ++i)
      {
        if (ArithmeticHelper.LessThan<T>(lst[i], retval.First))
        {
          retval.First = lst[i];
          retval.Second = i;
        }
      }
      return retval;
    }

    /// <summary>
    /// Retrieves the maximum value found inside a list.
    /// </summary>
    /// <typeparam name="T">The type contained within the list.</typeparam>
    /// <param name="lst">The list search for the maximum value.</param>
    /// <returns>The maximum value found.</returns>
    public static T FindMaximumValue<T>(List<T> lst) where T : IComparable<T>
    {
      return FindMaximum<T>(lst).First;
    }

    /// <summary>
    /// Retrieves the maximum value's index found inside a list.
    /// </summary>
    /// <typeparam name="T">The type contained within the list.</typeparam>
    /// <param name="lst">The list search for the maximum value.</param>
    /// <returns>The maximum value found.</returns>
    public static int FindMaximumValueIndex<T>(List<T> lst) where T : IComparable<T>
    {
      return FindMaximum<T>(lst).Second;
    }

    /// <summary>
    /// Retrieves the maximum value and the index of the maximum value found inside a list.
    /// </summary>
    /// <typeparam name="T">The type contained within the list.</typeparam>
    /// <param name="lst">The list search for the maximum value.</param>
    /// <returns>A tuple containing the maximum value and it's index.</returns>
    public static Tuple<T, int> FindMaximum<T>(List<T> lst) where T : IComparable<T>
    {
      if (lst.Count == 0)
        return new Tuple<T, int>(default(T), -1);

      Tuple<T, int> retval = new Tuple<T, int>(lst[0], 0);
      for (int i = 1; i < lst.Count; ++i)
      {
        if (ArithmeticHelper.GreaterThan<T>(lst[i], retval.First))
        {
          retval.First = lst[i];
          retval.Second = i;
        }
      }
      return retval;
    }

    /// <summary>
    /// Shuffles an enumeration.
    /// </summary>
    /// <typeparam name="T">The type of value inside the enumeration.</typeparam>
    /// <param name="source">The source enumeration.</param>
    /// <returns>A shuffled enumeration.</returns>
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
      Random rand = new Random((int)DateTime.Now.Ticks);
      return source.Select(t => new { Index = rand.Next(), Value = t }).OrderBy(p => p.Index).Select(p => p.Value);
    }
  }
}
