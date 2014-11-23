using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicsSystem;

namespace Alliance
{
  public sealed class FramedImage
  {
    public GsImage Texture { get; private set; }
    public GsVector[] Hull { get; private set; }
    public bool IsAnimated { get; private set; }
    public int NumberFrames { get; private set; }
    public int Columns { get; private set; }
    public int Rows { get; private set; }
    public GsSize FrameSize { get; private set; }
    public string Key { get; private set; }

    private FramedImage[] mFrames;
    public FramedImage this[int frameIndex]
    {
      get { return mFrames[frameIndex]; }
    }

    private static GsColor[] GetFrame(GsColor[,] data, GsRectangle src, int length)
    {
      GsColor[] arr = new GsColor[length];
      int x, y, i = 0;
      for (y = (int)src.Y; y < src.Bottom; ++y)
      {
        for (x = (int)src.X; x < src.Right; ++x)
        {
          arr[i++] = data[x, y];
        }
      }
      return arr;
    }

    public FramedImage(GsImage texture, string key, bool isAnimated, int cols, int rows)
    {
      Key = key;
      IsAnimated = isAnimated;
      NumberFrames = (cols * rows);
      Texture = texture;
      Hull = ImageProvider.CreateConvexHull(texture);
      Columns = cols;
      Rows = rows;
      mFrames = null;

      if (IsAnimated && NumberFrames > 0)
      {
        mFrames = new FramedImage[NumberFrames];

        var textureSize = ImageProvider.GetSize(texture);
        FrameSize = new GsSize(textureSize.Width / cols, textureSize.Height / rows);

        int frameLength = (int)(FrameSize.Width * FrameSize.Height);

        GsRectangle rect = new GsRectangle(GsVector.Zero, FrameSize);
        GsColor[,] data = ImageProvider.ToColorData(texture);

        for (int y = 0; y < rows; ++y)
        {
          rect.Y = y * FrameSize.Height;
          for (int x = 0; x < cols; ++x)
          {
            // extract the data from the image
            rect.X = x * FrameSize.Width;
            GsColor[] frame = GetFrame(data, rect, frameLength);

            // create a new texture
            var frameImage = ImageProvider.FromColorData(frame);

            // create a new Image frame
            int idx = (int)(x + (y * FrameSize.Width));
            mFrames[idx] = new FramedImage(frameImage, Key + idx, false, 0, 0);
          }
        }
      }
    }

    /// <summary>
    /// Returns the hash code for the key of this image.
    /// </summary>
    /// <returns>The hash code of this image.</returns>
    public override int GetHashCode()
    {
      return Key.GetHashCode();
    }

    /// <summary>
    /// Determines if this Image is equal to another object.
    /// </summary>
    /// <param name="obj">The object to test against.</param>
    /// <returns>A boolean value indicating if two objects are equal.</returns>
    public override bool Equals(object obj)
    {
      FramedImage image = obj as FramedImage;
      if (image == null) return false;
      return image.Key.Equals(Key);
    }
  }
}
