using System;
using System.Collections.Generic;
using System.Text;
using Alliance.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Alliance.Utilities;

namespace Alliance.Projectiles
{
  public class Projectile
  {
    public const float MovementPerSecond = 5f;

    private BoxF mBounds;
    private Vector2 mVelocity;
    private float mOrientation;
    private bool mIsAlive;
    private double mTimeToLive;
    private Color mColor;

    public BoxF Bounds
    {
      get { return mBounds; }
      set { mBounds = value; }
    }

    public Vector2 Position
    {
      get { return mBounds.Location; }
      set { mBounds.Location = value; }
    }

    public float X
    {
      get { return mBounds.X; }
      set { mBounds.X = value; }
    }

    public float Y
    {
      get { return mBounds.Y; }
      set { mBounds.Y = value; }
    }

    public SizeF Size
    {
      get { return mBounds.Size; }
      set { mBounds.Size = value; }
    }

    public float Width
    {
      get { return mBounds.Width; }
      set { mBounds.Width = value; }
    }

    public float Height
    {
      get { return mBounds.Height; }
      set { mBounds.Height = value; }
    }

    public Vector2 Velocity
    {
      get { return mVelocity; }
      set { mVelocity = value * MovementPerSecond; }
    }

    public float Orientation
    {
      get { return mOrientation; }
      set { mOrientation = value; }
    }

    public bool IsAlive
    {
      get { return mIsAlive; }
      set { mIsAlive = value; }
    }

    public Color Color
    {
      get { return mColor; }
      set { mColor = value; }
    }

    public Projectile(double timeToLive)
    {
      mTimeToLive = timeToLive;
    }

    public void Update(GameTime gameTime)
    {
      // update the time to live
      mTimeToLive -= gameTime.ElapsedGameTime.TotalSeconds;
      mTimeToLive = Math.Max(0, mTimeToLive);
      mIsAlive = mTimeToLive > 0;

      if (mIsAlive)
      {
        // if we're still alive, then move the projectile
        Position += Velocity;
      }
    }

    public void Draw(SpriteBatch mSpriteBatch, Vector2 offset)
    {
      // initialize if we need to
      if (Shapes.InternalPixel == null)
        Shapes.InitializePixelTexture(mSpriteBatch.GraphicsDevice);

      // otherwise, draw the pixel as a projectile
      Vector2 scale = Size.ToVector2();
      Vector2 myCenter = scale * .5f;
      mSpriteBatch.Draw(
        Shapes.InternalPixel,
        mBounds.Location + offset + myCenter,
        null,
        mColor,
        mOrientation,
        Vector2.One * .5f,
        scale,
        SpriteEffects.None,
        0f);
    }
  }
}
