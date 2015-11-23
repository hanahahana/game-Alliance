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

    protected BoxF mBounds;
    protected Vector2 mVelocity;
    protected float mOrientation;
    protected bool mIsAlive;
    protected double mTimeToLive;
    protected Color mColor;
    protected float mAttack;

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
      set { mVelocity = value; }
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

    public float Attack
    {
      get { return mAttack; }
      set { mAttack = value; }
    }

    public Projectile(double timeToLiveInSeconds)
    {
      mIsAlive = true;
      mTimeToLive = timeToLiveInSeconds;
      mColor = Color.White;
    }

    protected virtual Texture2D GetProjectileImage()
    {
      return AllianceGame.Textures["bullet"];
    }

    public virtual void Update(GameTime gameTime)
    {
      UpdateTimeToLive(gameTime);
      UpdatePosition(gameTime);
    }

    protected void UpdateTimeToLive(GameTime gameTime)
    {
      // update the time to live
      mTimeToLive -= gameTime.ElapsedGameTime.TotalSeconds;
      mTimeToLive = Math.Max(0, mTimeToLive);
      mIsAlive = mTimeToLive > 0;
    }

    protected void UpdatePosition(GameTime gameTime)
    {
      if (IsAlive)
      {
        // if we're still alive, then move the projectile
        Position += Velocity * MovementPerSecond;
      }
    }

    public virtual void Draw(SpriteBatch spriteBatch, Vector2 offset)
    {
      Texture2D projectile = GetProjectileImage();
      SizeF projectileSize = new SizeF(projectile.Width, projectile.Height);

      Vector2 origin = new Vector2(0, projectileSize.Height / 2);
      Vector2 scale = Utils.ComputeScale(projectileSize, Size);

      spriteBatch.Draw(
          projectile,
          Position + offset,
          null,
          mColor,
          mOrientation,
          origin,
          scale,
          SpriteEffects.None,
          0);
    }

    public virtual void UpdateOut(int frames)
    {
      // do nothing here
    }
  }
}
