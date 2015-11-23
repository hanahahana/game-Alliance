using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Alliance.Data;

namespace Alliance.Utilities
{
  public static class Shapes
  {
    private static bool IsPixelInitialized = false;

    private static Texture2D mInternalPixel;
    public static Texture2D InternalPixel
    {
      get { return mInternalPixel; }
    }

    public static void InitializePixelTexture(GraphicsDevice graphicsDevice)
    {
      if (!IsPixelInitialized)
      {
        // create pixel texture
        try
        {
          mInternalPixel = new Texture2D(graphicsDevice, 1, 1, 1, TextureUsage.None, SurfaceFormat.Color);
          Color[] pixels = new Color[1];
          pixels[0] = Color.White;
          mInternalPixel.SetData<Color>(pixels);
          mInternalPixel.GenerateMipMaps(TextureFilter.Anisotropic);
          IsPixelInitialized = true;
        }
        catch { }
      }
    }

    public static void DrawRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
    {
      DrawRectangle(spriteBatch, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, color);
    }

    public static void DrawRectangle(SpriteBatch spriteBatch, Box box, Color color)
    {
      DrawRectangle(spriteBatch, box.X, box.Y, box.Width, box.Height, color);
    }

    public static void DrawRectangle(SpriteBatch spriteBatch, int x, int y, int width, int height, Color color)
    {
      DrawRectangle(spriteBatch, (float)x, (float)y, (float)width, (float)height, color);
    }

    public static void DrawRectangle(SpriteBatch spriteBatch, BoxF box, Color color)
    {
      DrawRectangle(spriteBatch, box.X, box.Y, box.Width, box.Height, color);
    }

    public static void DrawRectangle(SpriteBatch spriteBatch, Vector2 position, Vector2 size, Color color)
    {
      DrawRectangle(spriteBatch, position.X, position.Y, size.X, size.Y, color);
    }

    public static void DrawRectangle(SpriteBatch spriteBatch, float x, float y, float width, float height, Color color)
    {
      if (!IsPixelInitialized)
        InitializePixelTexture(spriteBatch.GraphicsDevice);

      float thick = 1f;
      Vector2 horzScale = new Vector2(width + thick, thick);
      Vector2 vertScale = new Vector2(thick, height + thick);

      // stretch the pixel from [x, y] to [x + w, y]
      DrawLine(spriteBatch, new Vector2(x, y), horzScale, color);

      // stetch the pixel from [x, y + h] to [x + w, y + h] 
      DrawLine(spriteBatch, new Vector2(x, y + height), horzScale, color);

      // stretch the pixel from [x, y] to [x, y + h]
      DrawLine(spriteBatch, new Vector2(x, y), vertScale, color);

      // strecth the pixel from [x + w, y] to [x + w, y + h]
      DrawLine(spriteBatch, new Vector2(x + width, y), vertScale, color);
    }

    public static void DrawPolygon(SpriteBatch spriteBatch, Color color, Vector2[] vectors)
    {
      if (!IsPixelInitialized)
        InitializePixelTexture(spriteBatch.GraphicsDevice);

      // cycle through the vectors
      for (int i = 1; i < vectors.Length; i++)
      {
        // get the two vectors
        Vector2 v1 = vectors[i - 1];
        Vector2 v2 = vectors[i];

        // calculate the distance between the two vectors
        float distance = Vector2.Distance(v1, v2);

        // calculate the scale
        Vector2 scale = new Vector2(distance, 1f);

        // calculate the angle between the two vectors
        float angle = (float)Math.Atan2((double)(v2.Y - v1.Y), (double)(v2.X - v1.X));

        // stretch the pixel between the two vectors
        spriteBatch.Draw(
          InternalPixel,
          v1,
          null,
          color,
          angle,
          Vector2.Zero,
          scale,
          SpriteEffects.None,
          0f);
      }
    }

    public static void FillRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
    {
      FillRectangle(spriteBatch, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height, color);
    }

    public static void FillRectangle(SpriteBatch spriteBatch, Box box, Color color)
    {
      FillRectangle(spriteBatch, box.X, box.Y, box.Width, box.Height, color);
    }

    public static void FillRectangle(SpriteBatch spriteBatch, int x, int y, int width, int height, Color color)
    {
      FillRectangle(spriteBatch, (float)x, (float)y, (float)width, (float)height, color);
    }

    public static void FillRectangle(SpriteBatch spriteBatch, BoxF box, Color color)
    {
      FillRectangle(spriteBatch, box.X, box.Y, box.Width, box.Height, color);
    }

    public static void FillRectangle(SpriteBatch spriteBatch, Vector2 position, Vector2 size, Color color)
    {
      FillRectangle(spriteBatch, position.X, position.Y, size.X, size.Y, color);
    }

    public static void FillRectangle(SpriteBatch spriteBatch, float x, float y, float width, float height, Color color)
    {
      if (!IsPixelInitialized)
        InitializePixelTexture(spriteBatch.GraphicsDevice);

      Vector2 scale = new Vector2(width, height);
      DrawLine(spriteBatch, new Vector2(x, y), scale, color);
    }

    public static void DrawLine(SpriteBatch spriteBatch, Vector2 position, Vector2 scale, Color color)
    {
      if (!IsPixelInitialized)
        InitializePixelTexture(spriteBatch.GraphicsDevice);

      spriteBatch.Draw(mInternalPixel, position, null, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
    }
  }
}
