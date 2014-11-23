using System;
using System.Collections.Generic;
using SharpDX;

namespace Alliance
{
  /// <summary>
  /// Represents a plane figure that is bounded by a closed path, composed of a finite sequence of straight line segments (or edges).
  /// </summary>
  [Serializable]
  public sealed class Polygon
  {
    private Vector2[] mPoints;
    private Vector2[] mEdges;
    private float mMinX;
    private float mMaxX;
    private float mMinY;
    private float mMaxY;

    /// <summary>
    /// Gets the collection of points where the edges meet. Changing this collection will not modify the
    /// actual points of the polygon (e.g. this is read-only).
    /// </summary>
    public Vector2[] Points
    {
      get 
      {
        Vector2[] copy = new Vector2[mPoints.Length];
        Array.Copy(mPoints, copy, copy.Length);
        return copy;
      }
    }

    /// <summary>
    /// Gets the collection of edges which make up the polygon. Changing this collection will not modify the
    /// actual edges of the polygon (e.g. this is read-only).
    /// </summary>
    public Vector2[] Edges
    {
      get 
      {
        Vector2[] copy = new Vector2[mEdges.Length];
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
    public Polygon(Vector2[] points, Matrix transform)
    {
      mPoints = GetPoints(points, transform);
      mEdges = GetEdges(mPoints);
    }

    private Vector2[] GetPoints(Vector2[] hull, Matrix transform)
    {
      mMinX = float.MaxValue;
      mMaxX = float.MinValue;
      mMinY = float.MaxValue;
      mMaxY = float.MinValue;

      List<Vector2> points = new List<Vector2>(hull.Length);
      foreach (Vector2 pt in hull)
      {
        points.Add(AllianceUtilities.Transform(pt, transform));

        float x = points[points.Count - 1].X;
        float y = points[points.Count - 1].Y;

        mMinX = Math.Min(mMinX, x);
        mMinY = Math.Min(mMinY, y);

        mMaxX = Math.Max(mMaxX, x);
        mMaxY = Math.Max(mMaxY, y);
      }
      return points.ToArray();
    }

    private Vector2[] GetEdges(Vector2[] points)
    {
      Vector2 p1;
      Vector2 p2;

      List<Vector2> edges = new List<Vector2>(points.Length * 2);
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
      BoxF boxA = BoxF.FromLTRB(this.mMinX, this.mMinY, this.mMaxX, this.mMaxY);
      BoxF boxB = BoxF.FromLTRB(polygon.mMinX, polygon.mMinY, polygon.mMaxX, polygon.mMaxY);
      return boxA.IntersectsWith(boxB);

      //if (!boxA.IntersectsWith(boxB)) return false;

      //bool retval = true;
      //Vector2[] edgesA = mEdges;
      //Vector2[] edgesB = polygon.mEdges;

      //int edgeCountA = edgesA.Length;
      //int edgeCountB = edgesB.Length;
      //Vector2 edge = Vector2.Zero;

      //// Loop through all the edges of both polygons
      //for (int edgeIndex = 0; retval && (edgeIndex < edgeCountA + edgeCountB); edgeIndex++)
      //{
      //  if (edgeIndex < edgeCountA)
      //  {
      //    edge = edgesA[edgeIndex];
      //  }
      //  else
      //  {
      //    edge = edgesB[edgeIndex - edgeCountA];
      //  }

      //  // Find the axis perpendicular to the current edge
      //  Vector2 axis = new Vector2(-edge.Y, edge.X);
      //  axis.Normalize();

      //  // Find the projection of the polygon on the current axis
      //  float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
      //  ProjectPolygon(axis, this, ref minA, ref maxA);
      //  ProjectPolygon(axis, polygon, ref minB, ref maxB);

      //  // Check if the polygon projections are currentlty intersecting
      //  if (IntervalDistance(minA, maxA, minB, maxB) > 0)
      //    retval = false;
      //}

      //// return the result
      //return retval;
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
    private static void ProjectPolygon(Vector2 axis, Polygon hull, ref float min, ref float max)
    {
      // get the points
      Vector2[] points = hull.mPoints;

      // To project a point on an axis use the dot product
      float d = Vector2.Dot(axis, points[0]);
      min = d;
      max = d;
      for (int i = 0; i < points.Length; i++)
      {
        d = Vector2.Dot(points[i], axis);
        min = Math.Min(min, d);
        max = Math.Max(max, d);
      }
    }
  }
}
