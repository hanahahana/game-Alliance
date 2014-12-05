using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsSystem;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Alliance
{
  public class OpenGLGraphics : IGsGraphics, IGsTextMeasurer
  {
    private class AlphaBlendSettings : IDisposable
    {
      private bool enabled = false;

      public AlphaBlendSettings(GsColor color)
      {
        if (color.A < 255)
        {
          enabled = true;

          GL.Enable(EnableCap.Blend);
          GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        }
      }

      public void Dispose()
      {
        if (enabled)
        {
          GL.Disable(EnableCap.Blend);
        }
      }
    }

    static short[] rectangleIndices = new short[]
    { 
      0, 1,
      1, 2,
      2, 3,
      3, 0
    };

    static Dictionary<GsImageFlip, int[]> texCoords = new Dictionary<GsImageFlip, int[]>
    {
      { GsImageFlip.Both, new[] {2, 3, 0, 1} },
      { GsImageFlip.Horizontal, new[] {1, 0, 3, 2} },
      { GsImageFlip.None, new[] {0, 1, 2, 3} },
      { GsImageFlip.Vertical, new[] {3, 2, 1, 0} },
    };

    private Dictionary<string, Font> fonts;
    private Bitmap measureBitmap;
    private Graphics measureGraphics;
    private Dictionary<string, TextureData> stringTextures;
    private StringFormat centerCenter;

    public OpenGLGraphics()
    {
      fonts = new Dictionary<string, Font>();
      fonts["Tahoma"] = new Font("Tahoma", 10f);

      measureBitmap = new Bitmap(128, 128);
      measureGraphics = Graphics.FromImage(measureBitmap);
      stringTextures = new Dictionary<string, TextureData>();
      centerCenter = new StringFormat
      {
        Alignment = StringAlignment.Center,
        LineAlignment = StringAlignment.Center,
      };

      GsTextMeasurer.Register(this);
    }

    private IDisposable AlphaBlend(GsColor color)
    {
      return new AlphaBlendSettings(color);
    }

    public void Begin()
    {
      int[] viewPort = new int[4];
      GL.GetInteger(GetPName.Viewport, viewPort);

      GL.MatrixMode(MatrixMode.Projection);
      GL.PushMatrix();
      GL.LoadIdentity();
      GL.Ortho(0, viewPort[2], viewPort[3], 0, 0.0, 1.0);
      GL.MatrixMode(MatrixMode.Modelview);
      GL.PushMatrix();
      GL.LoadIdentity();
    }

    public void End()
    {
      GL.MatrixMode(MatrixMode.Projection);
      GL.PopMatrix();
      GL.MatrixMode(MatrixMode.Modelview);
      GL.PopMatrix();
    }

    private Font GetFont(GsFont font)
    {
      if (font == null)
      {
        return fonts["Tahoma"];
      }

      return (font.Data as Font) ?? fonts["Tahoma"];
    }

    private void DrawImage(TextureData data, float x, float y, float width, float height, GsColor color)
    {
      using (data.Bind())
      {
        GL.Begin(PrimitiveType.Quads);
        GL.Color4(color.R, color.G, color.B, color.A);

        GL.TexCoord2(0f, 0f);
        GL.Vertex2(x, y);

        GL.TexCoord2(1f, 0f);
        GL.Vertex2(x + width, y);

        GL.TexCoord2(1f, 1f);
        GL.Vertex2(x + width, y + height);

        GL.TexCoord2(0f, 1f);
        GL.Vertex2(x, y + height);

        GL.End();
      }
    }

    private void DrawSprite(TextureData data, GsRectangle dst, GsRectangle? source, GsVector origin, float rotation, GsImageFlip flip, GsColor color)
    {
      if (color.A <= 0)
        return;

      GsSize texSize = new GsSize(data.Bitmap.Width, data.Bitmap.Height);
      GsRectangle src = source.GetValueOrDefault(
        new GsRectangle(GsVector.Zero, texSize));

      using (data.Bind())
      {
        GL.Color4(color.R, color.G, color.B, color.A);

        // Setup the matrix
        GL.PushMatrix();
        if ((dst.X != 0) || (dst.Y != 0))
        {
          GL.Translate(dst.X, dst.Y, 0f);
        }

        if (rotation != 0)
        {
          GL.Rotate(MathHelper.RadiansToDegrees(rotation), 0, 0, 1);
        }

        if ((dst.Width != 0 && origin.X != 0) || (dst.Height != 0 && origin.Y != 0))
        {
          GL.Translate(
              -origin.X * (float)dst.Width / (float)src.Width,
              -origin.Y * (float)dst.Height / (float)src.Height, 0f);
        }

        // Calculate the points on the texture
        float x = src.X / texSize.Width;
        float y = src.Y / texSize.Height;
        float twidth = src.Width / texSize.Width;
        float theight = src.Height / texSize.Height;

        Vector2[] tx = new Vector2[]
        {
          new Vector2(x, y + theight),
          new Vector2(x + twidth, y + theight),
          new Vector2(x + twidth, y),
          new Vector2(x, y),
        };

        var tcs = texCoords[flip];
        int t = 0;

        GL.Begin(PrimitiveType.Quads);

        GL.TexCoord2(tx[tcs[t++]]);
        GL.Vertex2(0f, dst.Height);

        GL.TexCoord2(tx[tcs[t++]]);
        GL.Vertex2(dst.Width, dst.Height);

        GL.TexCoord2(tx[tcs[t++]]);
        GL.Vertex2(dst.Width, 0f);

        GL.TexCoord2(tx[tcs[t++]]);
        GL.Vertex2(0f, 0f);

        GL.End();
        GL.PopMatrix();
      }
    }

    public GsSize MeasureString(GsFont font, string text)
    {
      var sz = measureGraphics.MeasureString(text, GetFont(font));
      return new GsSize(sz.Width, sz.Height);
    }

    public void DrawLine(GsColor color, float x0, float y0, float x1, float y1)
    {
      using (AlphaBlend(color))
      {
        GL.Begin(PrimitiveType.Lines);
        GL.Color4(color.R, color.G, color.B, color.A);
        GL.Vertex2(x0, y0);
        GL.Vertex2(x1, y1);
        GL.End();
      }
    }

    public void DrawRectangle(GsColor color, float x, float y, float width, float height)
    {
      var pts = new[]
      {
        new Vector2(x, y),
        new Vector2(x + width, y),
        new Vector2(x + width, y + height),
        new Vector2(x, y + height),
      };

      using (AlphaBlend(color))
      {
        GL.Begin(PrimitiveType.Lines);
        GL.Color4(color.R, color.G, color.B, color.A);

        int i = 0;
        while (i < rectangleIndices.Length)
        {
          GL.Vertex2(pts[rectangleIndices[i++]]);
          GL.Vertex2(pts[rectangleIndices[i++]]);
        }

        GL.End();
      }
    }

    public void FillRectangle(GsColor color, float x, float y, float width, float height)
    {
      using (AlphaBlend(color))
      {
        GL.Begin(PrimitiveType.Quads);
        GL.Color4(color.R, color.G, color.B, color.A);
        GL.Vertex2(x, y);
        GL.Vertex2(x + width, y);
        GL.Vertex2(x + width, y + height);
        GL.Vertex2(x, y + height);
        GL.End();
      }
    }

    public void DrawEllipse(GsColor color, float x, float y, float width, float height)
    {
      var pts = Geometry.CreateEllipse(x, y, width, height);
      DrawPolygon(color, pts);
    }

    public void FillEllipse(GsColor color, float x, float y, float width, float height)
    {
      var pts = Geometry.CreateEllipse(x, y, width, height);
      FillPolygon(color, pts);
    }

    public void DrawPolygon(GsColor color, GsVector[] pts)
    {
      using (AlphaBlend(color))
      {
        GL.Begin(PrimitiveType.Lines);
        GL.Color4(color.R, color.G, color.B, color.A);
        for (int i = 1; i < pts.Length; ++i)
        {
          var p1 = pts[i - 1];
          var p2 = pts[i];
          GL.Vertex2(p1.X, p1.Y);
          GL.Vertex2(p2.X, p2.Y);
        }
        GL.End();
      }
    }

    public void FillPolygon(GsColor color, GsVector[] pts)
    {
      using (AlphaBlend(color))
      {
        var indices = Ratcliff.Triangulate(pts);
        GL.Begin(PrimitiveType.Triangles);
        GL.Color4(color.R, color.G, color.B, color.A);
        int i = 0;
        while (i < indices.Length)
        {
          var p1 = pts[indices[i++]];
          var p2 = pts[indices[i++]];
          var p3 = pts[indices[i++]];

          GL.Vertex2(p1.X, p1.Y);
          GL.Vertex2(p2.X, p2.Y);
          GL.Vertex2(p3.X, p3.Y);
        }
        GL.End();
      }
    }

    public void DrawString(GsFont font, string text, float x, float y, GsColor color)
    {
      var f = GetFont(font);

      TextureData d;
      if (!stringTextures.TryGetValue(text, out d))
      {
        var s = MeasureString(font, text);
        var bmp = new Bitmap((int)Math.Ceiling(s.Width) + 2, (int)Math.Ceiling(s.Height) + 2);
        using (var gra = Graphics.FromImage(bmp))
        {
          using (var b = new SolidBrush(Color.FromArgb(color.A, color.R, color.G, color.B)))
          {
            gra.SmoothingMode = SmoothingMode.AntiAlias;
            gra.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            gra.Clear(Color.Transparent);
            gra.DrawString(text, f, b, new RectangleF(0, 0, bmp.Width, bmp.Height), centerCenter);
          }
        }

        d = new TextureData
        {
          Bitmap = bmp,
          ID = Texture.BindImage(bmp),
        };

        stringTextures[text] = d;
      }

      DrawImage(d, x, y, d.Bitmap.Width, d.Bitmap.Height, color);
    }

    public void DrawImage(GsImage image, float x, float y, float width, float height, GsColor tint)
    {
      DrawImage(image.Data as TextureData, x, y, width, height, tint);
    }

    public void DrawImage(GsImage image, GsRectangle dest, GsRectangle? source, GsVector origin, float angle, GsImageFlip flip, GsColor tint)
    {
      DrawSprite(image.Data as TextureData, dest, source, origin, angle, flip, tint);
    }
  }
}
