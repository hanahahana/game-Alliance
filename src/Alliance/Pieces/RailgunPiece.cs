using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alliance.Data;
using Alliance.Utilities;
using Alliance.Entities;
using Alliance.Projectiles;
using Alliance.Parameters;
using Alliance.Objects;

namespace Alliance.Pieces
{
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
      mDescription = sb.ToString();
      mRadius = 180;
      mAttack = 2500f;
      mProjectilesPerSecond = .25f;
      mNumberProjectilesToFire = 3;
      mProjectileLifeInSeconds = 5.5f;
      mUpgradePercent = 15;
      mPrice = 250;
      mName = RailgunName;
      mUltimateName = UltimateRailgunName;
    }

    protected override Piece CreatePiece(GridCell[] cells)
    {
      RailgunPiece piece = new RailgunPiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      RailgunProjectile projectile = new RailgunProjectile(ProjectileLifeInSeconds);
      return projectile;
    }

    protected override string ImageKey
    {
      get { return "railgun"; }
    }
  }
}
