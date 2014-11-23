using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using GraphicsSystem;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Alliance
{
  public static class OpenGLExtensions
  {
    static readonly int PixelDataSize;
    static OpenGLExtensions()
    {
      PixelDataSize = Marshal.SizeOf(typeof(PixelData));
    }

    private class TextureBinder : IDisposable
    {
      public TextureBinder(TextureData data)
      {
        GL.Enable(EnableCap.Texture2D);
        GL.Enable(EnableCap.Blend);
        GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
        GL.BindTexture(TextureTarget.Texture2D, data.ID);
      }

      public void Dispose()
      {
        GL.Disable(EnableCap.Blend);
        GL.Disable(EnableCap.Texture2D);
      }
    }

    public static IDisposable Bind(this TextureData data)
    {
      return new TextureBinder(data);
    }

    public static T[,] To2D<T>(this T[] arr, int columns, int rows)
    {
      T[,] a = new T[columns, rows];
      int c = 0, r = 0;
      for (int i = 0; i < arr.Length; ++i)
      {
        a[c, r] = arr[i];
        ++c;
        if (c == columns)
        {
          c = 0;
          r++;
        }
      }
      return a;
    }

    public static GsColor ToGsColor(this PixelData data)
    {
      return new GsColor(data.A, data.R, data.G, data.B);
    }

    public static PixelData ToPixelData(this GsColor color)
    {
      return new PixelData { A = color.A, B = color.B, G = color.G, R = color.R };
    }

    public static GsVector[] CreateConvexHull(this Bitmap texture)
    {
      var colorData = texture.GetPixels();

      List<GsVector> pixels = new List<GsVector>(colorData.Length);
      int x, y;

      for (x = 0; x < texture.Width; ++x)
      {
        for (y = 0; y < texture.Height; ++y)
        {
          var color = colorData[x + (y * texture.Width)];
          if (color.A > 250)
          {
            pixels.Add(new GsVector(x, y));
          }
        }
      }

      GsVector[] polygon = pixels.ToArray();
      GsVector[] H = new GsVector[polygon.Length];

      ChainConvexHull.ComputeHull(polygon, polygon.Length, ref H);
      return H;
    }

    public unsafe static void SetPixels(this Bitmap bitmap, PixelData[] data)
    {
      Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
      BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadWrite, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
      var bytes = data.SelectMany(p => new byte[] { p.B, p.G, p.R, p.A }).ToArray();
      Marshal.Copy(bytes, 0, bmpData.Scan0, bytes.Length);
      bitmap.UnlockBits(bmpData);
    }

    public unsafe static PixelData[] GetPixels(this Bitmap bitmap)
    {
      // Lock the bitmap's bits.  
      Rectangle rect = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
      BitmapData bmpData = bitmap.LockBits(rect, ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

      // Get the address of the first line.
      IntPtr ptr = bmpData.Scan0;

      // Declare an array to hold the bytes of the bitmap.
      int bytes = Math.Abs(bmpData.Stride) * bitmap.Height;
      byte[] rgbValues = new byte[bytes];

      // Copy the RGB values into the array.
      Marshal.Copy(ptr, rgbValues, 0, bytes);

      // Unlock the bits.
      bitmap.UnlockBits(bmpData);

      var d = new PixelData[rgbValues.Length / PixelDataSize];
      fixed (byte* pBuffer = rgbValues)
      {
        for (int i = 0; i < d.Length; i++)
        {
          d[i] = ((PixelData*)pBuffer)[i];
        }
      }
      return d;
    }
  }
}
