using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit.Graphics;

namespace Alliance
{
  public static class Extensions
  {
    private class SpriteBatchDisposable : IDisposable
    {
      private SpriteBatch spriteBatch;
      private GraphicsDevice device;

      public SpriteBatchDisposable(SpriteBatch spriteBatch)
      {
        this.spriteBatch = spriteBatch;
        this.device = spriteBatch.GraphicsDevice;

        spriteBatch.Begin(SpriteSortMode.Deferred,
          device.BlendStates.NonPremultiplied,
          device.SamplerStates.LinearClamp,
          device.DepthStencilStates.None,
          device.RasterizerStates.CullBack,
          null);
      }

      void IDisposable.Dispose()
      {
        spriteBatch.End();
      }
    }

    public static IDisposable Batch(this SpriteBatch spriteBatch)
    {
      return new SpriteBatchDisposable(spriteBatch);
    }

    public static Color[] GetFrame(this Color[,] data, Rectangle src, int length)
    {
      Color[] arr = new Color[length];
      int x, y, i = 0;
      for (y = src.Y; y < src.Bottom; ++y)
      {
        for (x = src.X; x < src.Right; ++x)
        {
          arr[i++] = data[x, y];
        }
      }
      return arr;
    }
  }
}
