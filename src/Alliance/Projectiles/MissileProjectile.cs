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

    public override void UpdateByFrameCount(int frameCount)
    {
      Position += (frameCount * .5f * Velocity * MovementPerSecond);
    }

    protected override string ImageKey
    {
      get { return "rocket"; }
    }
  }
}
