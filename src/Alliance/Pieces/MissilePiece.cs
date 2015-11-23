using System;
using System.Text;
using Alliance.Objects;
using Alliance.Projectiles;
using Alliance.Invaders;
using Alliance.Enums;

namespace Alliance.Pieces
{
  /// <summary>
  /// The missile tower. It's meant to emulate firing missiles at an invader.
  /// </summary>
  [Serializable]
  public class MissilePiece : Piece
  {
    private const string MissileName = "Missile";
    private const string UltimateMissileName = "Blaster";

    public MissilePiece()
    {
      // setup the description
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Slams multiple missiles into flying enemies! They explode on contact and create debris.");

      // set the properties of the piece
      Attack = 75;
      Price = 25;
      Radius = 100;
      UpgradePercent = 20;
      LevelVisibility = 1;

      Description = sb.ToString();
      NumberProjectilesToFire = 1;
      Name = MissileName;
      UltimateName = UltimateMissileName;
      ImageKey = "missileLauncher";
      Specialty = PieceSpecialty.Flight;
    }

    protected override Piece CreatePiece(GridCell[] cells)
    {
      MissilePiece piece = new MissilePiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      MissileProjectile projectile = new MissileProjectile(this, ProjectileLifeInSeconds);
      return projectile;
    }
  }
}
