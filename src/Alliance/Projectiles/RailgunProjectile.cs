using System;
using GraphicsSystem;

namespace Alliance
{
  /// <summary>
  /// The projectile fired by the rail gun tower.
  /// </summary>
  [Serializable]
  public class RailgunProjectile : Projectile
  {
    public RailgunProjectile(Piece parent, double timeToLiveInSeconds)
      : base(parent, timeToLiveInSeconds)
    {
      Color = GsMath.Lerp(GsColor.Yellow, GsColor.Red, .65f);
      ImageKey = "pulse";
    }

    public override void UpdateByFrameCount(TimeSpan elapsed, int frameCount)
    {
      float time = (float)(elapsed.TotalSeconds * ((frameCount + 1.0) * 20.0) * .75);
      Position += (time * VelocityFactor * VelocityFactor);
    }
  }
}
