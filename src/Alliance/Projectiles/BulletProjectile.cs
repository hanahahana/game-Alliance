using System;
using GraphicsSystem;

namespace Alliance
{
  /// <summary>
  /// The projectile fired by the machine gun tower.
  /// </summary>
  [Serializable]
  public class BulletProjectile : Projectile
  {
    public BulletProjectile(Piece parent, double timeToLiveInSeconds)
      : base(parent, timeToLiveInSeconds)
    {
      Size = new GsSize(6f, 2f);
      ImageKey = "bullet";
    }

    public override void UpdateByFrameCount(TimeSpan elapsed, int frameCount)
    {
      float time = (float)(elapsed.TotalSeconds * (frameCount + 1.0));
      Position += (time * VelocityFactor * VelocityFactor);
    }
  }
}
