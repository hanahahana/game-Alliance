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
    private const string UltimateMissileName = "Missile Blaster";

    public MissilePiece()
    {
      // setup the description
      StringBuilder sb = new StringBuilder();
      sb.Append("Slams multiple missiles into the enemy! They explode on contact and create debri.");

      // set the properties of the piece
      mDescription = sb.ToString();
      mRadius = 80;
      mAttack = 50f;
      mNumberProjectilesToFire = 2;
      mUpgradePercent = 15;
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
