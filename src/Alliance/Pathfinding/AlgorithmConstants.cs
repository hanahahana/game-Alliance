using System;
using System.Collections.Generic;
using System.Text;

namespace Alliance
{
  /// <summary>
  /// Provides access to the constants used by algorithms under this assembly.
  /// </summary>
  public static class PathfindingConstants
  {
    /// <summary>
    /// The estimated cost to move orthogonally. Used by searching algorithms.
    /// </summary>
    public const int OrthogonalCost = 10;

    /// <summary>
    /// The estimated cost to move diagonally. Used by searching algorithms.
    /// </summary>
    public const int DiagonalCost = 14;
  }
}
