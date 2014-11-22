using System;

namespace Alliance
{
  /// <summary>
  /// The projectile fired by the machine gun tower.
  /// </summary>
  public class BulletProjectile : Projectile
  {
    public BulletProjectile(Piece parent, double timeToLiveInSeconds)
      : base(parent, timeToLiveInSeconds)
    {
      Size = new SizeF(6f, 2f);
      ImageKey = "bullet";
    }

    public override void UpdateByFrameCount(TimeSpan elapsed, int frameCount)
    {
      float time = (float)(elapsed.TotalSeconds * (frameCount + 1.0));
      Position += (time * VelocityFactor * VelocityFactor);
    }
  }
}
