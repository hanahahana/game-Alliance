using System;

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
      Color = ColorHelper.Blend(Color.LightBlue, Color.DarkBlue, .65f);
      Size = new SizeF(10f, 8f);
      ImageKey = "fragment";
    }
  }
}
