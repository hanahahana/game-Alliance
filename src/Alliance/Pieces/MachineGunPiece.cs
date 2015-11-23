using System;
using System.Text;
using Alliance.Objects;
using Alliance.Projectiles;
using Alliance.Invaders;
using Alliance.Enums;


namespace Alliance.Pieces
{
  /// <summary>
  /// The machine gun tower. It's meant to emulate rapidly firing a series of bullets.
  /// </summary>
  [Serializable]
  public class MachineGunPiece : Piece
  {
    private const string MachineGunName = "Machine Gun";
    private const string UltimateMachineGunName = "Destroyer";

    public MachineGunPiece()
    {
      // setup the description
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Fires a constant stream of bullets at the enemy. These are weak and fast but only hit non-flying invaders.");

      // set the properties of the piece
      Attack = 75;
      Price = 25;
      Radius = 100;
      UpgradePercent = 20;
      LevelVisibility = 1;

      Description = sb.ToString();
      NumberProjectilesToFire = 2;
      ProjectilesPerSecond = 15;
      ProjectileLifeInSeconds = 3.4567f;
      Name = MachineGunName;
      UltimateName = UltimateMachineGunName;
      ImageKey = "machinegun";
      Specialty = PieceSpecialty.Ground;
    }

    protected override void UpgradeProjectileVariables(float factor)
    {
      // don't upgrade the projectile variables
    }

    protected override Piece CreatePiece(GridCell[] cells)
    {
      MachineGunPiece piece = new MachineGunPiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      BulletProjectile projectile = new BulletProjectile(this, ProjectileLifeInSeconds);
      return projectile;
    }
  }
}
