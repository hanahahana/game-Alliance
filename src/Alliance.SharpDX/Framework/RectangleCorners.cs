using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Alliance
{
  /// <summary>
  /// Contains the different modes to use when rounding corners of a rectangle.
  /// </summary>
  [Flags]
  public enum RectangleCorners
  {
    /// <summary>
    /// Don't round any corners.
    /// </summary>
    None = 0, 

    /// <summary>
    /// Round the top-left corner.
    /// </summary>
    TopLeft = 1, 

    /// <summary>
    /// Round the top-right corner.
    /// </summary>
    TopRight = 2, 

    /// <summary>
    /// Round the bottom-left corner.
    /// </summary>
    BottomLeft = 4, 

    /// <summary>
    /// Round the bottom-right corner.
    /// </summary>
    BottomRight = 8,

    /// <summary>
    /// Round the top left and top right corners.
    /// </summary>
    Top = TopLeft | TopRight,

    /// <summary>
    /// Round the bottom left and bottom right corners.
    /// </summary>
    Bottom = BottomLeft | BottomRight,

    /// <summary>
    /// Rounds the top left and bottom left corners.
    /// </summary>
    Left = TopLeft | BottomLeft,

    /// <summary>
    /// Rounds the top right and bottom right corners.
    /// </summary>
    Right = TopRight | BottomRight,

    /// <summary>
    /// Round all of the corners.
    /// </summary>
    All = TopLeft | TopRight | BottomLeft | BottomRight,
  }
}
