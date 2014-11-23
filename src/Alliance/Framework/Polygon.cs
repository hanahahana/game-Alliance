using System;
using System.Collections.Generic;
using GraphicsSystem;

namespace Alliance
{
  /// <summary>
  /// Represents a plane figure that is bounded by a closed path, composed of a finite sequence of straight line segments (or edges).
  /// </summary>
  [Serializable]
  public sealed class Polygon
  {
    private GsVector[] mPoints;
    private GsVector[] mEdges;
    private float mMinX;
    private float mMaxX;
    private float mMinY;
    private float mMaxY;

    /// <summary>
    /// Gets the collection of points where the edges meet. Changing this collection will not modify the
    /// actual points of the polygon (e.g. this is read-only).
    /// </summary>
    public GsVector[] Points
    {
      get 
      {
        GsVector[] copy = new GsVector[mPoints.Length];
        Array.Copy(mPoints, copy, copy.Length);
        return copy;
      }
    }

    /// <summary>
    /// Gets the collection of edges which make up the polygon. Changing this collection will not modify the
    /// actual edges of the polygon (e.g. this is read-only).
    /// </summary>
    public GsVector[] Edges
    {
      get 
      {
        GsVector[] copy = new GsVector[mEdges.Length];
        Array.Copy(mEdges, copy, copy.Length);
        return copy;
      }
    }

    /// <summary>
    /// Creates a new polygon from a collection of points (connecting them together) and applies
    /// the transformation on the result.
    /// </summary>
    /// <param name="points">The points to use to construct the polygon.</param>
    /// <param name="transform">The transform to apply to the polygon.</param>
    public Polygon(GsVector[] points, GsMatrix transform)
    {
      mPoints = GetPoints(points, transform);
      mEdges = GetEdges(mPoints);
    }

    private GsVector[] GetPoints(GsVector[] hull, GsMatrix transform)
    {
      mMinX = float.MaxValue;
      mMaxX = float.MinValue;
      mMinY = float.MaxValue;
      mMaxY = float.MinValue;

      List<GsVector> points = new List<GsVector>(hull.Length);
      foreach (GsVector pt in hull)
      {
        points.Add(GsVector.Transform(pt, transform));

        float x = points[points.Count - 1].X;
        float y = points[points.Count - 1].Y;

        mMinX = Math.Min(mMinX, x);
        mMinY = Math.Min(mMinY, y);

        mMaxX = Math.Max(mMaxX, x);
        mMaxY = Math.Max(mMaxY, y);
      }
      return points.ToArray();
    }

    private GsVector[] GetEdges(GsVector[] points)
    {
      GsVector p1;
      GsVector p2;

      List<GsVector> edges = new List<GsVector>(points.Length * 2);
      for (int i = 0; i < points.Length; i++)
      {
        p1 = points[i];
        if (i + 1 >= points.Length)
        {
          p2 = points[0];
        }
        else
        {
          p2 = points[i + 1];
        }
        edges.Add(p2 - p1);
      }

      return edges.ToArray();
    }

    /// <summary>
    /// Determines if this polygon intersects with another polygon.
    /// </summary>
    /// <param name="polygon">The polygon to test against.</param>
    /// <returns>A value indicating if the two polygons intersect.</returns>
    public bool IntersectsWith(Polygon polygon)
    {
      GsRectangle boxA = GsRectangle.FromLTRB(this.mMinX, this.mMinY, this.mMaxX, this.mMaxY);
      GsRectangle boxB = GsRectangle.FromLTRB(polygon.mMinX, polygon.mMinY, polygon.mMaxX, polygon.mMaxY);
      return boxA.IntersectsWith(boxB);
    }

    /// <summary>
    /// Calculate the distance between [minA, maxA] and [minB, maxB]. The distance will 
    /// be negative if the intervals overlap
    /// </summary>
    private static float IntervalDistance(float minA, float maxA, float minB, float maxB)
    {
      if (minA < minB)
      {
        return minB - maxA;
      }
      else
      {
        return minA - maxB;
      }
    }

    /// <summary>
    /// Calculate the projection of a polygon on an axis and returns it as a [min, max] interval
    /// </summary>
    private static void ProjectPolygon(GsVector axis, Polygon hull, ref float min, ref float max)
    {
      // get the points
      GsVector[] points = hull.mPoints;

      // To project a point on an axis use the dot product
      float d = GsVector.Dot(axis, points[0]);
      min = d;
      max = d;
      for (int i = 0; i < points.Length; i++)
      {
        d = GsVector.Dot(points[i], axis);
        min = Math.Min(min, d);
        max = Math.Max(max, d);
      }
    }
  }
}
