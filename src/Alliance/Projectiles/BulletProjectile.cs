using System;
using Microsoft.Xna.Framework;
using MLA.Utilities.Xna;
using Alliance.Pieces;

namespace Alliance.Projectiles
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
      Size = new SizeF(6f, 2f);
      ImageKey = "bullet";
    }

    public override void UpdateByFrameCount(GameTime gameTime, int frameCount)
    {
      float time = (float)(gameTime.ElapsedGameTime.TotalSeconds * (frameCount + 1.0));
      Position += (time * VelocityFactor * VelocityFactor);
    }
  }
}
