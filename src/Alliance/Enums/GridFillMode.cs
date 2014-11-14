
namespace Alliance
{
  /// <summary>
  /// An enumeration containing the different fill modes for a GridComponent. This differs from
  /// the Microsoft.Xna.Framework.Graphics.FillMode.
  /// </summary>
  public enum GridFillMode
  {
    /// <summary>
    /// Draw the grid's objects normally with all their corresponding images
    /// </summary>
    Solid,

    /// <summary>
    /// Draw the grid's objects as their bounding convex polygons.
    /// </summary>
    Polygons,
  };
}
