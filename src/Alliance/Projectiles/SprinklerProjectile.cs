using System;
using GraphicsSystem;

namespace Alliance
{
  /// <summary>
  /// The projectile fired by the sprinkler tower.
  /// </summary>
  [Serializable]
  public class SprinklerProjectile : Projectile
  {
    public SprinklerProjectile(Piece parent, double timeToLiveInSeconds)
      : base(parent, timeToLiveInSeconds)
    {
      Color = GsMath.Lerp(GsColor.LightBlue, GsColor.DarkBlue, .65f);
      Size = new GsSize(10f, 8f);
      ImageKey = "fragment";
    }
  }
}
