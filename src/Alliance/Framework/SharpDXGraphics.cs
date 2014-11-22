using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GuiSystem;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Alliance
{
  public class SharpDXGraphics : GraphicsBase
  {
    private IGuiRenderer renderer;

    public SharpDXGraphics(IGuiRenderer renderer, GraphicsDevice device)
      : base(device)
    {
      this.renderer = renderer;
    }

    public override void DrawLine(float x0, float y0, float x1, float y1, Color color)
    {
      renderer.DrawLine(new GuiColor(color.A, color.R, color.G, color.B), x0, y0, x1, y1);
    }

    public override void FillPolygon(IEnumerable<Vector2> polygon, Color color)
    {
      float left = float.MaxValue;
      float top = float.MaxValue;
      float right = float.MinValue;
      float bottom = float.MinValue;

      foreach (var pt in polygon)
      {
        left = Math.Min(pt.X, left);
        top = Math.Min(pt.Y, top);
        right = Math.Max(pt.X, right);
        bottom = Math.Max(pt.Y, bottom);
      }

      var b = BoxF.FromLTRB(left, top, right, bottom);
      renderer.FillRectangle(new GuiColor(color.A, color.R, color.G, color.B), b.X, b.Y, b.Width, b.Height);
    }
  }
}
