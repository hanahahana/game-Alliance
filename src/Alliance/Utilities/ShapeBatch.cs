using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Alliance.Utilities
{
  /// <summary>
  /// Enables a group of shapes (rectangles, lines and ellipses) to be drawn with the
  /// same settings. Shapes are drawn immediately.
  /// </summary>
  public class ShapeBatch
  {
    const int WedgeCount = 72;
    const int EllipseIndexCount = WedgeCount * 3;

    private enum BatchMode
    {
      BeginExpected,
      EndExpected,
    };

    private BatchMode mCurrentExpectation;
    private VertexDeclaration mVertexDeclaration;
    private short[] EllipseIndices;
    private GraphicsDevice mGraphicsDevice;
    private BasicEffect mEffect;
    private Matrix mProjection;
    private Matrix mView;
    private bool previousAlpha;
    private Blend previousSource;
    private Blend previousDest;

    /// <summary>
    /// Gets the GraphicsDevice associated with this shapes batch.
    /// </summary>
    public GraphicsDevice GraphicsDevice
    {
      get { return mGraphicsDevice; }
    }

    /// <summary>
    /// Gets the Effect used to draw the shapes.
    /// </summary>
    public BasicEffect Effect
    {
      get { return mEffect; }
    }

    /// <summary>
    /// Gets the Projection matrix. The default is set to a customized Orthographic
    /// off the center of the back buffer. This can be cleared away by passing in the
    /// inverse of this matrix to the Begin() call along with the desired (projection/view)
    /// matrix.
    /// </summary>
    public Matrix Projection
    {
      get { return mProjection; }
    }

    /// <summary>
    /// Gets the View matrix. The default is the identity matrix.
    /// </summary>
    public Matrix View
    {
      get { return mView; }
    }

    /// <summary>
    /// Creates a new shape batch off of the graphics device.
    /// </summary>
    /// <param name="device">The graphics device to create off of.</param>
    public ShapeBatch(GraphicsDevice device)
    {
      mGraphicsDevice = device;
      mEffect = new BasicEffect(device, null);

      mProjection = Matrix.CreateOrthographicOffCenter(0, device.PresentationParameters.BackBufferWidth,
          device.PresentationParameters.BackBufferHeight, 0, 0, 1);
      mView = Matrix.Identity;

      mEffect.Projection = mProjection;
      mEffect.View = mView;
      mEffect.VertexColorEnabled = true;

      mCurrentExpectation = BatchMode.BeginExpected;
      mVertexDeclaration = new VertexDeclaration(device, VertexPositionColor.VertexElements);

      int iIndex = 0;
      EllipseIndices = new short[EllipseIndexCount];
      for (int iPrim = 0; iPrim < WedgeCount; iPrim++)
      {
        EllipseIndices[iIndex++] = 0;
        EllipseIndices[iIndex++] = (short)(iPrim + 1);
        if (iPrim == WedgeCount - 1)
          EllipseIndices[iIndex++] = 1;
        else
          EllipseIndices[iIndex++] = (short)(iPrim + 2);
      }
    }

    /// <summary>
    /// Sets up the batch for drawing. Must be called before drawing.
    /// </summary>
    public void Begin()
    {
      Begin(SaveStateMode.None);
    }

    /// <summary>
    /// Sets up the batch for drawing. Must be called before drawing.
    /// </summary>
    /// <param name="saveStateMode">The rendering state options.</param>
    public void Begin(SaveStateMode saveStateMode)
    {
      Begin(saveStateMode, Matrix.Identity);
    }

    /// <summary>
    /// Sets up the batch for drawing. Must be called before drawing.
    /// </summary>
    /// <param name="worldMatrix">The matrix to use on all of the drawing.</param>
    public void Begin(Matrix worldMatrix)
    {
      Begin(SaveStateMode.None, worldMatrix);
    }

    /// <summary>
    /// Sets up the batch for drawing. Must be called before drawing.
    /// </summary>
    /// <param name="saveStateMode">The rendering state options.</param>
    /// <param name="worldMatrix">The matrix to use on all of the drawing.</param>
    public void Begin(SaveStateMode saveStateMode, Matrix worldMatrix)
    {
      if (mCurrentExpectation != BatchMode.BeginExpected)
        throw new Exception("Begin() is not expected");

      previousAlpha = mGraphicsDevice.RenderState.AlphaBlendEnable;
      previousSource = mGraphicsDevice.RenderState.SourceBlend;
      previousDest = mGraphicsDevice.RenderState.DestinationBlend;

      mGraphicsDevice.RenderState.AlphaBlendEnable = true;
      mGraphicsDevice.RenderState.SourceBlend = Blend.SourceAlpha;
      mGraphicsDevice.RenderState.DestinationBlend = Blend.InverseSourceAlpha;

      mGraphicsDevice.VertexDeclaration = mVertexDeclaration;
      mEffect.World = worldMatrix;
      mEffect.Begin(saveStateMode);
      mEffect.CurrentTechnique.Passes[0].Begin();
      mCurrentExpectation = BatchMode.EndExpected;
    }

    /// <summary>
    /// Ends the drawing for the current batch of shapes.
    /// </summary>
    public void End()
    {
      if (mCurrentExpectation != BatchMode.EndExpected)
        throw new Exception("End() is not expected");

      mEffect.CurrentTechnique.Passes[0].End();
      mEffect.End();
      mCurrentExpectation = BatchMode.BeginExpected;
      mGraphicsDevice.RenderState.AlphaBlendEnable = false;
    }

    private void AssertInsideBeginCall()
    {
      if (mCurrentExpectation == BatchMode.BeginExpected)
        throw new Exception("Begin() expected");
    }

    #region Draw Line
    /// <summary>
    /// Draws a line on the screen at the specified position. 
    /// Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="pt1">The starting point.</param>
    /// <param name="pt2">The ending point.</param>
    /// <param name="color">The color of the line.</param>
    public void DrawLine(Point pt1, Point pt2, Color color)
    {
      DrawLine(pt1.X, pt1.Y, pt2.X, pt2.Y, color);
    }

    /// <summary>
    /// Draws a line on the screen at the specified position. 
    /// Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="x1">The starting x-coordinate.</param>
    /// <param name="y1">The starting y-coordinate.</param>
    /// <param name="x2">The ending x-coordinate</param>
    /// <param name="y2">The ending y-coordinate.</param>
    /// <param name="color">The color of the line.</param>
    public void DrawLine(int x1, int y1, int x2, int y2, Color color)
    {
      DrawLine((float)x1, (float)y1, (float)x2, (float)y2, color);
    }

    /// <summary>
    /// Draws a line on the screen at the specified position. 
    /// Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="pt1">The starting point.</param>
    /// <param name="pt2">The ending point.</param>
    /// <param name="color">The color of the line.</param>
    public void DrawLine(Vector2 pt1, Vector2 pt2, Color color)
    {
      DrawLine(pt1.X, pt1.Y, pt2.X, pt2.Y, color);
    }

    /// <summary>
    /// Draws a line on the screen at the specified position. 
    /// Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="x1">The starting x-coordinate.</param>
    /// <param name="y1">The starting y-coordinate.</param>
    /// <param name="x2">The ending x-coordinate</param>
    /// <param name="y2">The ending y-coordinate.</param>
    /// <param name="color">The color of the line.</param>
    public void DrawLine(float x1, float y1, float x2, float y2, Color color)
    {
      AssertInsideBeginCall();
      VertexPositionColor[] line = CreateLine(x1, y1, x2, y2, color);
      mGraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, line, 0, line.Length / 2);
    }
    #endregion

    #region Draw Rectangle
    /// <summary>
    /// Draws a rectangle on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="rect">The rectangle to draw.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void DrawRectangle(Rectangle rect, Color color)
    {
      DrawRectangle(rect.X, rect.Y, rect.Width, rect.Height, color);
    }

    /// <summary>
    /// Draws a rectangle on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="position">The position of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void DrawRectangle(Point position, Point size, Color color)
    {
      DrawRectangle(position.X, position.Y, size.X, size.Y, color);
    }

    /// <summary>
    /// Draws a rectangle on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void DrawRectangle(int x, int y, Point size, Color color)
    {
      DrawRectangle(x, y, size.X, size.Y, color);
    }

    /// <summary>
    /// Draws a rectangle on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="position">The position of the rectangle.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void DrawRectangle(Point position, int width, int height, Color color)
    {
      DrawRectangle(position.X, position.Y, width, height, color);
    }

    /// <summary>
    /// Draws a rectangle on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void DrawRectangle(int x, int y, int width, int height, Color color)
    {
      DrawRectangle((float)x, (float)y, (float)width, (float)height, color);
    }

    /// <summary>
    /// Draws a rectangle on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="position">The position of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void DrawRectangle(Vector2 position, Vector2 size, Color color)
    {
      DrawRectangle(position.X, position.Y, size.X, size.Y, color);
    }

    /// <summary>
    /// Draws a rectangle on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void DrawRectangle(float x, float y, Vector2 size, Color color)
    {
      DrawRectangle(x, y, size.X, size.Y, color);
    }

    /// <summary>
    /// Draws a rectangle on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="position">The position of the rectangle.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void DrawRectangle(Vector2 position, float width, float height, Color color)
    {
      DrawRectangle(position.X, position.Y, width, height, color);
    }

    /// <summary>
    /// Draws a rectangle on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    public void DrawRectangle(float x, float y, float width, float height, Color color)
    {
      AssertInsideBeginCall();
      VertexPositionColor[] rect = CreateRectangle(x, y, width, height, color);
      mGraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineList, rect, 0, rect.Length / 2);
    }
    #endregion

    #region Draw Ellipse
    /// <summary>
    /// Draws an ellipse on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="rect">The ellipse properties.</param>
    /// <param name="color">The color of the ellipse.</param>
    public void DrawEllipse(Rectangle rect, Color color)
    {
      DrawEllipse(rect.X, rect.Y, rect.Width, rect.Height, color);
    }

    /// <summary>
    /// Draws an ellipse on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="position">The center of the ellipse.</param>
    /// <param name="size">The size of the ellipse.</param>
    /// <param name="color">The color of the ellipse.</param>
    public void DrawEllipse(Point position, Point size, Color color)
    {
      DrawEllipse(position.X, position.Y, size.X, size.Y, color);
    }

    /// <summary>
    /// Draws an ellipse on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="position">The center of the ellipse.</param>
    /// <param name="width">The width of the ellipse.</param>
    /// <param name="height">The height of the ellipse.</param>
    /// <param name="color">The color of the ellipse.</param>
    public void DrawEllipse(Point position, int width, int height, Color color)
    {
      DrawEllipse(position.X, position.Y, width, height, color);
    }

    /// <summary>
    /// Draws an ellipse on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="x">The center x-coordinate.</param>
    /// <param name="y">The center y-coordinate.</param>
    /// <param name="size">The size of the ellipse.</param>
    /// <param name="color">The color of the ellipse.</param>
    public void DrawEllipse(int x, int y, Point size, Color color)
    {
      DrawEllipse(x, y, size.X, size.Y, color);
    }

    /// <summary>
    /// Draws an ellipse on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="x">The center x-coordinate.</param>
    /// <param name="y">The center y-coordinate.</param>
    /// <param name="width">The width of the ellipse.</param>
    /// <param name="height">The height of the ellipse.</param>
    /// <param name="color">The color of the ellipse.</param>
    public void DrawEllipse(int x, int y, int width, int height, Color color)
    {
      DrawEllipse((float)x, (float)y, (float)width, (float)height, color);
    }

    /// <summary>
    /// Draws an ellipse on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="position">The center of the ellipse.</param>
    /// <param name="size">The size of the ellipse.</param>
    /// <param name="color">The color of the ellipse.</param>
    public void DrawEllipse(Vector2 position, Vector2 size, Color color)
    {
      DrawEllipse(position.X, position.Y, size.X, size.Y, color);
    }

    /// <summary>
    /// Draws an ellipse on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="position">The center of the ellipse.</param>
    /// <param name="width">The width of the ellipse.</param>
    /// <param name="height">The height of the ellipse.</param>
    /// <param name="color">The color of the ellipse.</param>
    public void DrawEllipse(Vector2 position, float width, float height, Color color)
    {
      DrawEllipse(position.X, position.Y, width, height, color);
    }

    /// <summary>
    /// Draws an ellipse on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="x">The center x-coordinate.</param>
    /// <param name="y">The center y-coordinate.</param>
    /// <param name="size">The size of the ellipse.</param>
    /// <param name="color">The color of the ellipse.</param>
    public void DrawEllipse(float x, float y, Vector2 size, Color color)
    {
      DrawEllipse(x, y, size.X, size.Y, color);
    }

    /// <summary>
    /// Draws an ellipse on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="x">The center x-coordinate.</param>
    /// <param name="y">The center y-coordinate.</param>
    /// <param name="width">The width of the ellipse.</param>
    /// <param name="height">The height of the ellipse.</param>
    /// <param name="color">The color of the ellipse.</param>
    public void DrawEllipse(float x, float y, float width, float height, Color color)
    {
      AssertInsideBeginCall();
      VertexPositionColor[] ellipse = CreateEllipse(x, y, width, height, color);
      mGraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, ellipse, 0, ellipse.Length - 1);
    }
    #endregion

    #region Fill Rectangle

    /// <summary>
    /// Fills a rectangle on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="rect">The rectangle to draw filled.</param>
    /// <param name="color">The fill color of the rectangle.</param>
    public void FillRectangle(Rectangle rect, Color color)
    {
      FillRectangle(rect.X, rect.Y, rect.Width, rect.Height, color);
    }

    /// <summary>
    /// Fills a rectangle on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="position">The position of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="color">The fill color of the rectangle.</param>
    public void FillRectangle(Point position, Point size, Color color)
    {
      FillRectangle(position.X, position.Y, size.X, size.Y, color);
    }

    /// <summary>
    /// Fills a rectangle on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="color">The fill color of the rectangle.</param>
    public void FillRectangle(int x, int y, Point size, Color color)
    {
      FillRectangle(x, y, size.X, size.Y, color);
    }

    /// <summary>
    /// Fills a rectangle on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="position">The position of the rectangle.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="color">The fill color of the rectangle.</param>
    public void FillRectangle(Point position, int width, int height, Color color)
    {
      FillRectangle(position.X, position.Y, width, height, color);
    }

    /// <summary>
    /// Fills a rectangle on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="color">The fill color of the rectangle.</param>
    public void FillRectangle(int x, int y, int width, int height, Color color)
    {
      FillRectangle((float)x, (float)y, (float)width, (float)height, color);
    }

    /// <summary>
    /// Fills a rectangle on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="position">The position of the rectangle.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="color">The fill color of the rectangle.</param>
    public void FillRectangle(Vector2 position, Vector2 size, Color color)
    {
      FillRectangle(position.X, position.Y, size.X, size.Y, color);
    }

    /// <summary>
    /// Fills a rectangle on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <param name="size">The size of the rectangle.</param>
    /// <param name="color">The fill color of the rectangle.</param>
    public void FillRectangle(float x, float y, Vector2 size, Color color)
    {
      FillRectangle(x, y, size.X, size.Y, color);
    }

    /// <summary>
    /// Fills a rectangle on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="position">The position of the rectangle.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="color">The fill color of the rectangle.</param>
    public void FillRectangle(Vector2 position, float width, float height, Color color)
    {
      FillRectangle(position.X, position.Y, width, height, color);
    }

    /// <summary>
    /// Fills a rectangle on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="x">The x-coordinate.</param>
    /// <param name="y">The y-coordinate.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="color">The fill color of the rectangle.</param>
    public void FillRectangle(float x, float y, float width, float height, Color color)
    {
      AssertInsideBeginCall();
      VertexPositionColor[] rect = CreateFilledRectangle(x, y, width, height, color);
      mGraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, rect, 0, rect.Length / 3);
    }
    #endregion

    #region Fill Ellipse

    /// <summary>
    /// Fills an ellipse on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="rect">The ellipse properties.</param>
    /// <param name="color">The fill color of the ellipse.</param>
    public void FillEllipse(Rectangle rect, Color color)
    {
      FillEllipse(rect.X, rect.Y, rect.Width, rect.Height, color);
    }

    /// <summary>
    /// Fills an ellipse on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="position">The center of the ellipse.</param>
    /// <param name="size">The size of the ellipse.</param>
    /// <param name="color">The fill color of the ellipse.</param>
    public void FillEllipse(Point position, Point size, Color color)
    {
      FillEllipse(position.X, position.Y, size.X, size.Y, color);
    }

    /// <summary>
    /// Fills an ellipse on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="position">The center of the ellipse.</param>
    /// <param name="width">The width of the ellipse.</param>
    /// <param name="height">The height of the ellipse.</param>
    /// <param name="color">The fill color of the ellipse.</param>
    public void FillEllipse(Point position, int width, int height, Color color)
    {
      FillEllipse(position.X, position.Y, width, height, color);
    }

    /// <summary>
    /// Fills an ellipse on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="x">The center x-coordinate.</param>
    /// <param name="y">The center y-coordinate.</param>
    /// <param name="size">The size of the ellipse.</param>
    /// <param name="color">The fill color of the ellipse.</param>
    public void FillEllipse(int x, int y, Point size, Color color)
    {
      FillEllipse(x, y, size.X, size.Y, color);
    }

    /// <summary>
    /// Fills an ellipse on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="x">The center x-coordinate.</param>
    /// <param name="y">The center y-coordinate.</param>
    /// <param name="width">The width of the ellipse.</param>
    /// <param name="height">The height of the ellipse.</param>
    /// <param name="color">The fill color of the ellipse.</param>
    public void FillEllipse(int x, int y, int width, int height, Color color)
    {
      FillEllipse((float)x, (float)y, (float)width, (float)height, color);
    }

    /// <summary>
    /// Fills an ellipse on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="position">The center of the ellipse.</param>
    /// <param name="size">The size of the ellipse.</param>
    /// <param name="color">The fill color of the ellipse.</param>
    public void FillEllipse(Vector2 position, Vector2 size, Color color)
    {
      FillEllipse(position.X, position.Y, size.X, size.Y, color);
    }

    /// <summary>
    /// Fills an ellipse on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="position">The center of the ellipse.</param>
    /// <param name="width">The width of the ellipse.</param>
    /// <param name="height">The height of the ellipse.</param>
    /// <param name="color">The fill color of the ellipse.</param>
    public void FillEllipse(Vector2 position, float width, float height, Color color)
    {
      FillEllipse(position.X, position.Y, width, height, color);
    }

    /// <summary>
    /// Fills an ellipse on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="x">The center x-coordinate.</param>
    /// <param name="y">The center y-coordinate.</param>
    /// <param name="size">The size of the ellipse.</param>
    /// <param name="color">The fill color of the ellipse.</param>
    public void FillEllipse(float x, float y, Vector2 size, Color color)
    {
      FillEllipse(x, y, size.X, size.Y, color);
    }

    /// <summary>
    /// Fills an ellipse on the screen. Upper-left corner is the default origin point.
    /// </summary>
    /// <param name="x">The center x-coordinate.</param>
    /// <param name="y">The center y-coordinate.</param>
    /// <param name="width">The width of the ellipse.</param>
    /// <param name="height">The height of the ellipse.</param>
    /// <param name="color">The fill color of the ellipse.</param>
    public void FillEllipse(float x, float y, float width, float height, Color color)
    {
      AssertInsideBeginCall();
      VertexPositionColor[] ellipse = CreateEllipse(x, y, width, height, color);
      mGraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, ellipse, 0, WedgeCount + 1,
        EllipseIndices, 0, WedgeCount);
    }
    #endregion

    /// <summary>
    /// Creates an array filled with vertices for drawing a rectangle.
    /// </summary>
    /// <param name="x">The x position of the rectangle.</param>
    /// <param name="y">The y position of the rectangle.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    /// <returns>An array of vertices.</returns>
    public static VertexPositionColor[] CreateRectangle(float x, float y, float width, float height, Color color)
    {
      List<VertexPositionColor> rect = new List<VertexPositionColor>(8);
      float t = 1;
      float right = x + width + t;
      float bottom = y + height + t;

      // horizontal
      rect.AddRange(CreateLine(x, y, right, y, color));
      rect.AddRange(CreateLine(x, y + height, right, y + height, color));

      // vertical
      rect.AddRange(CreateLine(x, y, x, bottom, color));
      rect.AddRange(CreateLine(x + width, y, x + width, bottom, color));

      return rect.ToArray();
    }

    /// <summary>
    /// Creates an array filled with vertices for drawing a filled rectangle.
    /// </summary>
    /// <param name="x">The x position of the rectangle.</param>
    /// <param name="y">The y position of the rectangle.</param>
    /// <param name="width">The width of the rectangle.</param>
    /// <param name="height">The height of the rectangle.</param>
    /// <param name="color">The color of the rectangle.</param>
    /// <returns>An array of vertices.</returns>
    public static VertexPositionColor[] CreateFilledRectangle(float x, float y, float width, float height, Color color)
    {
      VertexPositionColor[] retval = new VertexPositionColor[6];
      float t = 1;
      float right = x + width + t;
      float bottom = y + height + t;

      // triangle one
      retval[0].Position = new Vector3(x, y, 0);
      retval[1].Position = new Vector3(right, y, 0);
      retval[2].Position = new Vector3(x, bottom, 0);

      // triangle two
      retval[3].Position = new Vector3(right, y, 0);
      retval[4].Position = new Vector3(right, bottom, 0);
      retval[5].Position = new Vector3(x, bottom, 0);

      for (int i = 0; i < retval.Length; ++i)
        retval[i].Color = color;
      return retval;
    }

    /// <summary>
    /// Creates an array filled with vertices for drawing a line.
    /// </summary>
    /// <param name="x1">The starting x-coordinate.</param>
    /// <param name="y1">The starting y-coordinate.</param>
    /// <param name="x2">The ending x-coordinate.</param>
    /// <param name="y2">The ending y-coordinate.</param>
    /// <param name="color">The color of the line.</param>
    /// <returns>An array of vertices.</returns>
    public static VertexPositionColor[] CreateLine(float x1, float y1, float x2, float y2, Color color)
    {
      VertexPositionColor[] retval = new VertexPositionColor[2];
      retval[0].Position = new Vector3(x1, y1, 0);
      retval[0].Color = color;

      retval[1].Position = new Vector3(x2, y2, 0);
      retval[1].Color = color;
      return retval;
    }

    /// <summary>
    /// Creates an array filled with vertices for drawing an ellipse.
    /// </summary>
    /// <param name="x">The center x-coordinate of the ellipse.</param>
    /// <param name="y">The center y-coordinate of the ellipse.</param>
    /// <param name="width">The width of the ellipse.</param>
    /// <param name="height">The height of the ellipse.</param>
    /// <param name="color">The color of the ellipse.</param>
    /// <returns>An array of vertices.</returns>
    public static VertexPositionColor[] CreateEllipse(float x, float y, float width, float height, Color color)
    {
      float max = MathHelper.TwoPi;
      float step = max / (float)WedgeCount;

      VertexPositionColor[] vertices = new VertexPositionColor[WedgeCount + 1];
      int idx = 0;
      for (float t = 0.0f; t < max; t += step)
      {
        float i = (float)(x + width * Math.Cos(t));
        float j = (float)(y + height * Math.Sin(t));

        Vector3 v = new Vector3(i, j, 0);
        vertices[idx++] = new VertexPositionColor(v, color);
      }

      if (idx < vertices.Length)
        vertices[idx] = vertices[0];
      return vertices;
    }
  }
}
