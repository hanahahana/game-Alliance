using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alliance.Data;
using Alliance.Utilities;
using Alliance.Entities;
using Alliance.Parameters;

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

    public virtual bool IsAlive
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
      Size = new SizeF(20f, 6.5f);
    }

    protected virtual Texture2D GetProjectileImage()
    {
      return AllianceGame.Textures["bullet"];
    }

    public virtual Color[,] GetProjectileImageData()
    {
      return AllianceGame.TextureData["bullet"];
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

    public virtual void UpdateByFrameCount(int frameCount)
    {
      // do nothing here
    }

    public virtual DrawData GetDrawData(Vector2 offset)
    {
      // get the projectile image and size
      Texture2D projectile = GetProjectileImage();
      SizeF projectileSize = new SizeF(projectile.Width, projectile.Height);

      // compute the origin and the scale
      Vector2 origin = new Vector2(0, projectileSize.Height / 2);
      Vector2 scale = Utils.ComputeScale(projectileSize, Size);

      // return the data
      return new DrawData(projectile, projectileSize, Position + offset, origin, scale);
    }

    public virtual Matrix ComputeTransform(Vector2 offset)
    {
      return ComputeTransform(GetDrawData(offset));
    }

    public virtual Matrix ComputeTransform(DrawData data)
    {
      // create the matrix for transforming the center
      Matrix transform =
        Matrix.CreateTranslation(-data.Origin.X, -data.Origin.Y, 0) *
        Matrix.CreateRotationZ(mOrientation) *
        Matrix.CreateScale(data.Scale.X, data.Scale.Y, 1f) *
        Matrix.CreateTranslation(data.Position.X, data.Position.Y, 0);

      // return the transform
      return transform;
    }

    public virtual Vector2 GetCenter(Vector2 offset)
    {
      // get the drawing data
      DrawData data = GetDrawData(offset);

      // get the center of the image
      Vector2 center = (data.TextureSize / 2f).ToVector2();

      // compute the transform
      Matrix transform = ComputeTransform(data);

      // return the center transformated
      Vector2 result;
      Vector2.Transform(ref center, ref transform, out result);
      return result;
    }

    public virtual BoxF GetBoundingBox(Vector2 offset)
    {
      // get the center of the projectile
      Vector2 center = GetCenter(offset);

      // create a rough box that has the projectile inside of it
      float dW = Width * .5f;
      float dH = Height * .5f;
      return new BoxF(
        center.X - dW,
        center.Y - dH,
        dW * 2f,
        dH * 2f);
    }

    public virtual void Draw(DrawParams dparams)
    {
      SpriteBatch spriteBatch = dparams.SpriteBatch;
      Vector2 offset = dparams.Offset;

      DrawData data = GetDrawData(offset);
      spriteBatch.Draw(
          data.Texture,
          data.Position,
          null,
          mColor,
          mOrientation,
          data.Origin,
          data.Scale,
          SpriteEffects.None,
          0);
    }
  }
}
