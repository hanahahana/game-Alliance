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
  public class Projectile : Sprite
  {
    public const float MovementPerSecond = 5f;

    protected Vector2 mVelocity;
    protected bool mIsAlive;
    protected double mTimeToLive;
    protected float mAttack;
    protected Vector2 mOrigin;

    public Vector2 Velocity
    {
      get { return mVelocity; }
      set { mVelocity = value; }
    }

    public virtual bool IsAlive
    {
      get { return mIsAlive; }
      set { mIsAlive = value; }
    }

    public float Attack
    {
      get { return mAttack; }
      set { mAttack = value; }
    }

    protected override string ImageKey
    {
      get { return "bullet"; }
    }

    protected override Vector2 Origin
    {
      get { return mOrigin; }
    }

    public Projectile(double timeToLiveInSeconds)
    {
      mIsAlive = true;
      mTimeToLive = timeToLiveInSeconds;
      mOrigin = new Vector2(0, GetImage().Height / 2f);
      Color = Color.White;
      Size = new SizeF(20f, 6.5f);
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

    public virtual void OnCollidedWithEntity(Entity entity)
    {
      IsAlive = false;
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
          Color,
          mOrientation,
          data.Origin,
          data.Scale,
          SpriteEffects.None,
          0);
    }
  }
}
