using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Alliance.Utilities;
using Alliance.Data;

namespace Alliance.Projectiles
{
  public class SprinklerProjectile : Projectile
  {
    public SprinklerProjectile(double timeToLiveInSeconds)
      : base(timeToLiveInSeconds)
    {
      mColor = Utils.GetIntermediateColor(Color.LightBlue, Color.DarkBlue, .65f, 0f, 1f);
      Size = new SizeF(10f, 8f);
    }

    protected override Texture2D GetProjectileImage()
    {
      return AllianceGame.Textures["fragment"];
    }

    public override Color[,] GetProjectileImageData()
    {
      return AllianceGame.TextureData["fragment"];
    }
  }
}
