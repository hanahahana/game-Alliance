using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alliance
{
  public sealed class Polygon
  {
    private APoint[] mPoints;
    private APoint[] mEdges;

    private float mMinX;
    private float mMaxX;
    private float mMinY;
    private float mMaxY;

    public APoint[] Points
    {
      get
      {
        APoint[] copy = new APoint[this.mPoints.Length];
        Array.Copy(this.mPoints, copy, copy.Length);
        return copy;
      }
    }

    public APoint[] Edges
    {
      get
      {
        APoint[] copy = new APoint[this.mEdges.Length];
        Array.Copy(this.mEdges, copy, copy.Length);
        return copy;
      }
    }

    public Polygon(APoint[] points, AMatrix transform)
    {
      this.mPoints = this.GetPoints(points, transform);
      this.mEdges = this.GetEdges(this.mPoints);
    }

    private APoint[] GetPoints(APoint[] hull, AMatrix transform)
    {
      this.mMinX = 3.40282347E+38f;
      this.mMaxX = -3.40282347E+38f;
      this.mMinY = 3.40282347E+38f;
      this.mMaxY = -3.40282347E+38f;
      List<APoint> points = new List<APoint>(hull.Length);
      for (int i = 0; i < hull.Length; i++)
      {
        APoint pt = hull[i];
        points.Add(APoint.Transform(pt, transform));
        float x = points[points.Count - 1].X;
        float y = points[points.Count - 1].Y;
        this.mMinX = Math.Min(this.mMinX, x);
        this.mMinY = Math.Min(this.mMinY, y);
        this.mMaxX = Math.Max(this.mMaxX, x);
        this.mMaxY = Math.Max(this.mMaxY, y);
      }
      return points.ToArray();
    }

    private APoint[] GetEdges(APoint[] points)
    {
      List<APoint> edges = new List<APoint>(points.Length * 2);
      for (int i = 0; i < points.Length; i++)
      {
        APoint p = points[i];
        APoint p2;
        if (i + 1 >= points.Length)
        {
          p2 = points[0];
        }
        else
        {
          p2 = points[i + 1];
        }
        edges.Add(p2 - p);
      }
      return edges.ToArray();
    }

    public bool IntersectsWith(Polygon polygon)
    {
      ARect boxA = ARect.FromLTRB(this.mMinX, this.mMinY, this.mMaxX, this.mMaxY);
      ARect boxB = ARect.FromLTRB(polygon.mMinX, polygon.mMinY, polygon.mMaxX, polygon.mMaxY);
      return boxA.IntersectsWith(boxB);
    }

    private static float IntervalDistance(float minA, float maxA, float minB, float maxB)
    {
      if (minA < minB)
      {
        return minB - maxA;
      }
      return minA - maxB;
    }

    private static void ProjectPolygon(APoint axis, Polygon hull, ref float min, ref float max)
    {
      APoint[] points = hull.mPoints;
      float d = APoint.Dot(axis, points[0]);
      min = d;
      max = d;
      for (int i = 0; i < points.Length; i++)
      {
        d = APoint.Dot(points[i], axis);
        min = Math.Min(min, d);
        max = Math.Max(max, d);
      }
    }
  }
}
