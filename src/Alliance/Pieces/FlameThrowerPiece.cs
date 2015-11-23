using System;
using System.Text;
using Alliance.Enums;
using Alliance.Objects;
using Alliance.Projectiles;
using Microsoft.Xna.Framework;
using MLA.Utilities.Xna;
using MLA.Utilities.Xna.Helpers;

namespace Alliance.Pieces
{
  /// <summary>
  /// The flame thrower tower. It's meant to emulate firing a flame wave, or a constant flame.
  /// </summary>
  [Serializable]
  public class FlameThrowerPiece : Piece
  {
    private const string FlameThrowerPieceName = "Flame Thrower";
    private const string UltimateFlameThrowerPieceName = "Flamewave";

    public FlameThrowerPiece()
    {
      // setup the description
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Ignites gasoline to produce a constant flame. Careful, some enemies can't be burned.");

      // set the properties of the piece
      Attack = 10000;
      Price = 1000000;
      Radius = 300;
      UpgradePercent = 10;
      LevelVisibility = 90;

      Description = sb.ToString();
      NumberProjectilesToFire = 1;
      ProjectilesPerSecond = 15;
      ProjectileLifeInSeconds = .1876f;
      Name = FlameThrowerPieceName;
      UltimateName = UltimateFlameThrowerPieceName;
      Grouping = PieceGrouping.Four;
      ImageKey = "flamethrower";
      Element = Element.Fire;
      Specialty = PieceSpecialty.Ground;
    }

    public override void Update(GameTime gameTime)
    {
      base.Update(gameTime);
      if (Level == MaxLevel)
      {
        Orientation = Environment.TickCount / 1000f;
        Orientation = MathematicsHelper.WrapAngle(Orientation);
      }
    }

    protected override void FinalizeUpgrade()
    {
      base.FinalizeUpgrade();
      if (Level == MaxLevel)
      {
        StringBuilder sb = new StringBuilder();
        sb.AppendLine("Rotates to gain energy and releases a VERY powerful flame wave. Careful, some enemies can't be burned.");

        Orientation = 0;
        FaceTarget = false;

        NumberProjectilesToFire = 1;
        ProjectileLifeInSeconds = DefaultProjectileLifeInSeconds * 5;
        Radius = 100;

        ProjectilesPerSecond = 10;
        ProjectileLifeInSeconds = 1.1676f;

        Description = sb.ToString();
      }
    }

    protected override void UpgradeProjectileVariables(float factor)
    {
      // don't uprade the variables
    }

    protected override Piece CreatePiece(GridCell[] cells)
    {
      FlameThrowerPiece piece = new FlameThrowerPiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      Projectile projectile = null;
      if (Level == MaxLevel)
      {
        BoxF bounds = new BoxF(this.Position, this.Size);
        projectile = new FlamewaveProjectile(this, bounds, ProjectileLifeInSeconds);
        projectile.Size = new SizeF(Width, Height);
      }
      else
      {
        float extra = Radius / 3f;
        projectile = new FlameProjectile(this, ProjectileLifeInSeconds);
        projectile.Size = new SizeF(Radius, projectile.Height + extra);
      }
      return projectile;
    }
  }
}
