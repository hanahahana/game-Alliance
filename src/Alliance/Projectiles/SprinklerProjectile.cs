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
      Color = Utils.BlendColors(Color.LightBlue, Color.DarkBlue, .65f);
      Size = new SizeF(10f, 8f);
    }

    protected override string ImageKey
    {
      get { return "fragment"; }
    }
  }
}
