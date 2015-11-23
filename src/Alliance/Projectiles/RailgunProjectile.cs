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

    public RailgunProjectile(double timeToLiveInSeconds)
      : base(timeToLiveInSeconds)
    {
      Color = Utils.GetIntermediateColor(Color.Yellow, Color.Red, .65f, 0f, 1f);
    }

    protected override string ImageKey
    {
      get { return "pulse"; }
    }

    public override void UpdateByFrameCount(int frameCount)
    {
      Position += (Velocity * MovementPerSecond * ((float)frameCount * .75f));
    }
  }
}
