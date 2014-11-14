using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Toolkit.Content;
using SharpDX.Toolkit.Graphics;
using Size = System.Drawing.Size;

namespace Alliance
{
  /// <summary>
  /// Represents a collection of pixels. Stores the frames (if any) as seperate images, and keeps
  /// a copy of the convex hull (or bounding polygon) of the image.
  /// </summary>
  public sealed class FramedImage
  {
    private Texture2D mTexture;
    private Vector2[] mHull;
    private bool mIsAnimated;
    private int mNumberFrames;
    private string mKey;
    private FramedImage[] mFrames;
    private int mColumns;
    private int mRows;
    private Size mFrameSize;

    /// <summary>
    /// Gets the texture associated with this image.
    /// </summary>
    public Texture2D Texture
    {
      get { return mTexture; }
    }

    /// <summary>
    /// Gets the convex hull (or bounding polygon) of this image.
    /// </summary>
    public Vector2[] Hull
    {
      get { return mHull; }
    }

    /// <summary>
    /// Gets a value indicating if this image is animated. This comes directly from the 
    /// constructor and ignores any other value for validation.
    /// </summary>
    public bool IsAnimated
    {
      get { return mIsAnimated; }
    }

    /// <summary>
    /// Gets the number of frames contained within this image. This is the result of multiplying
    /// the number of columns by the number of rows.
    /// </summary>
    public int NumberFrames
    {
      get { return mNumberFrames; }
    }

    /// <summary>
    /// Gets the number of columns in the image. This is used for animation.
    /// </summary>
    public int Columns
    {
      get { return mColumns; }
    }

    /// <summary>
    /// Gets the number of rows in the image. This is used for animation.
    /// </summary>
    public int Rows
    {
      get { return mRows; }
    }

    /// <summary>
    /// Gets the size of each frame inside the image. This is inferred by the image passed in,
    /// and the number of rows and columns.
    /// </summary>
    public Size FrameSize
    {
      get { return mFrameSize; }
    }

    /// <summary>
    /// Gets the frame (another Image) at the specified index.
    /// </summary>
    /// <param name="frameIndex">The index of the frame to retrieve.s</param>
    /// <returns>An image object representing the frame.</returns>
    public FramedImage this[int frameIndex]
    {
      get { return mFrames[frameIndex]; }
    }

    /// <summary>
    /// Gets the key of this image.
    /// </summary>
    public string Key
    {
      get { return mKey; }
    }

    /// <summary>
    /// Create a new image object using the ContentManager.
    /// </summary>
    public FramedImage(ContentManager content, string imageName, bool isAnimated, int cols, int rows)
      : this(content.Load<Texture2D>(imageName), imageName, isAnimated, cols, rows)
    {

    }

    /// <summary>
    /// Create a new image object using the ContentManager.
    /// </summary>
    public FramedImage(ContentManager content, string folder, string imageName, bool isAnimated, int cols, int rows)
      : this(content.Load<Texture2D>(Path.Combine(folder, imageName)), imageName, isAnimated, cols, rows)
    {

    }

    /// <summary>
    /// Creates a new image object using a Textre.
    /// </summary>
    public FramedImage(Texture2D texture, string imageName, bool isAnimated, int cols, int rows)
    {
      mKey = imageName;
      mIsAnimated = isAnimated;
      mNumberFrames = (cols * rows);
      mTexture = texture;
      mHull = mTexture.CreateConvexHull();
      mFrames = null;
      mColumns = cols;
      mRows = rows;

      if (mIsAnimated && mNumberFrames > 0)
      {
        mFrames = new FramedImage[mNumberFrames];
        mFrameSize = new Size(mTexture.Width / cols, mTexture.Height / rows);

        GraphicsDevice device = texture.GraphicsDevice;
        int frameLength = mFrameSize.Width * mFrameSize.Height;

        Rectangle rect = new Rectangle(0, 0, mFrameSize.Width, mFrameSize.Height);
        Color[,] data = mTexture.GetData<Color>().To2D(mTexture.Width, mTexture.Height);

        for (int y = 0; y < rows; ++y)
        {
          rect.Y = y * mFrameSize.Height;
          for (int x = 0; x < cols; ++x)
          {
            // extract the data from the image
            rect.X = x * mFrameSize.Width;
            Color[] frame = data.GetFrame(rect, frameLength);

            // create a new texture
            Texture2D frameTexture = Texture2D.New<Color>(device,
              mFrameSize.Width,
              mFrameSize.Height,
              PixelFormat.R8G8B8A8.UNorm,
              frame);

            // create a new Image frame
            int idx = x + (y * mFrameSize.Width);
            mFrames[idx] = new FramedImage(frameTexture, mKey + idx, false, 0, 0);
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
      return mKey.GetHashCode();
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
      return image.mKey.Equals(mKey);
    }
  }
}
