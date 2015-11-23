using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Alliance.Data;

namespace Alliance.Projectiles
{
  public class BulletProjectile : Projectile
  {
    public BulletProjectile(double timeToLiveInSeconds)
      : base(timeToLiveInSeconds)
    {
      Size = new SizeF(6f, 2f);
    }

    public override void UpdateByFrameCount(int frameCount)
    {
      Position += ((frameCount + 1) * Velocity * MovementPerSecond * 4);
    }

    protected override string ImageKey
    {
      get { return "bullet"; }
    }
  }
}
