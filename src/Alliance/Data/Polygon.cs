using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Alliance.Parameters;
using Alliance.Utilities;
using Microsoft.Xna.Framework.Graphics;

namespace Alliance.Data
{
  public class Polygon
  {
    protected Vector2[] Points;
    protected Vector2[] Edges;

    protected float mMinX;
    protected float mMaxX;
    protected float mMinY;
    protected float mMaxY;

    public Polygon(Vector2[] points, Matrix transform)
    {
      Points = GetPoints(points, transform);
      Edges = GetEdges(Points);
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
        points.Add(Vector2.Transform(pt, transform));

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

    public bool IntersectsWith(Polygon polygon)
    {
      BoxF boxA = BoxF.FromLTRB(this.mMinX, this.mMinY, this.mMaxX, this.mMaxY);
      BoxF boxB = BoxF.FromLTRB(polygon.mMinX, polygon.mMinY, polygon.mMaxX, polygon.mMaxY);
      if (!boxA.IntersectsWith(boxB)) return false;

      bool retval = true;
      Vector2[] edgesA = Edges;
      Vector2[] edgesB = polygon.Edges;

      int edgeCountA = edgesA.Length;
      int edgeCountB = edgesB.Length;
      Vector2 edge = Vector2.Zero;

      // Loop through all the edges of both polygons
      for (int edgeIndex = 0; retval && (edgeIndex < edgeCountA + edgeCountB); edgeIndex++)
      {
        if (edgeIndex < edgeCountA)
        {
          edge = edgesA[edgeIndex];
        }
        else
        {
          edge = edgesB[edgeIndex - edgeCountA];
        }

        // Find the axis perpendicular to the current edge
        Vector2 axis = new Vector2(-edge.Y, edge.X);
        axis.Normalize();

        // Find the projection of the polygon on the current axis
        float minA = 0; float minB = 0; float maxA = 0; float maxB = 0;
        ProjectPolygon(axis, this, ref minA, ref maxA);
        ProjectPolygon(axis, polygon, ref minB, ref maxB);

        // Check if the polygon projections are currentlty intersecting
        if (IntervalDistance(minA, maxA, minB, maxB) > 0)
          retval = false;
      }

      // return the result
      return retval;
    }

    /// <summary>
    /// Calculate the distance between [minA, maxA] and [minB, maxB]. The distance will 
    /// be negative if the intervals overlap
    /// </summary>
    /// <param name="minA"></param>
    /// <param name="maxA"></param>
    /// <param name="minB"></param>
    /// <param name="maxB"></param>
    /// <returns></returns>
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
      Vector2[] points = hull.Points;

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

    public void Render(DrawParams dparams, Color color)
    {
      Shapes.DrawPolygon(dparams.SpriteBatch, color, Points);
    }
  }
}
