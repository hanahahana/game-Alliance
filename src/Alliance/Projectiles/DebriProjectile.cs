using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alliance.Utilities;
using Alliance.Data;
using Alliance.Pieces;
using Alliance.Entities;

namespace Alliance.Projectiles
{
  public class DebriProjectile : Projectile
  {
    public DebriProjectile(Projectile projectile, float orientation)
      : base(.3)
    {
      mVelocity = Utils.ComputeProjectileDirection(orientation);
      mAttack = projectile.Attack * 3f;

      Color = Utils.GetIntermediateColor(Color.Yellow, Color.Black, .55f, 0, 1);
      mOrientation = orientation;
      Position = projectile.Position;
      Size = new SizeF(3.5f, 3.5f);
    }

    protected override string ImageKey
    {
      get { return "debri"; }
    }

    public static DebriProjectile[] Create(Projectile projectile, int count)
    {
      List<DebriProjectile> retval = new List<DebriProjectile>(count + 1);
      float angle = MathHelper.ToRadians(90f);
      float step = MathHelper.ToRadians(360f) / count;

      for (int i = 0; i < count; ++i, angle += step)
      {
        retval.Add(new DebriProjectile(projectile, angle));
      }
      return retval.ToArray();
    }
  }
}
