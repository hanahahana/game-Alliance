using System;

namespace Alliance
{
  /// <summary>
  /// The projectile fired by the missile tower.
  /// </summary>
  [Serializable]
  public class MissileProjectile : Projectile
  {
    public MissileProjectile(Piece parent, double timeToLiveInSeconds)
      : base(parent, timeToLiveInSeconds)
    {
      ImageKey = "rocket";
    }

    public override void UpdateByFrameCount(TimeSpan elapsed, int frameCount)
    {
      float time = (float)(elapsed.TotalSeconds * (frameCount * .5));
      Position += (time * VelocityFactor * VelocityFactor);
    }
  }
}
