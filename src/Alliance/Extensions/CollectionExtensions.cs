using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alliance
{
  public static class CollectionExtensions
  {
    public static int ForEachIf<T>(this IList<T> list, Func<T, bool> predicate, Action<T> action)
    {
      int count = 0;
      foreach (T item in list)
      {
        if (!predicate(item))
          continue;
        ++count;
        action(item);
      }
      return count;
    }

    public static T[,] To2D<T>(this T[] arr, int columns, int rows)
    {
      T[,] a = new T[columns, rows];
      int c = 0, r = 0;
      for (int i = 0; i < arr.Length; ++i)
      {
        a[c, r] = arr[i];
        ++c;
        if (c == columns)
        {
          c = 0;
          r++;
        }
      }
      return a;
    }
  }
}
