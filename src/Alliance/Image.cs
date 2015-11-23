
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MLA.Utilities.Xna;


namespace Alliance
{
  /// <summary>
  /// Represents a collection of pixels. Stores the frames (if any) as seperate images, and keeps
  /// a copy of the convex hull (or bounding polygon) of the image.
  /// </summary>
  public sealed class Image
  {
    private Texture2D mTexture;
    private Vector2[] mHull;
    private bool mIsAnimated;
    private int mNumberFrames;
    private string mKey;
    private Image[] mFrames;
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
    public Image this[int frameIndex]
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
    /// <param name="content">The ContentManager to load an image from.</param>
    /// <param name="imageName">The key to assign to this image and to use for loading from the ContentManager.</param>
    /// <param name="isAnimated">A value indicating if the image is animated.</param>
    /// <param name="cols">The number of columns in an image.</param>
    /// <param name="rows">The number of rows in an image.</param>
    public Image(ContentManager content, string imageName, bool isAnimated, int cols, int rows)
      : this(content.Load<Texture2D>(imageName), imageName, isAnimated, cols, rows)
    {

    }

    /// <summary>
    /// Creates a new image object using a Textre.
    /// </summary>
    /// <param name="texture">The texture to load data from.</param>
    /// <param name="imageName">The key to assign to this image.</param>
    /// <param name="isAnimated">A value indicating if the image is animated.</param>
    /// <param name="cols">The number of columns in an image.</param>
    /// <param name="rows">The number of rows in an image.</param>
    public Image(Texture2D texture, string imageName, bool isAnimated, int cols, int rows)
    {
      mKey = imageName;
      mIsAnimated = isAnimated;
      mNumberFrames = (cols * rows);
      mTexture = texture;
      mHull = AllianceUtilities.CreateConvexHull(mTexture);
      mFrames = null;
      mColumns = cols;
      mRows = rows;

      if (mIsAnimated && mNumberFrames > 0)
      {
        mFrames = new Image[mNumberFrames];
        mFrameSize = new Size(mTexture.Width / cols, mTexture.Height / rows);

        GraphicsDevice device = texture.GraphicsDevice;
        int frameLength = mFrameSize.Width * mFrameSize.Height;

        Rectangle rect = new Rectangle(0, 0, mFrameSize.Width, mFrameSize.Height);
        for (int y = 0; y < rows; ++y)
        {
          rect.Y = y * mFrameSize.Height;
          for (int x = 0; x < cols; ++x)
          {
            // extract the data from the image
            rect.X = x * mFrameSize.Width;
            Color[] frame = new Color[frameLength];
            mTexture.GetData<Color>(0, rect, frame, 0, frame.Length);

            // create a new texture
            Texture2D frameTexture = new Texture2D(device, mFrameSize.Width, mFrameSize.Height, 0, TextureUsage.AutoGenerateMipMap, SurfaceFormat.Color);
            frameTexture.SetData<Color>(frame);
            frameTexture.GenerateMipMaps(TextureFilter.GaussianQuad);

            // create a new Image frame
            int idx = x + (y * mFrameSize.Width);
            mFrames[idx] = new Image(frameTexture, mKey + idx, false, 0, 0);
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
      Image image = obj as Image;
      if (image == null) return false;
      return image.mKey.Equals(mKey);
    }
  }
}
