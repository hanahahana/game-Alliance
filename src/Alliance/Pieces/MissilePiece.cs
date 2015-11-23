using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Alliance.Data;
using Alliance.Utilities;
using Alliance.Invaders;
using Alliance.Projectiles;
using Alliance.Parameters;
using Alliance.Objects;

namespace Alliance.Pieces
{
  public class MissilePiece : Piece
  {
    private const string MissileName = "Missile";
    private const string UltimateMissileName = "Blaster";

    public MissilePiece()
    {
      // setup the description
      StringBuilder sb = new StringBuilder();
      sb.AppendLine("Slams multiple missiles into the enemy! They explode on contact and create debri.");

      // set the properties of the piece
      mDescription = sb.ToString();
      mRadius = 100;
      mAttack = 250;
      mNumberProjectilesToFire = 2;
      mUpgradePercent = 20;
      mPrice = 15;
      mName = MissileName;
      mUltimateName = UltimateMissileName;
    }

    protected override Piece CreatePiece(GridCell[] cells)
    {
      MissilePiece piece = new MissilePiece();
      return piece;
    }

    protected override Projectile CreateProjectile()
    {
      MissileProjectile projectile = new MissileProjectile(ProjectileLifeInSeconds);
      return projectile;
    }

    protected override string ImageKey
    {
      get { return "missileLauncher"; }
    }
  }
}
