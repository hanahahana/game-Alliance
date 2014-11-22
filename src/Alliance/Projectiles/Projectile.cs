using System;

namespace Alliance
{
  /// <summary>
  /// An abstract class representing a projectile. Each projectile class should inherit from this.
  /// </summary>
  [Serializable]
  public class Projectile : Sprite
  {
    protected double mTimeToLive;

    public bool IsAlive { get; set; }
    public float Attack { get; set; }
    public bool StayAlive { get; set; }
    public Piece Parent { get; protected set; }

    public Projectile(Piece parent, double timeToLiveInSeconds)
    {
      IsAlive = true;
      mTimeToLive = timeToLiveInSeconds;

      ImageKey = "bullet";
      Origin = new Vector2(0, GetImage().Height / 2f);
      Color = Color.White;
      Size = new SizeF(20f, 6.5f);
      StayAlive = false;
      Parent = parent;
    }

    public virtual void Update(TimeSpan elapsed)
    {
      UpdateTimeToLive(elapsed);
      UpdatePosition(elapsed);
    }

    protected virtual void UpdateTimeToLive(TimeSpan elapsed)
    {
      // update the time to live
      mTimeToLive -= elapsed.TotalSeconds;
      mTimeToLive = Math.Max(0, mTimeToLive);
      IsAlive = mTimeToLive > 0;
    }

    protected virtual void UpdatePosition(TimeSpan elapsed)
    {
      if (IsAlive)
      {
        // if we're still alive, then move the projectile
        float time = (float)elapsed.TotalSeconds;
        Position += (time * Velocity * VelocityFactor);
      }
    }

    public virtual void UpdateByFrameCount(TimeSpan elapsed, int frameCount)
    {
      float time = (float)(elapsed.TotalSeconds * (frameCount + 1.0));
      Position += (time * VelocityFactor * VelocityFactor);
    }

    public virtual void OnCollidedWithInvader(Invader invader)
    {
      IsAlive = StayAlive || false;
    }

    public virtual bool CanHit(Invader invader)
    {
      return (Parent != null) && (Parent.InvaderMatchesSpecialty(invader));
    }

    public virtual void Draw(DrawParams dparams)
    {
      SpriteBatch spriteBatch = dparams.SpriteBatch;
      Vector2 offset = dparams.Offset;

      TextureDrawData data = GetTextureDrawData(offset);
      spriteBatch.Draw(
          data.Texture,
          data.Position,
          null,
          Color,
          Orientation,
          data.Origin,
          data.Scale,
          SpriteEffects.None,
          0);
    }
  }
}
