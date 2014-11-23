using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsSystem;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Alliance
{
  public class SharpDXGraphics : IGsGraphics, IGsTextMeasurer
  {
    enum Mode { None, Sprites, Primitives };
    private Mode currentMode = Mode.None;

    private SpriteBatch spriteBatch;
    private PrimitiveBatch<VertexPositionColor> primitiveBatch;
    private BasicEffect basicEffect;

    public GraphicsDevice GraphicsDevice { get; private set; }

    public SharpDXGraphics(GraphicsDevice device)
    {
      GsTextMeasurer.Register(this);
      GraphicsDevice = device;
      spriteBatch = new SpriteBatch(device);
      primitiveBatch = new PrimitiveBatch<VertexPositionColor>(device);
      basicEffect = new BasicEffect(device);
      basicEffect.VertexColorEnabled = true;
    }

    public void Begin()
    {
      basicEffect.Projection = Matrix.OrthoOffCenterRH(0,
        GraphicsDevice.Viewport.Width,
        GraphicsDevice.Viewport.Height,
        0, 0, 1);
    }

    public void End()
    {
      switch (currentMode)
      {
        case Mode.None:
          break;
        case Mode.Primitives:
          primitiveBatch.End();
          break;
        case Mode.Sprites:
          spriteBatch.End();
          break;
      }
      currentMode = Mode.None;
    }

    private void SwitchMode(Mode desired)
    {
      if (desired == currentMode)
        return;

      // end the current mode
      End();
      Begin(desired);
    }

    private void Begin(Mode desired)
    {
      switch (desired)
      {
        case Mode.Primitives:
          basicEffect.CurrentTechnique.Passes[0].Apply();
          primitiveBatch.Begin();
          break;
        case Mode.Sprites:
          spriteBatch.Begin(SpriteSortMode.Deferred,
            GraphicsDevice.BlendStates.NonPremultiplied,
            GraphicsDevice.SamplerStates.LinearClamp,
            GraphicsDevice.DepthStencilStates.None,
            GraphicsDevice.RasterizerStates.CullBack,
            null);
          break;
      }

      currentMode = desired;
    }

    public GsSize MeasureString(GsFont font, string text)
    {
      var f = font.Data as SpriteFont;
      var s = f.MeasureString(text);
      return new GsSize(s.X, s.Y);
    }

    private static VertexPositionColor CreateVertex(GsColor color, GsVector v)
    {
      return new VertexPositionColor(new Vector3(v.X, v.Y, 0), color.ToColor());
    }

    private static VertexPositionColor CreateVertex(GsColor color, float x, float y)
    {
      return new VertexPositionColor(new Vector3(x, y, 0), color.ToColor());
    }

    public void DrawLine(GsColor color, float x0, float y0, float x1, float y1)
    {
      SwitchMode(Mode.Primitives);
      primitiveBatch.DrawLine(CreateVertex(color, x0, y0), CreateVertex(color, x1, y1));
    }

    public void DrawRectangle(GsColor color, float x, float y, float width, float height)
    {
      SwitchMode(Mode.Primitives);
      
      var verts = new[]
      {
        CreateVertex(color,x - 1, y),
        CreateVertex(color,x + width, y),
        CreateVertex(color,x + width, y + height),
        CreateVertex(color,x, y + height)
      };

      primitiveBatch.DrawIndexed(PrimitiveType.LineList,
        new short[]{ 
          0, 1,
          1, 2,
          2, 3,
          3, 0}, verts);
    }

    public void FillRectangle(GsColor color, float x, float y, float width, float height)
    {
      SwitchMode(Mode.Primitives);
      primitiveBatch.DrawQuad(
        CreateVertex(color, x, y),
        CreateVertex(color, x + width - 1, y),
        CreateVertex(color, x + width - 1, y + height - 1),
        CreateVertex(color, x, y + height - 1));
    }

    public void DrawEllipse(GsColor color, float x, float y, float width, float height)
    {
      DrawPolygon(color, Geometry.CreateEllipse(x, y, width, height));
    }

    public void FillEllipse(GsColor color, float x, float y, float width, float height)
    {
      FillPolygon(color, Geometry.CreateEllipse(x, y, width, height));
    }

    public void DrawPolygon(GsColor color, GsVector[] pts)
    {
      SwitchMode(Mode.Primitives);
      for (int i = 1; i < pts.Length; ++i)
      {
        var p1 = pts[i - 1];
        var p2 = pts[i];
        primitiveBatch.DrawLine(CreateVertex(color, p1), CreateVertex(color, p2));
      }
    }

    public void FillPolygon(GsColor color, GsVector[] pts)
    {
      SwitchMode(Mode.Primitives);
      var indices = Ratcliff.Triangulate(pts).Select(i => (short)i).ToArray();
      primitiveBatch.DrawIndexed(PrimitiveType.TriangleList, indices,
        pts.Select(p => CreateVertex(color, p)).ToArray());
    }

    public void DrawString(GsFont font, string text, float x, float y, GsColor color)
    {
      SwitchMode(Mode.Sprites);
      spriteBatch.DrawString(font.Data as SpriteFont, text, new Vector2(x, y), color.ToColor());
    }

    public void DrawImage(GsImage image, float x, float y, float width, float height, GsColor tint)
    {
      SwitchMode(Mode.Sprites);
      spriteBatch.Draw(image.Data as Texture2D, new RectangleF(x, y, width, height), tint.ToColor());
    }

    public void DrawImage(GsImage image, GsRectangle dest, GsRectangle? source, GsVector origin, float angle, GsImageFlip flip, GsColor tint)
    {
      SwitchMode(Mode.Sprites);
      Rectangle? src = null;
      if (source.HasValue)
      {
        var s = source.Value;
        src = new Rectangle((int)s.X, (int)s.Y, (int)s.Width, (int)s.Height);
      }

      var dst = new RectangleF(dest.X, dest.Y, dest.Width, dest.Height);
      spriteBatch.Draw(image.Data as Texture2D,
        dst, src, tint.ToColor(), angle, new Vector2(origin.X, origin.Y), flip.ToSpriteEffects(), 0f);
    }
  }
}
