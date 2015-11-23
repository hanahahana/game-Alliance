using System;
using System.Text;
using Alliance.Objects;
using Alliance.Projectiles;
using Alliance.Invaders;
using Alliance.Enums;

namespace Alliance.Pieces
{
  /// <summary>
  /// The railgun tower. It's meant to emulate firing a powerful, yet slow blast of energy.
  /// </summary>
  [Serializable]
  public class RailgunPiece : Piece
  {
    private const string RailgunName = "Rail Gun";
    private const string UltimateRailgunName = "Annihilator";

    public RailgunPiece()
    {
      // setup the description
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Fires an electrical pulse at the enemy. This can shoot very far, and is extremely powerful, but isn't very fast.");

      // set the properties of the piece
      Attack = 2000;
      Price = 600;
      Radius = 200;
      UpgradePercent = 15;
      LevelVisibility = 20;

      Description = sb.ToString();
      ProjectilesPerSecond = 0.5f;
      NumberProjectilesToFire = 1;
      ProjectileLifeInSeconds = 5.5f;
      Name = RailgunName;
      UltimateName = UltimateRailgunName;
      ImageKey = "railgun";
      Element = Element.Electricity;
      Specialty = PieceSpecialty.Flight;
    }

    protected override Piece CreatePiece(GridCell[] cells)
    {
      RailgunPiece piece = new RailgunPiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      RailgunProjectile projectile = new RailgunProjectile(this, ProjectileLifeInSeconds);
      return projectile;
    }
  }
}
