using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Alliance
{
  public class PrimitiveBatchGraphics : GraphicsBase
  {
    private PrimitiveBatch<VertexPositionColor> batch;
    private BasicEffect effect;

    public PrimitiveBatchGraphics(GraphicsDevice device)
      : base(device)
    {
      batch = new PrimitiveBatch<VertexPositionColor>(device);
      effect = new BasicEffect(device);
      effect.VertexColorEnabled = true;
    }

    private static VertexPositionColor VPC(float x, float y, Color color)
    {
      return new VertexPositionColor(new Vector3(x, y, 0), color);
    }

    public override void DrawLine(float x0, float y0, float x1, float y1, Color color)
    {
      effect.Projection = Matrix.OrthoOffCenterRH(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, 1);
      effect.CurrentTechnique.Passes[0].Apply();

      batch.Begin();
      batch.DrawLine(VPC(x0, y0, color), VPC(x1, y1, color));
      batch.End();
    }

    public override void FillPolygon(IEnumerable<Vector2> polygon, Color color)
    {
      effect.Projection = Matrix.OrthoOffCenterRH(0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height, 0, 0, 1);
      effect.CurrentTechnique.Passes[0].Apply();

      var pts = polygon.ToArray();
      var indices = Ratcliff.Triangulate(pts).Cast<short>().ToArray();

      batch.Begin();
      batch.DrawIndexed(PrimitiveType.TriangleList, indices, pts.Select(v => VPC(v.X, v.Y, color)).ToArray());
      batch.End();
    }
  }
}
