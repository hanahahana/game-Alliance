using System;
using SharpDX.Toolkit;

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

    public override void UpdateByFrameCount(GameTime gameTime, int frameCount)
    {
      float time = (float)(gameTime.ElapsedGameTime.TotalSeconds * (frameCount * .5));
      Position += (time * VelocityFactor * VelocityFactor);
    }
  }
}
