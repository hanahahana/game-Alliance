using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Alliance.Utilities;

namespace Alliance.Projectiles
{
  public class RailgunProjectile : Projectile
  {
    const float PixelsPerSecond = 150f;
    const float FactorOfWidth = 6f;
    const float SecondsBeforeUpdate = 1f / PixelsPerSecond;

    public RailgunProjectile(double timeToLiveInSeconds)
      : base(timeToLiveInSeconds)
    {
      mColor = Utils.GetIntermediateColor(Color.Yellow, Color.Red, .55f, 0f, 1f);
    }

    protected override Texture2D GetProjectileImage()
    {
      return AllianceGame.Textures["pulse"];
    }

    public override Color[,] GetProjectileImageData()
    {
      return AllianceGame.TextureData["pulse"];
    }

    public override void UpdateByFrameCount(int frameCount)
    {
      Position += (Velocity * MovementPerSecond * frameCount);
    }
  }
}
