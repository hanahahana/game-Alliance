using System;
using Microsoft.Xna.Framework.Graphics;
using MLA.Utilities.Xna;
using MLA.Utilities.Xna.Helpers;
using Alliance.Pieces;

namespace Alliance.Projectiles
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
