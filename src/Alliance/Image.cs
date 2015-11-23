using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Alliance.Utilities;
using Alliance.Data;

namespace Alliance
{
  public class Image
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

    public Texture2D Texture
    {
      get { return mTexture; }
    }

    public Vector2[] Hull
    {
      get { return mHull; }
    }

    public bool IsAnimated
    {
      get { return mIsAnimated; }
    }

    public int NumberFrames
    {
      get { return mNumberFrames; }
    }

    public int Columns
    {
      get { return mColumns; }
    }

    public int Rows
    {
      get { return mRows; }
    }

    public Size FrameSize
    {
      get { return mFrameSize; }
    }

    public Image this[int frameIndex]
    {
      get { return mFrames[frameIndex]; }
    }

    public string Key
    {
      get { return mKey; }
    }

    public Image(ContentManager content, string imageName, bool isAnimated, int cols, int rows)
      : this(content.Load<Texture2D>(imageName), imageName, isAnimated, cols, rows)
    {

    }

    public Image(Texture2D texture, string imageName, bool isAnimated, int cols, int rows)
    {
      mKey = imageName;
      mIsAnimated = isAnimated;
      mNumberFrames = (cols * rows);
      mTexture = texture;
      mHull = Utils.CreateConvexHull(mTexture);
      mFrames = null;

      if (mIsAnimated && mNumberFrames > 0)
      {
        mFrames = new Image[mNumberFrames];
        mFrameSize = new Size(mTexture.Width / cols, mTexture.Height / rows);

        GraphicsDevice device = texture.GraphicsDevice;
        int frameLength = mFrameSize.Width * mFrameSize.Height;

        Rectangle rect = new Rectangle(0, 0, mFrameSize.Width, mFrameSize.Height);
        for (int y = 0; y < rows; ++y)
        {
          rect.Y = y;
          for (int x = 0; x < cols; ++x)
          {
            // extract the data from the image
            rect.X = x;
            Color[] frame = new Color[frameLength];
            mTexture.GetData<Color>(0, rect, frame, 0, frame.Length);

            // create a new texture
            Texture2D frameTexture = new Texture2D(device, mFrameSize.Width, mFrameSize.Height, 0, TextureUsage.AutoGenerateMipMap, SurfaceFormat.Color);
            frameTexture.SetData<Color>(frame);
            frameTexture.GenerateMipMaps(TextureFilter.Linear);

            // create a new Image frame
            int idx = x + (y * mFrameSize.Width);
            mFrames[idx] = new Image(frameTexture, mKey + idx, false, 0, 0);
          }
        }
      }
    }

    public override int GetHashCode()
    {
      return mKey.GetHashCode();
    }

    public override bool Equals(object obj)
    {
      Image image = obj as Image;
      if (image == null) return false;
      return image.mKey.Equals(mKey);
    }
  }
}
