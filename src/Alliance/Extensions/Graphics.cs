using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsSystem;

namespace Alliance
{
  public static partial class Extensions
  {
    public static void DrawImage(this IGsGraphics graphics, GsImage image, GsColor color, GsVector location, GsVector scale)
    {
      var size = ImageProvider.GetSize(image).ToVector() * scale;
      graphics.DrawImage(image, location.X, location.Y, size.X, size.Y, color);
    }

    public static void DrawImage(this IGsGraphics graphics, ImageParams data, GsColor color)
    {
      var size = data.ImageSize.ToVector() * data.Scale;
      var dest = new GsRectangle(data.Position, size.ToSize());
      graphics.DrawImage(data.Image, dest, null, data.Origin, 0f, GsImageFlip.None, color);
    }

    public static void DrawImage(this IGsGraphics graphics, ImageParams data, GsColor color, GsRectangle source)
    {
      var size = data.ImageSize.ToVector() * data.Scale;
      var dest = new GsRectangle(data.Position, size.ToSize());
      graphics.DrawImage(data.Image, dest, source, data.Origin, 0f, GsImageFlip.None, color);
    }

    public static void DrawImage(this IGsGraphics graphics, ImageParams data, GsColor color, float rotation)
    {
      var size = data.ImageSize.ToVector() * data.Scale;
      var dest = new GsRectangle(data.Position, size.ToSize());
      graphics.DrawImage(data.Image, dest, null, data.Origin, rotation, GsImageFlip.None, color);
    }

    public static void DrawImage(this IGsGraphics graphics, ImageParams data, GsColor color, GsVector offset, float rotation)
    {
      var size = data.ImageSize.ToVector() * data.Scale;
      var dest = new GsRectangle(data.Position + offset, size.ToSize());
      graphics.DrawImage(data.Image, dest, null, data.Origin, rotation, GsImageFlip.None, color);
    }

    public static void DrawImage(this IGsGraphics graphics, ImageParams data, GsColor color, GsVector offset, float rotation, GsImageFlip flip)
    {
      var size = data.ImageSize.ToVector() * data.Scale;
      var dest = new GsRectangle(data.Position + offset, size.ToSize());
      graphics.DrawImage(data.Image, dest, null, data.Origin, rotation, flip, color);
    }

    public static void DrawString(this IGsGraphics graphics, GsFont font, string text, GsVector pos, GsColor color)
    {
      graphics.DrawString(font, text, pos.X, pos.Y, color);
    }

    public static void DrawEllipse(this IGsGraphics graphics, GsColor color, GsVector loc, GsSize size)
    {
      graphics.DrawEllipse(color, loc.X, loc.Y, size.Width, size.Height);
    }

    public static void FillEllipse(this IGsGraphics graphics, GsColor color, GsVector loc, GsSize size)
    {
      graphics.FillEllipse(color, loc.X, loc.Y, size.Width, size.Height);
    }
  }
}
