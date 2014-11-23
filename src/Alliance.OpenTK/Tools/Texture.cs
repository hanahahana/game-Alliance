using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace Alliance
{
  public static class Texture
  {
    static readonly List<string> names;
    static readonly Assembly assembly; 

    static Texture()
    {
      assembly = typeof(Texture).Assembly;
      names = assembly.GetManifestResourceNames().ToList();
    }

    public static int BindImage(Bitmap bmp)
    {
      int id = GL.GenTexture();
      GL.BindTexture(TextureTarget.Texture2D, id);
      var data = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height),
        System.Drawing.Imaging.ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
         OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
      bmp.UnlockBits(data);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
      return id;
    }

    public static Stream Load(string path)
    {
      path = path.Replace("\\", ".");
      var name = names.Single(n => n.EndsWith(path));
      var stream = assembly.GetManifestResourceStream(name);
      stream.Seek(0, SeekOrigin.Begin);
      return stream;
    }

    public static Bitmap LoadAsBitmap(string path)
    {
      return (Bitmap)Image.FromStream(Load(path));
    }
  }
}
