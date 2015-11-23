using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace Alliance.Projectiles
{
  public class MissileProjectile : Projectile
  {
    public MissileProjectile(double timeToLiveInSeconds)
      : base(timeToLiveInSeconds)
    {

    }

    public override void UpdateOut(int frames)
    {
      Position += (frames * .5f * Velocity * MovementPerSecond);
    }

    public override Texture2D GetProjectileImage()
    {
      return AllianceGame.Textures["rocket"];
    }
  }
}
