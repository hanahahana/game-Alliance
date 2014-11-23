using System;
using System.Collections.Generic;
using GraphicsSystem;

namespace Alliance
{
  /// <summary>
  /// The projectile fired when a missile projectile expoldes.
  /// </summary>
  [Serializable]
  public class DebriProjectile : Projectile
  {
    public DebriProjectile(Piece parent, Projectile projectile, float orientation)
      : base(parent, .3)
    {
      Velocity = Calculator.ComputeProjectileDirection(orientation) * 100f;
      Attack = projectile.Attack * 3f;
      Color = GsMath.Lerp(GsColor.Yellow, GsColor.Black, .55f);
      Orientation = orientation;
      Position = projectile.Position;
      Size = new GsSize(3.5f, 3.5f);
      ImageKey = "debri";
    }

    public static DebriProjectile[] Create(Projectile projectile, int count)
    {
      List<DebriProjectile> retval = new List<DebriProjectile>(count + 1);
      float angle = GsMath.ToRadians(90f);
      float step = GsMath.ToRadians(360f) / count;

      for (int i = 0; i < count; ++i, angle += step)
      {
        retval.Add(new DebriProjectile(projectile.Parent, projectile, angle));
      }
      return retval.ToArray();
    }
  }
}
