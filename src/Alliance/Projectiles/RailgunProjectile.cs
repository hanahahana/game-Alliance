using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


using MLA.Utilities.Xna.Helpers;
using Alliance.Pieces;

namespace Alliance.Projectiles
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
      Color = ColorHelper.Blend(Color.Yellow, Color.Red, .65f);
      ImageKey = "pulse";
    }

    public override void UpdateByFrameCount(GameTime gameTime, int frameCount)
    {
      float time = (float)(gameTime.ElapsedGameTime.TotalSeconds * ((frameCount + 1.0) * 20.0) * .75);
      Position += (time * VelocityFactor * VelocityFactor);
    }
  }
}
