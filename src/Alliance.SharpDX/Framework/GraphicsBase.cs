using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using SharpDX.Toolkit.Graphics;
using SharpDX;

namespace Alliance
{
  /// <summary>
  /// Provides a base for graphics classes.
  /// </summary>
  public abstract class GraphicsBase
  {
    /// <summary>The default number of slices to use when constructing an ellipse.</summary>
    internal const int DefaultSlices = 32;

    /// <summary>The amount to increment the angle by when construction an arc.</summary>
    internal const int AngleIncrement = 20;

    /// <summary>The amount to step when incrementing a delta.</summary>
    internal const float ThetaStep = MathHelper.TwoPi / DefaultSlices;

    /// <summary>
    /// Gets the GraphicsDevice that is tied to this graphics base.
    /// </summary>
    public GraphicsDevice GraphicsDevice { get; protected set; }

    /// <summary>
    /// Gets or sets the number of slices to use when constructing an ellipse. The default is 32.
    /// </summary>
    public int Slices { get; set; }

    /// <summary>
    /// Initializes all the settings to their default values.
    /// </summary>
    public GraphicsBase(GraphicsDevice device)
    {
      Slices = DefaultSlices;
      GraphicsDevice = device;
    }

    #region Helper Functions

    #region Create Arc

    internal static IEnumerable<Vector2> CreateArc(float cx, float cy, float width, float height, int startAngle, int sweepAngle)
    {
      List<Vector2> retval = new List<Vector2>();
      int endAngle = startAngle + sweepAngle;
      int sign = Math.Sign(endAngle - startAngle);
      int negsign = -sign;
      int d = sign * AngleIncrement;

      for (int angle = startAngle; angle.CompareTo(endAngle).Equals(negsign); angle += d)
      {
        retval.Add(new Vector2
        {
          X = cx + width * (float)MathHelper.Cos(angle),
          Y = cy + height * (float)MathHelper.Sin(angle),
        });
      }

      return retval;
    }

    #endregion

    #region Create Bezier

    internal static IEnumerable<Vector2> CreateBezier(float x1, float y1, float x2, float y2, float x3, float y3, float x4, float y4)
    {
      Vector2[] array = new Vector2[]
      {
        new Vector2(x1, y1),
        new Vector2(x2, y2),
        new Vector2(x3, y3),
        new Vector2(x4, y4),
      };
      return CreateBezier(array);
    }

    internal static IEnumerable<Vector2> CreateBezier(IEnumerable<Vector2> points)
    {
      Vector2[] array = points.ToArray();
      int len = array.Length;
      int count = len * 3;

      double[] input = new double[len * 2];
      double[] output = new double[count * 2];

      for (int i = 0; i < input.Length; i += 2)
      {
        int j = i / 2;
        input[i] = array[j].X;
        input[i + 1] = array[j].Y;
      }

      BeizerMaker.Bezier2D(ref input, count, ref output);

      List<Vector2> retval = new List<Vector2>();
      for (int i = 0; i < output.Length; i += 2)
      {
        retval.Add(new Vector2
        {
          X = (float)output[i],
          Y = (float)output[i + 1],
        });
      }

      return retval;
    }

    #endregion

    #region Create Round Rectangle

    internal static IEnumerable<Vector2> CreateRoundRect(BoxF rect, float radius, RectangleCorners corners)
    {
      float l = rect.Left;
      float t = rect.Top;
      float r = rect.Right;
      float b = rect.Bottom;

      float lr = rect.Left + radius;
      float tr = rect.Top + radius;
      float rr = rect.Right - radius;
      float br = rect.Bottom - radius;

      List<Vector2> polygon = new List<Vector2>();

      // upper-left
      if ((corners & RectangleCorners.TopLeft) != 0)
      {
        Vector2[] upleft_corner = new Vector2[3]
        {
          new Vector2(l, tr),
          new Vector2(l, t),
          new Vector2(lr, t),
        };
        polygon.AddRange(CreateBezier(upleft_corner));
      }
      else
      {
        polygon.Add(new Vector2(l, t));
      }

      // upper-right
      if ((corners & RectangleCorners.TopRight) != 0)
      {
        Vector2[] upright_corner = new Vector2[3]
        {
          new Vector2(rr, t),
          new Vector2(r, t),
          new Vector2(r, tr),
        };
        polygon.AddRange(CreateBezier(upright_corner));
      }
      else
      {
        polygon.Add(new Vector2(r, t));
      }

      // bottom-right
      if ((corners & RectangleCorners.BottomRight) != 0)
      {
        Vector2[] bottomright_corner = new Vector2[3]
        {
          new Vector2(r, br),
          new Vector2(r, b),
          new Vector2(rr, b),
        };
        polygon.AddRange(CreateBezier(bottomright_corner));
      }
      else
      {
        polygon.Add(new Vector2(r, b));
      }

      // bottom-left
      if ((corners & RectangleCorners.BottomLeft) != 0)
      {
        Vector2[] bottomleft_corner = new Vector2[3]
        {
          new Vector2(lr, b),
          new Vector2(l, b),
          new Vector2(l, br),
        };
        polygon.AddRange(CreateBezier(bottomleft_corner));
      }
      else
      {
        polygon.Add(new Vector2(l, b));
      }

      // close it up
      polygon.Add(polygon[0]);

      // return it!
      return polygon;
    }

    #endregion

    #region Create Ellipse

    internal static Vector2[] CreateEllipse(float h, float k, float width, float height, int slices)
    {
      // create a list of points of the circle
      float max = MathHelper.TwoPi;
      float step = max / (float)slices;
      List<Vector2> vectors = new List<Vector2>();
      for (float t = 0.0f; t < max; t += step)
      {
        float x = (float)(h + width * (float)MathHelper.Cos(t));
        float y = (float)(k + height * (float)MathHelper.Sin(t));
        vectors.Add(new Vector2(x, y));
      }
      vectors.Add(vectors[0]);

      // return the points
      return vectors.ToArray();
    }

    #endregion

    #region Create Donut

    internal static void CreateDonut(float cx, float cy, float outerRadius, float innerRadius, out int[] indices, out Vector2[] vertices)
    {
      List<Vector2> opts = new List<Vector2>();
      List<Vector2> ipts = new List<Vector2>();
      for (float theta = 0; theta < MathHelper.TwoPi; theta += ThetaStep)
      {
        float cos = (float)Math.Cos((double)theta);
        float sin = (float)Math.Sin((double)theta);

        float xOut = cx + (outerRadius * cos);
        float yOut = cy + (outerRadius * sin);

        float xIn = cx + (innerRadius * cos);
        float yIn = cy + (innerRadius * sin);

        opts.Add(new Vector2(xOut, yOut));
        ipts.Add(new Vector2(xIn, yIn));
      }

      opts.Add(opts[0]);
      ipts.Add(ipts[0]);

      List<Vector2> pointData = new List<Vector2>();
      List<int> indexData = new List<int>();

      int outerOffset = 0;
      pointData.AddRange(opts);

      int innerOffset = pointData.Count;
      pointData.AddRange(ipts);

      for (int i = 0; i < opts.Count - 1; ++i)
      {
        indexData.Add(innerOffset + i);
        indexData.Add(outerOffset + i);
        indexData.Add(outerOffset + i + 1);

        indexData.Add(innerOffset + i);
        indexData.Add(outerOffset + i + 1);
        indexData.Add(innerOffset + i + 1);
      }

      vertices = pointData.ToArray();
      indices = indexData.ToArray();
    }

    #endregion

    #endregion

    #region Draw Line

    /// <summary>
    /// Draws a line.
    /// </summary>
    /// <param name="x0">The starting x-coordinate.</param>
    /// <param name="y0">The starting y-coordinate.</param>
    /// <param name="x1">The ending x-coordinate.</param>
    /// <param name="y1">The ending y-coordinate.</param>
    /// <param name="color">The color of the line to draw.</param>
    public abstract void DrawLine(float x0, float y0, float x1, float y1, Color color);

    /// <summary>
    /// Draws a line.
    /// </summary>
    /// <param name="p0">The starting point of the line.</param>
    /// <param name="p1">The ending point of the line.</param>
    /// <param name="color">The color of the line to draw.</param>
    public void DrawLine(Vector2 p0, Vector2 p1, Color color) 
    { 
      DrawLine(p0.X, p0.Y, p1.X, p1.Y, color); 
    }

    #endregion

    #region Draw Arc

    /// <summary>
    /// Draws an arc representing a portion of an ellipse. 
    /// </summary>
    /// <param name="cx">The center x-coordinate of the arc (or ellipse).</param>
    /// <param name="cy">The center y-coordinate of the arc (or ellipse).</param>
    /// <param name="width">The width of the arc (or ellipse).</param>
    /// <param name="height">The height of the arc (or ellipse).</param>
    /// <param name="startAngle">Angle in degrees measured clockwise from the x-axis to the starting point of the arc.</param>
    /// <param name="sweepAngle">Angle in degrees measured clockwise from the startAngle parameter to ending point of the arc.</param>
    /// <param name="color">The color of the arc</param>
    public virtual void DrawArc(float cx, float cy, float width, float height, int startAngle, int sweepAngle, Color color)
    {
      IEnumerable<Vector2> arc = CreateArc(cx, cy, width, height, startAngle, sweepAngle);
      DrawPolygon(arc, color);
    }

    /// <summary>
    /// Draws an arc representing a portion of an ellipse.
    /// </summary>
    /// <param name="center">The center of the arc (or ellipse).</param>
    /// <param name="size">The size of the arc (or ellipse).</param>
    /// <param name="startAngle">Angle in degrees measured clockwise from the x-axis to the starting point of the arc.</param>
    /// <param name="sweepAngle">Angle in degrees measured clockwise from the startAngle parameter to ending point of the arc.</param>
    /// <param name="color">The color of the arc</param>
    public void DrawArc(Vector2 center, Vector2 size, int startAngle, int sweepAngle, Color color)
    {
      DrawArc(center.X, center.Y, size.X, size.Y, startAngle, sweepAngle, color);
    }

    /// <summary>
    /// Draws an arc representing a portion of an ellipse.
    /// </summary>
    /// <param name="center">The center of the arc (or ellipse).</param>
    /// <param name="size">The size of the arc (or ellipse).</param>
    /// <param name="startAngle">Angle in degrees measured clockwise from the x-axis to the starting point of the arc.</param>
    /// <param name="sweepAngle">Angle in degrees measured clockwise from the startAngle parameter to ending point of the arc.</param>
    /// <param name="color">The color of the arc</param>
    public void DrawArc(Vector2 center, SizeF size, int startAngle, int sweepAngle, Color color)
    {
      DrawArc(center.X, center.Y, size.Width, size.Height, startAngle, sweepAngle, color);
    }

    /// <summary>
    /// Draws an arc representing a portion of an ellipse. 
    /// </summary>
    /// <param name="rect">The bounds of the arc (or ellipse). The x,y must correspond to the center of the arc.</param>
    /// <param name="startAngle">Angle in degrees measured clockwise from the x-axis to the starting point of the arc.</param>
    /// <param name="sweepAngle">Angle in degrees measured clockwise from the startAngle parameter to ending point of the arc.</param>
    /// <param name="color">The color of the arc</param>
    public virtual void DrawArc(BoxF rect, int startAngle, int sweepAngle, Color color)
    {
      DrawArc(rect.X, rect.Y, rect.Width, rect.Height, startAngle, sweepAngle, color);
    }

    #endregion

    #region Draw Beizer

    /// <summary>
    /// Draws a beizer curve.
    /// </summary>
    /// <param name="x0">The x-coordinate of the starting point of the curve.</param>
    /// <param name="y0">The y-coordinate of the starting point of the curve.</param>
    /// <param name="x1">The x-coordinate of the first control point of the curve.</param>
    /// <param name="y1">The y-coordinate of the first control point of the curve.</param>
    /// <param name="x2">The x-coordinate of the second control point of the curve.</param>
    /// <param name="y2">The y-coordinate of the second control point of the curve</param>
    /// <param name="x3">The x-coordinate of the ending point of the curve.</param>
    /// <param name="y3">The y-coordinate of the ending point of the curve.</param>
    /// <param name="color">The color of the curve.</param>
    public virtual void DrawBeizer(float x0, float y0, float x1, float y1, float x2, float y2, float x3, float y3, Color color)
    {
      IEnumerable<Vector2> beizer = CreateBezier(x0, y0, x1, y1, x2, y2, x3, y3);
      DrawPolygon(beizer, color);
    }

    /// <summary>
    /// Draws a beizer curve.
    /// </summary>
    /// <param name="controlPoints">The points to use for interpolating the curve.</param>
    /// <param name="color">The color of the curve.</param>
    public virtual void DrawBeizer(IEnumerable<Vector2> controlPoints, Color color)
    {
      IEnumerable<Vector2> beizer = CreateBezier(controlPoints);
      DrawPolygon(beizer.ToArray(), color);
    }

    #endregion

    #region Draw Rectangle

    /// <summary>
    /// Draws a rectangle.
    /// </summary>
    /// <param name="x">The upper-left x-coordinate.</param>
    /// <param name="y">The upper-left y-coordinate.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public virtual void DrawRectangle(float x, float y, float width, float height, Color color)
    {
      BoxF rect = new BoxF(x, y, width, height);
      DrawPolygon(CreateRoundRect(rect, 0, RectangleCorners.None).ToArray(), color);
    }

    /// <summary>
    /// Draws a rectangle.
    /// </summary>
    /// <param name="location">The upper-left corner of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void DrawRectangle(Vector2 location, SizeF size, Color color) 
    { 
      DrawRectangle(location.X, location.Y, size.Width, size.Height, color); 
    }

    /// <summary>
    /// Draws a rectangle.
    /// </summary>
    /// <param name="location">The upper-left corner of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void DrawRectangle(Vector2 location, Vector2 size, Color color) 
    { 
      DrawRectangle(location.X, location.Y, size.X, size.Y, color); 
    }

    /// <summary>
    /// Draws a rectangle.
    /// </summary>
    /// <param name="rect">The bounds of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void DrawRectangle(BoxF rect, Color color) 
    { 
      DrawRectangle(rect.X, rect.Y, rect.Width, rect.Height, color); 
    }

    #endregion

    #region Draw Round Rectangle

    /// <summary>
    /// Draws a rounded rectangle.
    /// </summary>
    /// <param name="x">The upper-left x-coordinate.</param>
    /// <param name="y">The upper-left y-coordinate.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="radius">The radius of each corner of the rectangle.</param>
    /// <param name="part">The parts of the rectangle to round.</param>
    /// <param name="color">The color of the rectangle.</param>
    public virtual void DrawRoundRectangle(float x, float y, float width, float height, float radius, RectangleCorners part, Color color)
    {
      BoxF rect = new BoxF(x, y, width, height);
      DrawPolygon(CreateRoundRect(rect, radius, part), color);
    }

    /// <summary>
    /// Draws a rounded rectangle.
    /// </summary>
    /// <param name="x">The upper-left x-coordinate.</param>
    /// <param name="y">The upper-left y-coordinate.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="radius">The radius of each corner of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void DrawRoundRectangle(float x, float y, float width, float height, float radius, Color color)
    {
      DrawRoundRectangle(x, y, width, height, radius, RectangleCorners.All, color);
    }

    /// <summary>
    /// Draws a rounded rectangle.
    /// </summary>
    /// <param name="location">The upper-left corner of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="radius">The radius of each corner of the rectangle.</param>
    /// <param name="part">The parts of the rectangle to round.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void DrawRoundRectangle(Vector2 location, SizeF size, float radius, RectangleCorners part, Color color)
    {
      DrawRoundRectangle(location.X, location.Y, size.Width, size.Height, radius, part, color);
    }

    /// <summary>
    /// Draws a rounded rectangle.
    /// </summary>
    /// <param name="location">The upper-left corner of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="radius">The radius of each corner of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void DrawRoundRectangle(Vector2 location, SizeF size, float radius, Color color) 
    {
      DrawRoundRectangle(location.X, location.Y, size.Width, size.Height, radius, RectangleCorners.All, color); 
    }

    /// <summary>
    /// Draws a rounded rectangle.
    /// </summary>
    /// <param name="location">The upper-left corner of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="radius">The radius of each corner of the rectangle.</param>
    /// <param name="part">The parts of the rectangle to round.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void DrawRoundRectangle(Vector2 location, Vector2 size, float radius, RectangleCorners part, Color color)
    {
      DrawRoundRectangle(location.X, location.Y, size.X, size.Y, radius, part, color);
    }

    /// <summary>
    /// Draws a rounded rectangle.
    /// </summary>
    /// <param name="location">The upper-left corner of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="radius">The radius of each corner of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void DrawRoundRectangle(Vector2 location, Vector2 size, float radius, Color color) 
    {
      DrawRoundRectangle(location.X, location.Y, size.X, size.Y, radius, RectangleCorners.All, color); 
    }

    /// <summary>
    /// Draws a rectangle.
    /// </summary>
    /// <param name="rect">The bounds of the rectangle.</param>
    /// <param name="radius">The radius of each corner of the rectangle.</param>
    /// <param name="part">The parts of the rectangle to round.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void DrawRoundRectangle(BoxF rect, float radius, RectangleCorners part, Color color)
    {
      DrawRoundRectangle(rect.X, rect.Y, rect.Width, rect.Height, radius, part, color);
    }

    /// <summary>
    /// Draws a rectangle.
    /// </summary>
    /// <param name="rect">The bounds of the rectangle.</param>
    /// <param name="radius">The radius of each corner of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void DrawRoundRectangle(BoxF rect, float radius, Color color) 
    {
      DrawRoundRectangle(rect.X, rect.Y, rect.Width, rect.Height, radius, RectangleCorners.All, color); 
    }

    #endregion

    #region Draw Circle

    /// <summary>
    /// Draws a circle.
    /// </summary>
    /// <param name="h">The center x-coordinate of the circle.</param>
    /// <param name="k">The center y-coordinate of the circle.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="color">The color of the circle.</param>
    public void DrawCircle(float h, float k, float radius, Color color)
    {
      DrawEllipse(h, k, radius, radius, color);
    }

    /// <summary>
    /// Draws a circle.
    /// </summary>
    /// <param name="center">The center of the circle.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="color">The color of the circle.</param>
    public void DrawCircle(Vector2 center, float radius, Color color) { DrawCircle(center.X, center.Y, radius, color); }

    #endregion

    #region Draw Ellipse

    /// <summary>
    /// Draws an ellipse.
    /// </summary>
    /// <param name="h">The center x-coordinate of the ellipse.</param>
    /// <param name="k">The center y-coordinate of the ellipse.</param>
    /// <param name="width">The width of the ellipse.</param>
    /// <param name="height">The height of the ellipse.</param>
    /// <param name="color">The color of the ellipse.</param>
    public virtual void DrawEllipse(float h, float k, float width, float height, Color color)
    {
      Vector2[] ellipse = CreateEllipse(h, k, width, height, Slices);
      DrawPolygon(ellipse, color);
    }

    /// <summary>
    /// Draws an ellipse.
    /// </summary>
    /// <param name="center">The center of the ellipse</param>
    /// <param name="size">The size of the ellipse.</param>
    /// <param name="color">The color of the ellipse.</param>
    public void DrawEllipse(Vector2 center, SizeF size, Color color) { DrawEllipse(center.X, center.Y, size.Width, size.Height, color); }

    /// <summary>
    /// Draws an ellipse.
    /// </summary>
    /// <param name="center">The center of the ellipse</param>
    /// <param name="size">The size of the ellipse.</param>
    /// <param name="color">The color of the ellipse.</param>
    public void DrawEllipse(Vector2 center, Vector2 size, Color color) { DrawEllipse(center.X, center.Y, size.X, size.Y, color); }

    /// <summary>
    /// Draws an ellipse.
    /// </summary>
    /// <param name="rect">The bounds of the ellipse.</param>
    /// <param name="color">The color of the ellipse.</param>
    public void DrawEllipse(BoxF rect, Color color) { DrawEllipse(rect.X, rect.Y, rect.Width, rect.Height, color); }

    #endregion

    #region Draw Polygon

    /// <summary>
    /// Draws a polygon.
    /// </summary>
    /// <param name="polygon">The collection of points to draw.</param>
    /// <param name="color">The color of the polygon.</param>
    public virtual void DrawPolygon(IEnumerable<Vector2> polygon, Color color)
    {
      Vector2[] data = polygon.ToArray();
      for (int i = 1; i < data.Length; ++i)
      {
        Vector2 v0 = data[i - 1];
        Vector2 v1 = data[i];
        DrawLine(v0, v1, color);
      }
    }

    #endregion

    #region Fill Rectangle

    /// <summary>
    /// Fills a rectangle.
    /// </summary>
    /// <param name="x">The upper-left x-coordinate.</param>
    /// <param name="y">The upper-left y-coordinate.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public virtual void FillRectangle(float x, float y, float width, float height, Color color)
    {
      Vector2[] vectors = new Vector2[4];

      vectors[0] = new Vector2(x, y);
      vectors[1] = new Vector2(x, y + height);
      vectors[2] = new Vector2(x + width, y + height);
      vectors[3] = new Vector2(x + width, y);

      FillPolygon(vectors, color);
    }

    /// <summary>
    /// Fills a rectangle.
    /// </summary>
    /// <param name="location">The upper-left corner of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void FillRectangle(Vector2 location, SizeF size, Color color) 
    { 
      FillRectangle(location.X, location.Y, size.Width, size.Height, color); 
    }

    /// <summary>
    /// Fills a rectangle.
    /// </summary>
    /// <param name="location">The upper-left corner of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void FillRectangle(Vector2 location, Vector2 size, Color color) 
    { 
      FillRectangle(location.X, location.Y, size.X, size.Y, color); 
    }

    /// <summary>
    /// Fills a rectangle.
    /// </summary>
    /// <param name="rect">The bounds of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void FillRectangle(BoxF rect, Color color) 
    { 
      FillRectangle(rect.X, rect.Y, rect.Width, rect.Height, color); 
    }

    #endregion

    #region Fill Round Rectangle

    /// <summary>
    /// Fills a rectangle.
    /// </summary>
    /// <param name="x">The upper-left x-coordinate.</param>
    /// <param name="y">The upper-left y-coordinate.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="radius">The radius of each corner of the rectangle.</param>
    /// <param name="part">The parts of the rectangle to round.</param>
    /// <param name="color">The color of the rectangle.</param>
    public virtual void FillRoundRectangle(float x, float y, float width, float height, float radius, RectangleCorners part, Color color)
    {
      BoxF rect = new BoxF(x, y, width, height);
      FillPolygon(CreateRoundRect(rect, radius, part), color);
    }

    /// <summary>
    /// Fills a rectangle.
    /// </summary>
    /// <param name="x">The upper-left x-coordinate.</param>
    /// <param name="y">The upper-left y-coordinate.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="radius">The radius of each corner of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void FillRoundRectangle(float x, float y, float width, float height, float radius, Color color)
    {
      FillRoundRectangle(x, y, width, height, radius, RectangleCorners.All, color);
    }    

    /// <summary>
    /// Fills a rectangle.
    /// </summary>
    /// <param name="location">The upper-left corner of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="radius">The radius of each corner of the rectangle.</param>
    /// <param name="part">The parts of the rectangle to round.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void FillRoundRectangle(Vector2 location, SizeF size, float radius, RectangleCorners part, Color color)
    {
      FillRoundRectangle(location.X, location.Y, size.Width, size.Height, radius, part, color);
    }

    /// <summary>
    /// Fills a rectangle.
    /// </summary>
    /// <param name="location">The upper-left corner of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="radius">The radius of each corner of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void FillRoundRectangle(Vector2 location, SizeF size, float radius, Color color) 
    {
      FillRoundRectangle(location.X, location.Y, size.Width, size.Height, radius, RectangleCorners.All, color); 
    }

    /// <summary>
    /// Fills a rectangle.
    /// </summary>
    /// <param name="location">The upper-left corner of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="radius">The radius of each corner of the rectangle.</param>
    /// <param name="part">The parts of the rectangle to round.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void FillRoundRectangle(Vector2 location, Vector2 size, float radius, RectangleCorners part, Color color)
    {
      FillRoundRectangle(location.X, location.Y, size.X, size.Y, radius, part, color);
    }

    /// <summary>
    /// Fills a rectangle.
    /// </summary>
    /// <param name="location">The upper-left corner of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="radius">The radius of each corner of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void FillRoundRectangle(Vector2 location, Vector2 size, float radius, Color color) 
    {
      FillRoundRectangle(location.X, location.Y, size.X, size.Y, radius, RectangleCorners.All, color); 
    }

    /// <summary>
    /// Fills a rectangle.
    /// </summary>
    /// <param name="rect">The bounds of the rectangle.</param>
    /// <param name="radius">The radius of each corner of the rectangle.</param>
    /// <param name="part">The parts of the rectangle to round.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void FillRoundRectangle(BoxF rect, float radius, RectangleCorners part, Color color)
    {
      FillRoundRectangle(rect.X, rect.Y, rect.Width, rect.Height, radius, part, color);
    }

    /// <summary>
    /// Fills a rectangle.
    /// </summary>
    /// <param name="rect">The bounds of the rectangle.</param>
    /// <param name="radius">The radius of each corner of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void FillRoundRectangle(BoxF rect, float radius, Color color) 
    {
      FillRoundRectangle(rect.X, rect.Y, rect.Width, rect.Height, radius, RectangleCorners.All, color); 
    }

    #endregion

    #region Fill Circle

    /// <summary>
    /// Fills a circle.
    /// </summary>
    /// <param name="h">The center x-coordinate of the circle.</param>
    /// <param name="k">The center y-coordinate of the circle.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="color">The color of the circle.</param>
    public void FillCircle(float h, float k, float radius, Color color)
    {
      FillEllipse(h, k, radius, radius, color);
    }

    /// <summary>
    /// Fills a circle
    /// </summary>
    /// <param name="center">The center of the circle.</param>
    /// <param name="radius">The radius of the circle.</param>
    /// <param name="color">The color of the circle.</param>
    public void FillCircle(Vector2 center, float radius, Color color) 
    { 
      FillCircle(center.X, center.Y, radius, color); 
    }

    #endregion

    #region Fill Ellipse

    /// <summary>
    /// Fills an ellipse.
    /// </summary>
    /// <param name="h">The center x-coordinate of the ellipse.</param>
    /// <param name="k">The center y-coordinate of the ellipse.</param>
    /// <param name="width">The width of the ellipse.</param>
    /// <param name="height">The height of the ellipse.</param>
    /// <param name="color">The color of the ellipse.</param>
    public virtual void FillEllipse(float h, float k, float width, float height, Color color)
    {
      Vector2[] points = CreateEllipse(h, k, width, height, Slices);
      FillPolygon(points, color);
    }

    /// <summary>
    /// Fills an ellipse.
    /// </summary>
    /// <param name="center">The center of the ellipse</param>
    /// <param name="size">The size of the ellipse.</param>
    /// <param name="color">The color of the ellipse.</param>
    public void FillEllipse(Vector2 center, SizeF size, Color color) 
    { 
      FillEllipse(center.X, center.Y, size.Width, size.Height, color); 
    }

    /// <summary>
    /// Fills an ellipse.
    /// </summary>
    /// <param name="center">The center of the ellipse</param>
    /// <param name="size">The size of the ellipse.</param>
    /// <param name="color">The color of the ellipse.</param>
    public void FillEllipse(Vector2 center, Vector2 size, Color color) 
    { 
      FillEllipse(center.X, center.Y, size.X, size.Y, color); 
    }

    /// <summary>
    /// Fills an ellipse.
    /// </summary>
    /// <param name="rect">The bounds of the ellipse.</param>
    /// <param name="color">The color of the ellipse.</param>
    public void FillEllipse(BoxF rect, Color color) 
    { 
      FillEllipse(rect.X, rect.Y, rect.Width, rect.Height, color); 
    }

    #endregion

    #region Fill Polygon

    /// <summary>
    /// Fills a polygon.
    /// </summary>
    /// <param name="polygon">The polygon to fill.</param>
    /// <param name="color">The color to fill the polygon with.</param>
    public abstract void FillPolygon(IEnumerable<Vector2> polygon, Color color);

    #endregion

    /// <summary>
    /// Prepares for rendereing.
    /// </summary>
    public virtual void Begin() { }

    /// <summary>
    /// Ends rendering.
    /// </summary>
    public virtual void End() { }
  }
}
